using System.Collections.ObjectModel;
using CC.Core.Devices;
using CC.Core.Devices.Impl;
using CC.Core.Models;
using CC.Core.Services.Impl;

namespace CC.Core.Services;

public interface IDeviceService
{
    ICameraDevice StatisticCameraService { get; set; }
    ICameraDevice ProductCamera1Service { get; set; }
    ICameraDevice ProductCamera2Service { get; set; }
    ICameraDevice VerificationCamera1Service { get; set; }
    ICameraDevice VerificationCamera2Service { get; set; }
    IPrinterDevice BoxPrinterService { get; set; }
    IPrinterDevice PalletPrinterService { get; set; }
    IPrinterDevice TransportPrinterService { get; set; }


    ObservableCollection<Device> Devices { get; set; }


    LocalDb LocalDbService { get; set; }
    ProcessingCodeService ProcessingCodeService { get; set; }
    ReportTaskService ReportTaskService { get; set; }
    ISettingsService SettingsService { get; set; }
    IVirtualBoxService VirtualBoxService { get; set; }


    void ChangeSetup(int numberSetup);
    void ConnectDevices();
    void DisconnectDevices();
    void FindedDevice();
    void LostedDevice();
    void StartDevices();
    void StopDevices();
    void CreateDevice();   
}