using Api.Database.Models;

namespace Api.Features.Departments.Add;

public static class Mappings
{
    public static Department AsDepartment(this Request request) => new()
    {
        Name = request.Name,
    };

    public static AddedDepartment AsAddedDepartment(this Department department) => new(
        department.Id,
        department.Name
    );
}
