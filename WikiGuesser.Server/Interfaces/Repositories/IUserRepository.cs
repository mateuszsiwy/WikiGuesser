using Microsoft.AspNetCore.Identity;
using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Interfaces.Repositories;

public interface IUserRepository
{
    Task<IdentityUser> GetUser(string username);
}