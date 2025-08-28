using CC.Core.Devices.Impl;
using CC.Data.Entities.Settings;

namespace CC.Core.Services;

public interface ISettingsService
{
    LocalDb LocalDbService { get; set; }
    Settings Settings { get; set; }

    void LoadSettings();
    void SaveSettings(Settings settings);
}