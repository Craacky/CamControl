using CC.Data.Entities.Base;

namespace CC.Data.Entities.Settings;

public class DbSettings : ObservableObject
{
    private string? _name;
    private string? _serverName;
    private string? _dataBaseName;
    private bool _isAuthentification;
    private string? _login;
    private string? _password;
    private bool _isUsed;

    public string? Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public string? ServerName
    {
        get => _serverName;
        set
        {
            _serverName = value;
            OnPropertyChanged(nameof(ServerName));
        }
    }

    public string? DatabaseName
    {
        get => _dataBaseName;
        set
        {
            _dataBaseName = value;
            OnPropertyChanged(nameof(DatabaseName));
        }
    }

    public bool IsAuthentification
    {
        get => _isAuthentification;
        set
        {
            _isAuthentification = value;
            OnPropertyChanged(nameof(IsAuthentification));
        }
    }

    public string? Login
    {
        get => _login;
        set
        {
            _login = value;
            OnPropertyChanged(nameof(Login));
        }
    }

    public string? Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    public bool IsUsed
    {
        get => _isUsed;
        set
        {
            _isUsed = value;
            OnPropertyChanged(nameof(IsUsed));
        }
    }

    public string ConnectionString => IsAuthentification
        ? @$"Data Source={ServerName};Initial Catalog={DatabaseName}; 
            User ID= {Login};Password= {Password};Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; 
                ApplicationIntent = ReadWrite; MultiSubnetFailover = False;"
        : @$"Data Source={ServerName};Initial Catalog={DatabaseName};Trusted_Connection=True;";
}