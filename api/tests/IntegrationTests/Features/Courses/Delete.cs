using Api.Features.Courses.Delete;
using Api.Tests.IntegrationTests.Extensions;
using Api.Tests.IntegrationTests.Features.Departments;
using CoursesApi = Api.Features.Courses.Api;
using CoursesPermissions = Api.Features.Courses.Permissions;

namespace Api.Tests.IntegrationTests.Features.Courses;

public class Delete : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public Delete(DatabaseFixture fixture)
    {
        this.fixture = fixture;
        fixture.Initialize(GetType());
    }

    [Fact]
    public async Task Delete_NoPermissions_ShouldReturnForbidden()
    {
        try
        {
            string accessToken = Jwt.OfNoPermissions;
            HttpResponseMessage response = await fixture.Http.DeleteAuthenticatedAsync(CoursesApi.DeleteUrl(1), accessToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenIdDoesNotExist()
    {
        try
        {
            string accessToken = Jwt.Of(CoursesPermissions.Delete);
            HttpResponseMessage response = await fixture.Http.DeleteAuthenticatedAsync(CoursesApi.DeleteUrl(100), accessToken);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenIdIsDeleted()
    {
        try
        {
            Department department = await fixture.NewRandomDepartment(saveToDatabase: true);
            Course course = await fixture.NewRandomCourse(department, isDeleted: true, saveToDatabase: true);

            string accessToken = Jwt.Of(CoursesPermissions.Delete);
            HttpResponseMessage response = await fixture.Http.DeleteAuthenticatedAsync(CoursesApi.DeleteUrl(course.Id), accessToken);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    [Fact]
    public async Task Delete_ShouldReturnOk()
    {
        try
        {
            Department department = await fixture.NewRandomDepartment(saveToDatabase: true);
            Course course = await fixture.NewRandomCourse(department, saveToDatabase: true);

            string accessToken = Jwt.Of(CoursesPermissions.Delete);
            HttpResponseMessage response = await fixture.Http.DeleteAuthenticatedAsync(CoursesApi.DeleteUrl(course.Id), accessToken);
            Course? deletedCourse = await fixture.GetCourse(course.Id);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(deletedCourse);
            Assert.True(deletedCourse.IsDeleted);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }
}
