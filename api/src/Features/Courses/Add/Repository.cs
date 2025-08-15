using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Api.Database;
using Api.Database.Models;

namespace Api.Features.Courses.Add;

public interface IRepository
{
    Task<AddedCourseDepartment?> GetDepartmentById(long deparmentId, CancellationToken cancellationToken);
    Task<bool> AnotherWithSameNameExistsAsync(long departmentId, string name, CancellationToken cancellationToken);
    Task AddAsync(Course course, CancellationToken cancellationToken);
}

public class Repository([FromServices] DatabaseContext database) : IRepository
{
    public async Task<AddedCourseDepartment?> GetDepartmentById(long deparmentId, CancellationToken cancellationToken)
    {
        return await database.Departments
            .Where(x => x.Id == deparmentId && !x.IsDeleted)
            .Select(x => new AddedCourseDepartment(x.Id, x.Name))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> AnotherWithSameNameExistsAsync(long deparmentId, string name, CancellationToken cancellationToken)
    {
        return await database.Courses
            .AnyAsync(x => x.DepartmentId == deparmentId && EF.Functions.ILike(x.Name, name) && !x.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Course course, CancellationToken cancellationToken)
    {
        database.Courses.Add(course);
        await database.SaveChangesAsync(cancellationToken);
    }
}
