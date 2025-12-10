using System;
using System.Linq;
using System.Windows;
using CC.Core.Devices.Impl;
using CC.Core.Models.Base;
using CC.Core.Services;
using CC.Data.Entities.Settings;
#nullable enable

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

    public event Action SettingsChanged;


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

            StatisticCamera = new DeviceSettings()
            {
                Name = "Камера статистики",
                Ip = "172.25.40.5",
                Port = 23,
                IsUsed = true,
                Path = ""
            },

            ProductCamera1 = new DeviceSettings()
            {
                Name = "Камера считывания продукта (master)",
                Ip = "172.25.40.4",
                Port = 23,
                IsUsed = true,
                Path = ""
            },

            ProductCamera2 = new DeviceSettings()
            {
                Name = "Камера считывания продукта (slave)",
                Ip = "172.25.40.3",
                Port = 23,
                IsUsed = true,
                Path = ""
            },

            VerificationCamera1 = new DeviceSettings()
            {
                Name = "Камера верификации продукта",
                Ip = "172.25.40.6",
                Port = 23,
                IsUsed = true,
                Path = ""
            },

            VerificationCamera2 = new DeviceSettings()
            {
                Name = "Камера верификации короба",
                Ip = "172.25.40.7",
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

            TransportPrinter = new DeviceSettings()
            {
                Name = "Принтер транспортной этикетки",
                Ip = "172.25.4.245",
                Port = 9100,
                IsUsed = true,
                Path = ""
            },

            ServerDb = new DbSettings()
            {
                Name = "База данных (сервер)",
                ServerName = "localhost",
                DatabaseName = "CamFusion",
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
            settings.StatisticCamera == null || settings.ProductCamera1 == null || settings.ProductCamera2 == null ||
            settings.VerificationCamera1 == null || settings.VerificationCamera2 == null ||
            settings.BoxPrinter == null || settings.PalletPrinter == null || settings.TransportPrinter == null) return;
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
        SettingsChanged?.Invoke();
        MessageBox.Show("Настройки обновлены");
    }
}