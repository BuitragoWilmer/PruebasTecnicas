namespace Application.TaskItems.Create;

public sealed class CreateTaskItemCommandValidator : AbstractValidator<CreateTaskItemCommand>
{
    public CreateTaskItemCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.AssignedUserId)
            .GreaterThan(0);

        RuleFor(x => x.StatusId)
            .GreaterThan((byte)0);
    }
}