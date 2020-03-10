using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

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
            get => (Array)GetValue(SettingsListProperty);
            set => SetValue(SettingsListProperty, value);
        }

        public SettingsDataGrid()
        {
            InitializeComponent();
        }
    }
}
