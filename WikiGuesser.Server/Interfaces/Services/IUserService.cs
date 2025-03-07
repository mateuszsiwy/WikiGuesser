using Microsoft.AspNetCore.Identity;
using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Interfaces.Services;

public interface IUserService
{
    Task<IdentityUser> GetUser(string username);
    Task<string> GetUserNameById(string id);

}