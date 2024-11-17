import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LandingPage from './components/LandingPage';
import GamePage from './components/GamePage';
import GameModesPage from './components/gamemodes/GameModesPage';

function App() {
    console.log("App is rendered");

    return (
        <Router>
            <Routes>
                <Route path="/" element={<LandingPage />} />
                <Route path="/gamemodes/game" element={<GamePage />} />
                <Route path="/gamemodes" element={<GameModesPage />} /> 
            </Routes>
        </Router>
    );
}

export default App;
