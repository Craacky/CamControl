using System.Windows;
using System.Windows.Input;
using CC.Core.Commands.Base;
using CC.Core.Navigators;
using CC.Core.Services;
using CC.Core.Services.Impl;
using CC.UI.ViewModels.Base;

namespace CC.UI.ViewModels.External;

public class MainWindowViewModel : ViewModel
{
    private ICommand closeWindowCommand;
    public ICommand CloseWindowCommand => closeWindowCommand;
    private bool CanCloseWindowCommandExecute(object p) => true;

    private void OnCloseWindowCommandExecuted(object p)
    {
        foreach (Window window in Application.Current.Windows)
        {
            window.Close();
        }
    }


    public MainNavigator Navigator { get; set; }
    public ISettingsService SettingsService { get; set; }
    public ErrorsService ErrorsService { get; set; }


    public MainWindowViewModel(MainNavigator navigator,
        ISettingsService settingsService,
        ErrorsService errorsService)
    {
        Navigator = navigator;
        SettingsService = settingsService;
        ErrorsService = errorsService;

        closeWindowCommand = new RelayCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecute);

        Navigator.UpdateCurrentViewModelCommand.Execute(MainWindowViewType.ReportTasksView);
    }
}

public class PrinterViewModel : ViewModel
{
}

public class EventsViewModel : ViewModel
{
}