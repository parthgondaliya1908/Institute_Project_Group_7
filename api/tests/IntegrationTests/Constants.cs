using Api.Common.Options;
using Api.Common.States;
using Api.Common.Types;
using Api.Tests.IntegrationTests.Utils;

namespace Api.Tests.IntegrationTests;

public static class Users
{
    public static readonly GoogleUser MockGoogleUser = new("sub", "test@example.com", "Test User", "https://pic.url");
}

public static class GoogleTokens
{
    public const string CorrectToken = "fake-google-id-token";
    public const string IncorrectToken = "fake-google-id-token-incorrect";
}

public static class Db
{
    public const string PostgresConnectionString = "User ID=postgres;Password=root;Host=localhost;Port=5432;Database=postgres;Connection Lifetime=0;Include Error Detail=true;";
    public static string GenerateConnectionString(string databaseName) => $"User ID=postgres;Password=root;Host=localhost;Port=5432;Database={databaseName};Connection Lifetime=0;Include Error Detail=true;";
    public static string NameFromType(Type type)
    {
        string[] parts = type.FullName!.Split('.');
        string typeName = parts[^1];
        string parentNamespace = parts[^2];
        string databaseName = $"{parentNamespace}_{typeName}";

        return databaseName;
    }
}

public static class Jwt
{
    public const string Invalid = "fake-token.fake-token.fake-token";
    public static string OfAllPermissions { get; private set; } = null!;
    public static string OfNoPermissions { get; private set; } = null!;
    public static IReadOnlyDictionary<Permission, string> Permissions { get; private set; } = null!;

    public static void Generate(JwtOptions jwtOptions)
    {
        OfAllPermissions = TokenUtils.GenerateAccessToken(jwtOptions, 1, [Permission.All]);
        OfNoPermissions = TokenUtils.GenerateAccessToken(jwtOptions, 1, []);

        Dictionary<Permission, string> permissions = [];
        foreach (Permission permission in Permission.List)
        {
            permissions.Add(permission, TokenUtils.GenerateAccessToken(jwtOptions, 1, [permission]));
        }

        Permissions = permissions;
    }
    
    public static string Of(Permission permission) => Permissions[permission];
}
