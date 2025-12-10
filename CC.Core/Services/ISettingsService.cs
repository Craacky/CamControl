using CC.Core.Devices.Impl;
using CC.Data.Entities.Settings;

namespace CC.Core.Services;

using System;

public interface ISettingsService
{
    LocalDb LocalDbService { get; set; }
    Settings? Settings { get; set; }

    event Action SettingsChanged;

    void LoadSettings();
    void SaveSettings(Settings settings);
}