using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace CC.UI.Controls.Settings;

public partial class CameraControl
{
    public CameraControl()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new CommonOpenFileDialog
        {
            IsFolderPicker = true,
            Title = "Select a folder"
        };

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            pathTextBlock.Text = dialog.FileName;
        }
    }
}