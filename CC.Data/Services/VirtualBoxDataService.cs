using CC.Data.Entities.Codes;
using CC.Data.Services.Base;

namespace CC.Data.Services;

public class VirtualBoxDataService : DataService<VirtualBox>
{
    public VirtualBoxDataService(string connectionString) : base(connectionString)
    {
    }
}

