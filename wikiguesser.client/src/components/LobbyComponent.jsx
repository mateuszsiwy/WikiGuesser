import React, {useEffect, useRef, useState} from 'react';
import {useNavigate} from 'react-router-dom';
import './LobbyComponent.css';

const LobbyComponent = ({connection}) => {
    const navigate = useNavigate();
    const [lobbies, setLobbies] = useState([]);
    const [currentLobby, setCurrentLobby] = useState(null);
    const currentLobbyRef = useRef(null);
    const [lobbyName, setLobbyName] = useState('');
    const [lobbyMessages, setLobbyMessages] = useState([]);
    const [message, setMessage] = useState('');
    const [isReady, setIsReady] = useState(false);

    useEffect(() => {
        currentLobbyRef.current = currentLobby;
    }, [currentLobby]);

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (!token) {
            navigate('/login');
            return;
        }

        const username = localStorage.getItem('username');
        if (!username) {
            navigate('/login');
            return;
        }

        if (!connection) {
            console.log("No connection available");
            return;
        }

        // Setup keepalive ping
        const keepAliveInterval = setInterval(() => {
            if (connection.state === "Connected") {
                connection.invoke("KeepAlive").catch(err => {
                    console.log("KeepAlive error:", err);
                });
            }
        }, 15000); // 15 seconds

        // Set up handlers for this component
        connection.on("LobbyCreated", (lobby) => {
            const uniquePlayers = [...new Map(lobby.players.map(p => [p.userId, p])).values()];
            setLobbies(prevLobbies => [...prevLobbies, {...lobby, players: uniquePlayers}]);
        });

        connection.on("UserJoined", (username, updatedLobby) => {
            const deduplicatedLobby = {
                ...updatedLobby,
                players: [...new Map(updatedLobby.players.map(p => [p.userId, p])).values()]
            };

            setLobbies(prevLobbies =>
                prevLobbies.map(lobby =>
                    lobby.lobbyId === deduplicatedLobby.lobbyId ? deduplicatedLobby : lobby
                )
            );

            if (currentLobbyRef.current && deduplicatedLobby.lobbyId === currentLobbyRef.current.lobbyId) {
                setCurrentLobby(deduplicatedLobby);
            }
        });

        connection.on("UserLeft", (username, updatedLobby) => {
            if (updatedLobby.players.length === 0) {
                setLobbies(prevLobbies =>
                    prevLobbies.filter(lobby => lobby.lobbyId !== updatedLobby.lobbyId)
                );
            } else {
                setLobbies(prevLobbies =>
                    prevLobbies.map(lobby =>
                        lobby.lobbyId === updatedLobby.lobbyId ? updatedLobby : lobby
                    )
                );
            }

            if (currentLobbyRef.current && updatedLobby.lobbyId === currentLobbyRef.current.lobbyId) {
                setCurrentLobby(updatedLobby);
            }
        });

        connection.on("UserReady", (username, readyStatus, updatedLobby) => {
            setLobbies(prevLobbies =>
                prevLobbies.map(lobby =>
                    lobby.lobbyId === updatedLobby.lobbyId ? updatedLobby : lobby
                )
            );

            if (currentLobbyRef.current && updatedLobby.lobbyId === currentLobbyRef.current.lobbyId) {
                setCurrentLobby(updatedLobby);
            }

            if (username === localStorage.getItem('username')) {
                setIsReady(readyStatus);
            }
        });

        connection.on("GameStarted", (gamelobbyId, articleDTO) => {
            console.log("GameStarted event received", gamelobbyId, articleDTO);
            if (currentLobbyRef.current && currentLobbyRef.current.lobbyId === gamelobbyId) {
                console.log("GAME STARTED", articleDTO);
                navigate(`/game/${gamelobbyId}`);
            }
        });

        connection.on("ReceiveLobbyMessage", (username, message) => {
            setLobbyMessages(prev => [...prev, {username, message}]);
        });

        const fetchLobbies = async () => {
            try {
                const response = await fetch('http://localhost:5084/api/lobby', {});

                if (response.ok) {
                    const data = await response.json();
                    setLobbies(data);
                } else {
                    console.error("Failed to fetch lobbies:", response.statusText);
                }
            } catch (err) {
                console.error("Error fetching lobbies:", err);
            }
        };

        if (connection.state === "Connected") {
            fetchLobbies();
        } else {
            connection.start().then(fetchLobbies);
        }

        return () => {
            clearInterval(keepAliveInterval);
            // Remove event listeners, but don't stop the connection
            connection.off("LobbyCreated");
            connection.off("UserJoined");
            connection.off("UserLeft");
            connection.off("UserReady");
            connection.off("ReceiveLobbyMessage");

            // Just leave the lobby if we're in one, don't close the connection
            if (currentLobbyRef.current && !window.location.pathname.includes('/game/')) {
                connection.invoke("LeaveLobby", currentLobbyRef.current.lobbyId)
                    .catch(err => console.error("Error leaving lobby:", err));
            }
        };
    }, [navigate, connection]);

    const createLobby = async () => {
        if (!connection || !lobbyName.trim()) return;

        try {
            await connection.invoke("CreateLobby", lobbyName);
            setLobbyName('');
            setLobbyMessages([]);
        } catch (err) {
            console.error("Error creating lobby:", err);
        }
    };

    const joinLobby = async (lobbyId) => {
        if (!connection) return;

        try {
            const lobby = lobbies.find(l => l.lobbyId === lobbyId);
            setCurrentLobby(lobby);
            currentLobbyRef.current = lobby;

            await connection.invoke("JoinLobby", lobbyId);

            setLobbyMessages([]);
        } catch (err) {
            console.error("Error joining lobby:", err);
        }
    };

    const leaveLobby = async () => {
        if (!connection || !currentLobby) return;

        try {
            await connection.invoke("LeaveLobby", currentLobby.lobbyId);
            setCurrentLobby(null);
            setLobbyMessages([]);
            setIsReady(false);
        } catch (err) {
            console.error("Error leaving lobby:", err);
        }
    };

    const toggleReady = async () => {
        if (!connection || !currentLobby) return;

        try {
            const newReadyStatus = !isReady;
            await connection.invoke("SetReady", currentLobby.lobbyId, newReadyStatus);
            setIsReady(newReadyStatus);
        } catch (err) {
            console.error("Error setting ready status:", err);
        }
    };

    const startGame = async () => {
        if (!connection || !currentLobby) return;

        try {
            await connection.invoke("StartGame", currentLobby.lobbyId);
        } catch (err) {
            console.error("Error starting game:", err);
        }
    };

    const sendMessage = async () => {
        if (!connection || !currentLobby || !message.trim()) return;

        try {
            await connection.invoke("SendLobbyMessage", currentLobby.lobbyId, message);
            setMessage('');
        } catch (err) {
            console.error("Error sending message:", err);
        }
    };

    if (!currentLobby) {
        return (
            <div className="lobby-container">
                <h2>Game Lobbies</h2>
                <div className="create-lobby">
                    <input
                        type="text"
                        placeholder="Lobby Name"
                        value={lobbyName}
                        onChange={(e) => setLobbyName(e.target.value)}
                    />
                    <button onClick={createLobby}>Create Lobby</button>
                </div>
                <div className="lobby-list">
                    {lobbies.length === 0 ? (
                        <p>No active lobbies found. Create one to start playing!</p>
                    ) : (
                        <ul>
                            {lobbies.map(lobby => (
                                <li key={lobby.lobbyId} className="lobby-item">
                                    <div className="lobby-info">
                                        <h3>{lobby.name}</h3>
                                        <p>Players: {lobby.players.length}</p>
                                        <p>Status: {lobby.gameState}</p>
                                    </div>
                                    <button
                                        onClick={() => joinLobby(lobby.lobbyId)}
                                        disabled={lobby.gameState !== "WaitingForPlayers"}
                                    >
                                        Join
                                    </button>
                                </li>
                            ))}
                        </ul>
                    )}
                </div>
            </div>
        );
    }

    return (
        <div className="lobby-detail">
            <div className="lobby-header">
                <h2>{currentLobby.name}</h2>
                <button onClick={leaveLobby}>Leave Lobby</button>
            </div>

            <div className="lobby-content">
                <div className="players-section">
                    <h3>Players</h3>
                    <ul>
                        {currentLobby.players.map((player, index) => (
                            <li key={`${player.playerId}-${index}`}
                                className={`player ${player.isReady ? 'ready' : ''}`}>
                                {player.userName} {player.isReady ? '✓' : ''}
                                {player.userId === currentLobby.ownerId && ' (Owner)'}
                            </li>
                        ))}
                    </ul>

                    <div className="controls">
                        <button onClick={toggleReady} className={isReady ? 'ready-btn active' : 'ready-btn'}>
                            {isReady ? 'Not Ready' : 'Ready'}
                        </button>

                        {localStorage.getItem('username') === currentLobby.players.find(p => p.userId === currentLobby.ownerId)?.userName && (
                            <button
                                onClick={startGame}
                                className="start-btn"
                                disabled={!currentLobby.players.every(p => p.isReady) || currentLobby.players.length < 1}
                            >
                                Start Game
                            </button>
                        )}
                    </div>
                </div>

                <div className="chat-section">
                    <h3>Chat</h3>
                    <div className="messages">
                        {lobbyMessages.map((msg, index) => (
                            <div key={index} className="message">
                                <strong>{msg.username}: </strong>{msg.message}
                            </div>
                        ))}
                    </div>
                    <div className="message-input">
                        <input
                            type="text"
                            placeholder="Type a message..."
                            value={message}
                            onChange={(e) => setMessage(e.target.value)}
                            onKeyPress={(e) => e.key === 'Enter' && sendMessage()}
                        />
                        <button onClick={sendMessage}>Send</button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default LobbyComponent;