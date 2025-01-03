using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);



builder.Host.UseSerilog((context, loggerConfiguration) =>
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



app.Run();