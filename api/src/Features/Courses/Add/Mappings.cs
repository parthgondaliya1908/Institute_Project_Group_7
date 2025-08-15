using Api.Database.Models;

namespace Api.Features.Courses.Add;

public static class Mappings
{
    public static Course AsCourse(this Request request) => new()
    {
        Name = request.Name,
        DepartmentId = request.DepartmentId
    };
    
    public static AddedCourse AsAddedCourse(this Course course, AddedCourseDepartment department) => new(
        course.Id,
        course.Name,
        department
    );
}
