using CC.Data.Entities.Tasks;
using CC.Data.Services.Base;

namespace CC.Data.Services;

public class NomenclatureDataService : DataService<Nomenclature>
{
    public NomenclatureDataService(string connectionString) : base(connectionString)
    {
    }
}