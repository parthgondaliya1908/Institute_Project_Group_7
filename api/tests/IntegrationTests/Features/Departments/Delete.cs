using Api.Tests.IntegrationTests.Extensions;
using DepartmentsApi = Api.Features.Departments.Api;
using DepartmentsPermissions = Api.Features.Departments.Permissions;

namespace Api.Tests.IntegrationTests.Features.Departments;

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
            HttpResponseMessage response = await fixture.Http.DeleteAuthenticatedAsync(DepartmentsApi.DeleteUrl(1), accessToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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
            Department department = await fixture.NewRandomDepartment(isDeleted: true, saveToDatabase: true);

            string accessToken = Jwt.Of(DepartmentsPermissions.Delete);
            HttpResponseMessage response = await fixture.Http.DeleteAuthenticatedAsync(DepartmentsApi.DeleteUrl(department.Id), accessToken);

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

            string accessToken = Jwt.Of(DepartmentsPermissions.Delete);
            HttpResponseMessage response = await fixture.Http.DeleteAuthenticatedAsync(DepartmentsApi.DeleteUrl(department.Id), accessToken);

            Department? deletedDepartment = await fixture.GetDepartment(department.Id);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(deletedDepartment);
            Assert.True(deletedDepartment.IsDeleted);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }
}
