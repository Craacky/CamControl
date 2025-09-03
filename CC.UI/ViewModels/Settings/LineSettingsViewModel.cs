using System.Windows.Input;
using CC.Core.Commands.Base;
using CC.UI.ViewModels.Base;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace CC.UI.ViewModels.Settings;

public class LineSettingsViewModel:ViewModel
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
            Title = "Select folder for Nomenclature Files"
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
            Title = "Select folder to save Report Task Files"
        };

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            CurrentSettings.Line.PathSaveReportTaskFiles = dialog.FileName;
        }
    }


    public Data.Entities.Settings.Settings CurrentSettings { get; set; }


    public LineSettingsViewModel(Data.Entities.Settings.Settings currentSettings)
    {
        CurrentSettings = currentSettings;

        choosePathLoadNomenclatureFilesCommand = new RelayCommand(OnChoosePathLoadNomenclatureFilesCommandExecuted, CanChoosePathLoadNomenclatureFilesCommandExecute);
        choosePathSaveReportTaskFilesCommand = new RelayCommand(OnChoosePathSaveReportTaskFilesCommandExecuted, CanChoosePathSaveReportTaskFilesCommandExecute);
    }
}