import React, { useState, useRef, useEffect } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMapEvents, Polyline } from 'react-leaflet';

const InteractiveMap = ({ position, setPosition, submitted, location, onScoreUpdate }) => {
    const [distance, setDistance] = useState(null);
    const [score, setScore] = useState(null);
    const mapRef = useRef(null);
    function calculateDistance(coord1, coord2) {
        const R = 6371;
        const lat1 = coord1.lat;
        const lon1 = coord1.lng;
        const lat2 = coord2.lat;
        const lon2 = coord2.lng;
        const dLat = ((lat2 - lat1) * Math.PI) / 180;
        const dLon = ((lon2 - lon1) * Math.PI) / 180;
        const a = Math.sin(dLat / 2) * Math.sin(dLat / 2) + Math.cos((lat1 * Math.PI) / 180) * Math.cos((lat2 * Math.PI) / 180) * Math.sin(dLon / 2) * Math.sin(dLon / 2);
        const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        return R * c;
    }

    function calculateScore(distance) {
        const maxPoints = 10000;
        const points = Math.max(0, Math.round(maxPoints - distance * 5));
        return points;
    }

    

    function LocationMarker() {
        
        const map = useMapEvents({
            click(e) {
                setPosition(e.latlng);
                const dist = calculateDistance(e.latlng, location);
                setDistance(dist);
                const points = calculateScore(dist);
                setScore(points);
                onScoreUpdate(points, dist);
                console.log("Distance: ", dist, "Score: ", points);
            }
        });

        return position === null ? null : (
            <Marker position={position}>
            </Marker>
        );
    }

    useEffect(() => {
        const map = mapRef.current; // Referencja do mapy
        if (map) {
            const bounds = [
                [position.lat, position.lng],
                [location.lat, location.lng]
            ];
            map.fitBounds(bounds, { padding: [50, 50] }); // Automatyczne dopasowanie widoku
        }
    }, [submitted, position, location]);

    return (
        <div>
            <MapContainer
                center={[51.505, -0.09]} // Domy�lne wsp�rz�dne
                zoom={6}                // Poziom zoomu
                style={{ height: "90%", width: "40%", position: "absolute", top: "5%", left: "58%" }}
                whenCreated={mapInstance => {
                    mapRef.current = mapInstance;
                }}
            >
                <TileLayer
                    url="https://api.maptiler.com/maps/streets-v2/{z}/{x}/{y}.png?key=Mq3JD1tc90o3DGNaxqEO"
                    attribution="Tiles &copy; MapTiler &copy; OpenStreetMap contributors"
                />
                <LocationMarker />

                {submitted && position && location && (
                    <Polyline
                        positions={[position, location]} 
                        color="red"
                        dashArray="5, 5"
                    />

                )}

                {location && submitted && (
                    <Marker position={location}>
                        <Popup>To jest poprawna lokalizacja!</Popup>
                    </Marker>
                )}
            </MapContainer>

            {/* {submitted && distance !== null && (
                <div style={{ position: "absolute", top: "10%", left: "5%", fontSize: "20px", color: "black" }}>
                    <p>Odleg�o��: {distance.toFixed(2)} km</p>
                    <p>Punkty: {score}</p>
                </div>
            )} */}
        </div>
    );
};

export default InteractiveMap;
