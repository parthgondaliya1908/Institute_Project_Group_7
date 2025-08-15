namespace Api.Common;

public interface IRegistry
{
    void MapEndpoints(WebApplication app);
    void AddServices(IServiceCollection services) { }
}
