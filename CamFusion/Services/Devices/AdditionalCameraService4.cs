using System;
using CC.Core.Devices;
using CC.Core.Devices.Impl;
using CC.Core.Models;
using CC.Data.Entities.Settings;
using SocketManager;

namespace CamFusion.Services.Devices;

public class AdditionalCameraService4 : CameraDevice, ICameraDevice
{
    public AdditionalCameraService4(DeviceSettings deviceSettings,
        LineSettings lineSettings) : base(deviceSettings, lineSettings)
    {
        _client.ConnectionChanged += Client_ConnectionChanged;
        _client.MessageReceived += Client_MessageReceived;
        _client.MessageSent += Client_MessageSent;
    }


    private void Client_MessageSent(Client client, DateTime datetime, string message)
    {
        if (Device.IsConnected && IsRun)
        {
        }
    }

    private void Client_MessageReceived(Client client, DateTime datetime, string message)
    {
        if (Device.IsConnected && IsRun)
        {
        }
    }

    private void Client_ConnectionChanged(Client client, DateTime datetime, SocketConnectionState connectionState)
    {
        if (Device.IsConnected && IsRun)
        {
        }
    }

    public override void SendCommandFindDevice()
    {
        if (Device.IsConnected)
        {
            string commandSetFindDevice = "||>SET OUTPUT.ACTION 2 1\r\n";
            _ = _client.SendMessageAsync(commandSetFindDevice);
            commandSetFindDevice = "||>SET OUTPUT.ACTION 3 1\r\n";
            _ = _client.SendMessageAsync(commandSetFindDevice);
        }
    }

    public override void SendCommandLostDevice()
    {
        if (Device.IsConnected)
        {
            string commandSetLostDevice = "||>SET OUTPUT.ACTION 2 0\r\n";
            _ = _client.SendMessageAsync(commandSetLostDevice);
            commandSetLostDevice = "||>SET OUTPUT.ACTION 3 0\r\n";
            _ = _client.SendMessageAsync(commandSetLostDevice);
        }
    }
}