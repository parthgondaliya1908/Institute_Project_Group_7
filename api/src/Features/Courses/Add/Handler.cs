using Microsoft.AspNetCore.Mvc;

using FluentValidation;
using FluentValidation.Results;

using Api.Common.Extensions;
using Api.Common.Types;

namespace Api.Features.Courses.Add;

public class Handler
{
    public static async Task<IResult> HandleAsync(
        [FromBody] Request request,
        [FromServices] IValidator<Request> requestValidator,
        [FromServices] Service service,
        ILogger<Handler> logger,
        CancellationToken cancellationToken
    )
    {
        ValidationResult validationResult = await requestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("[Courses/Add] Adding course request validation failed. Reason: {ErrorMessage}", validationResult.Errors[0].ErrorMessage);
            return validationResult.AsHttpError();
        }

        Result<AddedCourse> result = await service.AddAsync(request, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("[Courses/Add] Adding course request failed. Reason: {Message}", result.Error?.Message ?? "No message provided");
            return result.Error.AsHttpError();
        }

        logger.LogInformation("[Courses/Add] Adding course request completed successfully.");
        return result.AsHttpResponse();
    }
}
