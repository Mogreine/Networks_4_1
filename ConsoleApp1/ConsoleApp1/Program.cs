using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        public static int Main(String[] args)
        {
            int port = 123;
            string serverIp = "192.168.0.10";
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(serverIp, port);

                var data = new byte[1024];
                NetworkStream stream = client.GetStream();
                
                var request = "";
                Console.WriteLine("Write messages to the server");
                request = "";
                bool flag = true;
                do
                {
                    if (!client.Connected)
                        break;
                    request = Console.ReadLine();
                    var req_splt = request.Split(' ');
                    if (req_splt[0] == "bye")
                    {
                        flag = false;
                        data = Encoding.UTF8.GetBytes(request);
                    }
                    else if (req_splt[0].ToLower() == "encrypt" || req_splt[0].ToLower() == "decrypt")
                    {
                        var msg_bytes = File.ReadAllBytes(@"input.txt");
                        var tmp = new List<byte>();
                        tmp.AddRange(Encoding.UTF8.GetBytes(req_splt[0] + " "));
                        tmp.AddRange(msg_bytes);
                        tmp.AddRange(Encoding.UTF8.GetBytes(", " + req_splt[1]));
                        data = tmp.ToArray();
                    }
                    else
                    {
                        data = Encoding.UTF8.GetBytes(request);
                    }

                    stream.Write(data, 0, data.Length);

                    var responseBytes = new byte[1024];
                    int readBytes = stream.Read(responseBytes, 0, responseBytes.Length);
                    responseBytes = responseBytes.Take(readBytes).ToArray();
                    var response = new UTF8Encoding().GetString(responseBytes);
                    File.WriteAllBytes(@"output.txt", responseBytes);
                    if (req_splt[0].ToLower() == "encrypt")
                    {
                        File.WriteAllBytes(@"input.txt", responseBytes);
                    }

                    Console.WriteLine(response);
                } while (flag);                    
                // Закрываем потоки
                stream.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

            Console.WriteLine("Запрос завершен...");
            Console.Read();
            return 0;
        }
    }
}
