using System.Windows;
using System.Windows.Input;

namespace CC.UI.Views.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Height = SystemParameters.VirtualScreenHeight - 45;
        Width = SystemParameters.VirtualScreenWidth;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        this.DragMove();
    }
}