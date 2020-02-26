using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TLCGen.Updater
{
	/// <summary>
	/// Interaction logic for UpdaterHome.xaml
	/// </summary>
	public partial class UpdaterHome : UserControl, ISwitchable
	{
        #region Constructor

        public UpdaterHome()
        {
            InitializeComponent();

            // Find out if there is a newer version available via Wordpress REST API
            Task.Run(() =>
            {
                var webRequest = WebRequest.Create(@"https://www.codingconnected.eu/tlcgen/deploy/tlcgenversioning");
                webRequest.UseDefaultCredentials = true;
                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                    if (content != null)
                    {
                        using (var reader = new StreamReader(content))
                        {
                            var data = reader.ReadToEnd();
                            var all = data.Split('\n');
                            var tlcgenVer = all.FirstOrDefault(v => v.StartsWith("TLCGen="));
                            var newvers = tlcgenVer.Replace("TLCGen=", "").Split('.');
                            var newversrn = "";
                            var rn = false;
                            foreach (var l in all)
                            {
                                if (l.Contains("RN==TLCGen"))
                                {
                                    rn = !rn;
                                    continue;
                                }
                                if (rn)
                                {
                                    newversrn = newversrn + WebUtility.HtmlDecode(l).Replace("\r", "") + Environment.NewLine;
                                }
                            }
                            Dispatcher.Invoke(() => UpdateInfoTB.Text = (newversrn == "" ? "Geen informatie beschikbaar." : newversrn));
                        }
                    }
            });
        }

		#endregion // Constructor

		#region ISwitchable
		
		public void UtilizeState(object state)
		{
			this.DataContext = state;
		}
		
		#endregion // ISwitchable

		private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
		{
			Switcher.Switch(new UpdaterDownload(), this.DataContext);
		}

		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
