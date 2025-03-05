using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Interfaces.Repositories;

public interface IChatRepository
{
    Task<Chat> GetChatWithMessages(string chatName);
    Task saveMessage(Message message);
}