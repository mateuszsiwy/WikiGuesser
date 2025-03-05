import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

const createSignalRConnection = (token) => {
    const connection = new HubConnectionBuilder()
        .withUrl('http://localhost:5084/chatHub', {
            accessTokenFactory: () => token,
        })
        .configureLogging(LogLevel.Information)
        .build();

    return connection;
};

export default createSignalRConnection;