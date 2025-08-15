using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Api.Database;

namespace Api.Features.Auth.LogoutAll;

public interface IRepository
{
    Task<int> DeleteSessionsAsync(long userId, CancellationToken cancellationToken);
}

public class Repository([FromServices] DatabaseContext database) : IRepository
{
    public async Task<int> DeleteSessionsAsync(long userId, CancellationToken cancellationToken)
    {
        int rowsAffected = await database.UserSessions
            .Where(x => x.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        return rowsAffected;
    }
}
