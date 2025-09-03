using System.Windows.Input;
using CC.Core.Commands.Base;
using CC.Core.Models.Base;
using CC.UI.ViewModels.Aggregation;
using CC.UI.ViewModels.Base;

namespace CC.UI.Navigators;

public enum AggregationViewType
{
    CheckCodeView,
    AddProductView,
    AddBoxView,
    AddPalletView,
    DeleteProductView,
    DeleteBoxView,
    DeletePalletView
}

public class AggregationNavigator : ObservableObject
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


    public AddProductAggregationViewModel AddProductAggregationViewModel { get; set; }
    public DeleteProductAggregationViewModel DeleteProductAggregationViewModel { get; set; }
    public AddBoxAggregationViewModel AddBoxAggregationViewModel { get; set; }
    public DeleteBoxAggregationViewModel DeleteBoxAggregationViewModel { get; set; }
    public AddPalletAggregationViewModel AddPalletAggregationViewModel { get; set; }
    public DeletePalletAggregationViewModel DeletePalletAggregationViewModel { get; set; }
    public CheckCodeAggregationViewModel CheckCodeAggregationViewModel { get; set; }


    private ICommand _updateCurrentViewModelCommand;
    public ICommand UpdateCurrentViewModelCommand => _updateCurrentViewModelCommand;
    private bool CanUpdateCurrentViewModelCommandExecute(object p) => true;

    private void OnUpdateCurrentViewModelCommandExecuted(object p)
    {
        if (p is AggregationViewType viewType)
        {
            switch (viewType)
            {
                case AggregationViewType.CheckCodeView:
                    CurrentViewModel = CheckCodeAggregationViewModel;
                    break;
                case AggregationViewType.AddProductView:
                    CurrentViewModel = AddProductAggregationViewModel;
                    break;
                case AggregationViewType.DeleteProductView:
                    CurrentViewModel = DeleteProductAggregationViewModel;
                    break;
                case AggregationViewType.AddBoxView:
                    CurrentViewModel = AddBoxAggregationViewModel;
                    break;
                case AggregationViewType.DeleteBoxView:
                    CurrentViewModel = DeleteBoxAggregationViewModel;
                    break;
                case AggregationViewType.AddPalletView:
                    CurrentViewModel = AddPalletAggregationViewModel;
                    break;
                case AggregationViewType.DeletePalletView:
                    CurrentViewModel = DeletePalletAggregationViewModel;
                    break;
                default:
                    break;
            }
        }
    }


    public AggregationNavigator(AddProductAggregationViewModel addProductAggregationViewModel,
        DeleteProductAggregationViewModel deleteProductAggregationViewModel,
        AddBoxAggregationViewModel addBoxAggregationViewModel,
        DeleteBoxAggregationViewModel deleteBoxAggregationViewModel,
        AddPalletAggregationViewModel addPalletAggregationViewModel,
        DeletePalletAggregationViewModel deletePalletAggregationViewModel,
        CheckCodeAggregationViewModel checkCodeAggregationViewModel)
    {
        AddProductAggregationViewModel = addProductAggregationViewModel;
        DeleteProductAggregationViewModel = deleteProductAggregationViewModel;
        AddBoxAggregationViewModel = addBoxAggregationViewModel;
        DeleteBoxAggregationViewModel = deleteBoxAggregationViewModel;
        AddPalletAggregationViewModel = addPalletAggregationViewModel;
        DeletePalletAggregationViewModel = deletePalletAggregationViewModel;
        CheckCodeAggregationViewModel = checkCodeAggregationViewModel;

        _updateCurrentViewModelCommand = new RelayCommand(OnUpdateCurrentViewModelCommandExecuted,
            CanUpdateCurrentViewModelCommandExecute);
    }
}