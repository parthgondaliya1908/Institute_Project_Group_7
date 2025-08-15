namespace Api.Features.Courses.Update;

public class Request
{
    public string Name { get; set; } = null!;
    public long DepartmentId { get; set; }
}
