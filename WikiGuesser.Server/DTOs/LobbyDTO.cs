using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.DTOs;

public class LobbyDTO
{
    public Guid LobbyId { get; set; }
    public string Name { get; set; }
    public string OwnerId { get; set; }
    public bool IsActive { get; set; }
    public GameState GameState { get; set; }
    public Guid ChatId { get; set; }
    public List<PlayerDTO> Players { get; set; } = new List<PlayerDTO>();
}