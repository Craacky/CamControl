using CC.Core.Devices.Impl;
using CC.Core.Services.Impl;
using CC.UI.Navigators;
using CC.UI.ViewModels.Aggregation;
using CC.UI.ViewModels.Base;

namespace CC.UI.ViewModels.MainWindow;

public class AggregationViewModel : ViewModel
{
    public AddProductAggregationViewModel AddProductAggregationViewModel { get; set; }
    public DeleteProductAggregationViewModel DeleteProductAggregationViewModel { get; set; }
    public AddBoxAggregationViewModel AddBoxAggregationViewModel { get; set; }
    public DeleteBoxAggregationViewModel DeleteBoxAggregationViewModel { get; set; }
    public AddPalletAggregationViewModel AddPalletAggregationViewModel { get; set; }
    public DeletePalletAggregationViewModel DeletePalletAggregationViewModel { get; set; }
    public CheckCodeAggregationViewModel CheckCodeAggregationViewModel { get; set; }

    public AggregationNavigator Navigator { get; set; }


    public ReportTaskService ReportTaskService { get; set; }
    public ProcessingCodeService ProcessingCodeService { get; set; }
    public LocalDb LocalDBService { get; set; }
    public ErrorsService ErrorsService { get; set; }


    public AggregationViewModel(ReportTaskService? reportTaskService,
        ProcessingCodeService processingCodeService,
        LocalDb localDBService,
        ErrorsService errorsService)
    {
        ReportTaskService = reportTaskService;
        ProcessingCodeService = processingCodeService;
        LocalDBService = localDBService;
        ErrorsService = errorsService;

        AddProductAggregationViewModel = new AddProductAggregationViewModel(ProcessingCodeService,
            ReportTaskService,
            LocalDBService,
            ErrorsService);
        DeleteProductAggregationViewModel = new DeleteProductAggregationViewModel(ProcessingCodeService,
            ReportTaskService,
            LocalDBService,
            ErrorsService);
        AddBoxAggregationViewModel = new AddBoxAggregationViewModel(ProcessingCodeService,
            ReportTaskService,
            LocalDBService,
            ErrorsService);
        DeleteBoxAggregationViewModel = new DeleteBoxAggregationViewModel(ProcessingCodeService,
            ReportTaskService,
            LocalDBService,
            ErrorsService);
        AddPalletAggregationViewModel = new AddPalletAggregationViewModel(ProcessingCodeService,
            ReportTaskService,
            LocalDBService,
            ErrorsService);
        DeletePalletAggregationViewModel = new DeletePalletAggregationViewModel(ProcessingCodeService,
            ReportTaskService,
            LocalDBService,
            ErrorsService);

        CheckCodeAggregationViewModel = new CheckCodeAggregationViewModel(ProcessingCodeService,
            ReportTaskService,
            LocalDBService);

        Navigator = new AggregationNavigator(AddProductAggregationViewModel,
            DeleteProductAggregationViewModel,
            AddBoxAggregationViewModel,
            DeleteBoxAggregationViewModel,
            AddPalletAggregationViewModel,
            DeletePalletAggregationViewModel,
            CheckCodeAggregationViewModel);


        Navigator.UpdateCurrentViewModelCommand.Execute(AggregationViewType.CheckCodeView);
    }
}