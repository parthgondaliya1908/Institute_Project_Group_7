namespace Api.Tests.IntegrationTests.Features.Departments;

public static class Shared
{
    public static Api.Features.Departments.Add.Request NewAddRequest(string departmentName)
    {
        return new()
        {
            Name = departmentName
        };
    }

    public static Api.Features.Departments.Add.Request NewRandomAddRequest()
    {
        return new()
        {
            Name = $"Test Department {Guid.NewGuid()}"
        };
    }

    public static Api.Features.Departments.Update.Request NewUpdateRequest(string departmentName)
    {
        return new()
        {
            Name = departmentName
        };
    }

    public static Api.Features.Departments.Update.Request NewRandomUpdateRequest()
    {
        return new()
        {
            Name = $"Test Department {Guid.NewGuid()}"
        };
    }

    public static async Task<Department> NewRandomDepartment(this DatabaseFixture fixture, bool isDeleted = false, bool saveToDatabase = false)
    {
        Department department = new()
        {
            Name = $"Test Department {Guid.NewGuid()}",
            IsDeleted = isDeleted
        };

        if (saveToDatabase)
        {
            await fixture.Database.Departments.AddAsync(department);
            await fixture.Database.SaveChangesAsync();
        }

        return department;
    }

    public static async Task<Department?> GetDepartment(this DatabaseFixture fixture, long id)
    {
        return await fixture.Database.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
