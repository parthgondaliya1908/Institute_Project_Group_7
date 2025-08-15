using Microsoft.AspNetCore.Mvc;

using Api.Common.Extensions;
using Api.Common.Types;

namespace Api.Features.Departments.GetAll;

public class Handler
{
    public static async Task<IResult> HandleAsync(
        [FromServices] Service service,
        ILogger<Handler> logger,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("[Departments/GetAll] Getting all departments request received");
        Result<List<Projections.Department>> result = await service.GetAllAsync(cancellationToken);

        logger.LogInformation("[Departments/GetAll] Getting all departments request completed successfully.");
        return result.AsHttpResponse();
    }
}
