using System;
using System.Data;
using System.Threading.Tasks;
using CC.Core.Models;
using CC.Core.Services.Impl;
using CC.Data.Entities.Settings;
using SocketManager;

namespace CC.Core.Devices.Impl;

using System.Net.Sockets;
using System.Text;

public class PrinterDevice : IPrinterDevice
{
    protected DateTime _dateTimeLastReceivedMessage;
    protected Client _client;
    protected bool _isRun;
    protected string patternTask;
    protected string patternMessage;


    public Device Device { get; set; }


    public LocalDb LocalDbService { get; set; }
    public ReportTaskService ReportTaskService { get; set; }
    public DeviceSettings DeviceSettings { get; set; }
    public LineSettings LineSettings { get; set; }


    public PrinterDevice(DeviceSettings deviceSettings,
        LineSettings lineSettings, LocalDb localDBService, ReportTaskService reportTaskService)
    {
        DeviceSettings = deviceSettings;
        LineSettings = lineSettings;
        LocalDbService = localDBService;
        ReportTaskService = reportTaskService;

        _dateTimeLastReceivedMessage = DateTime.MinValue;
        _isRun = false;
        _client = new Client(DeviceSettings.Ip, DeviceSettings.Port);

        Device = new Device()
        {
            Name = DeviceSettings.Name,
            Address = _client.Address,
            IsConnected = false,
            IsUsed = DeviceSettings.IsUsed
        };

        _client.ConnectionChanged += Client_ConnectionChanged;
        _client.MessageReceived += Client_MessageReceived;
        _client.MessageSent += Client_MessageSent;
    }


    private void Client_MessageSent(Client client, DateTime datetime, string message)
    {
        if (Device.IsConnected && _isRun)
        {
        }
    }

    private void Client_MessageReceived(Client client, DateTime datetime, string message)
    {
        if (Device.IsConnected && _isRun)
        {
            MessageReceived?.Invoke(client, datetime, message);
            ProcessingMessage(message);
        }
    }

    private void Client_ConnectionChanged(Client client, DateTime datetime, SocketConnectionState connectionState)
    {
        if (client.IsConnected != Device.IsConnected)
        {
            Device.IsConnected = _client.IsConnected;
            ConnectionChanged?.Invoke(client, datetime, connectionState);
        }

        //if (Device.IsConnected)
        //{

        //    //PingingAsync();
        //    //CheskConnectionStateAsync();


        //    SendCommandToClearTask();
        //    SendCommadToSetAutoStatus();
        //}

        //if (client.IsConnected != Device.IsConnected && Device.IsConnected && _isRun)
        //{
        //    LoadTemplates();
        //}
    }


    public void ConnectAsync()
    {
        if (Device.IsUsed)
            _client.ConnectAsync();
    }

    public void Disconnect()
    {
        _client.Disconnect();
    }

    public void Start()
    {
        if (Device.IsUsed)
        {
            _isRun = true;
            // Don't necessarily load templates immediately, do it on demand
        }
    }

    public void Stop()
    {
        if (_isRun)
        {
            _isRun = false;
            // Optionally disconnect when stopping
            _client.Disconnect();
        }
    }

    protected void SendMessageInternal(string message)
    {
        if (Device.IsUsed)
        {
            _client.SendMessageAsync(message);
        }
    }

    public void SendMessage(string message)
    {
        // For printers, we may send messages even without a persistent connection
        // Connect temporarily, send message, then allow the connection to close
        if (!_client.IsConnected)
        {
            _client.ConnectAsync(); // Attempt to connect temporarily
            // Send message after a brief delay to allow connection
            Task.Delay(100).ContinueWith(_ => {
                if (_client.IsConnected || Device.IsUsed) // Allow sending even if not fully connected
                {
                    _client.SendMessageAsync(message);

                    // Optionally disconnect after sending if needed
                    // Task.Delay(500).ContinueWith(t => _client.Disconnect());
                }
            });
        }
        else
        {
            _client.SendMessageAsync(message);
        }
    }

    protected virtual async void PingingAsync()
    {
        //await Task.Run(() =>
        //{
        //    while (_client.IsConnected)
        //    {
        //        string sendCommandToStatus = $"\u0001S \u0017\r\n";
        //        _client.SendMessage(sendCommandToStatus);
        //        Thread.Sleep(1000);
        //    }
        //});
    }

    protected virtual async void CheskConnectionStateAsync()
    {
        //await Task.Run(() =>
        //{
        //    while (Device.IsConnected)
        //    {
        //        double differenceMillisecond = DateTime.Now.Subtract(_dateTimeLastReceivedMessage).TotalMilliseconds;
        //        if (_dateTimeLastReceivedMessage != DateTime.MinValue && differenceMillisecond > 3000)
        //        {
        //            if (Device.IsConnected)
        //            {
        //                Disconnect();
        //            }
        //            _client.RecoveryConnectAsync();
        //        }

        //        Thread.Sleep(500);
        //    }
        //});
    }

    protected virtual void SendCommandToClearTask()
    {
    }

    protected virtual void SendCommadToSetAutoStatus()
    {
    }

    protected virtual void ProcessingMessage(string message)
    {
    }

    public virtual void PrintCode()
    {
        // Connect temporarily for printing, send the print command, then allow disconnection
        if (Device.IsUsed)
        {
            _client.ConnectAsync();
            // Wait briefly for connection establishment
            Task.Delay(100).ContinueWith(_ => {
                if (_client.IsConnected)
                {
                    // Actually load templates and print
                    LoadTemplates();
                    string messageToSend = patternMessage;
                    _ = _client.SendMessageAsync(messageToSend);
                }
                else
                {
                    // Even if not fully connected, try to send command
                    LoadTemplates();
                    string messageToSend = patternMessage;
                    _ = _client.SendMessageAsync(messageToSend);
                }
            });
        }
    }

    public virtual void RepeatePrinteCode()
    {
    }

    public virtual void LoadTemplates()
    {
    }

    public event Action<Client, DateTime, SocketConnectionState> ConnectionChanged;
    public event Action<Client, DateTime, string> MessageReceived;
}