using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Api.Database;
using Api.Database.Models;

namespace Api.Features.Departments.Update;

public interface IRepository
{
    Task<Department?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<bool> AnotherWithSameNameExistsAsync(long id, string name, CancellationToken cancellationToken);
    Task UpdateAsync(Department department, CancellationToken cancellationToken);
}

public class Repository([FromServices] DatabaseContext database) : IRepository
{
    public async Task<Department?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await database.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<bool> AnotherWithSameNameExistsAsync(long id, string name, CancellationToken cancellationToken)
    {
        return await database.Departments
            .AsNoTracking()
            .AnyAsync(x => EF.Functions.ILike(x.Name, name) && x.Id != id && !x.IsDeleted, cancellationToken);
    }

    public async Task UpdateAsync(Department department, CancellationToken cancellationToken)
    {
        database.Departments.Update(department);
        await database.SaveChangesAsync(cancellationToken);
    }
}
