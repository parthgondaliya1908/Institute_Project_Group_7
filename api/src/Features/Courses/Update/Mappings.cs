using Api.Database.Models;

namespace Api.Features.Courses.Update;

public static class Mappings
{
    public static void UpdateFrom(this Course course, Request request)
    {
        course.Name = request.Name;
        course.DepartmentId = request.DepartmentId;
    }

    public static UpdatedCourse AsUpdatedCourse(this Course course) => new(
        course.Id,
        course.Name,
        course.DepartmentId
    );
}
