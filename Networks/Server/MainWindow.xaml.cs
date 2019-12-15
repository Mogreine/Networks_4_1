using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Server.src;
using System.Net;
using System.Threading;

namespace Server
{
    public partial class MainWindow : Window
    {
        public void shit()
        {

        }

        public MainWindow()
        {
            InitializeComponent();
            Action<string> lambda = AddMessage;
            OutputBox.Text = "232323232";
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            AsyncServer.AddText = lambda;
            new Task(AsyncServer.StartListening).Start();
        }

        public void AddMessage(string text)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                OutputBox.Text += text;
            }));
        }
    }
}
