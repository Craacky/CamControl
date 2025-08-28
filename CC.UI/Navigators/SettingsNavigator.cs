using System.Windows.Input;
using CC.Core.Commands.Base;
using CC.Core.Models.Base;
using CC.UI.ViewModels.Base;
using CC.UI.ViewModels.Settings;

namespace CC.Core.Navigators;

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


        public LineViewModel LineSettingsViewModel { get; set; }
        public CamerasViewModel CamerasSettingsViewModel { get; set; }
        public PrinterViewModel PrintersSettingsViewModel { get; set; }
        public DataBaseViewModel DataBasesSettingsViewModel { get; set; }


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


        public SettingsNavigator(LineViewModel lineSettingsViewModel, 
                                 CamerasViewModel camerasSettingsViewModel, 
                                 PrinterViewModel printersSettingsViewModel, 
                                 DataBaseViewModel dataBasesSettingsViewModel)
        {
            LineSettingsViewModel = lineSettingsViewModel;
            CamerasSettingsViewModel = camerasSettingsViewModel;
            PrintersSettingsViewModel = printersSettingsViewModel;
            DataBasesSettingsViewModel = dataBasesSettingsViewModel;

            updateCurrentViewModelCommand = new RelayCommand(OnUpdateCurrentViewModelCommandExecuted, CanUpdateCurrentViewModelCommandExecute);
        }
    }