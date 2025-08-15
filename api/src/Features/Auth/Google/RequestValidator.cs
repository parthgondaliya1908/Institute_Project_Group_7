using FluentValidation;

namespace Api.Features.Auth.Google;

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.GoogleToken)
            .NotEmpty();
    }
}

public class RequestWebValidator : AbstractValidator<RequestWeb>
{
    public RequestWebValidator()
    {
        RuleFor(x => x.GoogleToken)
            .NotEmpty();
    }
}
