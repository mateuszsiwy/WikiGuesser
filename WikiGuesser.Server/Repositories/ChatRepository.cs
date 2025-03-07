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
        Chat? chat = await GetChat(chatName);
        if (chat == null)
        {
            var newChat = new Chat
            {
                ChatId = Guid.NewGuid(),
                ChatName = chatName,
                CreatedAt = DateTime.Now,
                Messages = new List<Message>()
            };
            
            _context.Chats.Add(newChat);
            await _context.SaveChangesAsync();
            
            return newChat;
        }
    
        return chat;
    }
    
    public async Task saveMessage(Message message)
    {
        try
        {
            // The issue is here. Don't use message.Chat.ChatName as it creates a circular reference
            // Instead, first find the chat by its ID
            var chat = await _context.Chats.FindAsync(message.ChatId);
            if (chat == null)
            {
                throw new Exception($"Chat with ID {message.ChatId} not found");
            }
    
            // Don't set the navigation properties directly
            message.Chat = null;  // Remove circular reference
    
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in saveMessage: {ex.Message}");
            throw;
        }
    }

    private async Task<Chat?> GetChat(string chatName)
    {
        // Use EF.Functions.Collate for case-insensitive comparison
        var chat = await _context.Chats
            .FirstOrDefaultAsync(c => c.ChatName.Equals(chatName));
        
        if (chat != null)
        {
            // Explicitly load messages with their senders to avoid type casting issues
            await _context.Entry(chat)
                .Collection(c => c.Messages)
                .LoadAsync();
                
            foreach (var message in chat.Messages)
            {
                await _context.Entry(message)
                    .Reference(m => m.Sender)
                    .LoadAsync();
            }
        }
        
        return chat;
    }
}