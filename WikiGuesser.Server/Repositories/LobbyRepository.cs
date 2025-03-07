using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WikiGuesser.Server.Hubs;
using WikiGuesser.Server.Interfaces.Repositories;
using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Repositories;

public class LobbyRepository : ILobbyRepository
{
    private readonly WikiGuesserDbContext _context;

    public LobbyRepository(WikiGuesserDbContext context)
    {
        _context = context;
    }

    public async Task<List<Lobby>> GetLobbiesAsync()
    {
        return await _context.Lobbies
            .Include(l => l.Players)
            .ToListAsync();
    }

    public async Task<Lobby> GetLobbyAsync(Guid id)
    {
        return await _context.Lobbies
            .Include(l => l.Players)
            .FirstOrDefaultAsync(l => l.LobbyId == id);
    }

    public async Task<Lobby> AddLobbyAsync(Lobby lobby)
    {
        await _context.Lobbies.AddAsync(lobby);
        await _context.SaveChangesAsync();
        return lobby;
    }

    public async Task<Player> AddPlayerAsync(Player player)
    {
        await _context.Players.AddAsync(player);
        await _context.SaveChangesAsync();
        return player;
    }

    public async Task<Lobby> UpdateLobbyAsync(Lobby lobby)
    {
        _context.Lobbies.Update(lobby);
        await _context.SaveChangesAsync();
        return lobby;
    }

    public async Task<Player> GetPlayerAsync(Guid playerId)
    {
        return await _context.Players.FindAsync(playerId);
    }

    public async Task<Player> GetPlayerByUserIdAsync(string userId)
    {
        return await _context.Players
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task RemovePlayerAsync(Player player)
    {
        _context.Players.Remove(player);
        await _context.SaveChangesAsync();
    }

    public async Task<IdentityUser> GetUserAsync(string userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<Chat> AddChatAsync(Chat chat)
    {
        await _context.Chats.AddAsync(chat);
        await _context.SaveChangesAsync();
        return chat;
    }
}