using CC.Data.Entities.Codes;
using CC.Data.Services.Base;

namespace CC.Data.Services;

public class PalletDataService : DataService<Pallet>
{
    public PalletDataService(string connectionString) : base(connectionString)
    {
    }
}