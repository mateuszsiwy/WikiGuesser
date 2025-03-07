using WikiGuesser.Server.Interfaces.Services;
using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Services;

public class LobbyService : ILobbyService
{
    public Task<List<Lobby>> GetLobbies()
    {
        throw new NotImplementedException();
    }

    public Task<Lobby> GetLobby(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Lobby> CreateLobby(string name, string ownerId)
    {
        throw new NotImplementedException();
    }

    public Task<Lobby> JoinLobby(Guid lobbyId, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<Lobby> LeaveLobby(Guid lobbyId, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<Lobby> SetReadyStatus(Guid lobbyId, string userId, bool ready)
    {
        throw new NotImplementedException();
    }

    public Task<Lobby> StartGame(Guid lobbyId)
    {
        throw new NotImplementedException();
    }

    public Task<Lobby> EndGame(Guid lobbyId)
    {
        throw new NotImplementedException();
    }

    public Task<Lobby> UpdateScore(Guid lobbyId, string userId, int score)
    {
        throw new NotImplementedException();
    }
}