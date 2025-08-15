namespace Api.Common.States;

public interface IPermissionsProvider
{
    IReadOnlyList<Permission> GetAll();
}

public record struct Permission(string Name)
{
    public static readonly Permission All = new("All");
    public static IReadOnlyList<Permission> List { get; private set; } = null!;

    public static void LoadAllFromAssembly()
    {
        List<Permission> allPermissions = [];

        IEnumerable<Type> permissionTypes = typeof(Program)
            .Assembly
            .GetTypes()
            .Where(type => type
                .GetInterfaces()
                .Contains(typeof(IPermissionsProvider))
            );

        foreach (Type permissionType in permissionTypes)
        {
            if (Activator.CreateInstance(permissionType) is not IPermissionsProvider instance)
            {
                continue;
            }

            allPermissions.AddRange(instance.GetAll());
        }

        List = allPermissions;
    }
}
