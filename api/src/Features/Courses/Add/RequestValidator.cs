using FluentValidation;

namespace Api.Features.Courses.Add;

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .GreaterThan(0);
    }
}
