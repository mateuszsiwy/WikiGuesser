import {HubConnectionBuilder, LogLevel} from '@microsoft/signalr';

const createSignalRConnection = (token) => {
    console.log("🔹 Token JWT przekazany do SignalR:", token);
    const API_URL = import.meta.env.VITE_API_URL;

    const connection = new HubConnectionBuilder()
        .withUrl(`${API_URL}/chatHub`, {
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