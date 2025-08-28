using System.Windows.Input;
using CC.Core.Commands.Base;
using CC.Core.Models.Base;
using CC.UI.ViewModels.Aggregation;
using CC.UI.ViewModels.Base;

namespace CC.Core.Navigators;

public enum HandleAggregationViewType
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


    public AddProductAggregationViewModel HandleAggregationAddProductViewModel { get; set; }
    public DeleteProductAggregationViewModel HandleAggregationDeleteProductViewModel { get; set; }
    public AddBoxAggregationViewModel HandleAggregationAddBoxViewModel { get; set; }
    public DeleteBoxAggregationViewModel HandleAggregationDeleteBoxViewModel { get; set; }
    public AddPalletAggregationViewModel HandleAggregationAddPalletViewModel { get; set; }
    public DeletePalletAggregationViewModel HandleAggregationDeletePalletViewModel { get; set; }
    public CheckCodeAggregationViewModel HandleAggregationCheckCodeViewModel { get; set; }


    private ICommand updateCurrentViewModelCommand;
    public ICommand UpdateCurrentViewModelCommand => updateCurrentViewModelCommand;
    private bool CanUpdateCurrentViewModelCommandExecute(object p) => true;

    private void OnUpdateCurrentViewModelCommandExecuted(object p)
    {
        if (p is HandleAggregationViewType viewType)
        {
            switch (viewType)
            {
                case HandleAggregationViewType.CheckCodeView:
                    CurrentViewModel = HandleAggregationCheckCodeViewModel;
                    break;
                case HandleAggregationViewType.AddProductView:
                    CurrentViewModel = HandleAggregationAddProductViewModel;
                    break;
                case HandleAggregationViewType.DeleteProductView:
                    CurrentViewModel = HandleAggregationDeleteProductViewModel;
                    break;
                case HandleAggregationViewType.AddBoxView:
                    CurrentViewModel = HandleAggregationAddBoxViewModel;
                    break;
                case HandleAggregationViewType.DeleteBoxView:
                    CurrentViewModel = HandleAggregationDeleteBoxViewModel;
                    break;
                case HandleAggregationViewType.AddPalletView:
                    CurrentViewModel = HandleAggregationAddPalletViewModel;
                    break;
                case HandleAggregationViewType.DeletePalletView:
                    CurrentViewModel = HandleAggregationDeletePalletViewModel;
                    break;
                default:
                    break;
            }
        }
    }


    public AggregationNavigator(AddProductAggregationViewModel handleAggregationAddProductViewModel,
        DeleteProductAggregationViewModel handleAggregationDeleteProductViewModel,
        AddBoxAggregationViewModel handleAggregationAddBoxViewModel,
        DeleteBoxAggregationViewModel handleAggregationDeleteBoxViewModel,
        AddPalletAggregationViewModel handleAggregationAddPalletViewModel,
        DeletePalletAggregationViewModel handleAggregationDeletePalletViewModel,
        CheckCodeAggregationViewModel handleAggregationCheckCodeViewModel)
    {
        HandleAggregationAddProductViewModel = handleAggregationAddProductViewModel;
        HandleAggregationDeleteProductViewModel = handleAggregationDeleteProductViewModel;
        HandleAggregationAddBoxViewModel = handleAggregationAddBoxViewModel;
        HandleAggregationDeleteBoxViewModel = handleAggregationDeleteBoxViewModel;
        HandleAggregationAddPalletViewModel = handleAggregationAddPalletViewModel;
        HandleAggregationDeletePalletViewModel = handleAggregationDeletePalletViewModel;
        HandleAggregationCheckCodeViewModel = handleAggregationCheckCodeViewModel;

        updateCurrentViewModelCommand = new RelayCommand(OnUpdateCurrentViewModelCommandExecuted,
            CanUpdateCurrentViewModelCommandExecute);
    }
}