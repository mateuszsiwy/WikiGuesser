import React from 'react';
import './styles/Round.css';

const Round = ({ round, timeLeft, totalScore }) => {
    return (
        <div className="roundInfo">
            <p>Round: {round}/5</p>
            <p>Time left: {Math.floor(timeLeft / 60)}:{timeLeft % 60 < 10 ? `0${timeLeft % 60}` : timeLeft % 60}</p>
            <p>Total Score: {totalScore}</p>
        </div>
    );
};

export default Round;