using Application.Users.Create;

namespace Application.Users.Create;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200)
            .WithName("Full Name");
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithName("Email");
    }
}
