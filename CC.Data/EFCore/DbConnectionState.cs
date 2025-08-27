namespace CC.Data.EFCore;

public enum DbConnectionState
{
    Connected,
    Created,
    NotFoundDb,
    Disconnected,
    InvalidConnectionString
}

public static class ConnectionStateExtensions
{
    public static string ConvertToStringRus(this DbConnectionState connectionState)
    {
        return connectionState switch
        {
            DbConnectionState.Connected => "База данных подключена",
            DbConnectionState.Created => "База данных создана и подключена",
            DbConnectionState.NotFoundDb => "Найдена база данных с данным именем, не соответсвующая формату",
            DbConnectionState.Disconnected => "База данных отключена",
            DbConnectionState.InvalidConnectionString => "Не корректная строка подключения",
            _ => "",
        };
    }

    public static string ConvertToStringEng(this DbConnectionState connectionState)
    {
        return connectionState switch
        {
            DbConnectionState.Connected => "The database is connected",
            DbConnectionState.Created => "The database has been created and connected",
            DbConnectionState.NotFoundDb => "Found a database with name data that does not match the format",
            DbConnectionState.Disconnected => "Database is disconnected",
            DbConnectionState.InvalidConnectionString => "Incorrect connection string",
            _ => "",
        };
    }
}