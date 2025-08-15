using Microsoft.AspNetCore.Mvc;

using Api.Common.Types;

namespace Api.Features.Departments.Delete;

public class Service([FromServices] IRepository repo)
{
    public async Task<Result> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        int rowsAffected = await repo.DeleteAsync(id, cancellationToken);
        if (rowsAffected == 0)
        {
            return Result.NotFoundError($"Department with ID '{id}' not found");
        }

        return Result.Success();
    }
}
