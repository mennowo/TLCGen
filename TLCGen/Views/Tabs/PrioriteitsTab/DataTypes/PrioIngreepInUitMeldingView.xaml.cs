using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TLCGen.Models.Enumerations;

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for PrioIngreepInUitMeldingView.xaml
    /// </summary>
    public partial class PrioIngreepInUitMeldingView : UserControl
    {
        public PrioIngreepInUitMeldingView()
        {
            InitializeComponent();
        }
    }

    public class InmeldingToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = (PrioIngreepInUitMeldingTypeEnum)value;
            switch (t)
            {
                case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                    return Visibility.Visible;
                case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                    return Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InmeldingToBoolConverter : IValueConverter
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
