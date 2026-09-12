namespace Domain.Users;

public sealed class User : AggregateRoot
{
    private User()
    {
    }

    public User(string fullName, string email, DateTime createdAt)
    {
        FullName = fullName;
        Email = email;
        CreatedAt = createdAt;
    }

    public  int UserId { get; set; }
	public string FullName { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
