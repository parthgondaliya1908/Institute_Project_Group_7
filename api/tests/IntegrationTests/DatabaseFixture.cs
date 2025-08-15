using Microsoft.Extensions.Options;

using Api.Common.Options;
using Api.Database.Models.Identity;
using Api.Tests.IntegrationTests.Utils;

namespace Api.Tests.IntegrationTests;

public class DatabaseFixture : IAsyncLifetime
{
    private bool initialized = false;
    private bool testsPassed = true;
    private string connectionString = null!;

    public void Initialize(Type type)
    {
        if (initialized)
        {
            return;
        }

        string databaseName = Db.NameFromType(type);

        connectionString = Db.GenerateConnectionString(databaseName);
        PostgresUtils.RecreateDatabaseAsync(databaseName).Wait();

        Factory = new TestWebAppFactory(connectionString);
        Http = Factory.CreateClient();
        Scope = Factory.Services.CreateAsyncScope();

        Database = Scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        Jwt.Generate(Scope.ServiceProvider.GetRequiredService<IOptions<JwtOptions>>().Value);

        initialized = true;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        if (testsPassed)
        {
            await PostgresUtils.DropDatabaseAsync(connectionString);
        }

        Scope.Dispose();
        Http.Dispose();
        Factory.Dispose();
    }

    public void MarkTestsFailed()
    {
        testsPassed = false;
    }

    public void PreserveDatabase()
    {
        testsPassed = false;
    }

    public TestWebAppFactory Factory { get; private set; } = null!;
    public HttpClient Http { get; private set; } = null!;
    public IServiceScope Scope { get; private set; } = null!;
    public DatabaseContext Database { get; private set; } = null!;
    public UserManager<User> UserManager { get; private set; } = null!;
}
