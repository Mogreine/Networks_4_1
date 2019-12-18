using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using TaskServer.src;

namespace TaskServer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CancellationTokenSource cancelTokenSource;
        private CancellationToken token;
        private ServerAsync server; 

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartServer(object sender, RoutedEventArgs e)
        {
            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;
            Action<string> AddMessageAction = AddMessage;
            Action<string> PrintIp = EditPortBox;
            int port = Int32.Parse(PortBox.Text);
            server = new ServerAsync(port, AddMessageAction, token);
            server.StartServer();
            IpBox.Text = server.IP.ToString();
        }

        private void StopServer(object sender, RoutedEventArgs e)
        {
            cancelTokenSource.Cancel();
            server.StopServer();
            LogBox.Text += "Server is down.\r\n";
        }

        public void AddMessage(string text)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                LogBox.Text += text + "\r\n";
            }));
        }

        public void EditPortBox(string text)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                PortBox.Text += text;
            }));
        }
    }
}
