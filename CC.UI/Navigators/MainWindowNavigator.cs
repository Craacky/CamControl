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
        public LoginViewModel LoginViewModel { get; set; }

        public bool IsSettingsAuthorized { get; set; }


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
                        if (CurrentViewModel is SettingsViewModel)
                            IsSettingsAuthorized = false;
                        CurrentViewModel = MainViewModel;
                        break;
                    case MainWindowViewType.ReportTasksView:
                        if (CurrentViewModel is SettingsViewModel)
                            IsSettingsAuthorized = false;
                        CurrentViewModel = ReportTasksViewModel;
                        break;
                    case MainWindowViewType.HandleAggregationView:
                        if (CurrentViewModel is SettingsViewModel)
                            IsSettingsAuthorized = false;
                        CurrentViewModel = HandleAggregationViewModel;
                        break;
                    case MainWindowViewType.EventsView:
                        if (CurrentViewModel is SettingsViewModel)
                            IsSettingsAuthorized = false;
                        CurrentViewModel = EventsViewModel;
                        break;
                    case MainWindowViewType.PrinterView:
                        if (CurrentViewModel is SettingsViewModel)
                            IsSettingsAuthorized = false;
                        CurrentViewModel = PrinterViewModel;
                        break;
                    case MainWindowViewType.ErrorsView:
                        if (CurrentViewModel is SettingsViewModel)
                            IsSettingsAuthorized = false;
                        CurrentViewModel = ErrorsViewModel;
                        break;
                    case MainWindowViewType.SettingsView:
                        if (!IsSettingsAuthorized)
                        {
                            CurrentViewModel = LoginViewModel;
                        }
                        else
                        {
                            CurrentViewModel = new SettingsViewModel(SettingsViewModel.SettingsService, SettingsViewModel.DeviceService, SettingsViewModel.ReportTaskService);
                        }
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
                                   SettingsViewModel settingsViewModel,
                                   LoginViewModel loginViewModel)
        {
            MainViewModel = mainViewModel;
            ReportTasksViewModel = reportTasksViewModel;
            HandleAggregationViewModel = handleAggregationViewModel;
            EventsViewModel = eventsViewModel;
            PrinterViewModel = printerViewModel;
            ErrorsViewModel = errorsViewModel;
            SettingsViewModel = settingsViewModel;
            LoginViewModel = loginViewModel;

            updateCurrentViewModelCommand = new RelayCommand(OnUpdateCurrentViewModelCommandExecuted, CanUpdateCurrentViewModelCommandExecute);
        }
    }