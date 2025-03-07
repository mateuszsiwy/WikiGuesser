﻿using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Interfaces.Repositories;

public interface ILobbyRepository
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
}