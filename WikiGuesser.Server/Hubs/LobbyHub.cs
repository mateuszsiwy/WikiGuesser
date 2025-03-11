using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using WikiGuesser.Server.DTOs;
using WikiGuesser.Server.Interfaces.Services;
using WikiGuesser.Server.Models;
using WikiGuesser.Server.Services;

namespace WikiGuesser.Server.Hubs;

public class LobbyHub : Hub
{
    private readonly UserConnectionService _userConnectionService;
    private readonly ILobbyService _lobbyService;
    private readonly IChatService _chatService; 
    private readonly IUserService _userService;
    private readonly IWikipediaService _wikipediaService;
    private static readonly Dictionary<Guid, HashSet<string>> _playerSubmissions = new Dictionary<Guid, HashSet<string>>();
    private static readonly Dictionary<Guid, List<PlayerScoreDTO>> _roundScores = new Dictionary<Guid, List<PlayerScoreDTO>>();
    private static readonly Dictionary<Guid, int> _currentRounds = new Dictionary<Guid, int>();

    public LobbyHub(UserConnectionService userConnectionService, ILobbyService lobbyService, IChatService chatService, IUserService userService, IWikipediaService wikipediaService)
    {
        _userConnectionService = userConnectionService;
        _lobbyService = lobbyService;
        _chatService = chatService;
        _userService = userService;
        _wikipediaService = wikipediaService;
    }

    private string GetUsername() 
    {
        return _userConnectionService.GetClaimValue(Context.User, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
    }
    
    public override async Task OnConnectedAsync()
    {
        var username = GetUsername();
        if (!string.IsNullOrEmpty(username))
        {
            _userConnectionService.AddConnection(username, Context.ConnectionId);
        }
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var username = GetUsername();
        if (!string.IsNullOrEmpty(username))
        {
            _userConnectionService.RemoveConnection(username);
            
            // TODO: Remove user from all lobbies 
            
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task<LobbyDTO> CreateLobby(string lobbyName)
    {
        var username = GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            throw new Exception("Username is empty");    
        }
        var user = await _userService.GetUser(username);
        var lobby = await _lobbyService.CreateLobby(lobbyName, user.Id);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, lobby.LobbyId.ToString());
        var lobbyDTO = ConvertToLobbyDTO(lobby);
        await Clients.All.SendAsync("LobbyCreated", lobbyDTO);
        return lobbyDTO;
    }
    
    

    public async Task JoinLobby(Guid lobbyId)
    {
        var username = GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            throw new Exception("Username is empty");    
        }
        var user = await _userService.GetUser(username);
        await _lobbyService.JoinLobby(lobbyId, user.Id);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
        var updatedLobby = await _lobbyService.GetLobby(lobbyId);
        var lobbyDTO = ConvertToLobbyDTO(updatedLobby);
        await Clients.All.SendAsync("UserJoined", username, lobbyDTO);
    }

