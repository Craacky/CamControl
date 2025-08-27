using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SocketManager;

public class Server : IDisposable
{
    private readonly List<Client> _clients = new();
    private readonly object _clientsLock = new();
    private Socket _server = null!;
    private CancellationTokenSource _cts = null!;

    public bool IsStarted { get; private set; }
    public string Ip { get; private set; }
    public int Port { get; private set; }
    public string Address => $"{Ip}:{Port}";

    public event Action<Server, DateTime, SocketConnectionState>? ConnectionChanged;
    public event Action<Server, DateTime, Client>? ClientConnected;
    public event Action<Server, DateTime, Client>? ClientDisconnected;
    public event Action<Server, DateTime, string, Client>? MessageReceived;
    public event Action<Server, DateTime, string, Client>? MessageSent;

    public Server(int port, string ip = "127.0.0.1")
    {
        Port = port;
        Ip = ip;
    }

    public async Task StartAsync()
    {
        if (IsStarted) return;

        _cts = new CancellationTokenSource();
        _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _server.Bind(new IPEndPoint(IPAddress.Parse(Ip), Port));
        _server.Listen(100);

        IsStarted = true;
        ConnectionChanged?.Invoke(this, DateTime.Now, SocketConnectionState.ServerStarted);

        await AcceptClientsLoopAsync(_cts.Token);
        await Task.CompletedTask; // для соответствия async signature
    }

    public void Stop()
    {
        if (!IsStarted) return;

        _cts.Cancel();
        IsStarted = false;

        lock (_clientsLock)
        {
            foreach (var client in _clients)
                client.Disconnect();
            _clients.Clear();
        }

        try
        {
            _server.Shutdown(SocketShutdown.Both);
        }
        catch
        {
            /* ignore */
        }
        finally
        {
            _server.Close();
            _server.Dispose();
            ConnectionChanged?.Invoke(this, DateTime.Now, SocketConnectionState.ServerStopped);
        }
    }

    public void SendMessage(string message)
    {
        lock (_clientsLock)
        {
            foreach (var client in _clients)
            {
                _ = client.SendMessageAsync(message);
            }
        }
    }

    private async Task AcceptClientsLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var newClientSocket = await _server.AcceptAsync(token);
                var client = new Client(newClientSocket);

                client.MessageReceived += (c, dt, msg) =>
                    Task.Run(() => MessageReceived?.Invoke(this, dt, msg, c));

                client.MessageSent += (c, dt, msg) =>
                    Task.Run(() => MessageSent?.Invoke(this, dt, msg, c));

                client.ConnectionChanged += (c, dt, _) =>
                {
                    if (!c.IsConnected)
                    {
                        lock (_clientsLock) _clients.Remove(c);
                        ClientDisconnected?.Invoke(this, dt, c);
                    }
                };

                lock (_clientsLock) _clients.Add(client);
                ClientConnected?.Invoke(this, DateTime.Now, client);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (SocketException)
            {
                if (!token.IsCancellationRequested)
                    Stop();
                break;
            }
        }
    }

    public void Dispose()
    {
        Stop();
        _cts.Dispose();
    }
}