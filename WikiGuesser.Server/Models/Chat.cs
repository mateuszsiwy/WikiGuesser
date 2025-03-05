namespace WikiGuesser.Server.Models;

public class Chat
{
    public Guid ChatId { get; set; }
    public string ChatName { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Message> Messages { get; set; }
}