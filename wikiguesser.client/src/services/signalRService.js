import {HubConnectionBuilder, LogLevel} from '@microsoft/signalr';

const createSignalRConnection = (token) => {
    console.log("🔹 Token JWT przekazany do SignalR:", token);

    const connection = new HubConnectionBuilder()
        .withUrl('http://localhost:5084/chatHub', {
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



export default createSignalRConnection;