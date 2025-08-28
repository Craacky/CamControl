using System.Windows;

namespace CC.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Height = SystemParameters.VirtualScreenHeight - 45;
        Width = SystemParameters.VirtualScreenWidth;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        this.DragMove();
    }
}