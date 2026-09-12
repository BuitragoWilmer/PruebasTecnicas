using Domain.Users;
using MediatR;
using Domain.Primitives;

namespace Application.Users.Create;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ErrorOr<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ErrorOr<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {

        string normalizedEmail = request.Email.Trim();
        var exists = await _userRepository.ExistsByEmailAsync(normalizedEmail);

        if (exists)
            throw new InvalidOperationException("Ya existe un usuario con este correo.");

        User user = new User(
            request.FullName.Trim(),
            normalizedEmail,
            DateTime.UtcNow);

        await _userRepository.AddAsync(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserResponse(user.UserId, user.FullName, user.Email);
    }
}

