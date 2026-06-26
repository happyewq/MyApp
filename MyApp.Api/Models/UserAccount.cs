namespace MyApp.Api.Models;

public class UserAccount
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "user";
    public DateTimeOffset CreatedAt { get; set; }
}
