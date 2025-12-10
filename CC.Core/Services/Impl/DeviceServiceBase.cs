using System;
using System.Collections.ObjectModel;
using CC.Core.Devices;
using CC.Core.Devices.Impl;
using CC.Core.Models;

namespace CC.Core.Services.Impl;

public class DeviceServiceBase : IDeviceService
{
    protected int countCamera1 = 0;
    protected int countCamera2 = 0;
    protected int countCamera3 = 0;
    protected int countCamera4 = 0;
    protected int countCamera5 = 0;

    public ICameraDevice StatisticCameraService { get; set; }
    public ICameraDevice ProductCamera1Service { get; set; }
    public ICameraDevice ProductCamera2Service { get; set; }
    public ICameraDevice VerificationCamera1Service { get; set; }
    public ICameraDevice VerificationCamera2Service { get; set; }
    public IPrinterDevice BoxPrinterService { get; set; }
    public IPrinterDevice PalletPrinterService { get; set; }
    public IPrinterDevice TransportPrinterService { get; set; }
    public ObservableCollection<Device> Devices { get; set; }

    public LocalDb LocalDbService { get; set; }
    public ProcessingCodeService ProcessingCodeService { get; set; }
    public ReportTaskService ReportTaskService { get; set; }
    public ISettingsService SettingsService { get; set; }
    public IVirtualBoxService VirtualBoxService { get; set; }

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

        VirtualBoxService = new VirtualBoxService(localDbService, reportTaskService, processingCodeService);

        CreateDevice();
    }

    public virtual void CreateDevice()
    {
    }


    public void ConnectDevices()
    {
        StatisticCameraService?.ConnectAsync();
        ProductCamera1Service?.ConnectAsync();
        ProductCamera2Service?.ConnectAsync();
        VerificationCamera1Service?.ConnectAsync();
        VerificationCamera2Service?.ConnectAsync();
        BoxPrinterService?.ConnectAsync();
        PalletPrinterService?.ConnectAsync();
        TransportPrinterService?.ConnectAsync();
    }

    public void DisconnectDevices()
    {
        StatisticCameraService?.Disconnect();
        ProductCamera1Service?.Disconnect();
        ProductCamera2Service?.Disconnect();
        VerificationCamera1Service?.Disconnect();
        VerificationCamera2Service?.Disconnect();
        BoxPrinterService?.Disconnect();
        PalletPrinterService?.Disconnect();
        TransportPrinterService?.Disconnect();
    }

    public void StartDevices()
    {
        countCamera1 = 0;
        countCamera2 = 0;
        countCamera3 = 0;
        countCamera4 = 0;
        countCamera5 = 0;

        countBoxInPallet = Convert.ToInt32(ReportTaskService.CurrentReportTask.CountBoxInPallet);
        countProductInBox = Convert.ToInt32(ReportTaskService.CurrentReportTask.CountProductInBox);

        FindedDevice();

        StatisticCameraService?.Start();
        ProductCamera1Service?.Start();
        ProductCamera2Service?.Start();
        VerificationCamera1Service?.Start();
        VerificationCamera2Service?.Start();
        BoxPrinterService?.Start();
        PalletPrinterService?.Start();
        TransportPrinterService?.Start();

        if (ReportTaskService.Statistic.PalletCodes.Count == 0 || ReportTaskService.Statistic.PalletCodes[^1].IsFulled)
        {
            ReportTaskService.GeneratePalletCode();
        }
    }

    public void StopDevices()
    {
        StatisticCameraService?.Stop();
        ProductCamera1Service?.Stop();
        ProductCamera2Service?.Stop();
        VerificationCamera1Service?.Stop();
        VerificationCamera2Service?.Stop();
        BoxPrinterService?.Stop();
        PalletPrinterService?.Stop();
        TransportPrinterService?.Stop();
    }

    public void LostedDevice()
    {
        StatisticCameraService?.SendCommandLostDevice();
        ProductCamera1Service?.SendCommandLostDevice();
        ProductCamera2Service?.SendCommandLostDevice();
        VerificationCamera1Service?.SendCommandLostDevice();
        VerificationCamera2Service?.SendCommandLostDevice();
    }

    public void FindedDevice()
    {
        if (StatisticCameraService != null &&
            (!StatisticCameraService.Device.IsUsed || StatisticCameraService.Device.IsConnected) &&
            ProductCamera1Service != null &&
            (!ProductCamera1Service.Device.IsUsed || ProductCamera1Service.Device.IsConnected) &&
            ProductCamera2Service != null && (!ProductCamera2Service.Device.IsUsed || ProductCamera2Service.Device.IsConnected) &&
            VerificationCamera1Service != null && (!VerificationCamera1Service.Device.IsUsed || VerificationCamera1Service.Device.IsConnected) &&
            VerificationCamera2Service != null && (!VerificationCamera2Service.Device.IsUsed || VerificationCamera2Service.Device.IsConnected) &&
            BoxPrinterService != null && (!BoxPrinterService.Device.IsUsed || BoxPrinterService.Device.IsConnected) &&
            PalletPrinterService != null &&
            (!PalletPrinterService.Device.IsUsed || PalletPrinterService.Device.IsConnected) &&
            TransportPrinterService != null && (!TransportPrinterService.Device.IsUsed || TransportPrinterService.Device.IsConnected) &&
            LocalDbService != null && (!LocalDbService.Device.IsUsed || LocalDbService.Device.IsConnected))
        {
            StatisticCameraService?.SendCommandFindDevice();
            ProductCamera1Service?.SendCommandFindDevice();
            ProductCamera2Service?.SendCommandFindDevice();
            VerificationCamera1Service?.SendCommandFindDevice();
            VerificationCamera2Service?.SendCommandFindDevice();
        }
    }

    public void ChangeSetup(int numberSetup)
    {
        StatisticCameraService?.SendCommandChangeSetup(numberSetup);
        ProductCamera1Service?.SendCommandChangeSetup(numberSetup);
        ProductCamera2Service?.SendCommandChangeSetup(numberSetup);
        VerificationCamera1Service?.SendCommandChangeSetup(numberSetup);
        VerificationCamera2Service?.SendCommandChangeSetup(numberSetup);
    }
}