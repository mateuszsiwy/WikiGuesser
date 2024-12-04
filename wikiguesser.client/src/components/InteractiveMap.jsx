import React, { useState } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMapEvents } from 'react-leaflet';

const InteractiveMap = ({position, setPosition }) => {
    function LocationMarker() {
/*        const [position, setPosition] = useState(null);
*/        const map = useMapEvents({
            click(e) {
                setPosition(e.latlng);
            }
        });

        return position === null ? null : (
            <Marker position={position}>
                <Popup>You are here</Popup>
            </Marker>
        );
    }

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
            <LocationMarker />
        </MapContainer>

    );
};

export default InteractiveMap;
