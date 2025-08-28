using CC.Data.Entities.Tasks;
using CC.Data.Services.Base;

namespace CC.Data.Services;

public class AttributeDataService : DataService<Attribute>
{
    public AttributeDataService(string connectionString) : base(connectionString)
    {
    }
}