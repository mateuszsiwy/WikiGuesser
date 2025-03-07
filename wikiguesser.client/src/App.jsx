import React, { useEffect, useState } from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LandingPage from './components/LandingPage';
import GamePage from './components/GamePage';
import GameModesPage from './components/gamemodes/GameModesPage';
import LoginRegister from './components/login-register/LoginRegister';
import ChatComponent from "./components/ChatComponent.jsx";
import LobbyComponent from './components/LobbyComponent'; // Import the LobbyComponent

function App() {
    console.log("App is rendered");
    const [username, setUsername] = useState('');

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            try {
                const username = localStorage.getItem('username');
                setUsername(username);
                localStorage.setItem('username', username);
            } catch(error) {
                console.error('invalid token', error);
            }
        }
    }, []); // Add empty dependency array to prevent continuous re-rendering

    return (
        <Router>
            <Routes>
                <Route path="/" element={<LandingPage username={username} />} />
                <Route path="/gamemodes/game" element={<GamePage username={username}/>} />
                <Route path="/gamemodes" element={<GameModesPage />} /> 
                <Route path="/login" element={<LoginRegister setUsername={setUsername} />} />
                <Route path="/chat" element={<ChatComponent />} />
                <Route path="/lobby" element={<LobbyComponent />} /> {/* Add this route for lobbies */}
                <Route path="/game/:lobbyId" element={<GamePage username={username}/>} /> {/* Add route for game with specific lobby */}
            </Routes>
        </Router>
    );
}

export default App;