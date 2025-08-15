using Npgsql;

namespace Api.Tests.IntegrationTests.Utils;

public static class PostgresUtils
{
    public static async Task DropDatabaseAsync(string connectionString)
    {
        NpgsqlConnectionStringBuilder builder = new(connectionString);
        string databaseName = builder.Database!;
        string adminConnectionString = new NpgsqlConnectionStringBuilder(connectionString) { Database = "postgres" }.ToString();

        await using NpgsqlConnection adminConn = new(adminConnectionString);
        await adminConn.OpenAsync();

        await using NpgsqlCommand terminate = new($@"
            SELECT pg_terminate_backend(pg_stat_activity.pid)
            FROM pg_stat_activity
            WHERE pg_stat_activity.datname = '{databaseName}' AND pid <> pg_backend_pid();
        ", adminConn);
        await terminate.ExecuteNonQueryAsync();

        await using NpgsqlCommand dropCommand = new($"DROP DATABASE IF EXISTS \"{databaseName}\";", adminConn);
        await dropCommand.ExecuteNonQueryAsync();
    }

    public static async Task RecreateDatabaseAsync(string databaseName)
    {
        await using NpgsqlConnection adminConn = new(Db.PostgresConnectionString);
        await adminConn.OpenAsync();

        await using NpgsqlCommand terminate = new($@"
            SELECT pg_terminate_backend(pg_stat_activity.pid)
            FROM pg_stat_activity
            WHERE pg_stat_activity.datname = '{databaseName}' AND pid <> pg_backend_pid();
        ", adminConn);
        await terminate.ExecuteNonQueryAsync();

        await using NpgsqlCommand dropCommand = new($"DROP DATABASE IF EXISTS \"{databaseName}\";", adminConn);
        await dropCommand.ExecuteNonQueryAsync();

        await adminConn.CloseAsync();
    }

    public static async Task DropAllTestDatabasesAsync(string prefix, string postgresConnectionString)
    {
        await using NpgsqlConnection adminConn = new(postgresConnectionString);
        await adminConn.OpenAsync();

        List<string> databasesToDelete = [];

        string fetchDatabasesQuery = $@"
            SELECT datname
            FROM pg_database
            WHERE datistemplate = false
            AND datname ILIKE @prefix || '%'
            AND datname NOT IN ('postgres')
        ";

        string terminateQuery = $@"
            SELECT pg_terminate_backend(pid)
            FROM pg_stat_activity
            WHERE datname = @dbName AND pid <> pg_backend_pid();
        ";

        await using (NpgsqlCommand fetchDatabases = new(fetchDatabasesQuery, adminConn))
        {
            fetchDatabases.Parameters.AddWithValue("prefix", prefix);
            await using NpgsqlDataReader reader = await fetchDatabases.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                databasesToDelete.Add(reader.GetString(0));
            }
        }

        foreach (string dbName in databasesToDelete)
        {
            await using (NpgsqlCommand terminate = new(terminateQuery, adminConn))
            {
                terminate.Parameters.AddWithValue("dbName", dbName);
                await terminate.ExecuteNonQueryAsync();
            }

            await using NpgsqlCommand dropCommand = new($@"DROP DATABASE IF EXISTS ""{dbName}"";", adminConn);
            await dropCommand.ExecuteNonQueryAsync();
        }

        await adminConn.CloseAsync();
        await adminConn.DisposeAsync();
    }
}
