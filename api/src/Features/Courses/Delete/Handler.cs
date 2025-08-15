using Microsoft.AspNetCore.Mvc;

using Api.Common.Extensions;
using Api.Common.Types;

namespace Api.Features.Courses.Delete;

public class Handler
{
    public static async Task<IResult> HandleAsync(
        [FromRoute] long courseId,
        [FromServices] Service service,
        ILogger<Handler> logger,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("[Courses/Delete] Delete course with ID '{courseId}' request received", courseId);

        Result result = await service.DeleteAsync(courseId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("[Courses/Delete] Delete course with ID '{courseId}' request failed. Reason: {Message}", courseId, result.Error?.Message ?? "No message provided");
            return result.Error.AsHttpError();
        }

        logger.LogInformation("[Courses/Delete] Delete course with ID '{courseId}' request completed successfully.", courseId);
        return result.AsHttpResponse();
    }
}
