using Microsoft.AspNetCore.Mvc;

using Api.Common.Types;

namespace Api.Features.Departments.GetAll;

public class Service([FromServices] IRepository repo)
{
    public async Task<Result<List<Projections.Department>>> GetAllAsync(CancellationToken cancellationToken)
    {
        List<Projections.Department> departments = await repo.GetAllAsync(cancellationToken);
        return Result.Success(departments);
    }
}
