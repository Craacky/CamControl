using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CC.Data.Entities.Base;
using CC.Data.Entities.Codes;

namespace CC.Data.Entities.Tasks;

public class ReportTask : Entity
{
    public Guid Guid { get; set; } = Guid.NewGuid();


    private string? _lotNumber;

    [MaxLength(255)]
    public string? LotNumber
    {
        get => _lotNumber;
        set
        {
            _lotNumber = value;
            OnPropertyChanged(nameof(LotNumber));
        }
    }

    private string? _countProductInBox;

    [MaxLength(255)]
    public string? CountProductInBox
    {
        get => _countProductInBox;
        set
        {
            _countProductInBox = value;
            OnPropertyChanged(nameof(CountProductInBox));
        }
    }

    private string? _countBoxInPallet;

    [MaxLength(255)]
    public string? CountBoxInPallet
    {
        get => _countBoxInPallet;
        set
        {
            _countBoxInPallet = value;
            OnPropertyChanged(nameof(CountBoxInPallet));
        }
    }

    private DateTime _manufactureDate;

    public DateTime ManufactureDate
    {
        get => _manufactureDate;
        set
        {
            _manufactureDate = value;
            ExpiryDateInDays = Nomenclature != null
                ? Convert.ToInt32(Nomenclature.Attributes.FirstOrDefault(c => c.Code == 11)!.Value)
                : 0;
            ExpiryDate = ManufactureDate.AddDays(ExpiryDateInDays);
            OnPropertyChanged(nameof(ManufactureDate));
        }
    }

    private DateTime _expiryDate;

    public DateTime ExpiryDate
    {
        get => _expiryDate;
        set
        {
            _expiryDate = value;
            OnPropertyChanged(nameof(ExpiryDate));
        }
    }

    private DateTime _startTime;

    public DateTime StartTime
    {
        get => _startTime;
        set
        {
            _startTime = value;
            OnPropertyChanged(nameof(StartTime));

            Status = "Запущено";
        }
    }

    private DateTime _stopTime;

    public DateTime StopTime
    {
        get => _stopTime;
        set
        {
            _stopTime = value;
            OnPropertyChanged(nameof(StopTime));

            Status = "Завершено";
        }
    }

    private bool _isUsedCap;

    public bool IsUsedCap
    {
        get => _isUsedCap;
        set
        {
            _isUsedCap = value;
            OnPropertyChanged(nameof(IsUsedCap));
        }
    }

    private string _status = "Новое";

    [MaxLength(255)]
    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged(nameof(Status));
        }
    }


    public int LineId { get; set; }

    public virtual List<Pallet> Pallets { get; set; } = new List<Pallet>();

    public int? NomenclatureId { get; set; }
    public virtual Nomenclature? Nomenclature { get; set; }

    public virtual List<VirtualBox> VirtualBoxes { get; set; } = new List<VirtualBox>();

    [NotMapped] private int _expiryDateInDays;

    [NotMapped]
    public int ExpiryDateInDays
    {
        get => _expiryDateInDays;
        set
        {
            _expiryDateInDays = value;
            OnPropertyChanged(nameof(ExpiryDateInDays));
        }
    }
}