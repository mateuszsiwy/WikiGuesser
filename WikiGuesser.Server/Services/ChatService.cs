using WikiGuesser.Server.Interfaces.Repositories;
using WikiGuesser.Server.Interfaces.Services;
using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Services;


public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;

    public ChatService(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }
    
    public async Task<Chat> GetChatWithMessages(string chatName)
    {
        return await _chatRepository.GetChatWithMessages(chatName);
    }

    public async Task saveMessage(Message message)
    {
        await _chatRepository.saveMessage(message);
    }
}