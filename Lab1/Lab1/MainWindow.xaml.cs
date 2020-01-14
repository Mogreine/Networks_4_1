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

namespace Lab1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string RunCommand(string command)
        {
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = $"/C {command}",
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                StandardOutputEncoding = Encoding.GetEncoding(866)
            };
            process.StartInfo = startInfo;
            process.Start();

            var result = process.StandardOutput.ReadToEnd();
            return result;
        }

        private void RunPing(object sender, RoutedEventArgs e)
        {
            var command = new StringBuilder();
            command.Append("ping ");
            if (APing.IsChecked.Value)
            {
                command.Append("-a ");
            }
            if (NPing.IsChecked.Value && NValPing.Text != "")
            {
                command.Append($"-n {NValPing.Text} ");
            }
            if (LPing.IsChecked.Value && LValPing.Text != "")
            {
                command.Append($"-l {LValPing.Text} ");
            }
            if (FPing.IsChecked.Value)
            {
                command.Append("-f ");
            }
            if (IPing.IsChecked.Value && IValPing.Text != "")
            {
                command.Append($"-i {IValPing.Text} ");
            }
            if (WPing.IsChecked.Value && WValPing.Text != "")
            {
                command.Append($"-w {WValPing.Text} ");
            }
            command.Append(AddressValPing.Text);
            var respond = RunCommand(command.ToString());
            LogBox.Text = respond;
        }

        private void RunIpConfig(object sender, RoutedEventArgs e)
        {
            var command = new StringBuilder();
            command.Append("ipconfig ");
            if (AllIpConfig.IsChecked.Value)
            {
                command.Append("/all ");
            }
            else if (ReleaseIpConfig.IsChecked.Value)
            {
                command.Append($"/release {ReleaseValueIpConfig.Text} ");
            }
            else if (RenewIpConfig.IsChecked.Value)
            {
                command.Append($"/renew {RenewValueIpConfig.Text} ");
            }
            else if (FlushDNSIpConfig.IsChecked.Value)
            {
                command.Append("/flushdns");
            }
            else if (RegisterDNSIpConfig.IsChecked.Value)
            {
                command.Append($"/registerdns");
            }
            var respond = RunCommand(command.ToString());
            LogBox.Text = respond;
        }

        private void RunTracert(object sender, RoutedEventArgs e)
        {
            var command = new StringBuilder();
            command.Append("tracert ");
            if (DTracert.IsChecked.Value)
            {
                command.Append("-d ");
            }
            if (HTracert.IsChecked.Value && HValueTracert.Text != "")
            {
                command.Append($"-h {HValueTracert.Text} ");
            }
            if (WTracert.IsChecked.Value && WValueTracert.Text != "")
            {
                command.Append($"-w {WValueTracert.Text} ");
            }
            command.Append(AddressValueTracert.Text);
            var respond = RunCommand(command.ToString());
            LogBox.Text = respond;
        }
    }
}
