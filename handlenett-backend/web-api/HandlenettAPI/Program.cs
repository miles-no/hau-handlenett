using Azure.Identity;
using HandlenettAPI.Configurations;
using HandlenettAPI.Interfaces;
using HandlenettAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.ExternalConnectors;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
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

//Inject specific HttpClients
builder.Services.AddHttpClient<WeatherService>();
builder.Services.AddHttpClient("SlackClient", client =>
{
    client.BaseAddress = new Uri("https://slack.com/api/");
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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


// Add services to the container.
//TODO: null handling errors for config sections
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi()
        .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
        .AddInMemoryTokenCaches();



//Dependency Service Injections
builder.Services.AddScoped<SlackService>();
builder.Services.AddScoped<ICosmosDBService>(provider =>
{
    var settings = provider.GetRequiredService<IOptions<AzureCosmosDBSettings>>().Value;

    return new CosmosDBService(
        new CosmosClient(settings.ConnectionString),
        settings.DatabaseName,
        settings.ContainerName
    );
});

//builder.Services.AddSingleton(graphClient); // A single instance is shared across the entire application lifetime, do not use for databaseconnections (maybe just a static cache)
//TODO: add AzureSQLContext singleton



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
        c.OAuthClientId("6409e25f-f9b7-4b70-a84c-6c077440d740");
        c.OAuthAppName("Your API Swagger");
    });
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireCors(MyAllowSpecificOrigins);

ConfigurationHelper.Initialize(app.Configuration);

app.Run();