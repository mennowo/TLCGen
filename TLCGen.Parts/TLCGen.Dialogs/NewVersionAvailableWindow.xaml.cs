using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private void UpdateNowButton_Click(object sender, RoutedEventArgs e)
        {
	        var p = Process.GetCurrentProcess();
	        var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Updater", "TLCGen.Updater.exe");
            Process.Start(path, p.Id.ToString());
			Application.Current.Shutdown();
        }

        private void UpdateLaterButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
