using EmailTamer.Auth.Config;
using EmailTamer.Auth.Controllers;
using EmailTamer.Core.FluentValidation;
using EmailTamer.Database;
using EmailTamer.Database.Entities;
using EmailTamer.Infrastructure.Auth;
using EmaiTamer.Parts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EmailTamer.Auth;

public static class ServiceCollectionExtensions
{
    public static IMvcCoreBuilder AddAuthPart(this IMvcCoreBuilder builder, IConfigurationRoot configuration)
    {
        builder.AddEmailTamerPart<AuthController>();

        var services = builder.Services;
        
        const string jwtConfigSectionName = "JWT";
        services.AddOptionsWithValidator<JwtConfig, JwtConfig.Validator>(jwtConfigSectionName);

        var jwtConfig = new JwtConfig();
        configuration.Bind(jwtConfigSectionName, jwtConfig);

        services.ConfigureIdentity();
        services.ConfigureAuth(jwtConfig);

        services.AddScoped<IConfigurableUserContextAccessor, UserContextAccessor>();
        services.AddScoped<IUserContextAccessor>(sp => sp.GetRequiredService<IConfigurableUserContextAccessor>());

        return builder;
    }
    
    public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<EmailTamerUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 12;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 0;
            })
            .AddEntityFrameworkStores<EmailTamerDbContext>();
        
        return services;
    }

    public static IServiceCollection ConfigureAuth(this IServiceCollection services, JwtConfig jwtConfig)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = 
                options.DefaultChallengeScheme = 
                    options.DefaultForbidScheme = 
                        options.DefaultScheme = 
                            options.DefaultSignInScheme = 
                                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtConfig.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(jwtConfig.Key)
                )
            };
        });

        services.AddAuthorization(options =>
        {
            var adminRole = UserRole.Admin.ToString("G");
            var userRole = UserRole.User.ToString("G");
            
            options.AddPolicy(Policies.Admin, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(adminRole);
            });
            options.AddPolicy(Policies.User, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(adminRole, userRole);
            });
        });

        return services;
    }
}