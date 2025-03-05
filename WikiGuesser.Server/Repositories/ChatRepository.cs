using Microsoft.EntityFrameworkCore;
using WikiGuesser.Server.Interfaces.Repositories;
using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly WikiGuesserDbContext _context;
    

    public ChatRepository(WikiGuesserDbContext context)
    {
        _context = context;
    }

    public async Task<Chat> GetChatWithMessages(string chatName)
    {
        try
        {
            Chat? chat = await GetChat(chatName);
            if (chat == null)
            {
                var createdChat = _context.Chats.Add(new Chat
                {
                    ChatId = Guid.NewGuid(),
                    ChatName = chatName,
                    CreatedAt = DateTime.Now,
                    Messages = new List<Message>()
                });
                return createdChat.Entity;
                throw new Exception("Chat not found");
            }

            return chat;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    public async Task saveMessage(Message message)
    {
        try
        {
            Chat? chat = await GetChat(message.Chat.ChatName);
            if (chat == null)
            {
                throw new Exception("Chat not found");
            }

            chat.Messages.Add(message);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private async Task<Chat?> GetChat(string chatName)
    {
        return await _context.Chats
            .Where(c => c.ChatName == chatName)
            .Include(c => c.Messages)
            .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync();    }
}