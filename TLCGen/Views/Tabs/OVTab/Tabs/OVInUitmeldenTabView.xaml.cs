using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using TLCGen.Models.Enumerations;

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for OVInUitmeldenTabView.xaml
    /// </summary>
    public partial class OVInUitmeldenTabView : UserControl
    {
        public OVInUitmeldenTabView()
        {
            InitializeComponent();
        }
    }

    public class IsInmeldingTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = (OVIngreepInUitMeldingTypeEnum)value;
            switch (t)
            {
                case OVIngreepInUitMeldingTypeEnum.Inmelding:
                    return true;
                case OVIngreepInUitMeldingTypeEnum.Uitmelding:
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
