import React, {useEffect, useState} from 'react';
import './DailyChallenge.css';
import InteractiveMap from './InteractiveMap';

function DailyChallenge({username}) {
    const [city, setCity] = useState("");
    const [position, setPosition] = useState(null);
    const [location, setLocation] = useState(null);
    const [currentHint, setCurrentHint] = useState(null);
    const [hintLevel, setHintLevel] = useState(0);
    const [guesses, setGuesses] = useState(0);
    const [success, setSuccess] = useState(false);
    const [gameOver, setGameOver] = useState(false);
    const [loading, setLoading] = useState(true);
    const [hintData, setHintData] = useState({});
    const [distance, setDistance] = useState(null);
    const [error, setError] = useState(null);
    const [hasSubmitted, setHasSubmitted] = useState(false);
    const [submittedDistance, setSubmittedDistance] = useState(null);

    const MAX_GUESSES = 7;

    useEffect(() => {
        fetchDailyCity();
    }, []);

    const fetchDailyCity = async () => {
        try {
            setLoading(true);
            setError(null);
            const API_URL = import.meta.env.VITE_API_URL;

            const response = await fetch(`${API_URL}/api/wikipedia/daily`);

            if (!response.ok) {
                throw new Error(`Server responded with status: ${response.status}`);
            }

            const dailyCity = await response.text();
            console.log(dailyCity);
            if (!dailyCity || dailyCity.includes("Error")) {
                throw new Error("Invalid city data received");
            }

            setCity(dailyCity);
            const locationResponse = await fetch(`${API_URL}/api/wikipedia/location/${encodeURIComponent(dailyCity)}`);

            if (!locationResponse.ok) {
                throw new Error(`Failed to fetch location data: ${locationResponse.status}`);
            }

            const locationData = await locationResponse.json();

            if (!locationData || !locationData.latitude || !locationData.longitude) {
                throw new Error("Invalid location data received");
            }

            setLocation({
                lat: locationData.latitude,
                lng: locationData.longitude,
                countryName: locationData.countryName || "Unknown"
            });
        } catch (error) {
            console.error("Error fetching daily city:", error);
            setError(`Failed to load daily challenge: ${error.message}`);
        } finally {
            setLoading(false);
        }
    };

    const handleGuess = async () => {
        if (!position || !location) return;
        setHasSubmitted(true);
        const newGuessCount = guesses + 1;
        setGuesses(newGuessCount);

        const R = 6371; 
        const dLat = (position.lat - location.lat) * Math.PI / 180;
        const dLng = (position.lng - location.lng) * Math.PI / 180;
        const a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
            Math.cos(location.lat * Math.PI / 180) * Math.cos(position.lat * Math.PI / 180) *
            Math.sin(dLng / 2) * Math.sin(dLng / 2);
        const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        const distanceInKm = R * c;

        setDistance(Math.round(distanceInKm));
        setSubmittedDistance(Math.round(distanceInKm));

        if (distanceInKm < 50) {
            setSuccess(true);
        } else {
            if (newGuessCount >= MAX_GUESSES) {
                setGameOver(true);
                return;
            }

            const newHintLevel = hintLevel + 1;
            setHintLevel(newHintLevel);

            try {
                const API_URL = import.meta.env.VITE_API_URL;

                const response = await fetch(`${API_URL}/api/wikipedia/daily/${encodeURIComponent(city)}/hint/${newHintLevel}`);

                if (!response.ok) {
                    throw new Error(`Failed to fetch hint: ${response.status}`);
                }

                const hintResponse = await response.json();
                console.log("Received hint:", hintResponse);
                setCurrentHint(hintResponse);

                const updatedHintData = {
                    ...hintData
                };
                updatedHintData[hintResponse.type] = hintResponse.content;
                setHintData(updatedHintData);
            } catch (error) {
                console.error("Error fetching hint:", error);
            }
        }
    };

    if (loading) {
        return <div className="daily-challenge loading">Loading today's challenge...</div>;
    }

    if (error) {
        return (
            <div className="daily-challenge error">
                <h2>Oops! Something went wrong</h2>
                <p>{error}</p>
                <button onClick={fetchDailyCity} className="retry-button">
                    Retry
                </button>
            </div>
        );
    }

    const handleScoreUpdate = (newScore, newDistance) => {
        setDistance(newDistance);
    };

    return (
        <div className="daily-challenge">
            <h1>Daily Challenge</h1>
            <div className="challenge-header">
                <p>Today's challenge: Guess the mystery city!</p>
                <p>Your guesses so far: {guesses} / {MAX_GUESSES}</p>
            </div>

            {!success && !gameOver ? (
                <>
                    <div className="hints-container">
                        <h3>Hints Revealed:</h3>
                        {hintLevel === 0 && (
                            <p className="initial-hint">Make your first guess to reveal a hint!</p>
                        )}

                        {hasSubmitted && Object.keys(hintData).map((hintType) => {
                            const hintContent = hintData[hintType];

                            switch (hintType) {
                                case 'Weather':
                                    return (
                                        <div className="hint" key={hintType}>
                                            <h4>Weather:</h4>
                                            <p>Current temperature: {hintContent.current?.tempC}°C</p>
                                            <p>Local time: {hintContent.location?.localTime}</p>
                                        </div>
                                    );
                                case 'Population':
                                    return (
                                        <div className="hint" key={hintType}>
                                            <h4>Population:</h4>
                                            <p>{hintContent} people</p>
                                        </div>
                                    );
                                case 'Landmarks':
                                    return (
                                        <div className="hint" key={hintType}>
                                            <h4>Notable Places:</h4>
                                            <ul>
                                                {Array.isArray(hintContent)
                                                    ? hintContent.map((landmark, i) => (
                                                        <li key={i}>{landmark}</li>
                                                    ))
                                                    : <li>Information not available</li>
                                                }
                                            </ul>
                                        </div>
                                    );
                                case 'History':
                                    return (
                                        <div className="hint" key={hintType}>
                                            <h4>Historical Background:</h4>
                                            <div dangerouslySetInnerHTML={{__html: hintContent}}></div>
                                        </div>
                                    );
                                case 'ImageClue':
                                    return (
                                        <div className="hint" key={hintType}>
                                            <h4>Image Clue:</h4>
                                            <img
                                                src={`https://commons.wikimedia.org/wiki/Special:FilePath/${hintContent}?width=400`}
                                                alt="City clue"
                                                onError={(e) => {
                                                    e.target.onerror = null;
                                                    e.target.src = "https://via.placeholder.com/400x300?text=Image+Not+Available"
                                                }}
                                            />
                                        </div>
                                    );
                                default:
                                    return null;
                            }
                        })}

                        {hasSubmitted && submittedDistance && (
                            <div className="distance-feedback">
                                <p>Your last guess was {submittedDistance}km away from the target.</p>
                            </div>
                        )}
                    </div>

                    <div className="map-container">
                        <InteractiveMap
                            position={position}
                            setPosition={setPosition}
                            submitted={false}
                            location={location}
                            onScoreUpdate={handleScoreUpdate}
                        />
                    </div>

                    <button
                        className="guess-button"
                        onClick={handleGuess}
                        disabled={!position}
                    >
                        Submit Guess
                    </button>
                </>
            ) : success ? (
                <div className="success-container">
                    <h2>Congratulations!</h2>
                    <p>You found {city} in {guesses} guesses!</p>
                    <div className="city-info">
                        
                    </div>
                    <p>Come back tomorrow for a new challenge!</p>
                </div>
            ) : (
                <div className="game-over-container">
                    <h2>Game Over</h2>
                    <p>You've used all {MAX_GUESSES} guesses!</p>
                    <p>The mystery city was <strong>{city}</strong>.</p>
                    <div className="city-info">
                        
                    </div>
                    <p>Better luck tomorrow!</p>
                </div>
            )}
        </div>
    );
}

export default DailyChallenge;