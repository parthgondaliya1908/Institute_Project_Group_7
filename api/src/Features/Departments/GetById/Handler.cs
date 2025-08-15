using Microsoft.AspNetCore.Mvc;

using Api.Common.Extensions;
using Api.Common.Types;

namespace Api.Features.Departments.GetById;

public class Handler
{
    public static async Task<IResult> HandleAsync(
        [FromRoute] long departmentId,
        [FromServices] Service service,
        ILogger<Handler> logger,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("[Departments/GetById] Get department with ID '{departmentId}' request received", departmentId);
        Result<Projections.Department> result = await service.GetByIdAsync(departmentId, cancellationToken);

        logger.LogInformation("[Departments/GetById] Get department with ID '{departmentId}' request completed successfully.", departmentId);
        return result.AsHttpResponse();
    }
}
