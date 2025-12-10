using System.Windows.Input;
using CC.Core.Commands.Base;
using CC.UI.ViewModels.Base;

namespace CC.UI.ViewModels.MainWindow;

public class LoginViewModel : ViewModel
{
    public event System.Action LoginSucceeded;

    private string _username = string.Empty;
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }

    private ICommand _loginCommand;
    public ICommand LoginCommand => _loginCommand;
    private bool CanLoginCommandExecute(object p) => true;
    private void OnLoginCommandExecuted(object p)
    {
        if (Username == "admin" && Password == "admin123")
        {
            ErrorMessage = string.Empty;
            Password = string.Empty;
            LoginSucceeded?.Invoke();
        }
        else
        {
            ErrorMessage = "Неверный логин или пароль";
            Password = string.Empty;
        }
    }

    public void ResetCredentials()
    {
        Username = string.Empty;
        Password = string.Empty;
        ErrorMessage = string.Empty;
    }

    public LoginViewModel()
    {
        _loginCommand = new RelayCommand(OnLoginCommandExecuted, CanLoginCommandExecute);
    }
}


