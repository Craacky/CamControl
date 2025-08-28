using CC.Data.Entities.Codes;
using CC.Data.Services.Base;

namespace CC.Data.Services;

public class ProductDataService : DataService<Product>
{
    public ProductDataService(string connectionString) : base(connectionString)
    {
    }
}