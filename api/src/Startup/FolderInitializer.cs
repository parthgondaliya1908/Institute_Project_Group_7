namespace Api.Startup;

public static class FolderInitializer
{
    public static WebApplication CreateRequiredFolders(this WebApplication app)
    {
        IConfiguration configuration = app.Configuration;

        string[]? requiredFolders = configuration.GetSection("Defaults:Folders").Get<string[]>();
        if (requiredFolders is null)
            return app;

        foreach (string requiredFolder in requiredFolders)
        {
            string path = Path.Combine(app.Environment.WebRootPath, requiredFolder);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        return app;
    }
}
