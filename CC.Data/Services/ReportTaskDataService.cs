using CC.Data.Entities.Tasks;
using CC.Data.Services.Base;

namespace CC.Data.Services;

public class ReportTaskDataService : DataService<ReportTask>
{
    public ReportTaskDataService(string connectionString) : base(connectionString)
    {
    }
}