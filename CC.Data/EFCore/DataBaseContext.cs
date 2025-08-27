using System;
using CC.Data.Entities.Code;
using CC.Data.Entities.Codes;
using CC.Data.Entities.Settings;
using CC.Data.Entities.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Attribute = CC.Data.Entities.Tasks.Attribute;

namespace CC.Data.EFCore;

public class DataBaseContext : DbContext
{
    private DbConnectionState _connectionState;

    public string? ConnectionString { get; set; }

    public bool IsConnected =>
        _connectionState == DbConnectionState.Connected || _connectionState == DbConnectionState.Created;

    public event Action<DataBaseContext, DateTime, DbConnectionState>? ConnectionChanged;

    public DbSet<Nomenclature> Nomenclatures { get; set; }
    public DbSet<Attribute> Attributes { get; set; }
    public DbSet<Line> Lines { get; set; }
    public DbSet<ReportTask> ReportTasks { get; set; }
    public DbSet<Box> Boxes { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Pallet> Pallets { get; set; }
    public DbSet<Settings> Settings { get; set; }

    public DataBaseContext(string connectionString)
    {
        _connectionState = DbConnectionState.Disconnected;
        ConnectionString = connectionString;
        Connect();
    }

    private void Connect()
    {
        try
        {
            bool isDataBaseExists = Database.EnsureCreated();
            if (isDataBaseExists)
            {
                _connectionState = DbConnectionState.Created;
            }
            else
            {
                _connectionState = DbConnectionState.Connected;
            }
        }
        catch (ArgumentException)
        {
            _connectionState = DbConnectionState.InvalidConnectionString;
        }
        catch (SqlException)
        {
            _connectionState = DbConnectionState.NotFoundDb;
        }
        catch (Exception ex) when (ex.Message.Contains("server not found"))
        {
            _connectionState = DbConnectionState.NotFoundDb;
        }
        catch (Exception)
        {
            _connectionState = DbConnectionState.NotFoundDb;
        }
    }
}