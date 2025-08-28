using System;
using SocketManager;

namespace CC.Core.Devices.Impl;

public class PrinterDevice
{
    /// <summary>
    /// ToDO remake class for bartender
    /// </summary>
    public event Action<Client, DateTime, SocketConnectionState> ConnectionChanged;
    public event Action<Client, DateTime, string> MessageReceived;
}