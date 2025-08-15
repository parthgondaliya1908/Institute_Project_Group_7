using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Api.Database;

namespace Api.Features.Departments.GetById;

public interface IRepository
{
    Task<Projections.Department?> GetByIdAsync(long id, CancellationToken cancellationToken);
}

public class Repository([FromServices] DatabaseContext database) : IRepository
{
    public async Task<Projections.Department?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await database.Departments
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new Projections.Department(x.Id, x.Name))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
