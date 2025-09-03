using System.Windows.Input;
using CC.Core.Commands.Base;
using CC.Core.Models.Base;
using CC.UI.ViewModels.Base;
using CC.UI.ViewModels.MainWindow;
using CC.UI.ViewModels.Windows;

namespace CC.UI.Navigators;

public enum MainWindowViewType
    {
       MainView,
       ReportTasksView,
       HandleAggregationView,
       EventsView,
       PrinterView,
       ErrorsView,
       SettingsView
    }

    public class MainWindowNavigator : ObservableObject
    {
        private ViewModel _currentViewModel;
        public ViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }


        public MainViewModel MainViewModel { get; set; }
        public TasksViewModel ReportTasksViewModel { get; set; }
        public AggregationViewModel HandleAggregationViewModel { get; set; }
        public EventsViewModel EventsViewModel { get; set; }
        public PrinterViewModel PrinterViewModel { get; set; }
        public ErrorsViewModel ErrorsViewModel { get; set; }
        public SettingsViewModel SettingsViewModel { get; set; }


        private ICommand updateCurrentViewModelCommand;
        public ICommand UpdateCurrentViewModelCommand => updateCurrentViewModelCommand;
        private bool CanUpdateCurrentViewModelCommandExecute(object parameter) => true;
        private void OnUpdateCurrentViewModelCommandExecuted(object parameter)
        {
            if (parameter is MainWindowViewType viewType)
            {
                switch (viewType)
                {
                    case MainWindowViewType.MainView:
                        CurrentViewModel = MainViewModel;
                        break;
                    case MainWindowViewType.ReportTasksView:
                        CurrentViewModel = ReportTasksViewModel;
                        break;
                    case MainWindowViewType.HandleAggregationView:
                        CurrentViewModel = HandleAggregationViewModel;
                        break;
                    case MainWindowViewType.EventsView:
                        CurrentViewModel = EventsViewModel;
                        break;
                    case MainWindowViewType.PrinterView:
                        CurrentViewModel = PrinterViewModel;
                        break;
                    case MainWindowViewType.ErrorsView:
                        CurrentViewModel = ErrorsViewModel;
                        break;
                    case MainWindowViewType.SettingsView:
                        CurrentViewModel = new SettingsViewModel(SettingsViewModel.SettingsService, SettingsViewModel.DeviceService, SettingsViewModel.ReportTaskService);
                        break;
                    default:
                        break;
                }
            }
        }


        public MainWindowNavigator(MainViewModel mainViewModel, 
                                   TasksViewModel reportTasksViewModel, 
                                   AggregationViewModel handleAggregationViewModel, 
                                   EventsViewModel eventsViewModel, 
                                   PrinterViewModel printerViewModel, 
                                   ErrorsViewModel errorsViewModel,
                                   SettingsViewModel settingsViewModel)
        {
            MainViewModel = mainViewModel;
            ReportTasksViewModel = reportTasksViewModel;
            HandleAggregationViewModel = handleAggregationViewModel;
            EventsViewModel = eventsViewModel;
            PrinterViewModel = printerViewModel;
            ErrorsViewModel = errorsViewModel;
            SettingsViewModel = settingsViewModel;

            updateCurrentViewModelCommand = new RelayCommand(OnUpdateCurrentViewModelCommandExecuted, CanUpdateCurrentViewModelCommandExecute);
        }
    }