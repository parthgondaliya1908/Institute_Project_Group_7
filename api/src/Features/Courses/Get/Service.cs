using Api.Common.Types;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Courses.Get;

public class Service([FromServices] IRepository repo)
{
    public async Task<Result<List<Course>>> GetAsync(long departmentId, CancellationToken cancellationToken)
    {
        bool departmentExists = await repo.DepartmentExistsAsync(departmentId, cancellationToken);
        if (!departmentExists)
        {
            return Result.NotFoundError("Specifeid department doesn't exist");
        }

        return Result.Success(await repo.GetAsync(departmentId, cancellationToken));
    }
}
