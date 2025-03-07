﻿using WikiGuesser.Server.Interfaces.Repositories;
using WikiGuesser.Server.Interfaces.Services;
using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Services;

public class LobbyService : ILobbyService
{
    private readonly ILobbyRepository _lobbyRepository;
    private readonly ILogger<LobbyService> _logger;

    public LobbyService(ILobbyRepository lobbyRepository, ILogger<LobbyService> logger)
    {
        _lobbyRepository = lobbyRepository;
        _logger = logger;
    }

    public async Task<List<Lobby>> GetLobbies()
    {
        return await _lobbyRepository.GetLobbiesAsync();
    }

    public async Task<Lobby> GetLobby(Guid id)
    {
        var lobby = await _lobbyRepository.GetLobbyAsync(id);
        if (lobby == null)
        {
            _logger.LogWarning("Lobby with ID {LobbyId} not found", id);
            throw new KeyNotFoundException($"Lobby with ID {id} not found");
        }
        return lobby;
    }

    public async Task<Lobby> CreateLobby(string name, string ownerId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Lobby name cannot be empty", nameof(name));
        }
    
        var user = await _lobbyRepository.GetUserAsync(ownerId);
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found when creating lobby", ownerId);
            throw new KeyNotFoundException($"User with ID {ownerId} not found");
        }
    
        var newChat = new Chat
        {
            ChatId = Guid.NewGuid(),
            ChatName = name,
            CreatedAt = DateTime.Now,
            Messages = new List<Message>()
        };
        await _lobbyRepository.AddChatAsync(newChat);
    
        var newLobby = new Lobby
        {
            LobbyId = Guid.NewGuid(),
            Name = name,
            OwnerId = ownerId,
            IsActive = true,
            GameState = GameState.WaitingForPlayers,
            ChatId = newChat.ChatId,
            Players = new List<Player>()
        };
        
        var savedLobby = await _lobbyRepository.AddLobbyAsync(newLobby);
    
        var newPlayer = new Player
        {
            PlayerId = Guid.NewGuid(),
            UserId = ownerId,
            LobbyId = savedLobby.LobbyId,
            IsReady = false,
            Score = 0
        };
        
        await _lobbyRepository.AddPlayerAsync(newPlayer);
        
        savedLobby.Players.Add(newPlayer);
        
        return await _lobbyRepository.UpdateLobbyAsync(savedLobby);
    }

    public async Task<Lobby> JoinLobby(Guid lobbyId, string userId)
    {
        var lobby = await GetLobby(lobbyId);
        
        var user = await _lobbyRepository.GetUserAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }
        
        if (lobby.Players.Any(p => p.UserId == userId))
        {
            _logger.LogInformation("User {UserId} is already in lobby {LobbyId}", userId, lobbyId);
            return lobby;
        }

        if (lobby.GameState != GameState.WaitingForPlayers)
        {
            throw new InvalidOperationException("Cannot join a lobby that is not in the waiting state");
        }

        var newPlayer = new Player
        {
            PlayerId = Guid.NewGuid(),
            UserId = userId,
            LobbyId = lobbyId,
            IsReady = false,
            Score = 0
        };
        await _lobbyRepository.AddPlayerAsync(newPlayer);

        lobby.Players.Add(newPlayer);
        return await _lobbyRepository.UpdateLobbyAsync(lobby);
    }

    public async Task<Lobby> LeaveLobby(Guid lobbyId, string userId)
    {
        var lobby = await GetLobby(lobbyId);
        
        var player = lobby.Players.FirstOrDefault(p => p.UserId == userId);
        if (player == null)
        {
            throw new KeyNotFoundException($"Player with user ID {userId} not found in lobby");
        }

        if (userId == lobby.OwnerId && lobby.Players.Count > 1)
        {
            lobby.OwnerId = lobby.Players.First(p => p.UserId != userId).UserId;
        }

        lobby.Players.Remove(player);
        await _lobbyRepository.RemovePlayerAsync(player);
        
        if (lobby.Players.Count == 0)
        {
            lobby.IsActive = false;
        }
        
        return await _lobbyRepository.UpdateLobbyAsync(lobby);
    }

    public async Task<Lobby> SetReadyStatus(Guid lobbyId, string userId, bool ready)
    {
        var lobby = await GetLobby(lobbyId);
        
        var player = lobby.Players.FirstOrDefault(p => p.UserId == userId);
        if (player == null)
        {
            throw new KeyNotFoundException($"Player with user ID {userId} not found in lobby");
        }

        player.IsReady = ready;
        
        if (ready && lobby.Players.Count >= 2 && lobby.Players.All(p => p.IsReady))
        {
            _logger.LogInformation("All players in lobby {LobbyId} are ready", lobbyId);
        }
        
        return await _lobbyRepository.UpdateLobbyAsync(lobby);
    }

    public async Task<Lobby> StartGame(Guid lobbyId)
    {
        var lobby = await GetLobby(lobbyId);
        

        
        if (!lobby.Players.All(p => p.IsReady))
        {
            throw new InvalidOperationException("All players must be ready before starting the game");
        }

        lobby.GameState = GameState.InProgress;
        return await _lobbyRepository.UpdateLobbyAsync(lobby);
    }

    public async Task<Lobby> EndGame(Guid lobbyId)
    {
        var lobby = await GetLobby(lobbyId);
        
        if (lobby.GameState != GameState.InProgress)
        {
            throw new InvalidOperationException("Cannot end a game that is not in progress");
        }
        
        lobby.GameState = GameState.Finished;
        return await _lobbyRepository.UpdateLobbyAsync(lobby);
    }

    public async Task<Lobby> UpdateScore(Guid lobbyId, string userId, int score)
    {
        var lobby = await GetLobby(lobbyId);
        
        if (lobby.GameState != GameState.InProgress)
        {
            throw new InvalidOperationException("Cannot update score when game is not in progress");
        }
        
        var player = lobby.Players.FirstOrDefault(p => p.UserId == userId);
        if (player == null)
        {
            throw new KeyNotFoundException($"Player with user ID {userId} not found in lobby");
        }
        
        if (score < 0)
        {
            throw new ArgumentException("Score cannot be negative", nameof(score));
        }
        
        player.Score = score;
        return await _lobbyRepository.UpdateLobbyAsync(lobby);
    }
}