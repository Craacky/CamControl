using CC.UI.ViewModels.Base;

namespace CC.UI.ViewModels.Settings;

public class PrinterSettingsViewModel : ViewModel
{
    public Data.Entities.Settings.Settings CurrentSettings { get; set; }

    public PrinterSettingsViewModel(Data.Entities.Settings.Settings currentSettings)
    {
        CurrentSettings = currentSettings;
    }
}