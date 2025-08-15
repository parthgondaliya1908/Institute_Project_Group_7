using Microsoft.AspNetCore.Mvc;

using Api.Common.Types;
using Api.Database.Models;

namespace Api.Features.Courses.Update;

public class Service([FromServices] IRepository repo)
{
    public async Task<Result<UpdatedCourse>> UpdateAsync(long id, Request request, CancellationToken cancellationToken)
    {
        Course? course = await repo.GetAsync(id, cancellationToken);
        if (course is null)
        {
            return Result.NotFoundError($"Course with ID '{id}' not found");
        }

        bool anotherCourseWithSameNameExists = await repo.AnotherWithSameNameExistsAsync(id,request.Name, cancellationToken);
        if (anotherCourseWithSameNameExists)
        {
            return Result.ConflictError($"Course with name '{request.Name}' already exists");
        }

        course.UpdateFrom(request);
        await repo.UpdateAsync(course, cancellationToken);

        return Result.Success(course.AsUpdatedCourse());
    }
}
