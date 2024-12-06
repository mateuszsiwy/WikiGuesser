import React, { useEffect, useState } from 'react';
import './GamePage.css';
import InteractiveMap from './InteractiveMap';

function GamePage() {
    const [article, setArticle] = useState("");
    const [location, setLocation] = useState(null);
    const [position, setPosition] = useState(null);
    const [cityDesc, setCityDesc] = useState("");
    const [weather, setWeather] = useState(null);

    // Funkcja pomocnicza do pobierania danych
    const fetchData = async () => {
        try {
            // Pobierz artyku³
            let articleResponse = await fetch('http://localhost:5084/api/wikipedia/cities');
            let articleData = await articleResponse.text();
            
            setArticle(articleData);
            // Pobierz opis miasta
            let cityDescResponse = await fetch(`http://localhost:5084/api/wikipedia/citydesc/${articleData}`);
            let cityDescData = await cityDescResponse.text();

            while (!cityDescResponse.ok || cityDescData == "Strona nie zosta³a znaleziona.") {
                articleResponse = await fetch('http://localhost:5084/api/wikipedia/cities');
                articleData = await articleResponse.text();

                setArticle(articleData);
                // Pobierz opis miasta
                cityDescResponse = await fetch(`http://localhost:5084/api/wikipedia/citydesc/${articleData}`);
                cityDescData = await cityDescResponse.text();
            }

            // Pobierz lokalizacjê
            const locationResponse = await fetch(`http://localhost:5084/api/wikipedia/location/${articleData}`);
            const locationData = await locationResponse.json(); // Zmienione na JSON
            setLocation(locationData);

            // Pobierz dane pogodowe
            const weatherResponse = await fetch(`http://localhost:5084/api/wikipedia/citydesc/${articleData}/weather`);
            const weatherData = await weatherResponse.json();
            setWeather(weatherData.current.temp_c);

            
            

            if (locationData?.countryName) {
                // Zamieñ nazwê kraju w opisie
                cityDescData = cityDescData.replace(new RegExp(locationData.countryName, 'g'), "COUNTRY");
            }
            setCityDesc(cityDescData);
        } catch (error) {
            console.error('B³¹d podczas pobierania danych:', error);
        }
    };

    // Wywo³aj fetchData przy renderowaniu komponentu
    useEffect(() => {
        fetchData();
    }, []);

    return (
        <div className="gamePage">
            <div className="articleWindow" id="articleWindow">
                {article ? (
                    <>
                        <div dangerouslySetInnerHTML={{ __html: cityDesc }}></div>
                        <p>Temperature: {weather}C</p>
                    </>
                ) : (
                    <p>Loading article...</p>
                )}
            </div>
            <InteractiveMap position={position} setPosition={setPosition} />
            <button className="submitButton" id="submitButton" onClick={() => console.log('Submit clicked')}>
                Submit
            </button>
        </div>
    );
}

export default GamePage;
