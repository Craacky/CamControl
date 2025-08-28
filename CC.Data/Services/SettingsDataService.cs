using CC.Data.Entities.Settings;
using CC.Data.Services.Base;

namespace CC.Data.Services;

public class SettingsDataService : DataService<Settings>
{
    public SettingsDataService(string connectionString) : base(connectionString)
    {
    }
}