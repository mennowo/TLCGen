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
            System.Diagnostics.Process.Start("mailto:menno@codingconnected.eu");
        }

        private void FontHyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.marksimonson.com/fonts/view/anonymous-pro");
        }

        private void IconsHyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://p.yusukekamiyamane.com/");
        }
    }
}
