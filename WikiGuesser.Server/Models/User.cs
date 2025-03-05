namespace WikiGuesser.Server.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Message> Messages { get; set; }
}