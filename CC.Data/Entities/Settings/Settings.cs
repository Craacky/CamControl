using System;
using CC.Data.Entities.Base;

namespace CC.Data.Entities.Settings;

public class Settings : Entity, ICloneable
{
    private LineSettings _line;
    private DeviceSettings _statisticCamera;
    private DeviceSettings _productCamera1;
    private DeviceSettings _productCamera2;
    private DeviceSettings _verificationCamera1;
    private DeviceSettings _verificationCamera2;
    private DeviceSettings _boxPrinter;
    private DeviceSettings _palletPrinter;
    private DeviceSettings _transportPrinter;
    private DbSettings _serverDb;


    public LineSettings Line
    {
        get => _line;
        set
        {
            _line = value;
            OnPropertyChanged(nameof(Line));
        }
    }

    public DeviceSettings StatisticCamera
    {
        get => _statisticCamera;
        set
        {
            _statisticCamera = value;
            OnPropertyChanged(nameof(StatisticCamera));
        }
    }

    public DeviceSettings ProductCamera1
    {
        get => _productCamera1;
        set
        {
            _productCamera1 = value;
            OnPropertyChanged(nameof(ProductCamera1));
        }
    }

    public DeviceSettings ProductCamera2
    {
        get => _productCamera2;
        set
        {
            _productCamera2 = value;
            OnPropertyChanged(nameof(ProductCamera2));
        }
    }

    public DeviceSettings VerificationCamera1
    {
        get => _verificationCamera1;
        set
        {
            _verificationCamera1 = value;
            OnPropertyChanged(nameof(VerificationCamera1));
        }
    }

    public DeviceSettings VerificationCamera2
    {
        get => _verificationCamera2;
        set
        {
            _verificationCamera2 = value;
            OnPropertyChanged(nameof(VerificationCamera2));
        }
    }

    public DeviceSettings BoxPrinter
    {
        get => _boxPrinter;
        set
        {
            _boxPrinter = value;
            OnPropertyChanged(nameof(BoxPrinter));
        }
    }

    public DeviceSettings PalletPrinter
    {
        get => _palletPrinter;
        set
        {
            _palletPrinter = value;
            OnPropertyChanged(nameof(PalletPrinter));
        }
    }

    public DeviceSettings TransportPrinter
    {
        get => _transportPrinter;
        set
        {
            _transportPrinter = value;
            OnPropertyChanged(nameof(TransportPrinter));
        }
    }

    public DbSettings ServerDb
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

                StatisticCamera = new DeviceSettings()
                {
                    Name = StatisticCamera!.Name,
                    Ip = StatisticCamera.Ip,
                    Port = StatisticCamera.Port,
                    IsUsed = StatisticCamera.IsUsed,
                    Path = StatisticCamera.Path
                },

                ProductCamera1 = new DeviceSettings()
                {
                    Name = ProductCamera1!.Name,
                    Ip = ProductCamera1.Ip,
                    Port = ProductCamera1.Port,
                    IsUsed = ProductCamera1.IsUsed,
                    Path = ProductCamera1.Path
                },

                ProductCamera2 = new DeviceSettings()
                {
                    Name = ProductCamera2!.Name,
                    Ip = ProductCamera2.Ip,
                    Port = ProductCamera2.Port,
                    IsUsed = ProductCamera2.IsUsed,
                    Path = ProductCamera2.Path
                },

                VerificationCamera1 = new DeviceSettings()
                {
                    Name = VerificationCamera1!.Name,
                    Ip = VerificationCamera1.Ip,
                    Port = VerificationCamera1.Port,
                    IsUsed = VerificationCamera1.IsUsed,
                    Path = VerificationCamera1.Path
                },

                VerificationCamera2 = new DeviceSettings()
                {
                    Name = VerificationCamera2!.Name,
                    Ip = VerificationCamera2.Ip,
                    Port = VerificationCamera2.Port,
                    IsUsed = VerificationCamera2.IsUsed,
                    Path = VerificationCamera2.Path
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
                    Path = PalletPrinter.Path
                },

