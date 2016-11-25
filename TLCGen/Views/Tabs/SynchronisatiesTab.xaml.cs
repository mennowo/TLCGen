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
using TLCGen.ViewModels.Enums;

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for ConflictMatrixView.xaml
    /// </summary>
    public partial class SynchronisatiesTab : UserControl
    {
        public SynchronisatiesTab()
        {
            InitializeComponent();
        }
    }

    public class SelectedDisplayTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SynchronisatieTypeEnum e1 = (SynchronisatieTypeEnum)value;
            SynchronisatieTypeEnum e2 = (SynchronisatieTypeEnum)parameter;
            if (e2.HasFlag(e1))
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectedDisplayTypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SynchronisatieTypeEnum e1 = (SynchronisatieTypeEnum)value;
            SynchronisatieTypeEnum e2 = (SynchronisatieTypeEnum)parameter;
            if (e2.HasFlag(e1))
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ReferencesSelfToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            if (b)
                return null;
            else
                return Brushes.DarkGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
