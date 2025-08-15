using Microsoft.AspNetCore.Mvc;

using Api.Common.Types;

namespace Api.Features.Auth.Logout;

public class Service([FromServices] IRepository repo)
{
    public async Task<Result> LogoutAsync(long userId, string sessionId, CancellationToken cancellationToken)
    {
        int rowsAffected = await repo.DeleteSessionAsync(userId, sessionId, cancellationToken);
        return rowsAffected > 0
            ? Result.Success()
            : Result.UnauthorizedError();
    }
}
