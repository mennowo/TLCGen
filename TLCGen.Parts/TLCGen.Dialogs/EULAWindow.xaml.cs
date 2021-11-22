using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TLCGen.Extensions;

namespace TLCGen.Dialogs
{
    /// <summary>
    /// Interaction logic for EulaWindow.xaml
    /// </summary>
    public partial class EulaWindow : Window
    {
        public EulaWindow()
        {
            InitializeComponent();
            
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var eula = Path.Combine(path, "Licenses", "TLCGen_EULA.rtf");
            if (!File.Exists(eula))
            {
                Application.Current.Shutdown();
            }
            using var stream = new FileStream(eula, FileMode.Open, FileAccess.Read);
            EulaTextBox.Selection.Load(stream, DataFormats.Rtf);
        }

        private void MainGrid_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is Grid grid)
            {
                EulaTextBox.Document.PageWidth = grid.ActualWidth - 20;
                EulaTextBox.Document.PageHeight = 200;
                EulaTextBox.Width = grid.ActualWidth - 4;
            }
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
