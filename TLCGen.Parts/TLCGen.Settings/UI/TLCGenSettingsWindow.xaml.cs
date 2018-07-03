using System.Windows;

namespace TLCGen.Settings.Views
{
    /// <summary>
    /// Interaction logic for TLCGenSettingsWindow.xaml
    /// </summary>
    public partial class TLCGenSettingsWindow : Window
    {
        public TLCGenSettingsWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TemplatesProvider.Default.SaveSettings();
            this.Close();
        }
    }
}
