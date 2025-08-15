using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Api.Database;

namespace Api.Features.Departments.Delete;

public interface IRepository
{
    Task<int> DeleteAsync(long id, CancellationToken cancellationToken);
}

public class Repository([FromServices] DatabaseContext database) : IRepository
{
    public async Task<int> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        int rowsAffected = await database.Departments
            .Where(x => x.Id == id && !x.IsDeleted)
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.IsDeleted, true), cancellationToken);

        return rowsAffected;
    }
}
