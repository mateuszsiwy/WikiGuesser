﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
            using Microsoft.AspNetCore.SignalR;
            using WikiGuesser.Server.DTOs;
            using WikiGuesser.Server.Interfaces.Services;
            using WikiGuesser.Server.Models;
            using WikiGuesser.Server.Services;

            namespace WikiGuesser.Server.Hubs;
            
public class ChatHub : Hub
{
    private readonly UserConnectionService _userConnectionService;
    private readonly IChatService _chatService;
    private readonly IUserService _userService;
    
    private string GetUsername() 
    {
        return _userConnectionService.GetClaimValue(Context.User, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
    }

    public ChatHub(UserConnectionService userConnectionService, IChatService chatService, IUserService userService)
    {
        _userConnectionService = userConnectionService;
        _chatService = chatService;
        _userService = userService;
    }

    public override async Task OnConnectedAsync()
    {
        var username = GetUsername();
        
        if (string.IsNullOrEmpty(username))
        {
            Console.WriteLine("Username is null or empty in OnConnectedAsync");
            
            // Try to log available claims for debugging
            if (Context.User != null)
            {
                var claims = Context.User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
                Console.WriteLine($"Available claims: {string.Join(", ", claims)}");
            }
            else
            {
                Console.WriteLine("Context.User is null");
            }
            
            await base.OnConnectedAsync();
            return;
        }

        _userConnectionService.AddConnection(username, Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, "Global");
        await base.OnConnectedAsync();
    }


                public override async Task OnDisconnectedAsync(Exception exception)
                {
                    var _username = GetUsername();
                    if (_username != null)
                    {
                        _userConnectionService.RemoveConnection(_username);
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Global");
                    }
                    await base.OnDisconnectedAsync(exception);
                }
            
    public async Task SendMessageToChat(string chatName, string message)
    {
        var _username = GetUsername();
        if (_username != null)
        {
            Chat chat = await _chatService.GetChatWithMessages(chatName);
            var sender = await _userService.GetUser(_username);
            
            Message newMessage = new Message
            {
                MessageId = Guid.NewGuid(),
                ChatId = chat.ChatId,
                SenderId = sender.Id,
                MessageText = message,
                CreatedAt = DateTime.Now
            };
            
            await _chatService.saveMessage(newMessage);
            
            // Create a DTO to avoid circular references
            var messageDto = new MessageDTO
            {
                MessageId = newMessage.MessageId,
                MessageText = newMessage.MessageText,
                SenderUsername = sender.UserName,
                CreatedAt = newMessage.CreatedAt
            };
            
            await Clients.Group("Global").SendAsync("ReceiveMessage", sender.UserName, message);
        }
} 
}