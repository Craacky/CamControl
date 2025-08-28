using System.Collections.Generic;
using CC.Core.Models;
using CC.Core.Models.Base;

namespace CC.Core.Services.Impl;

public class ErrorsService : ObservableObject
{
    private List<Error>? _errors;

    public List<Error>? Errors
    {
        get => _errors;
        set
        {
            _errors = value;
            OnPropertyChanged(nameof(Errors));
        }
    }

    public ErrorsService()
    {
        Errors = new List<Error>();
    }
}