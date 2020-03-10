using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace TLCGen.Dialogs
{
    /// <summary>
    /// Interaction logic for VersionAlertWindow.xaml
    /// </summary>
    public partial class VersionInfoWindow : Window
    {
        private List<Tuple<Version, string>> _versionData;
        private int _index;

        public VersionInfoWindow(string cVer, List<Tuple<Version, string>> versionData)
        {
            InitializeComponent();

            _versionData = versionData;
            _index = _versionData.Count - 1;
            LoadVersionInfo();
            this.ControllerVersionLabel.Content = cVer;
        }

        private void LoadVersionInfo()
        {
            if (_index >= 0 && _index < _versionData.Count)
            {
                var stream = new MemoryStream(Encoding.Default.GetBytes(_versionData[_index].Item2));
                this.VersionInfoTB.SelectAll();
                this.VersionInfoTB.Selection.Load(stream, DataFormats.Rtf);
                this.VersionLabel.Content = _versionData[_index].Item1.ToString();
            }
            this.PreviousButton.IsEnabled = _index > 0;
            this.NextButton.IsEnabled = _index < _versionData.Count - 1;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_index > 0) _index--;
            LoadVersionInfo();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_index < _versionData.Count - 1) _index++;
            LoadVersionInfo();
        }
    }
}
