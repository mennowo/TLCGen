using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using TLCGen.ViewModels;
using TLCGen.ViewModels.Enums;

namespace TLCGen.Views
{
    public class ConflictBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Count() == 4)
            {
                var ReferencesSelf = (bool)values[0];
                var IsEnabled = (bool)values[1];
                var DisplayType = (SynchronisatieTypeEnum)values[2];
                var ConflictValue = (string)values[3];

                if (ReferencesSelf)
                {
                    return Brushes.DarkGray;
                }

                if (!IsEnabled)
                {
                    return Brushes.LightGray;
                }

                switch (DisplayType)
                {
                    case SynchronisatieTypeEnum.Conflict:
                    case SynchronisatieTypeEnum.GarantieConflict:
                        if (!string.IsNullOrEmpty(ConflictValue))
                        {
                            int i;
                            if (!Int32.TryParse(ConflictValue, out i))
                            {
                                switch (ConflictValue)
                                {
                                    case "FK":
                                        return Brushes.LightYellow;
                                    case "GK":
                                        return Brushes.DarkSeaGreen;
                                    case "GKL":
                                        return Brushes.MediumAquamarine;
                                    default:
                                        return Brushes.OrangeRed;
                                }
                            }
                            else
                                return Brushes.LightBlue;
                        }
                        return null;
                    case SynchronisatieTypeEnum.Gelijkstart:
                    case SynchronisatieTypeEnum.Naloop:
                    case SynchronisatieTypeEnum.Voorstart:
                        if (IsEnabled)
                            return null;
                        else
                            return Brushes.DarkGray;
                    default:
                        return null;
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
