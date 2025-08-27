namespace SocketManager;

public enum SocketConnectionState
{
    ClientConnected,
    ClientDisconnected,
    ClientRecoveryConnection,
    ServerStarted,
    ServerStopped,
    ConnectionRefused
}

public static class SocketConnectionStateExtensions
{
    public static string ConvertToStringRus(this SocketConnectionState connectionState)
    {
        return connectionState switch
        {
            SocketConnectionState.ClientDisconnected => "Клиент отключён",
            SocketConnectionState.ClientConnected => "Клиент подключён",
            SocketConnectionState.ConnectionRefused => "Попытка подключения отклонена",
            SocketConnectionState.ClientRecoveryConnection => "Восстановление подключения",
            SocketConnectionState.ServerStopped => "Сервер остановлен",
            SocketConnectionState.ServerStarted => "Сервер запущен",
            _ => "",
        };
    }

    public static string ConvertToStringEng(this SocketConnectionState connectionState)
    {
        return connectionState switch
        {
            SocketConnectionState.ClientDisconnected => "Client disconnected",
            SocketConnectionState.ClientConnected => "Client is connected",
            SocketConnectionState.ConnectionRefused => "Connection attempt rejected",
            SocketConnectionState.ClientRecoveryConnection => "Reconnecting",
            SocketConnectionState.ServerStopped => "Server is stopped",
            SocketConnectionState.ServerStarted => "Server is running",
            _ => "",
        };
    }
}