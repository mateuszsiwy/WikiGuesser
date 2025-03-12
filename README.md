# WikiGuesser

WikiGuesser is a multiplayer geo-guessing game where players read Wikipedia articles about cities and try to pinpoint their location on a map.

## Overview

WikiGuesser challenges players to guess the location of cities based on their Wikipedia descriptions with intentionally redacted country names. Players earn points based on how close their guess is to the actual location. The game supports both single-player and multiplayer modes via real-time lobbies.

## Architecture

### Backend (.NET Core API)
- **ASP.NET Core 8.0** - Web API with SignalR for real-time communication
- **EntityFrameworkCore** - ORM for database operations
- **SQL Server** - Database for storing game data, lobbies, and player information
- **External APIs Integration**:
  - Wikipedia API - For retrieving city articles and descriptions
  - GeoNames API - For location data and coordinates
  - WeatherAPI - For retrieving real-time weather information

### Frontend (React)
- **React** - UI library with hooks and context for state management
- **React Router** - For client-side routing
- **SignalR Client** - For real-time connection with the server
- **Leaflet** - Interactive mapping library for placing guesses

### Authentication
- **JWT Authentication** - Token-based authentication for securing API endpoints and SignalR connections

## Key Features

- **Wikipedia Integration**: Dynamically fetches city articles from Wikipedia
- **Location Masking**: Automatically redacts country names to increase difficulty
- **Real-time Multiplayer**: Play with friends in lobby-based game sessions
- **Interactive Map**: Leaflet integration for placing and evaluating guesses
- **Distance Calculation**: Calculates score based on distance from the actual location
- **Dynamic Weather Data**: Shows real-time weather for the location

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Authenticate and receive JWT token

### Lobby Management
- `GET /api/lobby` - Get list of active lobbies
- `GET /api/lobby/{id}` - Get a specific lobby
- `GET /api/lobby/{lobbyId}/gamestate` - Get current game state for a lobby

### Wikipedia Data
- `GET /api/wikipedia/cities` - Get random city name
- `GET /api/wikipedia/citydesc/{city}` - Get description of a city
- `GET /api/wikipedia/location/{city}` - Get coordinates for a city
- `GET /api/wikipedia/citydesc/{city}/weather` - Get current weather for a city

## SignalR Hub Methods

### Lobby Operations
- `CreateLobby(string lobbyName)` - Create a new game lobby
- `JoinLobby(Guid lobbyId)` - Join an existing lobby
- `LeaveLobby(Guid lobbyId)` - Leave a lobby
- `SetReady(Guid lobbyId, bool ready)` - Set player ready status
- `StartGame(Guid lobbyId)` - Start the game (lobby owner only)

### Game Operations
- `JoinGame(Guid lobbyId)` - Join a game in progress
- `SubmitGuess(Guid lobbyId, PlayerScoreDTO playerScore)` - Submit a location guess
- `NextRound(Guid lobbyId, List<PlayerScoreDTO> playerScores, int roundNumber)` - Progress to next round
- `EndGame(Guid lobbyId)` - End the game and show final scores

### Chat
- `SendLobbyMessage(Guid lobbyId, string message)` - Send a message in lobby chat

### Connection Management
- `KeepAlive()` - Maintain connection with the hub

## SignalR Events

### Lobby Events
- `LobbyCreated` - When a new lobby is created
- `UserJoined` - When a user joins a lobby
- `UserLeft` - When a user leaves a lobby
- `UserReady` - When a user changes their ready status

### Game Events
- `GameStarted` - When a game begins
- `NextRound` - When a new round starts
- `PlayerSubmitted` - When a player submits a guess
- `GameEnded` - When a game ends
- `PlayerReconnected` - When a player reconnects to the game

### Chat Events
- `ReceiveLobbyMessage` - When a new chat message is received

## Game Flow

1. Players create or join a lobby
2. Players mark themselves as ready
3. Lobby owner starts the game when all players are ready
4. Each round:
   - Players are shown a Wikipedia article about a city (country name redacted)
   - Players have 60 seconds to place their guess on the map
   - After guessing, players see how close they were to the actual location
   - Scores are calculated based on proximity
5. After 5 rounds, final scores are displayed and the winner is determined

## Development Setup

### Prerequisites
- .NET 8.0 SDK
- Node.js (v18+)
- SQL Server
- API keys for GeoNames and WeatherAPI

### Backend Setup
```bash
cd WikiGuesser.Server
dotnet restore
dotnet run
```

### Frontend Setup
```bash
cd wikiguesser.client
npm install
npm start
```

## Future Enhancements
- Leaderboards and player statistics
- Customizable game rules (time limits, number of rounds)
- More game modes (countries, landmarks, historical places)
- Mobile responsive design

## License
This project is licensed under the MIT License - see the LICENSE file for details.
