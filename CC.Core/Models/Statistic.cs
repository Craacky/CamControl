using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using CC.Core.Models.Base;
using CC.Data.Entities.Codes;

namespace CC.Core.Models;

public class Statistic : ObservableObject
{
    private List<Pallet> _palletCodes;

    public List<Pallet> PalletCodes
    {
        get => _palletCodes;
        set
        {
            _palletCodes = value;
            OnPropertyChanged(nameof(PalletCodes));
        }
    }

    public List<Box> BoxCodes { get; set; }

    public List<Product> ProductCodes { get; set; }

    private int _countBoxInCurrentPallet;

    public int CountBoxInCurrentPallet
    {
        get => _countBoxInCurrentPallet;
        set
        {
            _countBoxInCurrentPallet = value;
            OnPropertyChanged(nameof(CountBoxInCurrentPallet));
        }
    }

    private int _countProducts;

    public int CountProducts
    {
        get => _countProducts;
        set
        {
            _countProducts = value;
            OnPropertyChanged(nameof(CountProducts));
        }
    }

    private int _countBoxes;

    public int CountBoxes
    {
        get => _countBoxes;
        set
        {
            _countBoxes = value;
            OnPropertyChanged(nameof(CountBoxes));
        }
    }


    private List<CameraReadingResult> _cameraReadingResults;

    public ObservableCollection<CameraReadingResult> CameraReadingResults { get; set; }

    private object _syncLock = new object();

    public Statistic()
    {
        CameraReadingResults = new ObservableCollection<CameraReadingResult>();
        BindingOperations.EnableCollectionSynchronization(CameraReadingResults, _syncLock);
        PalletCodes = new List<Pallet>();
        BoxCodes = new List<Box>();
        ProductCodes = new List<Product>();
    }
}