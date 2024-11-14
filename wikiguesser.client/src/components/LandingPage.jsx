import React from 'react';
import { Link } from 'react-router-dom';
import './LandingPage.css';
function LandingPage() {
    console.log("LandingPage is rendered");
  return (
    <div class="mainPage">
      <h1>WikiGuesser</h1>
          <Link to="/game">
            <a>Start Game</a>
          </Link>
    </div>
  );
}

export default LandingPage;