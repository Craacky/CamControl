using System;
using System.Windows.Input;

namespace CC.Core.Commands.Base;

public class RelayCommand : ICommand
{
    private readonly Action<object>? _execute;
    private readonly Func<object?, bool>? _canExecute;

    public RelayCommand(Action<object>? execute, Func<object, bool> canExecute)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute.Invoke(parameter);
    }

    public void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
        {
            return;
        }

        _execute!(parameter!);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}