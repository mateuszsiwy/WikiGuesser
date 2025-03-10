using Microsoft.AspNetCore.Mvc;
using WikiGuesser.Server.DTOs;
using WikiGuesser.Server.Interfaces.Services;
using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LobbyController : ControllerBase
{
    private readonly ILobbyService _lobbyService;
    private readonly IUserService _userService;

    public LobbyController(ILobbyService lobbyService, IUserService userService)
    {
        _lobbyService = lobbyService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LobbyDTO>>> GetLobbies()
    {
        var lobbies = await _lobbyService.GetLobbies();
        var activeLobbies = lobbies.Where(l => l.IsActive).ToList();
        
        var lobbyDTOs = new List<LobbyDTO>();
        foreach (var lobby in activeLobbies)
        {
            var playerDTOs = new List<PlayerDTO>();
            foreach (var player in lobby.Players)
            {
                var userName = await _userService.GetUserNameById(player.UserId);
                playerDTOs.Add(new PlayerDTO
                {
                    PlayerId = player.PlayerId,
                    UserId = player.UserId,
                    UserName = userName,
                    IsReady = player.IsReady,
                    Score = player.Score
                });
            }

            lobbyDTOs.Add(new LobbyDTO
            {
                LobbyId = lobby.LobbyId,
                Name = lobby.Name,
                OwnerId = lobby.OwnerId,
                IsActive = lobby.IsActive,
                GameState = lobby.GameState.ToString(),
                ChatId = lobby.ChatId,
                Players = playerDTOs
            });
        }

        return Ok(lobbyDTOs);
    }
    [HttpGet("{lobbyId}/gamestate")]
    public async Task<IActionResult> GetGameState(Guid lobbyId)
    {
        try {
            var gameState = await _lobbyService.GetCurrentGameState(lobbyId);
            return Ok(gameState);
        }
        catch (Exception ex) {
            return StatusCode(500, $"Error getting game state: {ex.Message}");
        }
    }
}