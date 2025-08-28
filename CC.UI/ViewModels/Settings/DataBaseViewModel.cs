using CC.UI.ViewModels.Base;

namespace CC.UI.ViewModels.Settings;

public class DataBaseViewModel : ViewModel
{
    public Data.Entities.Settings.Settings CurrentSettings { get; set; }

    public DataBaseViewModel(Data.Entities.Settings.Settings currentSettings)
    {
        CurrentSettings = currentSettings;
    }
}