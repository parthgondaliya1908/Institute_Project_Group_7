using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using FluentValidation;
using FluentValidation.Results;

using Api.Common.Constants;
using Api.Common.Extensions;
using Api.Common.Options;
using Api.Common.Services;
using Api.Common.Types;

namespace Api.Features.Auth.Google;

public class Handler
{
    public static async Task<IResult> HandleAsync(
        [FromBody] Request request,
        [FromServices] IValidator<Request> requestValidator,
        [FromServices] ILogger<Handler> logger,
        [FromServices] Service service,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("[Auth/Google] Google login request received");

        ValidationResult validationResult = await requestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("[Auth/Google] Google login request validation failed. Reason: {ErrorMessage}", validationResult.Errors[0].ErrorMessage);
            return validationResult.AsHttpError();
        }

        Result<GeneratedTokens> loginWithGoogleResult = await service.LoginWithGoogleAsync(request.GoogleToken, request.SessionId, cancellationToken);
        if (!loginWithGoogleResult.IsSuccess)
        {
            logger.LogError("[Auth/Google] Google login request failed. Reason: {Message}", loginWithGoogleResult.Error?.Message ?? "No message provided");
            return loginWithGoogleResult.Error.AsHttpError();
        }

        logger.LogInformation("[Auth/Google] Google login request completed successfully.");
        return loginWithGoogleResult.AsHttpResponse();
    }

    public static async Task<IResult> HandleWebAsync(
        HttpRequest httpRequest,
        HttpResponse httpResponse,
        [FromBody] RequestWeb request,
        [FromServices] IValidator<RequestWeb> requestValidator,
        [FromServices] Service service,
        [FromServices] ICookieService cookieService,
        [FromServices] IOptions<JwtOptions> jwtOptions,
        [FromServices] ILogger<Handler> logger,
        CancellationToken cancellationToken
    )
    {
        string? sessionId = httpRequest.Cookies[TokenType.SessionId.Name];

        ValidationResult validationResult = await requestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("[Auth/Google] Google login request validation failed. Reason: {ErrorMessage}", validationResult.Errors[0].ErrorMessage);
            return validationResult.AsHttpError();
        }

        Result<GeneratedTokens> result = await service.LoginWithGoogleAsync(request.GoogleToken, sessionId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("[Auth/Google] Google login request failed. Reason: {Message}", result.Error?.Message ?? "No message provided");
            return result.Error.AsHttpError();
        }

        GeneratedTokens generatedTokens = result.Value!;

        cookieService.AppendCookie(
            httpResponse,
            TokenType.SessionId.Name,
            generatedTokens.SessionId,
            path: Api.BaseUrl,
            DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryInDays)
        );
        
        cookieService.AppendCookie(
            httpResponse,
            TokenType.RefreshToken.Name,
            generatedTokens.RefreshToken,
            Api.RefreshTokensWeb.Url,
            DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryInDays)
        );

        logger.LogInformation("[Auth/Google] Google login request completed successfully.");

        OnlyAccessToken onlyAccessToken = new(generatedTokens.AccessToken);
        Result<OnlyAccessToken> resultToSend = result.SuccessStatus!.Value == SuccessStatus.Created
            ? Result.Created(onlyAccessToken)
            : Result.Success(onlyAccessToken);

        return resultToSend.AsHttpResponse();
    }
}
