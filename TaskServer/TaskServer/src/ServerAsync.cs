using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskServer.src
{
    class ServerAsync
    {
        private object _lock; // sync lock 
        private List<Task> _connections; // pending connections
        private Action<string> _logWrite;

        public ServerAsync(int port, Action<string> logWrite)
        {
            _lock = new Object();
            _connections = new List<Task>();
            _logWrite = logWrite;
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

                    while (tcpClient.Connected)
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
                                respond = Encoding.UTF8.GetBytes($"hello variat {tmp[1]}");
                            }
                            else if (cmd == "bye")
                            {
                                respond = Encoding.UTF8.GetBytes($"bye variat {tmp[1]}");
                                break;
                            }
                            else if (cmd == "encrypt")
                            {
                                for (int i = 2; i < tmp.Length; i++)
                                {
                                    tmp[1] += tmp[i];
                                }
                                var prms = tmp[1].Trim().Split(',');
                                var msg = "";
                                var pass_start = msg.LastIndexOf(", ") + 2;
                                var pass = prms.Last();
                                for (int i = 0; i < prms.Length - 1; i++)
                                {
                                    msg += prms[i];
                                }

                                respond = Cypher.Encrypt(Encoding.UTF8.GetBytes(msg), pass);
                            }
                            else if (cmd == "decrypt")
                            {
                                for (int i = 2; i < tmp.Length; i++)
                                {
                                    tmp[1] += tmp[i];
                                }
                                var prms = tmp[1].Trim().Split(',');
                                var msg = "";
                                var pass = prms.Last();
                                for (int i = 0; i < prms.Length - 1; i++)
                                {
                                    msg += prms[i];
                                }

                                respond = Cypher.Decrypt(Encoding.UTF8.GetBytes(msg), pass);
                            }
                            else
                            {
                                respond = Encoding.UTF8.GetBytes($"Invalid command.");
                            }
                            await networkStream.WriteAsync(respond, 0, respond.Length);
                            Console.WriteLine($"[Server] Response has been written: {new UTF8Encoding().GetString(respond)}");
                            _logWrite($"[Server] Response has been written: {new UTF8Encoding().GetString(respond)}");
                        }
                    }

                    _logWrite($"[Server] Client disconected");
                    /*
                    var serverResponseBytes = Encoding.UTF8.GetBytes("Hello from server");
                    await networkStream.WriteAsync(serverResponseBytes, 0, serverResponseBytes.Length);
                    Console.WriteLine("[Server] Response has been written");
                    _logWrite("[Server] Response has been written");
                    */
                }
            });
        }

        public Task StartListener()
        {
            return Task.Run(async () =>
            {
                var tcpListener = TcpListener.Create(8000);
                tcpListener.Start();
                while (true)
                {
                    var tcpClient = await tcpListener.AcceptTcpClientAsync();
                    Console.WriteLine("[Server] Client has connected");
                    _logWrite("[Server] Client has connected");
                    var task = StartHandleConnectionAsync(tcpClient);
                    if (task.IsFaulted)
                        await task;
                }
            });
        }
    }
}
