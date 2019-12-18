using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskServer.src
{
    public static class Methods
    {
        public static Task<T> WithCancellation<T>(this Task<T> task,
            CancellationToken token)
        {
            return task.ContinueWith(t => t.GetAwaiter().GetResult(), token);
        }
    }

    class ServerAsync
    {
        private object _lock; // sync lock 
        private List<Task> _connections; // pending connections
        private Action<string> _logWrite;
        private CancellationToken _cancellationToken;
        public int Port;
        public IPAddress IP;
        private TcpListener tcpListener;

        public ServerAsync(int port, Action<string> logWrite, CancellationToken cancellationToken)
        {
            Port = port;
            _lock = new Object();
            _connections = new List<Task>();
            _logWrite = logWrite;
            _cancellationToken = cancellationToken;
            IP = GetLocalIPAddress();
        }

        public void StartServer()
        {
            tcpListener = new TcpListener(IP, Port);
            tcpListener.Start();
            StartListener();
        }

        public void StopServer()
        {
            tcpListener.Stop();
        }

        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        
        public Task StartListener()
        {
            return Task.Run(async () =>
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    var tcpClient = await tcpListener.AcceptTcpClientAsync().WithCancellation(_cancellationToken);
                    if (_cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    Console.WriteLine("[Server] Client has connected");
                    _logWrite("[Server] Client has connected");
                    var task = StartHandleConnectionAsync(tcpClient);
                    if (task.IsFaulted)
                        await task;
                }
            }, _cancellationToken);
        }

        // Register and handle the connection
        private async Task StartHandleConnectionAsync(TcpClient tcpClient)
        {
            // start the new connection task
            var connectionTask = HandleConnectionAsync(tcpClient);

            // add it to the list of pending task 
            lock (_lock)
                _connections.Add(connectionTask);

            // catch all errors of HandleConnectionAsync
            try
            {
                await connectionTask;
                // we may be on another thread after "await"
            }
            catch (Exception ex)
            {
                // log the error
                Console.WriteLine(ex.ToString());
                _logWrite(ex.ToString());
            }
            finally
            {
                // remove pending task
                lock (_lock)
                    _connections.Remove(connectionTask);
            }
        }

        private Task HandleConnectionAsync(TcpClient tcpClient)
        {
            return Task.Run(async () =>
            {
                using (var networkStream = tcpClient.GetStream())
                {
                    Console.WriteLine("[Server] Reading from client");
                    _logWrite("[Server] Reading from client");
                    bool stopCmd = false;

                    while (tcpClient.Connected && !stopCmd && !_cancellationToken.IsCancellationRequested)
                    {
                        if (networkStream.DataAvailable)
                        {
                            var buffer = new byte[4096];
                            var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                            var request = Encoding.UTF8.GetString(buffer, 0, byteCount);
                            Console.WriteLine("[Server] Client wrote {0}", request);
                            _logWrite($"[Server] Client wrote {request}");

                            request = request.Trim();
                            var tmp = request.Split(' ');
                            var cmd = tmp[0].ToLower();
                            byte[] respond = null;
                            if (tmp.Length == 1)
                            {
                                respond = Encoding.UTF8.GetBytes($"There's no parameter.");
                            }
                            else if (cmd == "hello")
                            {
                                respond = Encoding.UTF8.GetBytes($"hello variant {tmp[1]}");
                            }
                            else if (cmd == "bye")
                            {
                                respond = Encoding.UTF8.GetBytes($"bye variant {tmp[1]}");
                                stopCmd = true;
                            }
                            else if (cmd == "encrypt" || cmd == "decrypt")
                            {
                                for (int i = 2; i < tmp.Length; i++)
                                {
                                    tmp[1] += tmp[i];
                                }
                                var prms = tmp[1].Trim().Split(',');
                                var pass = prms.Last();

                                var pass_start = request.LastIndexOf(", ") + byteCount - request.Length;

                                var msgBytes = new List<byte>();
                                for (int i = 8; i < pass_start; i++)
                                {
                                    msgBytes.Add(buffer[i]);
                                }

                                if (cmd == "encrypt")
                                {
                                    respond = Cypher.Encrypt(msgBytes.ToArray(), pass);
                                }
                                else
                                {
                                    respond = Cypher.Decrypt(msgBytes.ToArray(), pass);
                                }
                            }
                            else
                            {
                                respond = Encoding.UTF8.GetBytes("Invalid command.");
                            }
                            await networkStream.WriteAsync(respond, 0, respond.Length);
                            Console.WriteLine($"[Server] Response has been written: {new UTF8Encoding().GetString(respond)}");
                            _logWrite($"[Server] Response has been written: {new UTF8Encoding().GetString(respond)}");
                        }
                    }

                    _logWrite($"[Server] Client disconected");
                }
            }, _cancellationToken);
        }
    }
}
