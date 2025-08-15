using Api.Common.States;

namespace Api.Features.Courses;

public class Permissions : IPermissionsProvider
{
    public const string Prefix = "Courses";
    public readonly static Permission Add = new($"{Prefix}.Add");
    public readonly static Permission Update = new($"{Prefix}.Update");
    public readonly static Permission Delete = new($"{Prefix}.Delete");
    public readonly static Permission View = new($"{Prefix}.GetAll");

    public IReadOnlyList<Permission> GetAll() => [
        Add,
        Update,
        Delete,
        View
    ];
}
