import React from 'react';
import './GameModesPage.css';
import GameMode from './GameMode';
import { Link } from 'react-router-dom';

function GameModesPage() {
  return (
    <div className="gameModesPage">
          <h1>Game Modes</h1>
          <div className="gameModesContainer">
            <Link to="cities" className="link">
              <GameMode image='/src/assets/miasto_tlo.jpg' h2='Cities' p='Collection of cities throughout the world.' />
             </Link>
            <Link to="landmarks" className="link">
              <GameMode image='/src/assets/landmark_tlo.jpg' h2='Landmarks' p='Collection of landmarks throughout the world.' />
            </Link>
            <Link to="historical" className="link">
               <GameMode image='/src/assets/historical_tlo.jpg' h2='Historical Events' p='Collection of historical events.' />
            </Link>
            <Link to="hardcore" className="link">
               <GameMode image='/src/assets/hardcore_tlo.jpg' h2='Hardcore' p='Only for veterans, a mix of all game modes.' />
            </Link>
          </div>
    </div>
  );
}

export default GameModesPage;