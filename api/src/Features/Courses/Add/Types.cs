namespace Api.Features.Courses.Add;

public record AddedCourseDepartment(long Id, string Name);

public record AddedCourse(
    long Id,
    string Name,
    AddedCourseDepartment Department
);
