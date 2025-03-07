using Microsoft.AspNetCore.Identity;
using WikiGuesser.Server.Interfaces.Repositories;
using WikiGuesser.Server.Interfaces.Services;
using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<IdentityUser> GetUser(string username)
    {
        return await _userRepository.GetUser(username);
    }
    
    public async Task<string> GetUserNameById(string id)
    {
        var user = await _userRepository.GetUserById(id);
        return user.UserName;
    }
}