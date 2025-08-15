using Api.Tests.IntegrationTests.Extensions;
using DepartmentsApi = Api.Features.Departments.Api;
using DepartmentsPermissions = Api.Features.Departments.Permissions;

namespace Api.Tests.IntegrationTests.Features.Departments;

public class GetById : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public GetById(DatabaseFixture fixture)
    {
        this.fixture = fixture;
        fixture.Initialize(GetType());
    }

    [Fact]
    public async Task GetById_NoPermissions_ShouldReturnForbidden()
    {
        try
        {
            string accessToken = Jwt.OfNoPermissions;
            HttpResponseMessage response = await fixture.Http.GetAuthenticatedAsync(DepartmentsApi.GetByIdUrl(1), accessToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    [Fact]
    public async Task GetById_ShouldReturnOk()
    {
        try
        {
            Department department = await fixture.NewRandomDepartment(saveToDatabase: true);

            string accessToken = Jwt.Of(DepartmentsPermissions.View);
            HttpResponseMessage response = await fixture.Http.GetAuthenticatedAsync(DepartmentsApi.GetByIdUrl(department.Id), accessToken);
            Department? fetchedDepartment = await response.Content.ReadFromJsonAsync<Department>()!;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(fetchedDepartment);
            Assert.Equal(department.Id, fetchedDepartment.Id);
            Assert.Equal(department.Name, fetchedDepartment.Name);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }
}