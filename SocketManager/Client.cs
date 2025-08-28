using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketManager
{
    public class Client : IDisposable
    {
        private Socket? _socket;
        private CancellationTokenSource? _cts;

        private readonly bool _isServerClient;
        private SocketConnectionState _connectionState;
        private Task? _listenTask;

        public bool IsConnected => _connectionState == SocketConnectionState.ClientConnected;
        public string Ip { get; }
        public int Port { get; }
        public string Address => $"{Ip}:{Port}";

        public event Action<Client, DateTime, SocketConnectionState>? ConnectionChanged;
        public event Action<Client, DateTime, string>? MessageReceived;
        public event Action<Client, DateTime, string>? MessageSent;

        public Client(string? ip, int port)
        {
            Ip = ip;
            Port = port;
            _connectionState = SocketConnectionState.ClientDisconnected;
            _isServerClient = false;
        }

        public Client(Socket socket)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _connectionState = SocketConnectionState.ClientConnected;
            _isServerClient = true;

            Ip = (_socket.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? "Unknown";
            Port = (_socket.RemoteEndPoint as IPEndPoint)?.Port ?? 0;

            StartListening();
        }

        private void StartListening()
        {
            _cts = new CancellationTokenSource();
            _listenTask = ListenAsync(_cts.Token);
        }

        public async Task ConnectAsync()
        {
            if (_isServerClient || IsConnected)
                return;

            _cts?.Cancel();
            _cts?.Dispose();

            _cts = new CancellationTokenSource();

            try
            {
                var ipPoint = new IPEndPoint(IPAddress.Parse(Ip), Port);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await _socket.ConnectAsync(ipPoint);

                ChangeConnectionState(SocketConnectionState.ClientConnected);

                _listenTask = ListenAsync(_cts.Token);
            }
            catch (SocketException)
            {
                ChangeConnectionState(SocketConnectionState.ConnectionRefused);
                await RecoveryConnectAsync();
            }
        }

        public async Task SendMessageAsync(string message)
        {
            if (!IsConnected || string.IsNullOrEmpty(message) || _socket == null)
                return;

            try
            {
                var data = Encoding.UTF8.GetBytes(message);
                await _socket.SendAsync(data, SocketFlags.None);
                MessageSent?.Invoke(this, DateTime.Now, message);
            }
            catch
            {
                await HandleDisconnectAsync();
            }
        }

        public void Disconnect()
        {
            _ = HandleDisconnectAsync();
        }

        private async Task HandleDisconnectAsync()
        {
            if (_connectionState == SocketConnectionState.ClientDisconnected)
                return;

            try
            {
                _cts?.Cancel();
                _socket?.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                /* ignore */
            }
            finally
            {
                _socket?.Close();
                _socket?.Dispose();
                _socket = null;

                ChangeConnectionState(SocketConnectionState.ClientDisconnected);
            }

            await Task.CompletedTask;
        }

        private async Task ListenAsync(CancellationToken token)
        {
            var buffer = new byte[1024];

            try
            {
                while (!token.IsCancellationRequested && _socket?.Connected == true)
                {
                    int bytes;
                    try
                    {
                        bytes = await _socket.ReceiveAsync(buffer, SocketFlags.None, token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (SocketException)
                    {
                        ChangeConnectionState(SocketConnectionState.ServerStopped);
                        if (!_isServerClient) await RecoveryConnectAsync();
                        break;
                    }

                    if (bytes == 0)
                    {
                        ChangeConnectionState(SocketConnectionState.ServerStopped);
                        if (!_isServerClient) await RecoveryConnectAsync();
                        break;
                    }

                    var msg = Encoding.UTF8.GetString(buffer, 0, bytes);
                    await Task.Run(() => MessageReceived?.Invoke(this, DateTime.Now, msg), token);
                }
            }
            catch
            {
                ChangeConnectionState(SocketConnectionState.ClientDisconnected);
                if (!_isServerClient) await RecoveryConnectAsync();
            }
        }

        public async Task RecoveryConnectAsync()
        {
            if (_isServerClient) return;

            ChangeConnectionState(SocketConnectionState.ClientRecoveryConnection);

            while (!IsConnected)
            {
                await Task.Delay(2000);
                await ConnectAsync();
            }
        }

        private void ChangeConnectionState(SocketConnectionState state)
        {
            _connectionState = state;
            Task.Run(() => ConnectionChanged?.Invoke(this, DateTime.Now, state));
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _socket?.Dispose();
        }
    }
}