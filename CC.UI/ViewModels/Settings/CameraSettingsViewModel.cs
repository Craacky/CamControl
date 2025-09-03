using CC.UI.ViewModels.Base;

namespace CC.UI.ViewModels.Settings;

public class CameraSettingsViewModel:ViewModel
{
    public Data.Entities.Settings.Settings CurrentSettings { get; set; }


    public CameraSettingsViewModel(Data.Entities.Settings.Settings currentSettings)
    {
        CurrentSettings = currentSettings;
    }
}