using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Api.Database;

namespace Api.Features.Departments.GetAll;

public interface IRepository
{
    Task<List<Projections.Department>> GetAllAsync(CancellationToken cancellationToken);
}

public class Repository([FromServices] DatabaseContext database) : IRepository
{
    public async Task<List<Projections.Department>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await database.Departments
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Where(x => !x.IsDeleted)
            .Select(x => new Projections.Department(x.Id, x.Name))
            .ToListAsync(cancellationToken);
    }
}
