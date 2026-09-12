using Application.Common.Repositories;
using Domain.Users;

namespace Application.Users;

public interface IUserRepository : IRepository<User, int>
{
	Task<bool> ExistsByEmailAsync(string email);

}
