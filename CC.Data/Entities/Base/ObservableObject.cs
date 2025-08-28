using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CC.Data.Entities.Base;

public class ObservableObject: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string? propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}