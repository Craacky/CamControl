using System.Windows.Input;
using CC.Core.Commands.Base;
using CC.Core.Services;
using CC.Core.Services.Impl;
using CC.UI.Navigators;
using CC.UI.ViewModels.Base;
using CC.UI.ViewModels.Settings;

namespace CC.UI.ViewModels.MainWindow;

public class SettingsViewModel:ViewModel
{
      private Data.Entities.Settings.Settings _currentSettings;
        public Data.Entities.Settings.Settings CurrentSettings 
        { 
            get => _currentSettings;
            set
            {
                _currentSettings = value;
                OnPropertyChanged(nameof(CurrentSettings));
            }
        }


        public LineSettingsViewModel LineSettingsViewModel { get; set; }
        public CameraSettingsViewModel CamerasSettingsViewModel { get; set; }
        public PrinterSettingsViewModel PrintersSettingsViewModel { get; set; }
        public DatabaseSettingsViewModel DataBasesSettingsViewModel { get; set; }


        private ICommand _updateSettingsCommand;
        public ICommand UpdateSettingsCommand => _updateSettingsCommand;
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


            LineSettingsViewModel = new LineSettingsViewModel(CurrentSettings);
            CamerasSettingsViewModel = new CameraSettingsViewModel(CurrentSettings);
            PrintersSettingsViewModel = new PrinterSettingsViewModel(CurrentSettings);
            DataBasesSettingsViewModel = new DatabaseSettingsViewModel(CurrentSettings);

            Navigator = new SettingsNavigator(LineSettingsViewModel,
                                              CamerasSettingsViewModel,
                                              PrintersSettingsViewModel,
                                              DataBasesSettingsViewModel);

            _updateSettingsCommand = new RelayCommand(OnUpdateSettingsCommandExecuted, CanUpdateSettingsCommandExecute);

            Navigator.UpdateCurrentViewModelCommand.Execute(SettingsViewType.LineView);
        }
}