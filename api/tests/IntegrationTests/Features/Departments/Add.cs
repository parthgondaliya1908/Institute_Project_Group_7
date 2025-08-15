using Api.Features.Departments.Add;
using Api.Tests.IntegrationTests.Extensions;
using DepartmentsApi = Api.Features.Departments.Api;
using DepartmentsPermissions = Api.Features.Departments.Permissions;

namespace Api.Tests.IntegrationTests.Features.Departments;

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
            string accessToken = Jwt.OfNoPermissions;

            Request request = Shared.NewRandomAddRequest();
            HttpResponseMessage response = await fixture.Http.PostAsJsonAuthenticatedAsync(DepartmentsApi.Add.Url, request, accessToken);
            
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
            Request request = Shared.NewAddRequest(department.Name);

            string accessToken = Jwt.Of(DepartmentsPermissions.Add);
            HttpResponseMessage conflictResponse = await fixture.Http.PostAsJsonAuthenticatedAsync(DepartmentsApi.Add.Url, request, accessToken);

            Assert.Equal(HttpStatusCode.Conflict, conflictResponse.StatusCode);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    [Fact]
    public async Task Add_ShouldReturnCreated_WhenNameConflictsWithADeletedDepartment()
    {
        try
        {
            Department department = await fixture.NewRandomDepartment(saveToDatabase: true, isDeleted: true);
            Request request = Shared.NewAddRequest(department.Name);

            string accessToken = Jwt.Of(DepartmentsPermissions.Add);
            HttpResponseMessage createdResponse = await fixture.Http.PostAsJsonAuthenticatedAsync(DepartmentsApi.Add.Url, request, accessToken);

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
            Request request = Shared.NewRandomAddRequest();

            string accessToken = Jwt.Of(DepartmentsPermissions.Add);
            HttpResponseMessage createdResponse = await fixture.Http.PostAsJsonAuthenticatedAsync(DepartmentsApi.Add.Url, request, accessToken);
            AddedDepartment? addedDepartment = await createdResponse.Content.ReadFromJsonAsync<AddedDepartment>();

            Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
            Assert.NotNull(addedDepartment);
            Assert.Equal(request.Name, addedDepartment.Name);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }
}
