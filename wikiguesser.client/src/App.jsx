import React, {useEffect, useState} from 'react';
import {BrowserRouter as Router, Route, Routes} from 'react-router-dom';
import LandingPage from './components/LandingPage';
import GamePage from './components/GamePage';
import GameModesPage from './components/gamemodes/GameModesPage';
import LoginRegister from './components/login-register/LoginRegister';
import ChatComponent from "./components/ChatComponent.jsx";
import LobbyComponent from './components/LobbyComponent';
import createLobbyConnection from './services/signalRLobbyService';

function App() {
    const [username, setUsername] = useState('');
    const [connection, setConnection] = useState(null);
    
    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            try {
                const username = localStorage.getItem('username');
                setUsername(username);

                const newConnection = createLobbyConnection(token);
                newConnection.start()
                    .then(() => setConnection(newConnection))
                    .catch(err => console.error("Failed to connect to LobbyHub:", err));
            } catch (error) {
                console.error('invalid token', error);
            }
        }
    }, []);

    return (
        <Router>
            <Routes>
                <Route path="/" element={<LandingPage username={username}/>}/>
                <Route path="/gamemodes/game" element={<GamePage username={username} connection={connection}/>}/>
                <Route path="/gamemodes" element={<GameModesPage/>}/>
                <Route path="/login" element={<LoginRegister setUsername={setUsername}/>}/>
                <Route path="/chat" element={<ChatComponent/>}/>
                <Route path="/lobby" element={<LobbyComponent connection={connection}/>}/>
                <Route path="/game/:lobbyId" element={<GamePage username={username} connection={connection}/>}/>
            </Routes>
        </Router>
    );
}

export default App;