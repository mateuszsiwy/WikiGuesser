import React, { useEffect, useState } from 'react';
import './GamePage.css';
import InteractiveMap from './InteractiveMap';

function GamePage() {
    const [article, setArticle] = useState(""); // Use state to track the article name

    useEffect(() => {
        // Fetch the article from the API
        fetch('http://localhost:5084/api/wikipedia/cities')
            .then(response => response.text())
            .then(data => {
                // Update the state with the fetched article name
                console.log(data);
                setArticle(data); // Trigger a re-render
            })
            .catch(error => {
                // Handle any errors that occur during the API call
                console.error(error);
            });
    }, []);

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
            <InteractiveMap />
        </div>
    );
}

export default GamePage;
