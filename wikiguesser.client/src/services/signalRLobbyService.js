import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';


const createLobbyConnection = (token) => {
    console.log("🔹 Token JWT przekazany do SignalR:", token);

    const connection = new HubConnectionBuilder()
        .withUrl('http://localhost:5084/lobbyHub', {
            accessTokenFactory: () => {
                console.log("📡 Wysyłanie tokena JWT do SignalR...");
                return token;
            },
        })
        .configureLogging(LogLevel.Information)
        .withAutomaticReconnect()
        .build();

    return connection;
};


export default createLobbyConnection;