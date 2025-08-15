using Microsoft.AspNetCore.Mvc;

using FluentValidation;
using FluentValidation.Results;

using Api.Common.Types;
using Api.Database.Models;
using Api.Common.Extensions;

namespace Api.Features.Departments.Update;

public class Handler
{
    public static async Task<IResult> HandleAsync(
        [FromRoute] long departmentId,
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
            logger.LogWarning("[Departments/Update] Update department with ID {departmentId} request validation failed. Reason: {ErrorMessage}", departmentId, validationResult.Errors[0].ErrorMessage);
            return validationResult.AsHttpError();
        }

        Result<UpdatedDepartment> result = await service.UpdateAsync(departmentId, request, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("[Departments/Update] Update department with ID {departmentId} request failed. Reason: {Message}", departmentId, result.Error?.Message ?? "No message provided");
            return result.Error.AsHttpError();
        }
        
        logger.LogInformation("[Departments/Update] Update department with ID {departmentId} request completed successfully.", departmentId);
        return result.AsHttpResponse();
    }
}
