using FluentValidation;

using Api.Common;
using Api.Common.Constants;
using Api.Common.Extensions;
using Api.Common.Types;

namespace Api.Features.Auth;

public class Registry : IRegistry
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapPost(Api.Google.Url, Google.Handler.HandleAsync)
            .WithDescription(Api.Google.Description)
            .WithTags(Api.Tag)
            .Produces<GeneratedTokens>()
            .ProducesValidationProblem()
            .ProducesProblems(
                StatusCodes.Status400BadRequest,
                StatusCodes.Status422UnprocessableEntity
            );

        app.MapPost(Api.GoogleWeb.Url, Google.Handler.HandleWebAsync)
            .WithDescription(Api.GoogleWeb.Description)
            .WithTags(Api.Tag)
            .Produces<OnlyAccessToken>()
            .ProducesValidationProblem()
            .ProducesProblems(
                StatusCodes.Status400BadRequest,
                StatusCodes.Status422UnprocessableEntity
            );

        app.MapPost(Api.RefreshTokens.Url, RefreshTokens.Handler.HandleAsync)
            .WithDescription(Api.RefreshTokens.Description)
            .WithTags(Api.Tag)
            .Produces<GeneratedTokens>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(Policy.AllowExpiredJwt.Name);

        app.MapPost(Api.RefreshTokensWeb.Url, RefreshTokens.Handler.HandleWebAsync)
            .WithDescription(Api.RefreshTokensWeb.Description)
            .WithTags(Api.Tag)
            .Produces<OnlyAccessToken>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(Policy.AllowExpiredJwt.Name);

        app.MapDelete(Api.Logout.Url, Logout.Handler.HandleAsync)
            .WithDescription(Api.Logout.Description)
            .WithTags(Api.Tag)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(Policy.AllowExpiredJwt.Name);

        app.MapDelete(Api.LogoutWeb.Url, Logout.Handler.HandleWebAsync)
            .WithDescription(Api.LogoutWeb.Description)
            .WithTags(Api.Tag)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(Policy.AllowExpiredJwt.Name);

        app.MapDelete(Api.LogoutAll.Url, LogoutAll.Handler.HandleAsync)
            .WithDescription(Api.LogoutAll.Description)
            .WithTags(Api.Tag)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(Policy.AllowExpiredJwt.Name);

        app.MapDelete(Api.LogoutAllWeb.Url, LogoutAll.Handler.HandleWebAsync)
            .WithDescription(Api.LogoutAllWeb.Description)
            .WithTags(Api.Tag)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(Policy.AllowExpiredJwt.Name);
    }

    public void AddServices(IServiceCollection services)
    {
        services.AddScoped<IValidator<Google.Request>, Google.RequestValidator>();
        services.AddScoped<IValidator<Google.RequestWeb>, Google.RequestWebValidator>();
        services.AddScoped<Google.Service>();

        services.AddScoped<IValidator<RefreshTokens.Request>, RefreshTokens.RequestValidator>();
        services.AddScoped<RefreshTokens.Service>();
    }
}
