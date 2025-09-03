using System;
using CC.Data.Entities.Base;

namespace CC.Data.Entities.Settings;

public class Settings : Entity, ICloneable
{
    private LineSettings? _line;
    private DeviceSettings? _productCameraMaster;
    private DeviceSettings? _productCameraSlave;
    private DeviceSettings? _boxCamera;
    private DeviceSettings? _boxPrinter;
    private DeviceSettings? _palletPrinter;
    private DbSettings? _serverDb;


    public LineSettings? Line
    {
        get => _line;
        set
        {
            _line = value;
            OnPropertyChanged(nameof(Line));
        }
    }

    public DeviceSettings? ProductCameraMaster
    {
        get => _productCameraMaster;
        set
        {
            _productCameraMaster = value;
            OnPropertyChanged(nameof(ProductCameraMaster));
        }
    }

    public DeviceSettings? ProductCameraSlave
    {
        get => _productCameraSlave;
        set
        {
            _productCameraSlave = value;
            OnPropertyChanged(nameof(ProductCameraSlave));
        }
    }

    public DeviceSettings? BoxCamera
    {
        get => _boxCamera;
        set
        {
            _boxCamera = value;
            OnPropertyChanged(nameof(BoxCamera));
        }
    }

    public DeviceSettings? BoxPrinter
    {
        get => _boxPrinter;
        set
        {
            _boxPrinter = value;
            OnPropertyChanged(nameof(BoxPrinter));
        }
    }

    public DeviceSettings? PalletPrinter
    {
        get => _palletPrinter;
        set
        {
            _palletPrinter = value;
            OnPropertyChanged(nameof(PalletPrinter));
        }
    }

    public DbSettings? ServerDb
    {
        get => _serverDb;
        set
        {
            _serverDb = value;
            OnPropertyChanged(nameof(ServerDb));
        }
    }

    public static DbSettings LocalDb { get; set; }

    static Settings()
    {
        LocalDb = new DbSettings()
        {
            Name = "База данных (локальная)",
            ServerName = "localhost",
            DatabaseName = "CamFusion",
            IsAuthentification = false,
            Login = null,
            Password = null,
            IsUsed = true,
        };
    }

    public object Clone()
    {
        Settings newSettings
            = new Settings
            {
                Line = new LineSettings()
                {
                    FullName = Line!.FullName,
                    ShortName = Line.ShortName,
                    LineId = Line.LineId,
                    PathSaveReportTaskFiles = Line.PathSaveReportTaskFiles,
                    PathLoadNomenclatureFiles = Line.PathLoadNomenclatureFiles,
                    JobStoragePeriodInDays = Line.JobStoragePeriodInDays
                },

                ProductCameraMaster = new DeviceSettings()
                {
                    Name = ProductCameraMaster!.Name,
                    Ip = ProductCameraMaster.Ip,
                    Port = ProductCameraMaster.Port,
                    IsUsed = ProductCameraMaster.IsUsed,
                    Path = ProductCameraMaster.Path
                },

                ProductCameraSlave = new DeviceSettings()
                {
                    Name = ProductCameraSlave!.Name,
                    Ip = ProductCameraSlave.Ip,
                    Port = ProductCameraSlave.Port,
                    IsUsed = ProductCameraSlave.IsUsed,
                    Path = ProductCameraSlave.Path
                },

                BoxCamera = new DeviceSettings()
                {
                    Name = BoxCamera!.Name,
                    Ip = BoxCamera.Ip,
                    Port = BoxCamera.Port,
                    IsUsed = BoxCamera.IsUsed,
                    Path = BoxCamera.Path
                },

                BoxPrinter = new DeviceSettings()
                {
                    Name = BoxPrinter!.Name,
                    Ip = BoxPrinter.Ip,
                    Port = BoxPrinter.Port,
                    IsUsed = BoxPrinter.IsUsed,
                    Path = BoxPrinter.Path
                },

                PalletPrinter = new DeviceSettings()
                {
                    Name = PalletPrinter!.Name,
                    Ip = PalletPrinter.Ip,
                    Port = PalletPrinter.Port,
                    IsUsed = PalletPrinter.IsUsed,
                    Path = BoxPrinter.Path
                },

                ServerDb = new DbSettings()
                {
                    Name = ServerDb!.Name,
                    ServerName = ServerDb.ServerName,
                    DatabaseName = ServerDb.DatabaseName,
                    IsAuthentification = ServerDb.IsAuthentification,
                    Login = ServerDb.Login,
                    Password = ServerDb.Password,
                    IsUsed = ServerDb.IsUsed,
                }
            };

