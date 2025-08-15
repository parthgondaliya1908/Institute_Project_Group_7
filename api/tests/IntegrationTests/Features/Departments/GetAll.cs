using Api.Tests.IntegrationTests.Extensions;
using DepartmentsApi = Api.Features.Departments.Api;
using DepartmentsPermissions = Api.Features.Departments.Permissions;

namespace Api.Tests.IntegrationTests.Features.Departments;

public class GetAll : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public GetAll(DatabaseFixture fixture)
    {
        this.fixture = fixture;
        fixture.Initialize(GetType());
    }

    [Fact]
    public async Task GetAll_NoPermissions_ShouldReturnForbidden()
    {
        try
        {
            string accessToken = Jwt.OfNoPermissions;
            HttpResponseMessage response = await fixture.Http.GetAuthenticatedAsync(DepartmentsApi.GetAll.Url, accessToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        try
        {
            await fixture.NewRandomDepartment(saveToDatabase: true);
            await fixture.NewRandomDepartment(saveToDatabase: true);

            string accessToken = Jwt.Of(DepartmentsPermissions.View);
            HttpResponseMessage response = await fixture.Http.GetAuthenticatedAsync(DepartmentsApi.GetAll.Url, accessToken);
            List<Department>? departments = await response.Content.ReadFromJsonAsync<List<Department>>()!;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(departments);
            Assert.Equal(2, departments.Count);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }
}