using System.Windows.Input;
using CC.Core.Commands.Base;
using CC.UI.ViewModels.Base;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace CC.UI.ViewModels.Settings;

public class LineViewModel: ViewModel
{
    private ICommand choosePathLoadNomenclatureFilesCommand;
    public ICommand ChoosePathLoadNomenclatureFilesCommand => choosePathLoadNomenclatureFilesCommand;

    private bool CanChoosePathLoadNomenclatureFilesCommandExecute(object p)
    {
        return true;
    }

    private void OnChoosePathLoadNomenclatureFilesCommandExecuted(object p)
    {
        var dialog = new CommonOpenFileDialog
        {
            IsFolderPicker = true,
            Title = "Select folder for loading nomenclature files"
        };

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            CurrentSettings.Line.PathLoadNomenclatureFiles = dialog.FileName;
        }
    }

    private ICommand choosePathSaveReportTaskFilesCommand;
    public ICommand ChoosePathSaveReportTaskFilesCommand => choosePathSaveReportTaskFilesCommand;

    private bool CanChoosePathSaveReportTaskFilesCommandExecute(object p)
    {
        return true;
    }


    private void OnChoosePathSaveReportTaskFilesCommandExecuted(object p)
    {
        var dialog = new CommonOpenFileDialog
        {
            IsFolderPicker = true,
            Title = "Выберите папку для сохранения отчётов по задачам"
        };

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            CurrentSettings.Line.PathSaveReportTaskFiles = dialog.FileName;
        }
    }



    public Data.Entities.Settings.Settings CurrentSettings { get; set; }


    public LineViewModel(Data.Entities.Settings.Settings currentSettings)
    {
        CurrentSettings = currentSettings;

        choosePathLoadNomenclatureFilesCommand = new RelayCommand(OnChoosePathLoadNomenclatureFilesCommandExecuted,
            CanChoosePathLoadNomenclatureFilesCommandExecute);
        choosePathSaveReportTaskFilesCommand = new RelayCommand(OnChoosePathSaveReportTaskFilesCommandExecuted,
            CanChoosePathSaveReportTaskFilesCommandExecute);
    }
}