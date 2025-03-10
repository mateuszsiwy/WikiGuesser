import React from 'react';
import {Link} from 'react-router-dom';
import './LandingPage.css';
import Footer from './Footer';

function LandingPage( { username }) {
  console.log("LandingPage is rendered");
  return (
    <>
      <div className="mainPage">
        <div className="userWindow">
          <h2>{username}</h2>
        </div>
        <div className="center">
          <h1>Welcome to <span className="highlight">WikiGuesser</span></h1>
          <div className="chooseLog">
            <Link to="/gamemodes" className="choose">
              <a className="choose">Play as a guest</a>
            </Link>
            <Link to="/login" className="choose">
              <a className="choose">Log in</a>
            </Link>
          </div>
        </div>
      </div>
      <Footer />
    </>
  );
}

export default LandingPage;