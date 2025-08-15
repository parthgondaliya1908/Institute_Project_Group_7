using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Api.Database;
using Api.Database.Models;

namespace Api.Features.Departments.Add;

public interface IRepository
{
    Task<bool> AnotherWithSameNameExistsAsync(string name, CancellationToken cancellationToken);
    Task AddAsync(Department department, CancellationToken cancellationToken);
}

public class Repository([FromServices] DatabaseContext database) : IRepository
{
    public async Task<bool> AnotherWithSameNameExistsAsync(string name, CancellationToken cancellationToken)
    {
        return await database.Departments
            .AsNoTracking()
            .AnyAsync(x => EF.Functions.ILike(x.Name, name) && !x.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Department department, CancellationToken cancellationToken)
    {
        database.Departments.Add(department);
        await database.SaveChangesAsync(cancellationToken);
    }
}
