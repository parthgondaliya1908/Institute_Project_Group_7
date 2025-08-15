using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Api.Database;
using Api.Database.Models;

namespace Api.Features.Courses.Update;

public interface IRepository
{
    Task<Course?> GetAsync(long id, CancellationToken cancellationToken);
    Task<bool> AnotherWithSameNameExistsAsync(long id, string name, CancellationToken cancellationToken);
    Task UpdateAsync(Course course, CancellationToken cancellationToken);
}

public class Repository([FromServices] DatabaseContext database) : IRepository
{
    public async Task<Course?> GetAsync(long id, CancellationToken cancellationToken)
    {
        return await database.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<bool> AnotherWithSameNameExistsAsync(long id, string name, CancellationToken cancellationToken)
    {
        return await database.Courses
            .AnyAsync(x => x.Id != id && EF.Functions.ILike(x.Name, name) && !x.IsDeleted, cancellationToken);
    }

    public async Task UpdateAsync(Course course, CancellationToken cancellationToken)
    {
        database.Courses.Update(course);
        await database.SaveChangesAsync(cancellationToken);
    }
}
