using System;
using System.Collections;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for SettingsDataGrid.xaml
    /// </summary>
    public partial class SettingsDataGrid : UserControl
    {
        // Using a DependencyProperty as the backing store for ConflictMatrix.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SettingsListProperty =
            DependencyProperty.Register("SettingsList", typeof(IList), typeof(SettingsDataGrid), new PropertyMetadata(default(IList)));

        public Array SettingsList
        {
            get { return (Array)GetValue(SettingsListProperty); }
            set { SetValue(SettingsListProperty, value); }
        }

        public SettingsDataGrid()
        {
            InitializeComponent();
        }
    }
}
