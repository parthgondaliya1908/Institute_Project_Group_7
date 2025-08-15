using Microsoft.AspNetCore.Mvc;

using Api.Common.Extensions;
using Api.Common.Types;

namespace Api.Features.Courses.Get;

public class Handler
{
    public static async Task<IResult> HandleAsync(
        [FromRoute] long departmentId,
        [FromServices] Service service,
        ILogger<Handler> logger,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("[Courses/Get] Get courses for department with ID '{departmentId}' request received", departmentId);

        Result<List<Course>> result = await service.GetAsync(departmentId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("[Courses/Get] Get courses for department with ID '{departmentId}' request failed. Reason: {Message}", departmentId, result.Error?.Message ?? "No message provided");
            return result.Error.AsHttpError();
        }
        
        logger.LogInformation("[Courses/Get] Get courses for department with ID '{departmentId}' request completed successfully.", departmentId);
        return result.AsHttpResponse();
    }
}
