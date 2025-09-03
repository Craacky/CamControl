using CC.Core.Devices.Impl;
using CC.Core.Services.Impl;
using CC.Data.Entities.Settings;

namespace CamFusion.Services.Devices;

public class BoxPrinterService : PrinterDevice
{
    public BoxPrinterService(DeviceSettings deviceSettings,
        LineSettings lineSettings,
        LocalDb localDbService,
        ReportTaskService reportTaskService) : base(deviceSettings, lineSettings, localDbService, reportTaskService)
    { }

}