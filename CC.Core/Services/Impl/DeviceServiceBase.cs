using System;
using System.Collections.ObjectModel;
using CC.Core.Devices;
using CC.Core.Devices.Impl;
using CC.Core.Models;

namespace CC.Core.Services.Impl;

public class DeviceServiceBase : IDeviceService
{
    protected int countBox = 0;
    protected int countSlvae = 0;
    protected int countMaster = 0;

    public ICameraDevice ProductMasterCameraService { get; set; }
    public ICameraDevice ProductSlaveCameraService { get; set; }
    public ICameraDevice BoxCameraService { get; set; }
    public IPrinterDevice BoxPrinterService { get; set; }
    public IPrinterDevice PalletPrinterService { get; set; }
    public ObservableCollection<Device> Devices { get; set; }

    public LocalDb LocalDbService { get; set; }
    public ProcessingCodeService ProcessingCodeService { get; set; }
    public ReportTaskService ReportTaskService { get; set; }
    public ISettingsService SettingsService { get; set; }

    protected int countBoxInPallet;
    protected int countProductInBox;

    public DeviceServiceBase(ISettingsService settingsService,
        ReportTaskService reportTaskService,
        LocalDb localDbService,
        ProcessingCodeService processingCodeService)
    {
        SettingsService = settingsService;
        ReportTaskService = reportTaskService;
        LocalDbService = localDbService;
        ProcessingCodeService = processingCodeService;

        CreateDevice();
    }

    public virtual void CreateDevice()
    {
    }


    public void ConnectDevices()
    {
        ProductMasterCameraService?.ConnectAsync();
        ProductSlaveCameraService?.ConnectAsync();
        BoxCameraService?.ConnectAsync();
        BoxPrinterService?.ConnectAsync();
        PalletPrinterService?.ConnectAsync();
    }

    public void DisconnectDevices()
    {
        ProductMasterCameraService?.Disconnect();
        ProductSlaveCameraService?.Disconnect();
        BoxCameraService?.Disconnect();
        BoxPrinterService?.Disconnect();
        PalletPrinterService?.Disconnect();
    }

    public void StartDevices()
    {
        countBox = 0;
        countSlvae = 0;
        countMaster = 0;

        countBoxInPallet = Convert.ToInt32(ReportTaskService.CurrentReportTask.CountBoxInPallet);
        countProductInBox = Convert.ToInt32(ReportTaskService.CurrentReportTask.CountProductInBox);

        FindedDevice();

        ProductMasterCameraService?.Start();
        ProductSlaveCameraService?.Start();
        BoxCameraService?.Start();
        BoxPrinterService?.Start();
        PalletPrinterService?.Start();

        if (ReportTaskService.Statistic.PalletCodes.Count == 0 || ReportTaskService.Statistic.PalletCodes[^1].IsFulled)
        {
            ReportTaskService.GeneratePalletCode();
        }
    }

    public void StopDevices()
    {
        ProductMasterCameraService?.Stop();
        ProductSlaveCameraService?.Stop();
        BoxCameraService?.Stop();
        BoxPrinterService?.Stop();
        PalletPrinterService?.Stop();
    }

    public void LostedDevice()
    {
        ProductMasterCameraService?.SendCommandLostDevice();
        ProductSlaveCameraService?.SendCommandLostDevice();
        BoxCameraService?.SendCommandLostDevice();
    }

    public void FindedDevice()
    {
        if (ProductMasterCameraService != null &&
            (!ProductMasterCameraService.Device.IsUsed || ProductMasterCameraService.Device.IsConnected) &&
            ProductSlaveCameraService != null &&
            (!ProductSlaveCameraService.Device.IsUsed || ProductSlaveCameraService.Device.IsConnected) &&
            BoxCameraService != null && (!BoxCameraService.Device.IsUsed || BoxCameraService.Device.IsConnected) &&
            BoxPrinterService != null && (!BoxPrinterService.Device.IsUsed || BoxPrinterService.Device.IsConnected) &&
            PalletPrinterService != null &&
            (!PalletPrinterService.Device.IsUsed || PalletPrinterService.Device.IsConnected) &&
            LocalDbService != null && (!LocalDbService.Device.IsUsed || LocalDbService.Device.IsConnected))
        {
            ProductSlaveCameraService?.SendCommandFindDevice();
            ProductMasterCameraService?.SendCommandFindDevice();
            BoxCameraService?.SendCommandFindDevice();
        }
    }

    public void ChangeSetup(int numberSetup)
    {
        ProductMasterCameraService?.SendCommandChangeSetup(numberSetup);
        ProductSlaveCameraService?.SendCommandChangeSetup(numberSetup);
        BoxCameraService?.SendCommandChangeSetup(numberSetup);
    }
}