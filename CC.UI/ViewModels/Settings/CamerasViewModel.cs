using CC.UI.ViewModels.Base;

namespace CC.UI.ViewModels.Settings;

public class CamerasViewModel:ViewModel
{
    public Data.Entities.Settings.Settings CurrentSettings { get; set; }


    public CamerasViewModel(Data.Entities.Settings.Settings currentSettings)
    {
        CurrentSettings = currentSettings;
    }
}