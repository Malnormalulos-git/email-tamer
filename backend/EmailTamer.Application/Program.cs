using EmailTamer.Auth;
using EmailTamer.Config;
using EmailTamer.Database;
using EmailTamer.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration
    .AddUserSecrets<Program>(true, true)
    .AddEnvironmentVariables()
    .Build();
var appConfig = configuration.Get<ApplicationConfig>();

var host = builder.Host;
var services = builder.Services;


services.AddSingleton(appConfig!);
services.AddSingleton(TimeProvider.System);

services.AddDbContext<EmailTamerDbContext>(dbContextOptionsBuilder =>
    {
        dbContextOptionsBuilder
            .UseMySQL(appConfig!.Database.ConnectionString, optionsBuilder => optionsBuilder
                .EnableRetryOnFailure(appConfig.Database.Retries)
                .CommandTimeout(appConfig.Database.Timeout));
    }, ServiceLifetime.Transient
);

host.UseSerilog((context, loggerConfiguration) =>
{
    const string logOutputTemplate = "[{Timestamp:HH:mm:ss.fff}] "
                                     + "[{RequestId}] "
                                     + "[{SourceContext:l}] "
                                     + "[{Level:u3}] "
                                     + "{Message:lj}{NewLine}{Exception}";

    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        .Enrich.WithThreadId();

    loggerConfiguration.WriteTo.Console(
        outputTemplate: logOutputTemplate,
        theme: AnsiConsoleTheme.Literate,
        restrictedToMinimumLevel: LogEventLevel.Debug);
});

var app = builder.Build();

app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EmailTamerDbContext>();
    dbContext.Database.Migrate();
}

app.Run();