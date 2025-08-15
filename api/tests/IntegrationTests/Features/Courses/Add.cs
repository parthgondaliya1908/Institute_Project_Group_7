using Api.Features.Courses.Add;
using Api.Tests.IntegrationTests.Extensions;
using Api.Tests.IntegrationTests.Features.Departments;
using CoursesApi = Api.Features.Courses.Api;
using CoursesPermissions = Api.Features.Courses.Permissions;

namespace Api.Tests.IntegrationTests.Features.Courses;

public class Add : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public Add(DatabaseFixture fixture)
    {
        this.fixture = fixture;
        fixture.Initialize(GetType());
    }

    [Fact]
    public async Task Add_NoPermissions_ShouldReturnForbidden()
    {
        try
        {
            Request request = Shared.NewRandomAddRequest(departmentId: 1);

            string accessToken = Jwt.OfNoPermissions;
            HttpResponseMessage response = await fixture.Http.PostAsJsonAuthenticatedAsync(CoursesApi.Add.Url, request, accessToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    [Fact]
    public async Task Add_ShouldReturnConflict_WhenNameIsNotUnique()
    {
        try
        {
            Department department = await fixture.NewRandomDepartment(saveToDatabase: true);
            Course course = await fixture.NewRandomCourse(department, saveToDatabase: true);
            Request request = Shared.NewAddRequest(department.Id, course.Name);

            string accessToken = Jwt.Of(CoursesPermissions.Add);
            HttpResponseMessage conflictResponse = await fixture.Http.PostAsJsonAuthenticatedAsync(CoursesApi.Add.Url, request, accessToken);

            Assert.Equal(HttpStatusCode.Conflict, conflictResponse.StatusCode);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    [Fact]
    public async Task Add_ShouldReturnCreated_WhenNameConflictsWithADeletedCourse()
    {
        try
        {
            Department department = await fixture.NewRandomDepartment(saveToDatabase: true);
            Course course = await fixture.NewRandomCourse(department, saveToDatabase: true, isDeleted: true);
            Request request = Shared.NewAddRequest(department.Id, course.Name);

            string accessToken = Jwt.Of(CoursesPermissions.Add);
            HttpResponseMessage createdResponse = await fixture.Http.PostAsJsonAuthenticatedAsync(CoursesApi.Add.Url, request, accessToken);

            Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    [Fact]
    public async Task Add_ShouldReturnCreated_WhenNameIsUnique()
    {
        try
        {
            Department department = await fixture.NewRandomDepartment(saveToDatabase: true);
            Request request = Shared.NewRandomAddRequest(department.Id);

            string accessToken = Jwt.Of(CoursesPermissions.Add);
            HttpResponseMessage createdResponse = await fixture.Http.PostAsJsonAuthenticatedAsync(CoursesApi.Add.Url, request, accessToken);
            AddedCourse? addedCourse = await createdResponse.Content.ReadFromJsonAsync<AddedCourse>();

            Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
            Assert.NotNull(addedCourse);
            Assert.Equal(request.Name, addedCourse.Name);
            Assert.Equal(department.Id, addedCourse.Department.Id);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }
}
