using Api.Tests.IntegrationTests.Extensions;
using Api.Tests.IntegrationTests.Features.Departments;
using CoursesApi = Api.Features.Courses.Api;
using CoursesPermissions = Api.Features.Courses.Permissions;

namespace Api.Tests.IntegrationTests.Features.Courses;

public class Get : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public Get(DatabaseFixture fixture)
    {
        this.fixture = fixture;
        fixture.Initialize(GetType());
    }

    [Fact]
    public async Task Get_NoPermissions_ShouldReturnForbidden()
    {
        try
        {
            Department department = await fixture.NewRandomDepartment(saveToDatabase: true);

            string accessToken = Jwt.OfNoPermissions;
            HttpResponseMessage response = await fixture.Http.GetAuthenticatedAsync(CoursesApi.GetUrl(department.Id), accessToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    [Fact]
    public async Task Get_ShouldReturnOk()
    {
        try
        {
            Department department = await fixture.NewRandomDepartment(saveToDatabase: true);
            Course course1 = await fixture.NewRandomCourse(department, saveToDatabase: true);
            Course course2 = await fixture.NewRandomCourse(department, saveToDatabase: true);

            string accessToken = Jwt.Of(CoursesPermissions.View);
            HttpResponseMessage response = await fixture.Http.GetAuthenticatedAsync(CoursesApi.GetUrl(department.Id), accessToken);

            List<Course>? courses = await response.Content.ReadFromJsonAsync<List<Course>>()!;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(courses);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }
}
