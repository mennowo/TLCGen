using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                var webRequest = WebRequest.Create(@"https://codingconnected.eu/wp-json/wp/v2/pages/1105");
                webRequest.UseDefaultCredentials = true;
                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                    if (content != null)
                    {
                        using (var reader = new StreamReader(content))
                        {
                            var strContent = reader.ReadToEnd().Replace("\n", "");
                            var jsonDeserializer = new JavaScriptSerializer();
                            var deserializedJson = jsonDeserializer.Deserialize<dynamic>(strContent);
                            if (deserializedJson == null) return;
                            var contentData = deserializedJson["content"];
                            if (contentData == null) return;
                            var renderedData = contentData["rendered"];
                            if (renderedData == null) return;
                            var data = renderedData as string;
                            if (data == null) return;
                            var all = data.Split('\r');
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
                                    newversrn = newversrn + WebUtility.HtmlDecode(l).Replace("\r", "").Replace("\n", "") + Environment.NewLine;
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
