using System;
using System.Collections.Generic;
using System.Linq;
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