                TransportPrinter = new DeviceSettings()
                {
                    Name = TransportPrinter!.Name,
                    Ip = TransportPrinter.Ip,
                    Port = TransportPrinter.Port,
                    IsUsed = TransportPrinter.IsUsed,
                    Path = TransportPrinter.Path
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
            settings.Line.FullName == Line!.FullName &&
            settings.Line.ShortName == Line.ShortName &&
            settings.Line.LineId == Line.LineId &&
            settings.Line.PathSaveReportTaskFiles == Line.PathSaveReportTaskFiles &&
            settings.Line.PathLoadNomenclatureFiles == Line.PathLoadNomenclatureFiles &&
            settings.Line.JobStoragePeriodInDays == Line.JobStoragePeriodInDays &&
            settings.StatisticCamera!.Name == StatisticCamera!.Name &&
            settings.StatisticCamera.Ip == StatisticCamera.Ip &&
            settings.StatisticCamera.Port == StatisticCamera.Port &&
            settings.StatisticCamera.IsUsed == StatisticCamera.IsUsed &&
            settings.StatisticCamera.Path == StatisticCamera.Path &&
            settings.ProductCamera1!.Name == ProductCamera1!.Name &&
            settings.ProductCamera1.Ip == ProductCamera1.Ip &&
            settings.ProductCamera1.Port == ProductCamera1.Port &&
            settings.ProductCamera1.IsUsed == ProductCamera1.IsUsed &&
            settings.ProductCamera1.Path == ProductCamera1.Path &&
            settings.ProductCamera2!.Name == ProductCamera2!.Name &&
            settings.ProductCamera2.Ip == ProductCamera2.Ip &&
            settings.ProductCamera2.Port == ProductCamera2.Port &&
            settings.ProductCamera2.IsUsed == ProductCamera2.IsUsed &&
            settings.ProductCamera2.Path == ProductCamera2.Path &&
            settings.VerificationCamera1!.Name == VerificationCamera1!.Name &&
            settings.VerificationCamera1.Ip == VerificationCamera1.Ip &&
            settings.VerificationCamera1.Port == VerificationCamera1.Port &&
            settings.VerificationCamera1.IsUsed == VerificationCamera1.IsUsed &&
            settings.VerificationCamera1.Path == VerificationCamera1.Path &&
            settings.VerificationCamera2!.Name == VerificationCamera2!.Name &&
            settings.VerificationCamera2.Ip == VerificationCamera2.Ip &&
            settings.VerificationCamera2.Port == VerificationCamera2.Port &&
            settings.VerificationCamera2.IsUsed == VerificationCamera2.IsUsed &&
            settings.VerificationCamera2.Path == VerificationCamera2.Path &&
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
            settings.TransportPrinter!.Name == TransportPrinter!.Name &&
            settings.TransportPrinter.Ip == TransportPrinter.Ip &&
            settings.TransportPrinter.Port == TransportPrinter.Port &&
            settings.TransportPrinter.IsUsed == TransportPrinter.IsUsed &&
            settings.TransportPrinter.Path == TransportPrinter.Path &&
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
        return Equals(_line, other._line) && Equals(_statisticCamera, other._statisticCamera) && Equals(_productCamera1, other._productCamera1) && Equals(_productCamera2, other._productCamera2) && Equals(_verificationCamera1, other._verificationCamera1) && Equals(_verificationCamera2, other._verificationCamera2) && Equals(_boxPrinter, other._boxPrinter) && Equals(_palletPrinter, other._palletPrinter) && Equals(_transportPrinter, other._transportPrinter) && Equals(_serverDb, other._serverDb);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(_line);
        hash.Add(_statisticCamera);
        hash.Add(_productCamera1);
        hash.Add(_productCamera2);
        hash.Add(_verificationCamera1);
        hash.Add(_verificationCamera2);
        hash.Add(_boxPrinter);
        hash.Add(_palletPrinter);
        hash.Add(_transportPrinter);
        hash.Add(_serverDb);
        return hash.ToHashCode();
    }
}