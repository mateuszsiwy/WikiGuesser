import React, {useEffect, useState} from 'react';
import {useParams, useNavigate} from 'react-router-dom';
import './GamePage.css';
import InteractiveMap from './InteractiveMap';
import Round from './Round';

function GamePage({username, connection}) {
    const [article, setArticle] = useState("");
    const [location, setLocation] = useState(null);
    const [position, setPosition] = useState(null);
    const [cityDesc, setCityDesc] = useState("");
    const [weather, setWeather] = useState(null);
    const [timezone, setTimezone] = useState(null);
    const [submitted, setSubmitted] = useState(false);
    const [score, setScore] = useState(0);
    const [distance, setDistance] = useState(null);
    const [totalScore, setTotalScore] = useState(0);
    const [timeLeft, setTimeLeft] = useState(60);
    const [round, setRound] = useState(1);
    const [players, setPlayers] = useState([]);
    const [isOnlineGame, setIsOnlineGame] = useState(false);
    const [gameFinished, setGameFinished] = useState(false);
    const [finalScores, setFinalScores] = useState([]);
    
    const {lobbyId} = useParams();
    const navigate = useNavigate();

    const fetchGameState = async () => {
        if (!lobbyId) return;

        try {
            const response = await fetch(`http://localhost:5084/api/lobby/${lobbyId}/gamestate`);
            if (response.ok) {
                const gameState = await response.json();
                console.log("Retrieved game state:", gameState);

                setArticle(gameState.articleName || "");
                setCityDesc(gameState.summary || "");

                if (gameState.location) {
                    setLocation({
                        lat: parseFloat(gameState.location.latitude),
                        lng: parseFloat(gameState.location.longitude),
                        countryName: gameState.location.countryName
                    });
                }

                setWeather(gameState.weather);
                setTimezone(gameState.timezone);
                setPlayers(gameState.players || []);
            }
        } catch (error) {
            console.error("Error fetching game state:", error);
        }
    };
    useEffect(() => {
        setIsOnlineGame(!!lobbyId);

        if (lobbyId) {
            setIsOnlineGame(true);
            console.log("THIS IS ONLINE GAME MODE");
            fetchGameState();
        }
        if (connection && lobbyId) {
            if (connection.state === "Connected") {
                connection.invoke("JoinGame", lobbyId)
                    .catch(err => console.error("Error joining game:", err));
            }

            connection.on("NextRound", (receivedLobbyId, articleDTO, updatedPlayers, roundNumber) => {
                console.log("Next round event received", receivedLobbyId, articleDTO, roundNumber);
                if (receivedLobbyId === lobbyId) {

                    setArticle(articleDTO.articleName);
                    setCityDesc(articleDTO.summary);
                    setLocation({
                        lat: articleDTO.location.latitude,
                        lng: articleDTO.location.longitude,
                        countryName: articleDTO.location.countryName
                    });
                    setWeather(articleDTO.weather);
                    setTimezone(articleDTO.timezone);
                    setPlayers(updatedPlayers);
                    setRound(roundNumber);
                    resetRoundState();
                }
            });
            
            connection.on("GameEnded", (gamelobbyId, players) => {
                if (gamelobbyId === lobbyId) {
                    console.log(players);
                    setPlayers(players);
                    
                    console.log("Game ended event received");
                    const sortedPlayers = [...players].sort((a, b) => b.score - a.score);
                    setFinalScores(sortedPlayers);
                    setGameFinished(true);
                }
            });

        }

        if (connection) {
            console.log("Connection state in GamePage:", connection.state);

            if (connection.state === "Disconnected") {
                console.log("Reconnecting to SignalR...");
                connection.start()
                    .then(() => {
                        console.log("Reconnected to SignalR hub");
                        if (lobbyId) {
                            connection.invoke("JoinGame", lobbyId)
                                .catch(err => console.error("Error joining game:", err));
                        }
                    })
                    .catch(err => console.error("Error reconnecting:", err));
            }

            if (lobbyId) {


                connection.on("GameEnded", (gamelobbyId) => {
                    if (gamelobbyId === lobbyId) {
                        setTimeout(() => {
                            navigate("/lobby");
                        }, 5000);
                    }
                });
            }

            const keepAliveInterval = setInterval(() => {
                if (connection.state === "Connected") {
                    connection.invoke("KeepAlive").catch(err => {
                        console.log("KeepAlive error:", err);
                    });
                }
            }, 30000);

            return () => {
                clearInterval(keepAliveInterval);
                if (lobbyId) {
                    connection.off("GameStarted");
                    connection.off("NextRound");
                    connection.off("GameEnded");
                }
            };
        } else if (!lobbyId) {
            fetchData();
        }
    }, [connection, lobbyId, navigate]);

    const fetchData = async () => {
        if (lobbyId) {
            console.log("Skipping fetch for online game");
            return;
        }
        try {
            let articleResponse = await fetch('http://localhost:5084/api/wikipedia/cities');
            let articleData = await articleResponse.text();

            setArticle(articleData);
            let cityDescResponse = await fetch(`http://localhost:5084/api/wikipedia/citydesc/${articleData}`);
            let cityDescData = await cityDescResponse.text();

            while (!cityDescResponse.ok || cityDescData == "Strona nie została znaleziona." || cityDescData.includes("may refer to") || cityDescData.includes("usually refers to") || cityDescData.includes("may mean")) {
                articleResponse = await fetch('http://localhost:5084/api/wikipedia/cities');
                articleData = await articleResponse.text();

                setArticle(articleData);
                cityDescResponse = await fetch(`http://localhost:5084/api/wikipedia/citydesc/${articleData}`);
                cityDescData = await cityDescResponse.text();
            }

            const locationResponse = await fetch(`http://localhost:5084/api/wikipedia/location/${articleData}`);
            const locationData = await locationResponse.json();

            const location = {
                countryName: locationData.countryName,
                lat: locationData.latitude,
                lng: locationData.longitude
            };

            setLocation(location);

            const weatherResponse = await fetch(`http://localhost:5084/api/wikipedia/citydesc/${articleData}/weather`);
            const weatherData = await weatherResponse.json();
            setWeather(weatherData.current.temp_c);
            setTimezone(weatherData.location.localtime);

            if (locationData?.countryName) {
                cityDescData = cityDescData.replace(new RegExp(locationData.countryName, 'g'), "COUNTRY");
            }
            setCityDesc(cityDescData);
        } catch (error) {
            console.error('Error fetching data:', error);
        }
    };

    useEffect(() => {
        if (!lobbyId && round > 1) {
            fetchData();
        }
    }, [round, lobbyId]);

    useEffect(() => {
        console.log("Current article state:", {
            article,
            cityDesc: cityDesc ? cityDesc.substring(0, 30) + "..." : "not set",
            location,
            players
        });
    }, [article, cityDesc, location, players]);

    const handleScoreUpdate = (newScore, newDistance) => {
        setScore(newScore);
        setDistance(newDistance);
    };

    useEffect(() => {
        if (submitted) return;

        if (timeLeft > 0) {
            const timer = setTimeout(() => {
                setTimeLeft(timeLeft - 1);
            }, 1000);
            return () => clearTimeout(timer);
        } else {
            onSubmit();
        }
    }, [timeLeft, submitted]);

    function onSubmit() {
        setSubmitted(true);
        const screen = document.getElementById("articleWindow");
        screen.style.filter = "blur(10px)";
        const scoreWindow = document.getElementById("scoreWindow");
        scoreWindow.style.display = "flex";
        setTotalScore(totalScore + score);

        if (lobbyId && connection) {
            console.log("Processing online game submission");

            const currentPlayer = players.find(p => p.userName === username);

            if (currentPlayer) {
                const playerScore = {
                    playerId: currentPlayer.playerId,
                    score: score || 0,
                    distance: distance || 0
                };

                console.log("Submitting player score:", playerScore);

                connection.invoke("SubmitGuess", lobbyId, playerScore)
                    .catch(err => console.error("Error submitting guess:", err));

                connection.on("PlayerSubmitted", (playerName, playerScore) => {
                    console.log(`Player ${playerName} submitted with score: ${playerScore}`);

                });
            }
        } else if (!isOnlineGame) {
            console.log("Processing offline game submission");

            if (round < 5) {
                setTimeout(() => {
                    handleNextRound();
                }, 5000);
            } else {
                setTimeout(() => {
                    navigate("/gamemodes");
                }, 5000);
            }
        }
    }

    function handleNextRound() {
        setRound(round + 1);
        resetRoundState();
    }

    function resetRoundState() {
        setSubmitted(false);
        const screen = document.getElementById("articleWindow");
        screen.style.filter = "none";
        const scoreWindow = document.getElementById("scoreWindow");
        scoreWindow.style.display = "none";
        setScore(null);
        setDistance(null);
        setTimeLeft(60);
        setPosition(null);
    }

    return (
        <>
            <div className="gamePage" id="gamePage">
                <div className="userWindow">
                    <h2>{username}</h2>
                    {isOnlineGame && (
                        <div className="players-list">
                            <h3>Players:</h3>
                            <ul>
                                {players.map(player => (
                                    <li key={player.playerId}>
                                        {player.userName}: {player.score} points
                                    </li>
                                ))}
                            </ul>
                        </div>
                    )}
                </div>
                <div className="articleWindow" id="articleWindow">
                    {article ? (
                        <>
                            <h1>{article}</h1>
                            <div dangerouslySetInnerHTML={{__html: cityDesc}}></div>
                            <p>Temperature: {weather}C</p>
                            <p>Timezone: {timezone}</p>
                        </>
                    ) : (
                        <p>Loading article...</p>
                    )}
                </div>
                <InteractiveMap
                    position={position}
                    setPosition={setPosition}
                    submitted={submitted}
                    location={location}
                    onScoreUpdate={handleScoreUpdate}
                />
                <button className="submitButton" id="submitButton" onClick={onSubmit}>
                    Submit
                </button>
                <Round round={round} timeLeft={timeLeft} totalScore={totalScore}/>
            </div>
            <div className="scoreWindow" id="scoreWindow">
                points: {score} <br/>distance: {distance}
            </div>
            {gameFinished && (
                <div className="finalScoresOverlay">
                    <div className="finalScoresWindow">
                        <h2>Final Scores</h2>
                        <table className="finalScoresTable">
                            <thead>
                                <tr>
                                    <th>Rank</th>
                                    <th>Player</th>
                                    <th>Score</th>
                                </tr>
                            </thead>
                            <tbody>
                                {finalScores.map((player, index) => (
                                    <tr key={player.playerId} className={player.userName === username ? "currentPlayer" : ""}>
                                        <td>{index + 1}</td>
                                        <td>{player.userName}</td>
                                        <td>{player.score} pts</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                        <button className="returnToLobbyBtn" onClick={() => navigate("/lobby")}>
                            Return to Lobby
                        </button>
                    </div>
                </div>
            )}
        </>
    );
}

export default GamePage;