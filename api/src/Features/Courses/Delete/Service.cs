using Microsoft.AspNetCore.Mvc;

using Api.Common.Types;

namespace Api.Features.Courses.Delete;

public class Service([FromServices] IRepository repo)
{
    public async Task<Result> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        int rowsAffected = await repo.DeleteAsync(id, cancellationToken);
        return rowsAffected > 0
            ? Result.Success()
            : Result.NotFoundError($"Course with ID '{id}' not found");
    }
}
