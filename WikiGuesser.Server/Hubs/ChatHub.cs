using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WikiGuesser.Server.Interfaces.Repositories;
using WikiGuesser.Server.Interfaces.Services;
using WikiGuesser.Server.Models;
using WikiGuesser.Server.Services;

namespace WikiGuesser.Server.Hubs;


public class ChatHub : Hub
{
        private readonly UserConnectionService _userConnectionService;
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private string _username => Context.User.Identity?.Name;
        
        
        public ChatHub(UserConnectionService userConnectionService, IChatService chatService, IUserService userService)
        {
                _userConnectionService = userConnectionService;
                _chatService = chatService;
                _userService = userService;
        }

        public override async Task OnConnectedAsync()
        {
                _userConnectionService.AddConnection(_username, Context.ConnectionId);
                await Groups.AddToGroupAsync(Context.ConnectionId, "Global");
                await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
                _userConnectionService.RemoveConnection(_username);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Global");
                await base.OnDisconnectedAsync(exception);
        }
        
        public async Task SendMessageToChat(string chatName, string message)
        {
                Chat chat = await _chatService.GetChatWithMessages(chatName);
                var sender = await _userService.GetUser(_username);
                Message newMessage = new Message
                {
                        Chat = chat,
                        Sender = sender,
                        MessageText = message
                };
                await _chatService.saveMessage(newMessage);
                await Clients.Group("Global").SendAsync("ReceiveMessage", newMessage);
        }
}