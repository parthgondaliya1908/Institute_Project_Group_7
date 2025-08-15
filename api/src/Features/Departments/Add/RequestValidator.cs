using FluentValidation;

namespace Api.Features.Departments.Add;

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