    public async Task LeaveLobby(Guid lobbyId)
    {
        var username = GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            throw new Exception("Username is empty");    
        }
        var user = await _userService.GetUser(username);
        await _lobbyService.LeaveLobby(lobbyId, user.Id);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId.ToString());
        var updatedLobby = await _lobbyService.GetLobby(lobbyId);
        var lobbyDTO = ConvertToLobbyDTO(updatedLobby);
        await Clients.All.SendAsync("UserLeft", username, lobbyDTO);
    }
    
    public async Task SetReady(Guid lobbyId, bool ready)
    {
        var username = GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            throw new Exception("Username is empty");    
        }
        var user = await _userService.GetUser(username);
        var updatedLobby = await _lobbyService.SetReadyStatus(lobbyId, user.Id, ready);
        var lobbyDTO = ConvertToLobbyDTO(updatedLobby);
        await Clients.All.SendAsync("UserReady", username, ready, lobbyDTO);
    }
    
    public async Task StartGame(Guid lobbyId)
    {
        var lobby = await _lobbyService.GetLobby(lobbyId);
        if (lobby.GameState != GameState.WaitingForPlayers)
        {
            throw new InvalidOperationException("Game cannot be started in the current state");
        }
    
        await _lobbyService.StartGame(lobbyId);
        var article = await _wikipediaService.GetRandomArticleAsync();
        
        await _lobbyService.SetCurrentArticle(lobbyId, article);
        
        var articleDTO = ConvertToWikipediaArticleDTO(article);
        await Clients.Group(lobbyId.ToString()).SendAsync("GameStarted", lobbyId, articleDTO);
    }
    
    public async Task EndGame(Guid lobbyId)
    {
        var lobby = await _lobbyService.EndGame(lobbyId);
        var finalPlayers = lobby.Players.Select(p => new PlayerDTO
        {
            PlayerId = p.PlayerId,
            UserId = p.UserId,
            UserName = _userService.GetUserNameById(p.UserId).Result,
            IsReady = p.IsReady,
            Score = p.Score
        }).ToList();

        
        await Clients.All.SendAsync("GameEnded", lobbyId, finalPlayers);
    }
    
    public async Task NextRound(Guid lobbyId, List<PlayerScoreDTO> playerScores, int roundNumber)
    {
        await _lobbyService.UpdatePlayerScores(lobbyId, playerScores);
        
        var article = await _wikipediaService.GetRandomArticleAsync();
        await _lobbyService.SetCurrentArticle(lobbyId, article);
        
        var articleDTO = ConvertToWikipediaArticleDTO(article);
        
        var updatedLobby = await _lobbyService.GetLobby(lobbyId);
        var updatedPlayers = updatedLobby.Players.Select(p => new PlayerDTO
        {
            PlayerId = p.PlayerId,
            UserId = p.UserId,
            UserName = _userService.GetUserNameById(p.UserId).Result,
            IsReady = p.IsReady,
            Score = p.Score
        }).ToList();
        
        await Clients.Group(lobbyId.ToString()).SendAsync("NextRound", lobbyId, articleDTO, updatedPlayers, roundNumber);
    }
    public async Task SubmitGuess(Guid lobbyId, PlayerScoreDTO playerScore)
    {
        if (!_playerSubmissions.ContainsKey(lobbyId))
        {
            _playerSubmissions[lobbyId] = new HashSet<string>();
            _roundScores[lobbyId] = new List<PlayerScoreDTO>();
            _currentRounds[lobbyId] = 1; 
        }

        var username = GetUsername();
        _playerSubmissions[lobbyId].Add(username);
        _roundScores[lobbyId].Add(playerScore);
    
        var lobby = await _lobbyService.GetLobby(lobbyId);
    
        await Clients.Group(lobbyId.ToString()).SendAsync("PlayerSubmitted", username, playerScore.Score);
    
        if (_playerSubmissions[lobbyId].Count >= lobby.Players.Count)
        {
            var currentRound = _currentRounds[lobbyId]; 
            await Task.Delay(7000);
            if (currentRound < 5)
            {
                _currentRounds[lobbyId] = currentRound + 1;
                
                await NextRound(lobbyId, _roundScores[lobbyId], currentRound + 1);
            }
            else
            {
                await EndGame(lobbyId);
            }
        
            _playerSubmissions[lobbyId].Clear();
            _roundScores[lobbyId].Clear();
        }
    }
    public async Task SendLobbyMessage(Guid lobbyId, string message)
    {
        var username = GetUsername();
        if (string.IsNullOrEmpty(username)) return;

        var user = await _userService.GetUser(username);
        var lobby = await _lobbyService.GetLobby(lobbyId);

        Message newMessage = new Message
        {
            MessageId = Guid.NewGuid(),
            ChatId = lobby.ChatId,
            SenderId = user.Id,
            MessageText = message,
            CreatedAt = DateTime.Now
        };

        await _chatService.saveMessage(newMessage);

        var messageDto = new MessageDTO
        {
            MessageId = newMessage.MessageId,
            MessageText = newMessage.MessageText,
            SenderUsername = user.UserName,
            CreatedAt = newMessage.CreatedAt
        };

        await Clients.Group(lobbyId.ToString()).SendAsync("ReceiveLobbyMessage", user.UserName, message);
    }

    public Task KeepAlive()
    {
        return Task.CompletedTask;
    }
    
    // Add this method to LobbyHub.cs
    public async Task JoinGame(Guid lobbyId)
    {
        var username = GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            throw new Exception("Username is empty");    
        }
        
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
        
        await Clients.OthersInGroup(lobbyId.ToString()).SendAsync("PlayerReconnected", username);
    }
    
    
    private LobbyDTO ConvertToLobbyDTO(Lobby lobby)
    {
        return new LobbyDTO
        {
            LobbyId = lobby.LobbyId,
            Name = lobby.Name,
            OwnerId = lobby.OwnerId,
            IsActive = lobby.IsActive,
            GameState = lobby.GameState.ToString(),
            ChatId = lobby.ChatId,
            Players = lobby.Players.Select(p => new PlayerDTO
            {
                PlayerId = p.PlayerId,
                UserId = p.UserId,
                UserName = _userService.GetUserNameById(p.UserId).Result, 
                IsReady = p.IsReady,
                Score = p.Score
            }).ToList()
        };
    }
    
    public WikipediaArticleDTO ConvertToWikipediaArticleDTO(WikipediaArticle article)
    {
        if (article == null)
            return null;
            
        return new WikipediaArticleDTO
        {
            ArticleName = article.ArticleName,
            Summary = article.Summary,
            Location = new LocationDTO
            {
                Latitude = article.Location?.Latitude,
                Longitude = article.Location?.Longitude,
                CountryName = article.Location?.CountryName
            },
            Weather = double.TryParse(article.Weather, out var temp) ? temp : 0,
            Timezone = article.Timezone
        };
    }
}