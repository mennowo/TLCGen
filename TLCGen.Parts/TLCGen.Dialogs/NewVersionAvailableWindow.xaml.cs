using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

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
