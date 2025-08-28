using CC.Data.Entities.Base;

namespace CC.Data.Entities.Settings;

public class DeviceSettings : ObservableObject
{
    private string? _name;
    private string? _ip;
    private int _port;
    private bool _isUsed;
    private string? _path;

    public string? Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public string? Ip
    {
        get => _ip;
        set
        {
            _ip = value;
            OnPropertyChanged(nameof(Ip));
        }
    }

    public int Port
    {
        get => _port;
        set
        {
            _port = value;
            OnPropertyChanged(nameof(Port));
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

    public string? Path
    {
        get => _path;
        set
        {
            _path = value;
            OnPropertyChanged(nameof(Path));
        }
    }
}