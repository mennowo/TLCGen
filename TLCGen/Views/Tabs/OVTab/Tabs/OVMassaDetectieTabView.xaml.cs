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
using TLCGen.Models.Enumerations;

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for OVMassaDetectieTabView.xaml
    /// </summary>
    public partial class OVMassaDetectieTabView : UserControl
    {
        public OVMassaDetectieTabView()
        {
            InitializeComponent();
        }
    }

    public class IsInmeldingTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = (OVIngreepInUitMeldingType)value;
            switch (t)
            {
                case OVIngreepInUitMeldingType.Inmelding:
                    return true;
                case OVIngreepInUitMeldingType.Uitmelding:
                    return false;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
