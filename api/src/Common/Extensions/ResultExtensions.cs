using FluentValidation.Results;

using Api.Common.Types;

namespace Api.Common.Extensions;

public static class ResultExtensions
{
    public static IResult AsHttpResponse(this Result result)
    {
        if (!result.IsSuccess)
        {
            return result.Error.AsHttpError();
        }

        return TypedResults.Ok();
    }

    public static IResult AsHttpResponse<T>(this Result<T> result) where T : class
    {
        if (!result.IsSuccess)
        {
            return result.Error.AsHttpError();
        }

        return result.SuccessStatus switch
        {
            SuccessStatus.Created => TypedResults.Created(result.Url, result.Value),
            _ => TypedResults.Ok(result.Value),
        };
    }

    public static IResult AsHttpError(this ValidationResult validationResult) => TypedResults.ValidationProblem(validationResult.ToDictionary());

    public static IResult AsHttpError(this Error? error)
    {
        return error switch
        {
            UnauthorizedError err => TypedResults.Problem(statusCode: StatusCodes.Status401Unauthorized, detail: err.Message),
            ConflictError err => TypedResults.Problem(statusCode: StatusCodes.Status409Conflict, detail: err.Message),
            NotFoundError err => TypedResults.Problem(statusCode: StatusCodes.Status404NotFound, detail: err.Message),
            CannotProcessError err => TypedResults.Problem(statusCode: StatusCodes.Status422UnprocessableEntity, detail: err.Message),
            BadRequestError err => TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest, detail: err.Message),

            Error err => TypedResults.Problem(err.Message),
            _ => TypedResults.Problem()
        };
    }
}
