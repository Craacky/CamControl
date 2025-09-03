using System;
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

    public static event Action<DataBaseContext, DateTime, DbConnectionState>? ConnectionChanged;

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

        ConnectionChanged?.Invoke(this, DateTime.Now, _connectionState);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Nomenclature>()
            .HasMany(n => n.Attributes)
            .WithOne(a => a.Nomenclature)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReportTask>()
            .HasMany(rt => rt.Pallets)
            .WithOne(p => p.ReportTask)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Pallet>()
            .HasMany(p => p.Boxes)
            .WithOne(b => b.Pallet)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Box>()
            .HasMany(b => b.Products)
            .WithOne(p => p.Box)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Settings>()
            .OwnsOne(s => s.Line);
        modelBuilder.Entity<Settings>()
            .OwnsOne(s => s.ProductCameraMaster);
        modelBuilder.Entity<Settings>()
            .OwnsOne(s => s.ProductCameraSlave);
        modelBuilder.Entity<Settings>()
            .OwnsOne(s => s.BoxCamera);
        modelBuilder.Entity<Settings>()
            .OwnsOne(s => s.BoxPrinter);
        modelBuilder.Entity<Settings>()
            .OwnsOne(s => s.PalletPrinter);
        modelBuilder.Entity<Settings>()
            .OwnsOne(s => s.ServerDb);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionString);
    }
}