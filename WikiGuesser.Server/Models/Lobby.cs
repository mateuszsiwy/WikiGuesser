namespace WikiGuesser.Server.Models;

public class Lobby
{
    public Guid LobbyId { get; set; }
    public string Name { get; set; }
    public string OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public GameState GameState { get; set; }   
    public ICollection<Player> Players { get; set; }
    public Chat Chat { get; set; }
    public Guid ChatId { get; set; }
}