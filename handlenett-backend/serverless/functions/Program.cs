using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

//logging not working
//var host = new HostBuilder()
//    .ConfigureFunctionsWebApplication()
//    .ConfigureServices(services =>
//    {
//        services.AddApplicationInsightsTelemetryWorkerService();
//        services.ConfigureFunctionsApplicationInsights();
//    })
//    .Build();

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication() // Keep this as ASP.NET Core-based Functions use this
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetry();

        // Optional: Custom logging configuration
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddApplicationInsights();
            loggingBuilder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("", LogLevel.Information);
            loggingBuilder.AddConsole();
        });
    })
    .ConfigureLogging(logging =>
    {
        logging.AddConsole(); // Adds Console Logging
        logging.AddApplicationInsights(); // Adds Application Insights Logging
    })
    .Build();


host.Run();
