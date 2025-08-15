namespace Api.Database.Models;

public class Department
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual List<Course> Courses { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
