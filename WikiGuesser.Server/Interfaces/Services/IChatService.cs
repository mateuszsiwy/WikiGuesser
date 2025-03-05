using WikiGuesser.Server.Models;
using WikiGuesser.Server.Services;

namespace WikiGuesser.Server.Interfaces.Services;

public interface IChatService
{
    Task<Chat> GetChatWithMessages(string chatName);
    Task saveMessage(Message message);
    
}