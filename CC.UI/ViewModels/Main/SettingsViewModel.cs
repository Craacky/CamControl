using System.Windows.Input;
using CC.Core.Commands.Base;
using CC.Core.Navigators;
using CC.Core.Services;
using CC.Core.Services.Impl;
using CC.UI.ViewModels.Base;
using CC.UI.ViewModels.Settings;

namespace CC.UI.ViewModels.Main;

public class SettingsViewModel : ViewModel
{
    private Data.Entities.Settings.Settings currentSettings;

    public Data.Entities.Settings.Settings CurrentSettings
    {
        get => currentSettings;
        set
        {
            currentSettings = value;
            OnPropertyChanged(nameof(CurrentSettings));
        }
    }


    public LineViewModel LineSettingsViewModel { get; set; }
    public CamerasViewModel CamerasSettingsViewModel { get; set; }
    public PrinterViewModel PrintersSettingsViewModel { get; set; }
    public DataBaseViewModel DataBasesSettingsViewModel { get; set; }


    private ICommand updateSettingsCommand;
    public ICommand UpdateSettingsCommand => updateSettingsCommand;

    private bool CanUpdateSettingsCommandExecute(object p)
    {
        return ReportTaskService.CurrentReportTask == null;
    }

    private void OnUpdateSettingsCommandExecuted(object p)
    {
        SettingsService.SaveSettings((Data.Entities.Settings.Settings)CurrentSettings.Clone());
        DeviceService.CreateDevice();
    }


    public SettingsNavigator Navigator { get; set; }
    public ISettingsService SettingsService { get; set; }
    public IDeviceService DeviceService { get; set; }
    public ReportTaskService ReportTaskService { get; set; }

    public SettingsViewModel(ISettingsService settingsService,
        IDeviceService deviceService,
        ReportTaskService reportTaskService)
    {
        SettingsService = settingsService;
        DeviceService = deviceService;
        ReportTaskService = reportTaskService;

        CurrentSettings = (Data.Entities.Settings.Settings)SettingsService.Settings.Clone();


        LineSettingsViewModel = new LineViewModel(CurrentSettings);
        CamerasSettingsViewModel = new CamerasViewModel(CurrentSettings);
        PrintersSettingsViewModel = new PrinterViewModel(CurrentSettings);
        DataBasesSettingsViewModel = new DataBaseViewModel(CurrentSettings);

        Navigator = new SettingsNavigator(LineSettingsViewModel,
            CamerasSettingsViewModel,
            PrintersSettingsViewModel,
            DataBasesSettingsViewModel);

        updateSettingsCommand = new RelayCommand(OnUpdateSettingsCommandExecuted, CanUpdateSettingsCommandExecute);

        Navigator.UpdateCurrentViewModelCommand.Execute(SettingsViewType.LineView);
    }
}