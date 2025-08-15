using Microsoft.AspNetCore.Mvc;

using FluentValidation;
using FluentValidation.Results;

using Api.Common.Extensions;
using Api.Common.Types;

namespace Api.Features.Courses.Update;

public class Handler
{
    public static async Task<IResult> HandleAsync(
        [FromRoute] long courseId,
        [FromBody] Request request,
        [FromServices] IValidator<Request> requestValidator,
        [FromServices] Service serice,
        ILogger<Handler> logger,
        CancellationToken cancellationToken
    )
    {
        ValidationResult validationResult = await requestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("[Courses/Update] Updating course request validation failed. Reason: {ErrorMessage}", validationResult.Errors[0].ErrorMessage);
            return validationResult.AsHttpError();
        }

        Result<UpdatedCourse> result = await serice.UpdateAsync(courseId, request, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("[Courses/Update] Updating course request failed. Reason: {Message}", result.Error?.Message ?? "No message provided");
            return result.Error.AsHttpError();
        }

        logger.LogInformation("[Courses/Update] Updating course request completed successfully.");
        return result.AsHttpResponse();
    }
}
