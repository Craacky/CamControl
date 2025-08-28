using System;
using CC.Core.Devices.Impl;
using CC.Core.Models;
using CC.Core.Services.Impl;
using CC.Data.Entities.Settings;
using SocketManager;

namespace CC.Core.Devices;

public interface IPrinterDevice
{
    Device Device { get; set; }


    DeviceSettings DeviceSettings { get; set; }
    LineSettings LineSettings { get; set; }
    LocalDb LocalDbService { get; set; }
    ReportTaskService ReportTaskService { get; set; }


    event Action<Client, DateTime, SocketConnectionState> ConnectionChanged;
    event Action<Client, DateTime, string> MessageReceived;


    void ConnectAsync();
    void Disconnect();
    void LoadTemplates();
    void PrintCode();
    void RepeatePrinteCode();
    void SendMessage(string message);
    void Start();
    void Stop();
}