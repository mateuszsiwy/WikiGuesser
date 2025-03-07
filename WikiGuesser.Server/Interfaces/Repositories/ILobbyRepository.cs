using Microsoft.AspNetCore.Identity;
using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Interfaces.Repositories;

public interface ILobbyRepository
{
    Task<List<Lobby>> GetLobbiesAsync();
    Task<Lobby> GetLobbyAsync(Guid id);
    Task<Lobby> AddLobbyAsync(Lobby lobby);
    Task<Player> AddPlayerAsync(Player player);
    Task<Lobby> UpdateLobbyAsync(Lobby lobby);
    Task<Player> GetPlayerAsync(Guid playerId);
    Task<Player> GetPlayerByUserIdAsync(string userId);
    Task RemovePlayerAsync(Player player);
    Task<IdentityUser> GetUserAsync(string userId);
    Task<Chat> AddChatAsync(Chat chat);
}