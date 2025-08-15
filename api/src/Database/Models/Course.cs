namespace Api.Database.Models;

public class Course
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;

    public long DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
