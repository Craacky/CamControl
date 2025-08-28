using CC.Data.Entities.Base;

namespace CC.Data.Entities.Settings;

public class LineSettings : ObservableObject
{
    private string? _fullName;
    private string? _shortName;
    private int _lineId;
    private string? _pathSaveReportTaskFiles;
    private string? _pathLoadNomenclatureFiles;
    private int _jobStoragePeriodInDays;


    public string? FullName
    {
        get => _fullName;
        set
        {
            _fullName = value;
            OnPropertyChanged(nameof(FullName));
        }
    }

    public string? ShortName
    {
        get => _shortName;
        set
        {
            _shortName = value;
            OnPropertyChanged(nameof(ShortName));
        }
    }

    public int LineId
    {
        get => _lineId;
        set
        {
            _lineId = value;
            OnPropertyChanged(nameof(LineId));
        }
    }

    public string? PathSaveReportTaskFiles
    {
        get => _pathSaveReportTaskFiles;
        set
        {
            _pathSaveReportTaskFiles = value;
            OnPropertyChanged(nameof(PathSaveReportTaskFiles));
        }
    }


    public string? PathLoadNomenclatureFiles
    {
        get => _pathLoadNomenclatureFiles;
        set
        {
            _pathLoadNomenclatureFiles = value;
            OnPropertyChanged(nameof(PathLoadNomenclatureFiles));
        }
    }

    public int JobStoragePeriodInDays
    {
        get => _jobStoragePeriodInDays;
        set
        {
            _jobStoragePeriodInDays = value;
            OnPropertyChanged(nameof(JobStoragePeriodInDays));
        }
    }
}