using System.Windows;

namespace TLCGen.Dialogs
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
            this.Close();
        }
    }
}
