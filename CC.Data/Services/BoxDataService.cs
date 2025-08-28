using CC.Data.Entities.Codes;
using CC.Data.Services.Base;

namespace CC.Data.Services;

public class BoxDataService : DataService<Box>
{
    public BoxDataService(string connectionString) : base(connectionString)
    {
    }
}