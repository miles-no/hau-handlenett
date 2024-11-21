using Azure.Identity;
using HandlenettAPI.Configurations;
using HandlenettAPI.Interfaces;
using HandlenettAPI.Middleware;
using HandlenettAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.ExternalConnectors;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Configuration;
using System.Net.Http.Headers;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Swagger configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Handlenett API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI...\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
    builder =>
    {
        builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod().WithOrigins("http://localhost:3000", "http://localhost:3000");
    });
});

var keyVaultName = builder.Configuration["AzureKeyVaultNameProd"];
if (string.IsNullOrEmpty(keyVaultName))
{
    throw new InvalidOperationException("Missing Azure Key Vault configuration: AzureKeyVaultNameProd");
}
builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential());
//DefaultAzureCredential() is handled by enabling system assigned identity on container app and creating access policy in kv

//Add strongly-typed configuration with runtime validation
builder.Services.AddOptions<AzureCosmosDBSettings>()
    .Bind(builder.Configuration.GetSection("AzureCosmosDBSettings"))
    .ValidateDataAnnotations() // Validates [Required] attributes at runtime
    .ValidateOnStart(); // Ensures validation happens at application startup
builder.Services.AddOptions<SlackSettings>()
    .Bind(builder.Configuration.GetSection("SlackSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Add services to the container.
//TODO: null handling errors for config sections
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi()
        .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
        .AddInMemoryTokenCaches();

//Dependency Injection
builder.Services.AddScoped<ICosmosDBService>(provider =>
{
    var settings = provider.GetRequiredService<IOptions<AzureCosmosDBSettings>>().Value;

    return new CosmosDBService(
        new CosmosClient(settings.ConnectionString),
        settings.DatabaseName,
        settings.ContainerName
    );
});
var redisConnString = builder.Configuration.GetConnectionString("AzureRedisCache") ?? throw new InvalidOperationException("Missing AzureRedisCache config");
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConnString)); //heavy resource and is designed to be reused
builder.Services.AddHttpClient<WeatherService>();
builder.Services.AddHttpClient<SlackService>("SlackClient", client =>
{
    client.BaseAddress = new Uri("https://slack.com/api/");
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    // Configure static Authorization header
    var slackToken = builder.Configuration["SlackSettings:SlackBotUserOAuthToken"];
    if (string.IsNullOrEmpty(slackToken))
    {
        throw new InvalidOperationException("Slack Bot User OAuth token is missing.");
    }
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", slackToken);
});
builder.Services.AddScoped<SlackService>();

//User
builder.Services.AddScoped<UserService>(sp =>
{
    var dbContext = sp.GetRequiredService<AzureSQLContext>();
    var blobStorageService = sp.GetRequiredService<AzureBlobStorageService>();
    var slackSettings = sp.GetRequiredService<IOptions<SlackSettings>>().Value;

    return new UserService(dbContext, blobStorageService, slackSettings.ContainerNameUserImages); //get from builder.configuration instead of strongly typed?
});
builder.Services.AddScoped<AzureSQLContext>();
builder.Services.AddScoped<AzureBlobStorageService>(sp =>
{
    var connectionString = builder.Configuration["ConnectionStrings:AzureStorageUsingAccessKey"];
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Azure Storage connection string is missing.");
    }

    return new AzureBlobStorageService(connectionString);
});

builder.Services.AddScoped<UserInitializationService>();

//builder.Services.AddSingleton(); // A single instance is shared across the entire application lifetime
//builder.Services.AddHttpClient<T>() //DI container registers T as a transient service by default.
//builder.Services.AddScoped<T>() // T
//TODO: add AzureSQLContext



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//TODO: Fiks dev miljø for key vault
//if (builder.Environment.IsProduction())
//{
//    builder.Configuration.AddAzureKeyVault(
//        new Uri($"https://{builder.Configuration["AzureKeyVaultNameProd"]}.vault.azure.net/"),
//        new DefaultAzureCredential());
//}
//else if (builder.Environment.IsDevelopment())
//{
//    builder.Configuration.AddAzureKeyVault(
//        new Uri($"https://{builder.Configuration["AzureKeyVaultNameDev"]}.vault.azure.net/"),
//        new DefaultAzureCredential());
//}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
        c.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
        c.OAuthAppName("Your API Swagger");
    });
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();
app.UseAuthentication(); //validates tokens
app.UseAuthorization(); //enforces [Authorize] attributes
app.UseMiddleware<UserInitializationMiddleware>(); //Custom implementation of SQL database user verification
app.MapControllers().RequireCors(MyAllowSpecificOrigins);

ConfigurationHelper.Initialize(app.Configuration);

app.Run();