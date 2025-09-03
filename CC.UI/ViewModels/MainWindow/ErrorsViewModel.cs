using CC.Core.Services.Impl;
using CC.UI.ViewModels.Base;

namespace CC.UI.ViewModels.MainWindow;

public class ErrorsViewModel : ViewModel
{
    public ErrorsService ErrorsService { get; set; }

    public ErrorsViewModel(ErrorsService errorsService)
    {
        ErrorsService = errorsService;
    }
}