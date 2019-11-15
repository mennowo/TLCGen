using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for ModulesAlternatievenPerBlokTabView.xaml
    /// </summary>
    public partial class ModulesAlternatievenPerBlokTabView : UserControl
    {
        public ModulesAlternatievenPerBlokTabView()
        {
            InitializeComponent();
        }
    }

    public class NumberOfModulesToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse((string)parameter) <= ((int)value) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
