using System;
using System.Collections.Generic;
using System.Linq;
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
using TaskServer.src;

namespace TaskServer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Action<string> AddMessageAction = AddMessage;
            var server = new ServerAsync(8000, AddMessageAction);
            server.StartListener();
        }

        public void AddMessage(string text)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                LogBox.Text += text + "\r\n";
            }));
        }
    }
}
