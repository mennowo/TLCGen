using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TLCGen.Views
{
    public class ConflictBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() == "FK")
                return Brushes.LightGray;
            else if (value != null && value.ToString() == "GK")
                return Brushes.DarkSeaGreen;
            else if (value != null && value.ToString() == "GKL")
                return Brushes.MediumAquamarine;
            else if (value != null && value.ToString() == "*")
                return Brushes.LightCoral;
            else if (value != null && value.ToString() != "")
                return Brushes.PowderBlue;
            return null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
