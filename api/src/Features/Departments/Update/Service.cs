using Microsoft.AspNetCore.Mvc;

using Api.Common.Types;
using Api.Database.Models;

namespace Api.Features.Departments.Update;

public class Service([FromServices] IRepository repo)
{
    public async Task<Result<UpdatedDepartment>> UpdateAsync(long id, Request request, CancellationToken cancellationToken)
    {
        Department? department = await repo.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return Result.NotFoundError($"Department with ID '{id}' not found");
        }

        bool anotherDepartmentWithSameNameExists = await repo.AnotherWithSameNameExistsAsync(id, request.Name, cancellationToken);
        if (anotherDepartmentWithSameNameExists)
        {
            return Result.ConflictError($"Department with name '{request.Name}' already exists");
        }

        department.UpdateFrom(request);
        await repo.UpdateAsync(department, cancellationToken);

        return Result.Success(department.AsUpdatedDepartment());
    }
}
