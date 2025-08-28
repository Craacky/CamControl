using CC.Core.Devices.Impl;
using CC.Core.Navigators;
using CC.Core.Services.Impl;
using CC.UI.ViewModels.Aggregation;
using CC.UI.ViewModels.Base;

namespace CC.UI.ViewModels.Main;

public class AggregationViewModel : ViewModel
{
    public AddProductAggregationViewModel AddProductViewModel { get; set; }
    public DeleteProductAggregationViewModel DeleteProductViewModel { get; set; }
    public AddBoxAggregationViewModel AddBoxViewModel { get; set; }
    public DeleteBoxAggregationViewModel DeleteBoxViewModel { get; set; }
    public AddPalletAggregationViewModel AddPalletViewModel { get; set; }
    public DeletePalletAggregationViewModel DeletePalletViewModel { get; set; }
    public CheckCodeAggregationViewModel CheckCodeViewModel { get; set; }

    public AggregationNavigator Navigator { get; set; }


    public ReportTaskService ReportTaskService { get; set; }
    public ProcessingCodeService ProcessingCodeService { get; set; }
    public LocalDb LocalDBService { get; set; }
    public ErrorsService ErrorsService { get; set; }


    public AggregationViewModel(ReportTaskService reportTaskService,
        ProcessingCodeService processingCodeService,
        LocalDb localDBService,
        ErrorsService errorsService)
    {
        ReportTaskService = reportTaskService;
        ProcessingCodeService = processingCodeService;
        LocalDBService = localDBService;
        ErrorsService = errorsService;

        AddProductViewModel = new AddProductAggregationViewModel(ProcessingCodeService,
            ReportTaskService,
            LocalDBService,
            ErrorsService);
        DeleteProductViewModel = new DeleteProductAggregationViewModel(ProcessingCodeService,
            ReportTaskService,
            LocalDBService,
            ErrorsService);
        AddBoxViewModel = new AddBoxAggregationViewModel(ProcessingCodeService,
            ReportTaskService,
            LocalDBService,
            ErrorsService);
        DeleteBoxViewModel = new DeleteBoxAggregationViewModel(ProcessingCodeService,
            ReportTaskService,
            LocalDBService,
            ErrorsService);
        AddPalletViewModel = new AddPalletAggregationViewModel(ProcessingCodeService,
            ReportTaskService,
            LocalDBService,
            ErrorsService);
        DeletePalletViewModel = new DeletePalletAggregationViewModel(ProcessingCodeService,
            ReportTaskService,
            LocalDBService,
            ErrorsService);

        CheckCodeViewModel = new CheckCodeAggregationViewModel(ProcessingCodeService,
            ReportTaskService,
            LocalDBService);

        Navigator = new AggregationNavigator(AddProductViewModel,
            DeleteProductViewModel,
            AddBoxViewModel,
            DeleteBoxViewModel,
            AddPalletViewModel,
            DeletePalletViewModel,
            CheckCodeViewModel);


        Navigator.UpdateCurrentViewModelCommand.Execute(HandleAggregationViewType.CheckCodeView);
    }
}