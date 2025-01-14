import React, { useEffect, useState } from 'react';
import './GamePage.css';
import InteractiveMap from './InteractiveMap';
import Round from './Round';
function GamePage({ username }) {
    const [article, setArticle] = useState("");
    const [location, setLocation] = useState(null);
    const [position, setPosition] = useState(null);
    const [cityDesc, setCityDesc] = useState("");
    const [weather, setWeather] = useState(null);
    const [timezone, setTimezone] = useState(null);
    const [submitted, setSubmitted] = useState(false);
    const [score, setScore] = useState(0);
    const [distance, setDistance] = useState(null);
    const [totalScore, setTotalScore] = useState(0);
    const [timeLeft, setTimeLeft] = useState(60);
    const [round, setRound] = useState(1);
    const fetchData = async () => {
        try {
            let articleResponse = await fetch('http://localhost:5084/api/wikipedia/cities');
            let articleData = await articleResponse.text();
            
            setArticle(articleData);
            let cityDescResponse = await fetch(`http://localhost:5084/api/wikipedia/citydesc/${articleData}`);
            let cityDescData = await cityDescResponse.text();

            while (!cityDescResponse.ok || cityDescData == "Strona nie zosta�a znaleziona." || cityDescData.includes("may refer to") || cityDescData.includes("usually refers to") || cityDescData.includes("may mean")) {
                articleResponse = await fetch('http://localhost:5084/api/wikipedia/cities');
                articleData = await articleResponse.text();

                setArticle(articleData);
                cityDescResponse = await fetch(`http://localhost:5084/api/wikipedia/citydesc/${articleData}`);
                cityDescData = await cityDescResponse.text();
            }

            const locationResponse = await fetch(`http://localhost:5084/api/wikipedia/location/${articleData}`);
            const locationData = await locationResponse.json(); 
            setLocation(locationData);

            const weatherResponse = await fetch(`http://localhost:5084/api/wikipedia/citydesc/${articleData}/weather`);
            const weatherData = await weatherResponse.json();
            setWeather(weatherData.current.temp_c);
            setTimezone(weatherData.location.localtime);
            
            

            if (locationData?.countryName) {
                cityDescData = cityDescData.replace(new RegExp(locationData.countryName, 'g'), "COUNTRY");
            }
            setCityDesc(cityDescData);
        } catch (error) {
            console.error('Blad podczas pobierania danych:', error);
        }
    };

    useEffect(() => {
        fetchData();
    }, [round]);

    // useEffect(() => {
    //     setRound(round+1);
    // }, [submitted])

    const handleScoreUpdate = (newScore, newDistance) => {
        setScore(newScore);
        setDistance(newDistance);
    }

    useEffect(() => {
        if(submitted) return;

        if(timeLeft > 0){
            const timer = setTimeout(() => {
                setTimeLeft(timeLeft - 1);
            }, 1000);
            return () => clearTimeout(timer);
        }
        else{
            onSubmit();
        }
    });

    function onSubmit() {
        setSubmitted(true);
        const screen = document.getElementById("articleWindow");
        screen.style.filter = "blur(10px)";
        const scoreWindow = document.getElementById("scoreWindow");
        scoreWindow.style.display = "flex";
        setTotalScore(totalScore + score);
        clearTimeout();
        if (round < 5) {
            setTimeout(() => {
                handleNextRound();
            }, 5000);
        }
        else{
            setTimeout(() => {
                window.location.href = "/gamemodes";
            }, 5000);
        }
    }

    function handleNextRound() {
        setRound(round+1);
        setSubmitted(false);
        const screen = document.getElementById("articleWindow");
        screen.style.filter = "none";
        const scoreWindow = document.getElementById("scoreWindow");
        scoreWindow.style.display = "none";
        setScore(null);
        setDistance(null);
        setTimeLeft(60);
        setPosition(null);
    }

    return (
        <>
        <div className="gamePage" id="gamePage">
            <div className="userWindow">
                <h2>{username}</h2>
            </div>
            <div className="articleWindow" id="articleWindow">
                {article ? (
                    <>
                        <div dangerouslySetInnerHTML={{ __html: cityDesc }}></div>
                        <p>Temperature: {weather}C</p>
                        <p>Timezone: {timezone}</p>
                    </>
                ) : (
                    <p>Loading article...</p>
                )}
            </div>
            <InteractiveMap position={position} setPosition={setPosition} submitted={submitted} location={location} onScoreUpdate={handleScoreUpdate}/>
            <button className="submitButton" id="submitButton" onClick={onSubmit}>
                Submit
            </button>
            <Round round={round} timeLeft={timeLeft} totalScore={totalScore}/>
            
        </div>
        <div className="scoreWindow" id="scoreWindow">
            points: {score} <br/>distance: {distance}
            {/* <button className="closeButton" onClick={handleNextRound}>Next article</button> */}
        </div>
        </>
    );
}

export default GamePage;
