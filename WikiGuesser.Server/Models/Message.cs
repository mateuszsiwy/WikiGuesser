using Microsoft.AspNetCore.Identity;

namespace WikiGuesser.Server.Models;

public class Message
{
    public Guid MessageId { get; set; }
    public Guid ChatId { get; set; }
    public string SenderId { get; set; }
    public string MessageText { get; set; }
    public DateTime CreatedAt { get; set; }
    public Chat Chat { get; set; }
    public IdentityUser Sender { get; set; }
}