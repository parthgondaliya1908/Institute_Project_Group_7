using Api.Features.Departments.Update;
using Api.Tests.IntegrationTests.Extensions;
using DepartmentsApi = Api.Features.Departments.Api;
using DepartmentsPermissions = Api.Features.Departments.Permissions;

namespace Api.Tests.IntegrationTests.Features.Departments;

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
            Request request = Shared.NewRandomUpdateRequest();

            string accessToken = Jwt.OfNoPermissions;
            HttpResponseMessage forbiddenResponse = await fixture.Http.PutAsJsonAuthenticatedAsync(DepartmentsApi.UpdateUrl(departmentId: 100), request, accessToken);

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
            Request request = Shared.NewRandomUpdateRequest();

            string accessToken = Jwt.Of(DepartmentsPermissions.Update);
            HttpResponseMessage notFoundResponse = await fixture.Http.PutAsJsonAuthenticatedAsync(DepartmentsApi.UpdateUrl(departmentId: 100), request, accessToken);

            Assert.Equal(HttpStatusCode.NotFound, notFoundResponse.StatusCode);
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
            Department department1 = await fixture.NewRandomDepartment(saveToDatabase: true);
            Department department2 = await fixture.NewRandomDepartment(saveToDatabase: true);
            Request request = Shared.NewUpdateRequest(department2.Name);

            string accessToken = Jwt.Of(DepartmentsPermissions.Update);
            HttpResponseMessage conflictResponse = await fixture.Http.PutAsJsonAuthenticatedAsync(DepartmentsApi.UpdateUrl(department1.Id), request, accessToken);

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
            Request request = Shared.NewRandomUpdateRequest();

            string accessToken = Jwt.Of(DepartmentsPermissions.Update);
            HttpResponseMessage okResponse = await fixture.Http.PutAsJsonAuthenticatedAsync(DepartmentsApi.UpdateUrl(department.Id), request, accessToken);
            UpdatedDepartment? updatedDepartment = await okResponse.Content.ReadFromJsonAsync<UpdatedDepartment>();

            Assert.Equal(HttpStatusCode.OK, okResponse.StatusCode);
            Assert.NotNull(updatedDepartment);
            Assert.Equal(department.Id, updatedDepartment.Id);
            Assert.Equal(request.Name, updatedDepartment.Name);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }
}
