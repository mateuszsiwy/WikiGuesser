using WikiGuesser.Server.DTOs;
using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Interfaces.Services;

public interface ILobbyService
{
    Task<List<Lobby>> GetLobbies();
    Task<Lobby> GetLobby(Guid id);
    Task<Lobby> CreateLobby(string name, string ownerId);
    Task<Lobby> JoinLobby(Guid lobbyId, string userId);
    Task<Lobby> LeaveLobby(Guid lobbyId, string userId);
    Task<Lobby> SetReadyStatus(Guid lobbyId, string userId, bool ready);
    Task<Lobby> StartGame(Guid lobbyId);
    Task<Lobby> EndGame(Guid lobbyId);
    Task<Lobby> UpdateScore(Guid lobbyId, string userId, int score);
    Task UpdatePlayerScores(Guid lobbyId, List<PlayerScoreDTO> playerScores);
    Task SetCurrentArticle(Guid lobbyId, WikipediaArticle article);
    Task<WikipediaArticleDTO> GetCurrentGameState(Guid lobbyId);
}