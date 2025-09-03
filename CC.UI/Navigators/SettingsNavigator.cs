using System.Windows.Input;
using CC.Core.Commands.Base;
using CC.Core.Models.Base;
using CC.UI.ViewModels.Base;
using CC.UI.ViewModels.Settings;

namespace CC.UI.Navigators;

public enum SettingsViewType
{
    LineView,
    CamerasView,
    PrintersView,
    DataBasesView
}

public class SettingsNavigator : ObservableObject
{
    private ViewModel currentViewModel;

    public ViewModel CurrentViewModel
    {
        get => currentViewModel;
        set
        {
            currentViewModel = value;
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }


    public LineSettingsViewModel LineSettingsViewModel { get; set; }
    public CameraSettingsViewModel CamerasSettingsViewModel { get; set; }
    public PrinterSettingsViewModel PrintersSettingsViewModel { get; set; }
    public DatabaseSettingsViewModel DataBasesSettingsViewModel { get; set; }


    private ICommand updateCurrentViewModelCommand;
    public ICommand UpdateCurrentViewModelCommand => updateCurrentViewModelCommand;
    private bool CanUpdateCurrentViewModelCommandExecute(object p) => true;

    private void OnUpdateCurrentViewModelCommandExecuted(object p)
    {
        if (p is SettingsViewType viewType)
        {
            switch (viewType)
            {
                case SettingsViewType.LineView:
                    CurrentViewModel = LineSettingsViewModel;
                    break;
                case SettingsViewType.CamerasView:
                    CurrentViewModel = CamerasSettingsViewModel;
                    break;
                case SettingsViewType.PrintersView:
                    CurrentViewModel = PrintersSettingsViewModel;
                    break;
                case SettingsViewType.DataBasesView:
                    CurrentViewModel = DataBasesSettingsViewModel;
                    break;
                default:
                    break;
            }
        }
    }


    public SettingsNavigator(LineSettingsViewModel lineSettingsViewModel,
        CameraSettingsViewModel camerasSettingsViewModel,
        PrinterSettingsViewModel printersSettingsViewModel,
        DatabaseSettingsViewModel dataBasesSettingsViewModel)
    {
        LineSettingsViewModel = lineSettingsViewModel;
        CamerasSettingsViewModel = camerasSettingsViewModel;
        PrintersSettingsViewModel = printersSettingsViewModel;
        DataBasesSettingsViewModel = dataBasesSettingsViewModel;

        updateCurrentViewModelCommand = new RelayCommand(OnUpdateCurrentViewModelCommandExecuted,
            CanUpdateCurrentViewModelCommandExecute);
    }
}