        return newSettings;
    }

    public override bool Equals(object? obj)
    {
        Settings? settings = obj as Settings;
        var isEqual =
            settings != null &&
            settings.Line != null &&
            settings.Line.FullName == Line!.FullName &&
            settings.Line.ShortName == Line.ShortName &&
            settings.Line.LineId == Line.LineId &&
            settings.Line.PathSaveReportTaskFiles == Line.PathSaveReportTaskFiles &&
            settings.Line.PathLoadNomenclatureFiles == Line.PathLoadNomenclatureFiles &&
            settings.Line.JobStoragePeriodInDays == Line.JobStoragePeriodInDays &&
            settings.ProductCameraMaster!.Name == ProductCameraMaster!.Name &&
            settings.ProductCameraMaster.Ip == ProductCameraMaster.Ip &&
            settings.ProductCameraMaster.Port == ProductCameraMaster.Port &&
            settings.ProductCameraMaster.IsUsed == ProductCameraMaster.IsUsed &&
            settings.ProductCameraMaster.Path == ProductCameraMaster.Path &&
            settings.ProductCameraSlave!.Name == ProductCameraSlave!.Name &&
            settings.ProductCameraSlave.Ip == ProductCameraSlave.Ip &&
            settings.ProductCameraSlave.Port == ProductCameraSlave.Port &&
            settings.ProductCameraSlave.IsUsed == ProductCameraSlave.IsUsed &&
            settings.ProductCameraSlave.Path == ProductCameraSlave.Path &&
            settings.BoxCamera!.Name == BoxCamera!.Name &&
            settings.BoxCamera.Ip == BoxCamera.Ip &&
            settings.BoxCamera.Port == BoxCamera.Port &&
            settings.BoxCamera.IsUsed == BoxCamera.IsUsed &&
            settings.BoxCamera.Path == BoxCamera.Path &&
            settings.BoxPrinter!.Name == BoxPrinter!.Name &&
            settings.BoxPrinter.Ip == BoxPrinter.Ip &&
            settings.BoxPrinter.Port == BoxPrinter.Port &&
            settings.BoxPrinter.IsUsed == BoxPrinter.IsUsed &&
            settings.BoxPrinter.Path == BoxPrinter.Path &&
            settings.PalletPrinter!.Name == PalletPrinter!.Name &&
            settings.PalletPrinter.Ip == PalletPrinter.Ip &&
            settings.PalletPrinter.Port == PalletPrinter.Port &&
            settings.PalletPrinter.IsUsed == PalletPrinter.IsUsed &&
            settings.PalletPrinter.Path == PalletPrinter.Path &&
            settings.ServerDb!.Name == ServerDb!.Name &&
            settings.ServerDb.ServerName == ServerDb.ServerName &&
            settings.ServerDb.DatabaseName == ServerDb.DatabaseName &&
            settings.ServerDb.IsAuthentification == ServerDb.IsAuthentification &&
            settings.ServerDb.Login == ServerDb.Login &&
            settings.ServerDb.Password == ServerDb.Password &&
            settings.ServerDb.IsUsed == ServerDb.IsUsed;

        return isEqual;
    }

    protected bool Equals(Settings other)
    {
        return Equals(_line, other._line) && Equals(_productCameraMaster, other._productCameraMaster) && Equals(_productCameraSlave, other._productCameraSlave) && Equals(_boxCamera, other._boxCamera) && Equals(_boxPrinter, other._boxPrinter) && Equals(_palletPrinter, other._palletPrinter) && Equals(_serverDb, other._serverDb);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_line, _productCameraMaster, _productCameraSlave, _boxCamera, _boxPrinter, _palletPrinter, _serverDb);
    }
}