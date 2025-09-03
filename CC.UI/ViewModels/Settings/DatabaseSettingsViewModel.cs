using CC.UI.ViewModels.Base;

namespace CC.UI.ViewModels.Settings;

public class DatabaseSettingsViewModel:ViewModel
{
    public Data.Entities.Settings.Settings CurrentSettings { get; set; }

    public DatabaseSettingsViewModel(Data.Entities.Settings.Settings currentSettings)
    {
        CurrentSettings = currentSettings;
    }
}