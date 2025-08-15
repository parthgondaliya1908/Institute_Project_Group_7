using Microsoft.AspNetCore.Mvc;

using Api.Common.Types;

namespace Api.Features.Auth.LogoutAll;

public class Service([FromServices] IRepository repo)
{
    public async Task<Result> LogoutAllAsync(long userId, CancellationToken cancellationToken)
    {
        int rowsAffected = await repo.DeleteSessionsAsync(userId, cancellationToken);
        return rowsAffected > 0
            ? Result.Success()
            : Result.NotFoundError("Specified user has no sessions");
    }
}
