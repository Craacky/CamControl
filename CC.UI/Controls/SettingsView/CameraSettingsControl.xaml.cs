using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using CC.Data.Entities.Settings;

namespace CC.UI.Controls.SettingsView;

public partial class CameraSettingsControl : UserControl
{
    public CameraSettingsControl()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new CommonOpenFileDialog();
        dialog.IsFolderPicker = true;

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            if (this.DataContext is DeviceSettings deviceSettings)
            {
                deviceSettings.Path = dialog.FileName;
            }
        }
    }
}