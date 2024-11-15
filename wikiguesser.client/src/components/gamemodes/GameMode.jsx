import React from 'react';
import './Gamemode.css';

function GameMode({ image, h2, p }) {
    const divStyle = {
        backgroundImage: `url(${image})`,
        backgroundSize: 'cover', 
        backgroundPosition: 'center' 
    };

    return (

        <a>
        <div className="gameMode" style={divStyle}>
            <h2>{h2}</h2>
            <p>{p}</p>
            </div>
        </a>
    );
}

export default GameMode;
