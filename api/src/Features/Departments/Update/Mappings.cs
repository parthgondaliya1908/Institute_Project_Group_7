using Api.Database.Models;

namespace Api.Features.Departments.Update;

public static class Mappings
{
    public static void UpdateFrom(this Department department, Request request)
    {
        department.Name = request.Name;
    }

    public static UpdatedDepartment AsUpdatedDepartment(this Department department) => new(
        department.Id,
        department.Name
    );
}
