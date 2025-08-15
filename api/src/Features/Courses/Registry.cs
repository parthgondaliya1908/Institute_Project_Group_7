using FluentValidation;

using Api.Common;
using Api.Common.Extensions;

namespace Api.Features.Courses;

public class Registry : IRegistry
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGet(Api.Get.Url, Get.Handler.HandleAsync)
            .WithDescription(Api.Get.Description)
            .WithTags(Api.Tag)
            .ProducesProblems(StatusCodes.Status404NotFound)
            .RequireAuthorization(Permissions.View.Name);

        app.MapPost(Api.Add.Url, Add.Handler.HandleAsync)
            .WithDescription(Api.Add.Description)
            .WithTags(Api.Tag)
            .Produces<Add.AddedCourse>()
            .ProducesValidationProblem()
            .ProducesProblems(
                StatusCodes.Status409Conflict,
                StatusCodes.Status404NotFound
            )
            .RequireAuthorization(Permissions.Add.Name);

        app.MapPut(Api.Update.Url, Update.Handler.HandleAsync)
            .WithDescription(Api.Update.Description)
            .WithTags(Api.Tag)
            .Produces<Update.UpdatedCourse>()
            .ProducesValidationProblem()
            .ProducesProblems(
                StatusCodes.Status409Conflict,
                StatusCodes.Status404NotFound
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
        services.AddScoped<Get.IRepository, Get.Repository>();
        services.AddScoped<Get.Service>();

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
