import React, { useState } from 'react';
import {Link} from 'react-router-dom';
import './LandingPage.css';
import Footer from './Footer';

function LandingPage( { username }) {
  console.log("LandingPage is rendered");
  const [showRules, setShowRules] = useState(false);

  return (
    <>
      <div className="mainPage">
        <div className="userWindow">
          <h2>{username}</h2>
        </div>
        <div className="center">
          <h1>Welcome to <span className="highlight">WikiGuesser</span></h1>
          <div className="chooseLog">
            <Link to="/daily" className="choose">
                <a className="choose">Daily Challenge</a>
            </Link>
            <Link to="/gamemodes/game" className="choose">
              <a className="choose">Quick Play</a>
            </Link>
            {username ? (
                <>
                  <Link to="/lobby" className="choose">
                    <a className="choose">Join Lobby</a>
                  </Link>
                  
                  <button
                      className="choose logout-btn"
                      onClick={() => {
                        localStorage.removeItem('token');
                        localStorage.removeItem('username');
                        window.location.href = '/';
                      }}
                  >
                    Log out
                  </button>
                </>
            ) : (
                <Link to="/login" className="choose">
                  <a className="choose">Log in</a> 
                </Link>
            )}
            <div className="choose rules-button" onClick={() => setShowRules(!showRules)}>
              Game Rules
            </div>
          </div>
        </div>
        {showRules && (
            <div className="rules-overlay" onClick={() => setShowRules(false)}>
              <div className="rules-container" onClick={(e) => e.stopPropagation()}>
                <h2>How to Play WikiGuesser</h2>

                <div className="rules-section">
                  <h3>QuickPlay / Online Play</h3>
                  <ul>
                    <li>Read the Wikipedia article about a mystery location</li>
                    <li>Find clues in the text - country names are hidden</li>
                    <li>Place your marker on the world map</li>
                    <li>Submit your guess before the timer runs out (60 seconds)</li>
                    <li>Score points based on how close your guess is</li>
                    <li>Play 5 rounds and compete for the highest total score</li>
                    <li>In online mode, compete against other players in real-time</li>
                  </ul>
                </div>

                <div className="rules-section">
                  <h3>Daily Challenge</h3>
                  <ul>
                    <li>Each day features a new mystery city to find</li>
                    <li>Start with no information - make your first guess to reveal a hint</li>
                    <li>Each incorrect guess provides a new hint (weather, population, landmarks, etc.)</li>
                    <li>Your goal is to find the city within 50km in as few guesses as possible</li>
                    <li>You have a maximum of 7 guesses before game over</li>
                    <li>Challenge yourself to solve the puzzle with minimal hints!</li>
                  </ul>
                </div>

                <button className="close-rules-btn" onClick={() => setShowRules(false)}>
                  Close
                </button>
              </div>
            </div>
        )}
      </div>
      <Footer />
    </>
  );
}

export default LandingPage;