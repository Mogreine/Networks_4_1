using System;
using System.Collections.Generic;
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
            int port = 8000;
            string serverIp = "192.168.0.10";
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(serverIp, port);

                var data = new byte[256];
                StringBuilder response = new StringBuilder();
                NetworkStream stream = client.GetStream();

                string msg = "I'm in bitch!!!";
                var msgBytes = Encoding.UTF8.GetBytes(msg);
                stream.Write(msgBytes, 0, msgBytes.Length);
                
                Console.WriteLine("Write messages to the server");
                msg = "";
                bool flag = true;
                do
                {
                    if (!client.Connected)
                        break;
                    msg = Console.ReadLine();
                    if (msg == "stop")
                    {
                        msg = "Bye server!";
                        flag = false;
                    }
                    data = Encoding.UTF8.GetBytes(msg);
                    stream.Write(data, 0, data.Length);
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
