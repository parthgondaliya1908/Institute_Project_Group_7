using Serilog;

using Api.Common.Options;
using Api.Common.States;
using Api.Middleware;
using Api.Startup;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string[] origins = builder.Configuration.GetRequiredSection("AllowedOrigins").Get<string[]>()!;
JwtOptions jwtOptions = builder.Configuration.GetRequiredSection(JwtOptions.SectionName).Get<JwtOptions>()!;

Services.LoadCommonStates();

builder.Services.AddProblemDetails()
    .AddCors()
    .AddCommonServices()
    .AddServicesFromAssembly()
    .ConfigureOptions(builder.Configuration)
    .AddDatabase(builder.Configuration.GetConnectionString("Default")!)
    .AddJwtAuth(Permission.List, jwtOptions)
    .AddEndpointsApiExplorer()
    .AddSwagger()
    .AddRazorPages();

builder.Host.UseSerilogWithConfig(builder.Configuration);


WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.CreateRequiredFolders();
await app.InitializeDatabaseAsync();

app.UseForwardedHeaders()
    .UseHsts()
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseRouting()
    .UseAndConfigureCors(origins)
    .UseAuthentication()
    .UseAuthorization()
    .UseSerilogRequestLogging()
    .UseExceptionHandler();

app.MapRazorPages();
app.MapEndpointsFromAssembly();

app.Run();
await Log.CloseAndFlushAsync();

public partial class Program { }
