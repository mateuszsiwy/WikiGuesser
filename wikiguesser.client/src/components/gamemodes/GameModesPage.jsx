import React, { useState } from 'react';
import './GameModesPage.css';
import GameMode from './GameMode';
import { Link } from 'react-router-dom';
import { GameModeContext } from '../context'; 

function GameModesPage() {
  const [selectedMode, setSelectedMode] = useState(null);

  const handleModeSelection = (mode) => {
    setSelectedMode(mode);
  };

  return (
    <div className="gameModesPage">
      <h1>Game Modes</h1>
      <GameModeContext.Provider value={{ username: 'Guest', selectedMode: selectedMode }}>
        <div className="gameModesContainer">
          <Link to="game" className="link" onClick={() => handleModeSelection('Cities')}>
            <GameMode image='/src/assets/miasto_tlo.jpg' h2='Cities' p='Collection of cities throughout the world.' />
          </Link>
          <Link to="game" className="link" onClick={() => handleModeSelection('Landmarks')}>
            <GameMode image='/src/assets/landmark_tlo.jpg' h2='Landmarks' p='Collection of landmarks throughout the world.' />
          </Link>
          <Link to="game" className="link" onClick={() => handleModeSelection('Historical Events')}>
            <GameMode image='/src/assets/historical_tlo.jpg' h2='Historical Events' p='Collection of historical events.' />
          </Link>
          <Link to="game" className="link" onClick={() => handleModeSelection('Hardcore')}>
            <GameMode image='/src/assets/hardcore_tlo.jpg' h2='Hardcore' p='Only for veterans, a mix of all game modes.' />
          </Link>
        </div>
      </GameModeContext.Provider>
    </div>
  );
}

export default GameModesPage;