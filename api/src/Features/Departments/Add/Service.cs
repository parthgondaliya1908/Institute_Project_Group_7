using Microsoft.AspNetCore.Mvc;

using Api.Common.Types;
using Api.Database.Models;

namespace Api.Features.Departments.Add;

public class Service([FromServices] IRepository repo)
{
    public async Task<Result<AddedDepartment>> AddAsync(Request request, CancellationToken cancellationToken)
    {
        bool anotherDepartmentWithSameNameExists = await repo.AnotherWithSameNameExistsAsync(request.Name, cancellationToken);
        if (anotherDepartmentWithSameNameExists)
        {
            return Result.ConflictError($"Department with name '{request.Name}' already exists");
        }

        Department department = request.AsDepartment();
        await repo.AddAsync(department, cancellationToken);

        return Result.Created(department.AsAddedDepartment(), Api.GetByIdUrl(department.Id));
    }
}
