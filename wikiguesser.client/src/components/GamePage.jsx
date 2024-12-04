import React, { useEffect, useState } from 'react';
import './GamePage.css';
import InteractiveMap from './InteractiveMap';

function GamePage() {
    const [article, setArticle] = useState(""); 
    const [location, setLocation] = useState(null);
    const [position, setPosition] = useState(null);

    useEffect(() => {
        fetch('http://localhost:5084/api/wikipedia/cities')
            .then(response => response.text())
            .then(data => {
                console.log(data);
                setArticle(data); 
            })
            .catch(error => {
                console.error(error);
            });
    }, []);

    useEffect(() => {
        fetch(`http://localhost:5084/api/wikipedia/location/${article}`)
            .then(response => response.text())
            .then(data => {
                console.log(data);

                setLocation(data); 
            })
            .catch(error => {
                console.error(error);
            });
    }, [article]);

    console.log(position);

    return (
        <div className="gamePage">
            <div className="articleWindow" id="articleWindow">
                {/* Show the iframe only when the article is loaded */}
                {article ? (
                    <iframe
                        src={`https://pl.wikipedia.org/wiki/${article}`}
                        title="Wikipedia Article"
                        width="100%"
                        height="100%"
                    ></iframe>
                ) : (
                    <p>Loading article...</p>
                )}
            </div>
            <h1>{location}</h1>

            <InteractiveMap position={position} setPosition={setPosition} />
            <a className="submitButton" id="submitButton">
                Submit
            </a>
        </div>
    );
}

export default GamePage;
