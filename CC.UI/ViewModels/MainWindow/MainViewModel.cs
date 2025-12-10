using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CC.Core.Commands.Base;
using CC.Core.Services;
using CC.Core.Services.Impl;
using CC.UI.ViewModels.Base;

namespace CC.UI.ViewModels.MainWindow;

public class MainViewModel:ViewModel
{
     private ICommand pauseCurrentTaskCommand;
        public ICommand PauseCurrentTaskCommand => pauseCurrentTaskCommand;
        private bool CanPauseCurrentTaskCommandExecute(object p)
        {
            return ReportTaskService.CurrentReportTask != null && ReportTaskService.CurrentReportTask.Status == "Запущено";
        }
        private void OnPauseCurrentTaskCommandExecuted(object p)
        {
            if (ReportTaskService.CurrentReportTask == null) return;
            ReportTaskService.CurrentReportTask.Status = "Остановлено";
            ReportTaskService.UpdateReportTask(ReportTaskService.CurrentReportTask);
            DeviceService.StopDevices();
        }
     private ICommand closePalletCommand;
        public ICommand ClosePalletCommand => closePalletCommand;
        private bool CanClosePalletCommandExecute(object p)
        {
            bool isClosedPallet = ReportTaskService.Statistic.PalletCodes.Count > 0 && ReportTaskService.Statistic.PalletCodes[^1].IsFulled;
            return !isClosedPallet && (ReportTaskService.Statistic.PalletCodes.Count > 0 && ReportTaskService.Statistic.CountBoxInCurrentPallet > 0) && ReportTaskService.CurrentReportTask != null;
        }
        private void OnClosePalletCommandlCommandExecuted(object p)
        {
            DeviceService.ReportTaskService.ClosePallet();
            MessageBox.Show("Паллета закрыта");
            DeviceService.PalletPrinterService?.PrintCode();
        }

        private ICommand addBoxesToPreviousPalletCommand;
        public ICommand AddBoxesToPreviousPalletCommand => addBoxesToPreviousPalletCommand;
        private bool CanAddBoxesToPreviousPalletCommandExecute(object p)
        {
            return ReportTaskService.Statistic.PalletCodes.Count > 1 && ReportTaskService.CurrentReportTask != null;
        }
        private void OnAddBoxesToPreviousPalletCommandExecuted(object p)
        {
            DeviceService.ReportTaskService.AddBoxesToPreviousPallet();
            DeviceService.PalletPrinterService?.PrintCode();
        }


        public ReportTaskService ReportTaskService { get; set; }
        public IDeviceService DeviceService { get; set; }
        public ISettingsService ISettingsService { get; set; }
        public ErrorsService ErrorsService { get; set; }

        public MainViewModel(ReportTaskService? reportTaskService,
                             ISettingsService settingsService,
                             IDeviceService deviceService)
        {
            ReportTaskService = reportTaskService;
            ISettingsService = settingsService;
            DeviceService = deviceService;

            closePalletCommand = new RelayCommand(OnClosePalletCommandlCommandExecuted, CanClosePalletCommandExecute);
            addBoxesToPreviousPalletCommand = new RelayCommand(OnAddBoxesToPreviousPalletCommandExecuted, CanAddBoxesToPreviousPalletCommandExecute);
            pauseCurrentTaskCommand = new RelayCommand(OnPauseCurrentTaskCommandExecuted, CanPauseCurrentTaskCommandExecute);
        }

}