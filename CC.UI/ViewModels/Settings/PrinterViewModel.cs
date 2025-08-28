using CC.UI.ViewModels.Base;

namespace CC.UI.ViewModels.Settings;

public class PrinterViewModel: ViewModel
{
    public Data.Entities.Settings.Settings CurrentSettings { get; set; }

    public PrinterViewModel(Data.Entities.Settings.Settings currentSettings)
    {
        CurrentSettings = currentSettings;
    }   
}