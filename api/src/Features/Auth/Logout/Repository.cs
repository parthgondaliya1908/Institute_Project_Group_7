using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Api.Database;

namespace Api.Features.Auth.Logout;

public interface IRepository
{
    Task<int> DeleteSessionAsync(long userId, string sessionId, CancellationToken cancellationToken);
}

public class Repository([FromServices] DatabaseContext database) : IRepository
{
    public async Task<int> DeleteSessionAsync(long userId, string sessionId, CancellationToken cancellationToken)
    {
        int rowsAffected = await database.UserSessions
            .Where(x => x.UserId == userId && x.SessionId == sessionId)
            .ExecuteDeleteAsync(cancellationToken);

        return rowsAffected;
    }
}
