using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TLCGen.Extensions;

namespace TLCGen.Dialogs
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class NewVersionAvailableWindow : Window
    {
        public NewVersionAvailableWindow(string newver)
        {
            InitializeComponent();

            UsedVersionTB.Text = Assembly.GetCallingAssembly().GetName().Version.ToString();
            NewVersionTB.Text = newver;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DownloadHyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.codingconnected.eu/software/tlcgen/");
        }
    }
}
