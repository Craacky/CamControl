using CC.Data.Entities.Tasks;
using CC.Data.Services.Base;

namespace CC.Data.Services;

public class LineDataService : DataService<Line>
{
    public LineDataService(string connectionString) : base(connectionString)
    {
    }
}