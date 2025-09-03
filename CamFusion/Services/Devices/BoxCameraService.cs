using CC.Core.Devices;
using CC.Core.Devices.Impl;
using CC.Data.Entities.Settings;

namespace CamFusion.Services.Devices;

public class BoxCameraService : CameraDevice, ICameraDevice
{
    public BoxCameraService(DeviceSettings deviceSettings,
        LineSettings lineSettings) : base(deviceSettings, lineSettings)
    {
    }
}