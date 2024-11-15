import React from 'react';
import { Link } from 'react-router-dom';
import './LandingPage.css';
import Footer from './Footer';

function LandingPage() {
  console.log("LandingPage is rendered");
  return (
    <>
      <div className="mainPage">
        <div className="center">
          <h1>Welcome to WikiGuesser</h1>
          <div className="chooseLog">
            <Link to="/game" className="choose">
              <a className="choose">Play as a guest</a>
            </Link>
            <a className="choose">Log in</a>
          </div>
        </div>
      </div>
      <Footer />
    </>
  );
}

export default LandingPage;