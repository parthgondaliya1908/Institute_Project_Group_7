using Api.Features.Courses.Update;
using Api.Tests.IntegrationTests.Extensions;
using Api.Tests.IntegrationTests.Features.Departments;
using CoursesApi = Api.Features.Courses.Api;
using CoursesPermissions = Api.Features.Courses.Permissions;

namespace Api.Tests.IntegrationTests.Features.Courses;

public class Update : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public Update(DatabaseFixture fixture)
    {
        this.fixture = fixture;
        fixture.Initialize(GetType());
    }

    [Fact]
    public async Task Update_NoPermissions_ShouldReturnForbidden()
    {
        try
        {
            Request request = Shared.NewRandomUpdateRequest(departmentId: 100);

            string accessToken = Jwt.OfNoPermissions;
            HttpResponseMessage forbiddenResponse = await fixture.Http.PutAsJsonAuthenticatedAsync(CoursesApi.UpdateUrl(1), request, accessToken);

            Assert.Equal(HttpStatusCode.Forbidden, forbiddenResponse.StatusCode);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenIdDoesNotExist()
    {
        try
        {
            Request request = Shared.NewRandomUpdateRequest(departmentId: 100);

            string accessToken = Jwt.Of(CoursesPermissions.Update);
            HttpResponseMessage forbiddenResponse = await fixture.Http.PutAsJsonAuthenticatedAsync(CoursesApi.UpdateUrl(100), request, accessToken);

            Assert.Equal(HttpStatusCode.NotFound, forbiddenResponse.StatusCode);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    [Fact]
    public async Task Update_ShouldReturnConflict_WhenNameIsNotUnique()
    {
        try
        {
            Department department = await fixture.NewRandomDepartment(saveToDatabase: true);
            Course course1 = await fixture.NewRandomCourse(department, saveToDatabase: true);
            Course course2 = await fixture.NewRandomCourse(department, saveToDatabase: true);

            Request request = Shared.NewUpdateRequest(department.Id, course1.Name);

            string accessToken = Jwt.Of(CoursesPermissions.Update);
            HttpResponseMessage conflictResponse = await fixture.Http.PutAsJsonAuthenticatedAsync(CoursesApi.UpdateUrl(course2.Id), request, accessToken);

            Assert.Equal(HttpStatusCode.Conflict, conflictResponse.StatusCode);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenNameIsUnique()
    {
        try
        {
            Department department = await fixture.NewRandomDepartment(saveToDatabase: true);
            Course course = await fixture.NewRandomCourse(department, saveToDatabase: true);
            Request request = Shared.NewRandomUpdateRequest(department.Id);

            string accessToken = Jwt.Of(CoursesPermissions.Update);
            HttpResponseMessage okResponse = await fixture.Http.PutAsJsonAuthenticatedAsync(CoursesApi.UpdateUrl(course.Id), request, accessToken);
            UpdatedCourse? updatedCourse = await okResponse.Content.ReadFromJsonAsync<UpdatedCourse>();

            Assert.Equal(HttpStatusCode.OK, okResponse.StatusCode);
            Assert.NotNull(updatedCourse);
            Assert.Equal(course.Id, updatedCourse.Id);
            Assert.Equal(request.Name, updatedCourse.Name);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }
}
