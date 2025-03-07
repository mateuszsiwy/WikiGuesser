using Microsoft.AspNetCore.Identity;

namespace WikiGuesser.Server.Models;

public class Player
{
    public Guid PlayerId { get; set; }
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
    public Guid LobbyId { get; set; }
    public Lobby Lobby { get; set; }
    public bool IsReady { get; set; }
    public int Score { get; set; }
}