using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Api.Database;

namespace Api.Features.Courses.Get;

public interface IRepository
{
    Task<bool> DepartmentExistsAsync(long id, CancellationToken cancellationToken);
    Task<List<Course>> GetAsync(long departmentId, CancellationToken cancellationToken);
}

public class Repository([FromServices] DatabaseContext database) : IRepository
{
    public async Task<bool> DepartmentExistsAsync(long id, CancellationToken cancellationToken)
    {
        return await database.Departments
            .AnyAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<List<Course>> GetAsync(long departmentId, CancellationToken cancellationToken)
    {
        return await database.Courses
            .Where(x => x.DepartmentId == departmentId && !x.IsDeleted)
            .OrderBy(x => x.Id)
            .Select(x => new Course(x.Id, x.Name))
            .ToListAsync(cancellationToken);
    }
}
