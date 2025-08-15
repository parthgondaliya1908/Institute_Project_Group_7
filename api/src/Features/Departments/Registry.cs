using FluentValidation;

using Api.Common;
using Api.Common.Extensions;

namespace Api.Features.Departments;

public class Registry : IRegistry
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGet(Api.GetAll.Url, GetAll.Handler.HandleAsync)
            .WithDescription(Api.GetAll.Description)
            .WithTags(Api.Tag)
            .Produces<List<GetAll.Projections.Department>>()
            .RequireAuthorization(Permissions.View.Name);

        app.MapGet(Api.GetById.Url, GetById.Handler.HandleAsync)
            .WithDescription(Api.GetById.Description)
            .WithTags(Api.Tag)
            .Produces<GetAll.Projections.Department>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(Permissions.View.Name);

        app.MapPost(Api.Add.Url, Add.Handler.HandleAsync)
            .WithDescription(Api.Add.Description)
            .WithTags(Api.Tag)
            .Produces<Add.AddedDepartment>()
            .ProducesValidationProblem()
            .ProducesProblems(StatusCodes.Status409Conflict)
            .RequireAuthorization(Permissions.Add.Name);

        app.MapPut(Api.Update.Url, Update.Handler.HandleAsync)
            .WithDescription(Api.Update.Description)
            .WithTags(Api.Tag)
            .Produces<Update.UpdatedDepartment>()
            .ProducesValidationProblem()
            .ProducesProblems(
                StatusCodes.Status404NotFound,
                StatusCodes.Status409Conflict
            )
            .RequireAuthorization(Permissions.Update.Name);

        app.MapDelete(Api.Delete.Url, Delete.Handler.HandleAsync)
            .WithDescription(Api.Delete.Description)
            .WithTags(Api.Tag)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(Permissions.Delete.Name);
    }

    public void AddServices(IServiceCollection services)
    {
        services.AddScoped<GetAll.IRepository, GetAll.Repository>();
        services.AddScoped<GetAll.Service>();

        services.AddScoped<GetById.IRepository, GetById.Repository>();
        services.AddScoped<GetById.Service>();

        services.AddScoped<IValidator<Add.Request>, Add.RequestValidator>();
        services.AddScoped<Add.IRepository, Add.Repository>();
        services.AddScoped<Add.Service>();

        services.AddScoped<IValidator<Update.Request>, Update.RequestValidator>();
        services.AddScoped<Update.IRepository, Update.Repository>();
        services.AddScoped<Update.Service>();

        services.AddScoped<Delete.IRepository, Delete.Repository>();
        services.AddScoped<Delete.Service>();
    }
}
