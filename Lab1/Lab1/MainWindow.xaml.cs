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
    }
}
