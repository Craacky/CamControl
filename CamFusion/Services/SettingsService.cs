using System.Linq;
using System.Windows;
using CC.Core.Devices.Impl;
using CC.Core.Models.Base;
using CC.Core.Services;
using CC.Data.Entities.Settings;

namespace CamFusion.Services;

public class SettingsService : ObservableObject, ISettingsService
{
    private Settings? _settings;

    public Settings? Settings
    {
        get => _settings;
        set
        {
            _settings = value;
            OnPropertyChanged(nameof(Settings));
        }
    }


    public LocalDb LocalDbService { get; set; }


    public SettingsService(LocalDb localDbService)
    {
        LocalDbService = localDbService;

        Settings = new Settings
        {
            Line = new LineSettings()
            {
                FullName = "test",
                ShortName = "test",
                LineId = 3,
                PathSaveReportTaskFiles = @"",
                PathLoadNomenclatureFiles = @"",
                JobStoragePeriodInDays = 60
            },

            ProductCameraMaster = new DeviceSettings()
            {
                Name = "Камера считывания продукта (master)",
                Ip = "172.25.40.5",
                Port = 23,
                IsUsed = true,
                Path = ""
            },

            ProductCameraSlave = new DeviceSettings()
            {
                Name = "Камера считывания продукта (slave)",
                Ip = "172.25.40.4",
                Port = 23,
                IsUsed = true,
                Path = ""
            },

            BoxCamera = new DeviceSettings()
            {
                Name = "Камера считывания короба",
                Ip = "172.25.40.3",
                Port = 23,
                IsUsed = true,
                Path = ""
            },

            BoxPrinter = new DeviceSettings()
            {
                Name = "Принтер этикетки короба",
                Ip = "172.25.40.7",
                Port = 9100,
                IsUsed = false,
                Path = ""
            },

            PalletPrinter = new DeviceSettings()
            {
                Name = "Принтер этикетки паллеты",
                //Ip = "172.25.40.7",
                //Ip = "172.25.4.104",
                Ip = "172.25.4.244",
                Port = 9100,
                IsUsed = true,
                Path = ""
            },

            ServerDb = new DbSettings()
            {
                Name = "База данных (сервер)",
                ServerName = "localhost",
                DatabaseName = "camfusion",
                IsAuthentification = false,
                Login = "",
                Password = "",
                IsUsed = false,
            }
        };
    }

    public void LoadSettings()
    {
        var settings = LocalDbService.SettingsDataService.GetAll(s =>
                s.Line != null && Settings != null && Settings.Line != null && s.Line.LineId == Settings.Line.LineId)
            .LastOrDefault();
        if (settings == null) return;
        if (settings.ServerDb == null ||
            settings.ProductCameraSlave == null || settings.BoxCamera == null ||
            settings.ProductCameraMaster == null ||
            settings.BoxPrinter == null || settings.PalletPrinter == null) return;
        Settings = settings;
        Settings.Id = 0;
    }

    public void SaveSettings(Settings settings)
    {
        var lastSettings = LocalDbService.SettingsDataService.GetAll(s =>
                s.Line != null && Settings != null && Settings.Line != null && s.Line.LineId == Settings.Line.LineId)
            .LastOrDefault();

        if (lastSettings != null && lastSettings.Equals(settings)) return;
        Settings = settings;
        Settings.Id = 0;
        LocalDbService.SettingsDataService.Create(Settings);
        MessageBox.Show("Настройки обновлены");
    }
}