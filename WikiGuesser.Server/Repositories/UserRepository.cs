using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WikiGuesser.Server.Interfaces.Repositories;
using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Repositories;

public class UserRepository : IUserRepository 

{
    private readonly WikiGuesserDbContext _context;

    public UserRepository(WikiGuesserDbContext context)
    {
        _context = context;
    }
    
    public async Task<IdentityUser> GetUser(string username)
    {
        var identityUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (identityUser == null)
        {
            throw new Exception("User not found");
        }
        /*var user = new User
        {
            Username = identityUser.UserName,
            Email = identityUser.Email
        };*/
        return identityUser;
    }
    
    public async Task<IdentityUser> GetUserById(string id)
    {
        var identityUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (identityUser == null)
        {
            throw new Exception("User not found");
        }

        return identityUser;
    }
}