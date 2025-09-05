using System;
using CC.Core.Models;
using CC.Data.EFCore;
using CC.Data.Entities.Settings;
using CC.Data.Services;

namespace CC.Core.Devices.Impl;

public class LocalDb
{
    public Device Device { get; set; }
    public NomenclatureDataService NomenclatureDataService { get; set; }
    public AttributeDataService AttributeDataService { get; set; }
    public ReportTaskDataService ReportTaskDataService { get; set; }
    public ProductDataService ProductDataService { get; set; }
    public BoxDataService BoxDataService { get; set; }
    public PalletDataService PalletDataService { get; set; }
    public LineDataService LineDataService { get; set; }
    public SettingsDataService SettingsDataService { get; set; }


    public delegate void ConnectionHandler(DataBaseContext db, DateTime datetime, DbConnectionState connectionState);

    public event ConnectionHandler? ConnectionChanged;


    public LocalDb()
    {
        Device = new Device
        {
            Name = Settings.LocalDb?.Name ?? "LocalDb",
            Address = Settings.LocalDb?.ConnectionString ?? throw new InvalidOperationException("No connection string"),
            IsUsed = Settings.LocalDb?.IsUsed ?? false
        };

        NomenclatureDataService = new NomenclatureDataService(Device.Address);
        AttributeDataService = new AttributeDataService(Device.Address);
        ReportTaskDataService = new ReportTaskDataService(Device.Address);
        ProductDataService = new ProductDataService(Device.Address);
        BoxDataService = new BoxDataService(Device.Address);
        PalletDataService = new PalletDataService(Device.Address);
        LineDataService = new LineDataService(Device.Address);
        SettingsDataService = new SettingsDataService(Device.Address);

        DataBaseContext.ConnectionChanged += DBContext_ConnectionChanged;
        using var dBContext = new DataBaseContext(Device.Address);
    }

    
    private void DBContext_ConnectionChanged(DataBaseContext db, DateTime datetime, DbConnectionState connectionState)
    {
        if (db.IsConnected != Device.IsConnected)
        {
            // вызов события только если есть подписчики
            ConnectionChanged?.Invoke(db, datetime, connectionState);

            if (db.ConnectionString == Device.Address)
            {
                Device.IsConnected = db.IsConnected;
            }
        }
    }

}