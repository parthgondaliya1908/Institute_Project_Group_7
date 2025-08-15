using Microsoft.AspNetCore.Mvc;

using Api.Common.Extensions;
using Api.Common.Types;

namespace Api.Features.Departments.Delete;

public class Handler
{
    public static async Task<IResult> HandleAsync(
        [FromRoute] long departmentId,
        [FromServices] Service service,
        ILogger<Handler> logger,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("[Departments/Delete] Delete department with ID '{departmentId}' request received", departmentId);

        Result result = await service.DeleteAsync(departmentId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("[Departments/Delete] Delete department with ID '{departmentId}' request failed. Reason: {Message}", departmentId, result.Error?.Message ?? "No message provided");
            return result.Error.AsHttpError();
        }

        logger.LogInformation("[Departments/Delete] Delete department with ID '{departmentId}' request completed successfully.", departmentId);
        return result.AsHttpResponse();
    }
}
