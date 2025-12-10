using System.Windows.Controls;
using System.Windows.Input;

namespace CC.UI.Views.MainWindow;

public partial class LoginView
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void PasswordBox_OnPasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ViewModels.MainWindow.LoginViewModel vm && sender is PasswordBox pb)
        {
            vm.Password = pb.Password;
        }
    }

    private void UsernameBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Down)
        {
            PasswordBox.Focus();
            e.Handled = true;
        }
        else if (e.Key == Key.Enter)
        {
            if (DataContext is ViewModels.MainWindow.LoginViewModel vm)
            {
                vm.LoginCommand.Execute(null);
                e.Handled = true;
            }
        }
    }

    private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up)
        {
            UsernameBox.Focus();
            e.Handled = true;
        }
        else if (e.Key == Key.Enter)
        {
            if (DataContext is ViewModels.MainWindow.LoginViewModel vm)
            {
                vm.LoginCommand.Execute(null);
                e.Handled = true;
            }
        }
    }
}


