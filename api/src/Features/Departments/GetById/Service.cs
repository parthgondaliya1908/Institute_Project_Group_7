using Microsoft.AspNetCore.Mvc;

using Api.Common.Types;

namespace Api.Features.Departments.GetById;

public class Service([FromServices] IRepository repo)
{
    public async Task<Result<Projections.Department>> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        Projections.Department? department = await repo.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return Result.NotFoundError($"Department with ID '{id}' not found");    
        }

        return Result.Success(department);
    }
}
