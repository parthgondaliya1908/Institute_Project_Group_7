namespace Api.Features.Auth.Google;

public class Request
{
    public string GoogleToken { get; set; } = null!;
    public string? SessionId { get; set; }
}

public class RequestWeb
{
    public string GoogleToken { get; set; } = null!;
}
