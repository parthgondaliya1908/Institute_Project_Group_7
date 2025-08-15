using Microsoft.AspNetCore.Mvc;

using Api.Common.Types;
using Api.Database.Models;

namespace Api.Features.Courses.Add;

public class Service([FromServices] IRepository repo)
{
    public async Task<Result<AddedCourse>> AddAsync(Request request, CancellationToken cancellationToken)
    {
        AddedCourseDepartment? department = await repo.GetDepartmentById(request.DepartmentId, cancellationToken);
        if (department is null)
        {
            return Result.NotFoundError($"Department with ID '{request.DepartmentId}' not found");
        }

        bool anotherCourseWithSameNameExists = await repo.AnotherWithSameNameExistsAsync(request.DepartmentId, request.Name, cancellationToken);
        if (anotherCourseWithSameNameExists)
        {
            return Result.ConflictError($"Course with name '{request.Name}' already exists");
        }

        Course course = request.AsCourse();
        await repo.AddAsync(course, cancellationToken);

        return Result.Created(course.AsAddedCourse(department));
    }
}
