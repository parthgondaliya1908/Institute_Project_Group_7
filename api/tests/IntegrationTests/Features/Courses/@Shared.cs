namespace Api.Tests.IntegrationTests.Features.Courses;

public static class Shared
{
    public static Api.Features.Courses.Add.Request NewAddRequest(long departmentId, string courseName)
    {
        return new()
        {
            Name = courseName,
            DepartmentId = departmentId
        };
    }

    public static Api.Features.Courses.Add.Request NewRandomAddRequest(long departmentId)
    {
        return new()
        {
            Name = $"Test course {Guid.NewGuid()}",
            DepartmentId = departmentId
        };
    }

    public static Api.Features.Courses.Update.Request NewUpdateRequest(long departmentId, string courseName)
    {
        return new()
        {
            Name = courseName,
            DepartmentId = departmentId
        };
    }

    public static Api.Features.Courses.Update.Request NewRandomUpdateRequest(long departmentId)
    {
        return new()
        {
            Name = $"Test course {Guid.NewGuid()}",
            DepartmentId = departmentId
        };
    }

    public static async Task<Course> NewRandomCourse(this DatabaseFixture fixture, Department department, bool isDeleted = false, bool saveToDatabase = false)
    {
        Course course = new()
        {
            Name = $"Test course {Guid.NewGuid()}",
            Department = department,
            IsDeleted = isDeleted
        };

        if (saveToDatabase)
        {
            await fixture.Database.Courses.AddAsync(course);
            await fixture.Database.SaveChangesAsync();
        }

        return course;
    }

    public static async Task<Course?> GetCourse(this DatabaseFixture fixture, long id)
    {
        return await fixture.Database.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
