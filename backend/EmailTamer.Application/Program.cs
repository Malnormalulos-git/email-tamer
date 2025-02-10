using AutoMapper.EquivalencyExpression;
using EmailTamer.Auth;
using EmailTamer.Core;
using EmailTamer.Core.Extensions;
using EmailTamer.Core.Startup;
using EmailTamer.Database;
using EmailTamer.Database.Tenant;
using EmailTamer.Dependencies.Amazon;
using EmailTamer.Infrastructure;
using EmailTamer.Parts.EmailBox;
using EmailTamer.Parts.Sync;
using EmailTamer.Startup;
using HealthChecks.UI.Client;
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

var host = builder.Host;
var services = builder.Services;
var isDevelopment = builder.Environment.IsDevelopment();


services.AddAutoMapper(x => x.AddCollectionMappers(), typeof(Program));
services.AddStartupAction<AutoMapperValidationAction>();

services.AddHealthChecks();

services.AddCore();

services.AddInfrastructure()
    .AddDatabase()
    .AddTenantDatabases()
    .AddAmazonServices(isDevelopment);

services.AddMvcCore()
    .AddAuthPart(configuration)
    .AddEmailBoxesPart()
    .AddSyncPart();

services.AddEndpointsApiExplorer();

if (isDevelopment) {
    services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", new ()
            {
                Title = "EmailTamer",
                Version = "v1"
            });
            o.UseAllOfForInheritance();
        }
    ).AddSwaggerGenNewtonsoftSupport();
}

services.AddCors(options =>
{
    options.AddDefaultPolicy(corsPolicyBuilder =>
        corsPolicyBuilder
            .Map(x => isDevelopment
                ? x.WithOrigins("http://localhost:5173")
                : x)
            .AllowAnyMethod()
            .AllowCredentials()
            .AllowAnyHeader()
            .Build());
});

services.AddControllers()
    .AddNewtonsoftJson(x =>
    {
        var settings = x.SerializerSettings;
        settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        settings.Converters.Add(new StringEnumConverter{NamingStrategy = new CamelCaseNamingStrategy()});
        settings.NullValueHandling = NullValueHandling.Include;
    });

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

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.UseEmailTamerAuth();

if (isDevelopment)
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.MapHealthChecks("/api/health",
    new()
    {
        AllowCachingResponses = false,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.MapControllers();

app.UseSerilogRequestLogging();

await RunAsync(app);

return;

async Task RunAsync(WebApplication application)
{
    var lifetime = application.Lifetime;

    var appRunner = application.RunAsync();
    lifetime.ApplicationStarted.WaitHandle.WaitOne();

    await using (var scope = application.Services.CreateAsyncScope())
    {
        var sp = scope.ServiceProvider;
        await sp
            .GetRequiredService<IStartupActionCoordinator>()
            .PerformStartupActionsAsync(lifetime.ApplicationStopping);
    }

    await appRunner;
}