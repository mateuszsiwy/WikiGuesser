namespace WikiGuesser.Server.DTOs;

public class MessageDTO
{
    public Guid MessageId { get; set; }
    public string MessageText { get; set; }
    public string SenderUsername { get; set; }
    public DateTime CreatedAt { get; set; }
}