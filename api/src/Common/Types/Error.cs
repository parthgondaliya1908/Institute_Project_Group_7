namespace Api.Common.Types;

public record Error(string? Message = null);

public record UnauthorizedError(string? Message = null) : Error(Message);
public record ConflictError(string? Message = null) : Error(Message);
public record NotFoundError(string? Message = null) : Error(Message);
public record CannotProcessError(string? Message = null) : Error(Message);
public record BadRequestError(string? Message = null) : Error(Message);
