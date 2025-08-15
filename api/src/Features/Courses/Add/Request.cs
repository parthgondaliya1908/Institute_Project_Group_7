namespace Api.Features.Courses.Add;

public class Request
{
    public string Name { get; set; } = null!;
    public long DepartmentId { get; set; }
}
