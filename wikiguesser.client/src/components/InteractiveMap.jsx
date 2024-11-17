import React from 'react';
import { MapContainer, TileLayer, useMapEvents } from 'react-leaflet';

const InteractiveMap = () => {
    return (
        <MapContainer
            center={[51.505, -0.09]} // Default coordinates
            zoom={13}                // Zoom level
            style={{ height: "90%", width: "40%", position: "absolute", top: "5%", left: "58%" }}
        >
            <TileLayer
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                attribution="&copy; <a href='https://www.openstreetmap.org/copyright'>OpenStreetMap</a> contributors"
            />
        </MapContainer>
    );
};

export default InteractiveMap;
