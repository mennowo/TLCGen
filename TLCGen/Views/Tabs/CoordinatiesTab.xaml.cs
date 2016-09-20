using System;
using System.Collections.Generic;
using System.Globalization;
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
using TLCGen.Extensions;

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for ConflictMatrixView.xaml
    /// </summary>
    public partial class CoordinatiesTab : UserControl
    {
        public CoordinatiesTab()
        {
            InitializeComponent();
        }
    }

    public class SelectedTabToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string tab = parameter as string;
            TabItem ti = value as TabItem;
            if (ti?.Name == tab)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
