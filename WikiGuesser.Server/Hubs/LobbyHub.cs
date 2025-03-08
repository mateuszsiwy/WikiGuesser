using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using WikiGuesser.Server.DTOs;
using WikiGuesser.Server.Interfaces.Services;
using WikiGuesser.Server.Models;
using WikiGuesser.Server.Services;

namespace WikiGuesser.Server.Hubs;

public class LobbyHub : Hub
{
    private readonly UserConnectionService _userConnectionService;
    private readonly ILobbyService _lobbyService;
    private readonly IChatService _chatService; 
    private readonly IUserService _userService;
    
    public LobbyHub(UserConnectionService userConnectionService, ILobbyService lobbyService, IChatService chatService, IUserService userService)
    {
        _userConnectionService = userConnectionService;
        _lobbyService = lobbyService;
        _chatService = chatService;
        _userService = userService;
    }

    private string GetUsername() 
    {
        return _userConnectionService.GetClaimValue(Context.User, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
    }
    
    public override async Task OnConnectedAsync()
    {
        var username = GetUsername();
        if (!string.IsNullOrEmpty(username))
        {
            _userConnectionService.AddConnection(username, Context.ConnectionId);
        }
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var username = GetUsername();
        if (!string.IsNullOrEmpty(username))
        {
            _userConnectionService.RemoveConnection(username);
            
            // TODO: Remove user from all lobbies 
            
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task<LobbyDTO> CreateLobby(string lobbyName)
    {
        var username = GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            throw new Exception("Username is empty");    
        }
        var user = await _userService.GetUser(username);
        var lobby = await _lobbyService.CreateLobby(lobbyName, user.Id);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, lobby.LobbyId.ToString());
        var lobbyDTO = ConvertToLobbyDTO(lobby);
        await Clients.All.SendAsync("LobbyCreated", lobbyDTO);
        return lobbyDTO;
    }
    
    

    public async Task JoinLobby(Guid lobbyId)
    {
        var username = GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            throw new Exception("Username is empty");    
        }
        var user = await _userService.GetUser(username);
        await _lobbyService.JoinLobby(lobbyId, user.Id);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
        var updatedLobby = await _lobbyService.GetLobby(lobbyId);
        var lobbyDTO = ConvertToLobbyDTO(updatedLobby);
        await Clients.All.SendAsync("UserJoined", username, lobbyDTO);
    }

    public async Task LeaveLobby(Guid lobbyId)
    {
        var username = GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            throw new Exception("Username is empty");    
        }
        var user = await _userService.GetUser(username);
        await _lobbyService.LeaveLobby(lobbyId, user.Id);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId.ToString());
        var updatedLobby = await _lobbyService.GetLobby(lobbyId);
        var lobbyDTO = ConvertToLobbyDTO(updatedLobby);
        await Clients.All.SendAsync("UserLeft", username, lobbyDTO);
    }
    
    public async Task SetReady(Guid lobbyId, bool ready)
    {
        var username = GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            throw new Exception("Username is empty");    
        }
        var user = await _userService.GetUser(username);
        var updatedLobby = await _lobbyService.SetReadyStatus(lobbyId, user.Id, ready);
        var lobbyDTO = ConvertToLobbyDTO(updatedLobby);
        await Clients.All.SendAsync("UserReady", username, ready, lobbyDTO);
    }
    
    public async Task StartGame(Guid lobbyId)
    {
        await _lobbyService.StartGame(lobbyId);
        
        await Clients.All.SendAsync("GameStarted", lobbyId);
    }

    public async Task SendLobbyMessage(Guid lobbyId, string message)
    {
        var username = GetUsername();
        if (string.IsNullOrEmpty(username)) return;

        var user = await _userService.GetUser(username);
        var lobby = await _lobbyService.GetLobby(lobbyId);

        Message newMessage = new Message
        {
            MessageId = Guid.NewGuid(),
            ChatId = lobby.ChatId,
            SenderId = user.Id,
            MessageText = message,
            CreatedAt = DateTime.Now
        };

        await _chatService.saveMessage(newMessage);

        var messageDto = new MessageDTO
        {
            MessageId = newMessage.MessageId,
            MessageText = newMessage.MessageText,
            SenderUsername = user.UserName,
            CreatedAt = newMessage.CreatedAt
        };

        await Clients.Group(lobbyId.ToString()).SendAsync("ReceiveLobbyMessage", user.UserName, message);
    }
    
    private LobbyDTO ConvertToLobbyDTO(Lobby lobby)
    {
        return new LobbyDTO
        {
            LobbyId = lobby.LobbyId,
            Name = lobby.Name,
            OwnerId = lobby.OwnerId,
            IsActive = lobby.IsActive,
            GameState = lobby.GameState.ToString(),
            ChatId = lobby.ChatId,
            Players = lobby.Players.Select(p => new PlayerDTO
            {
                PlayerId = p.PlayerId,
                UserId = p.UserId,
                UserName = _userService.GetUserNameById(p.UserId).Result, 
                IsReady = p.IsReady,
                Score = p.Score
            }).ToList()
        };
    }
}