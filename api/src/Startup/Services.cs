using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Serilog;
using Serilog.Core;

using Api.Common;
using Api.Common.Options;
using Api.Common.Repositories;
using Api.Common.Services;
using Api.Database;
using Api.Database.Models.Identity;
using Api.Common.States;

namespace Api.Startup;

public static class Services
{
    private static IEnumerable<IRegistry> GetRegistriesFromAssembly()
    {
        return typeof(Program).Assembly
            .GetTypes()
            .Where(t => typeof(IRegistry).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(t => Activator.CreateInstance(t) as IRegistry)
            .Where(r => r is not null)!;
    }

    public static IServiceCollection AddServicesFromAssembly(this IServiceCollection services)
    {
        foreach (IRegistry registry in GetRegistriesFromAssembly())
            registry.AddServices(services);

        return services;
    }

    public static WebApplication MapEndpointsFromAssembly(this WebApplication app)
    {
        foreach (IRegistry registry in GetRegistriesFromAssembly())
            registry.MapEndpoints(app);

        return app;
    }

    public static void LoadCommonStates()
    {
        Permission.LoadAllFromAssembly();
    }

    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Common.Options.CookieOptions>(configuration.GetRequiredSection(Common.Options.CookieOptions.SectionName));
        services.Configure<JwtOptions>(configuration.GetRequiredSection(JwtOptions.SectionName));
        services.Configure<OAuthOptions>(configuration.GetRequiredSection(OAuthOptions.SectionName));

        return services;
    }

    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();

        services.AddScoped<ICookieService, CookieService>();
        services.AddScoped<IGoogleTokenService, GoogleTokenService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IImageUploadService, ImageUploadService>();

        services.AddSingleton<IAuthorizationHandler, Handlers.PermissionHandler>();

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(DataSource.OfPostgres(connectionString)));
        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options => options.CustomSchemaIds(type => type.FullName!.Replace("+", ".")));
        return services;
    }

    public static ConfigureHostBuilder UseSerilogWithConfig(this ConfigureHostBuilder host, IConfiguration config)
    {
        Logger logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

        host.UseSerilog(logger);
        return host;
    }

    public static IApplicationBuilder UseAndConfigureCors(this IApplicationBuilder app, string[] origins)
    {
        app.UseCors(x => x
            .WithOrigins(origins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("Content-Disposition")
        );

        return app;
    }
}
