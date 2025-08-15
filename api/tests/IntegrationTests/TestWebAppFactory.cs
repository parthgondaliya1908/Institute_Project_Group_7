using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using Api.Common.Services;

namespace Api.Tests.IntegrationTests;

public class TestWebAppFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((context, config) => 
        {
            config.Sources.Clear();
            config.AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();
        });
        
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<DatabaseContext>>();
            services.RemoveAll<IGoogleTokenService>();
            
            services.AddDbContext<DatabaseContext>((serviceProvider, options) => 
            {
                options.UseNpgsql(DataSource.OfPostgres(connectionString));
            });

            services.AddScoped<IGoogleTokenService>(_ =>
            {
                Mocks.GoogleTokenService mockGoogleTokenService = new();
                mockGoogleTokenService.SetMockGoogleUser(MockGoogleUser);

                return mockGoogleTokenService;
            });
        });
    }
}
