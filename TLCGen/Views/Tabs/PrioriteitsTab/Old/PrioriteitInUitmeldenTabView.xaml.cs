using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using TLCGen.Models.Enumerations;

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for PrioriteitInUitmeldenTabView.xaml
    /// </summary>
    public partial class PrioriteitInUitmeldenTabView : UserControl
    {
        public PrioriteitInUitmeldenTabView()
        {
            InitializeComponent();
        }
    }

    public class IsInmeldingTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = (PrioIngreepInUitMeldingTypeEnum)value;
            switch (t)
            {
                case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                    return true;
                case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
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
