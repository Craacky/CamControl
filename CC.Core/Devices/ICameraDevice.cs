using System;
using CC.Core.Models;
using CC.Data.Entities.Settings;
using SocketManager;

namespace CC.Core.Devices;

public interface ICameraDevice
{
    Device Device { get; set; }
    DeviceSettings DeviceSettings { get; set; }
    LineSettings LineSettings { get; set; }

    event Action<Client, DateTime, SocketConnectionState> ConnectionChanged;
    event Action<Client, DateTime, string> MessageReceived;

    void ConnectAsync();
    void Disconnect();
    void SendCommandChangeSetup(int numberSetup);
    void SendCommandFindDevice();
    void SendCommandLostDevice();
    void SendMessage(string message);
    void Start();
    void Stop();
}