using System.Diagnostics;
using System.Reflection;
using System.Windows;
using TLCGen.Extensions;

namespace TLCGen.Dialogs
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            VersionTB.Text = Assembly.GetCallingAssembly().GetName().Version.ToString();
            DateTB.Text = Assembly.GetCallingAssembly().GetLinkerTime().ToLongDateString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void InfoHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "mailto:menno@codingconnected.eu",
                UseShellExecute = true
            });
        }

        private void WikiHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.codingconnected.eu/tlcgenwiki/",
                UseShellExecute = true
            });
        }

        private void FontHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "http://www.marksimonson.com/fonts/view/anonymous-pro",
                UseShellExecute = true
            });
        }

        private void IconsHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "http://p.yusukekamiyamane.com/",
                UseShellExecute = true
            });
        }
    }
}
