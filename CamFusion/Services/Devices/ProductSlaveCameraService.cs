using CC.Core.Devices;
using CC.Core.Devices.Impl;
using CC.Data.Entities.Settings;

namespace CamFusion.Services.Devices;

public class ProductSlaveCameraService : CameraDevice, ICameraDevice
{
    public ProductSlaveCameraService(DeviceSettings deviceSettings,
        LineSettings lineSettings) : base(deviceSettings, lineSettings)
    { }
}