using EmailTamer.Auth;
using EmailTamer.Core.DependencyInjection;
using EmailTamer.Core.Extensions;
using EmailTamer.Database;
using EmailTamer.Database.Config;
using EmailTamer.Infrastructure;
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

var dbConfig = configuration.GetSection("Database").Get<DatabaseConfig>();

var host = builder.Host;
var services = builder.Services;
var isDevelopment = builder.Environment.IsDevelopment();


services.AddSingleton(TimeProvider.System);


services.AddInfrastructure();
services.AddCoreServicesFromAssembly(typeof(Program).Assembly);

services.AddDatabase(dbConfig!);

services.AddMvcCore()
    .AddAuthPart(configuration);

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

if (isDevelopment)
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.MapControllers();

app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EmailTamerDbContext>();
    dbContext.Database.Migrate();
}

app.Run();