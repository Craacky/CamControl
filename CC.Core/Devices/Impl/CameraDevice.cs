using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using CC.Core.Models;
using CC.Data.Entities.Settings;
using SocketManager;

namespace CC.Core.Devices.Impl;

public class CameraDevice
{
    protected Client _client;
    protected bool IsRun;
    private CancellationTokenSource? _cts;

    public Device Device { get; set; }
    public DeviceSettings DeviceSettings { get; set; }
    public LineSettings LineSettings { get; set; }

    public event Action<Client, DateTime, SocketConnectionState>? ConnectionChanged;
    public event Action<Client, DateTime, string>? MessageReceived;

    public CameraDevice(DeviceSettings deviceSettings, LineSettings lineSettings)
    {
        IsRun = false;
        DeviceSettings = deviceSettings;
        LineSettings = lineSettings;

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
    }

    private void Client_MessageReceived(Client client, DateTime datetime, string message)
    {
        if (IsRun)
        {
            MessageReceived?.Invoke(client, datetime, message);
        }
    }

    private void Client_ConnectionChanged(Client client, DateTime datetime, SocketConnectionState connectionState)
    {
        if (client.IsConnected != Device.IsConnected)
        {
            Device.IsConnected = _client.IsConnected;
            ConnectionChanged?.Invoke(client, datetime, connectionState);

            if (client.IsConnected)
            {
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                _ = CheckConnectionStateAsync(_cts.Token);
            }
            else
            {
                _cts?.Cancel();
            }
        }
    }

    public void ConnectAsync()
    {
        if (Device.IsUsed)
        {
            _ = _client.ConnectAsync();
        }
    }

    public void Disconnect()
    {
        _cts?.Cancel();
        _client.Disconnect();
    }

    public void Start()
    {
        if (Device.IsConnected)
        {
            IsRun = true;
        }
    }

    public void Stop()
    {
        if (Device.IsConnected && IsRun)
        {
            IsRun = false;
        }
    }

    public void SendMessage(string message)
    {
        if (Device.IsConnected && IsRun)
        {
            _ = _client.SendMessageAsync(message);
        }
    }

    protected virtual void ProcessingMessage(string message)
    {
    }

    public virtual void SendCommandLostDevice()
    {
    }

    public virtual void SendCommandFindDevice()
    {
    }

    public virtual void SendCommandChangeSetup(int numberSetup)
    {
    }

    /// <summary>
    /// Асинхронная проверка состояния соединения (пинг каждые 5 сек).
    /// Отменяется через CancellationToken.
    /// </summary>
    protected virtual async Task CheckConnectionStateAsync(CancellationToken token)
    {
        while (Device.IsConnected && !token.IsCancellationRequested)
        {
            try
            {
                using var pinger = new Ping();
                var reply = await pinger.SendPingAsync(_client.Ip);

                if (reply.Status != IPStatus.Success)
                {
                    if (Device.IsConnected)
                    {
                        Disconnect();
                    }

                    await _client.RecoveryConnectAsync();
                }
            }
            catch (PingException)
            {
            }
            catch (OperationCanceledException)
            {
                break;
            }

            await Task.Delay(5000, token);
        }
    }
}