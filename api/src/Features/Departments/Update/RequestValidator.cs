using FluentValidation;

namespace Api.Features.Departments.Update;

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
