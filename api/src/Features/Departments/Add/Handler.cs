using Microsoft.AspNetCore.Mvc;

using FluentValidation;
using FluentValidation.Results;

using Api.Common.Types;
using Api.Common.Extensions;
using System.Security.Claims;

namespace Api.Features.Departments.Add;

public class Handler
{
    public static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
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
            logger.LogWarning("[Departments/Add] Adding department request validation failed. Reason: {ErrorMessage}", validationResult.Errors[0].ErrorMessage);
            return validationResult.AsHttpError();
        }

        Result<AddedDepartment> result = await service.AddAsync(request, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("[Departments/Add] Adding department request failed. Reason: {Message}", result.Error?.Message ?? "No message provided");
            return result.Error.AsHttpError();
        }
        
        logger.LogInformation("[Departments/Add] Adding department request completed successfully.");
        return result.AsHttpResponse();
    }
}
