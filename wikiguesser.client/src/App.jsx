import React, { useEffect, useState } from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LandingPage from './components/LandingPage';
import GamePage from './components/GamePage';
import GameModesPage from './components/gamemodes/GameModesPage';
import LoginRegister from './components/login-register/LoginRegister';
import ChatComponent from "./components/ChatComponent.jsx";

function App() {
    console.log("App is rendered");
    const [username, setUsername] = useState('');

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            try{
            const username = localStorage.getItem('username');
            setUsername(username);
            localStorage.setItem('username', username);
            } catch(error){
                console.error('invalid token', error);
            }
        }
    });

    return (
        <Router>
            <Routes>
                <Route path="/" element={<LandingPage username={username} />} />
                <Route path="/gamemodes/game" element={<GamePage username={username}/>} />
                <Route path="/gamemodes" element={<GameModesPage />} /> 
                <Route path="/login" element={<LoginRegister setUsername={setUsername} />} />
                <Route path="/chat" element={<ChatComponent />} />
            </Routes>
        </Router>
    );
}

export default App;
