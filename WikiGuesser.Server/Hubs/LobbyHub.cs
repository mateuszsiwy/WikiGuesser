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

    public async Task CreateLobby(string lobbyName)
    {
        var username = GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            throw new Exception("Username is empty");    
        }
        var user = await _userService.GetUser(username);
        var lobby = await _lobbyService.CreateLobby(lobbyName, user.Id);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, lobby.LobbyId.ToString());
        await Clients.All.SendAsync("LobbyCreated", lobby);
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
        await Clients.All.SendAsync("UserJoined", username, updatedLobby);
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
        await Clients.All.SendAsync("UserLeft", username, updatedLobby);
    }
    
    public async Task SetReady(Guid lobbyId, bool ready)
    {
        var username = GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            throw new Exception("Username is empty");    
        }
        var user = await _userService.GetUser(username);
        await _lobbyService.SetReadyStatus(lobbyId, user.Id, ready);
        await Clients.All.SendAsync("UserReady", username, ready);
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
}