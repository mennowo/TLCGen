using System;
using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class StarPeriodeDataViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        #region Properties

        public StarPeriodeDataModel StarPeriode { get; }

        public string Periode => StarPeriode.Periode;

        public string StarProgramma
        {
            get => StarPeriode.StarProgramma;
            set
            {
                StarPeriode.StarProgramma = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem() => StarPeriode;

        #endregion // IViewModelWithItem

        #region IComparable

        public int CompareTo(object obj)
        {
            return string.Compare(Periode, ((StarPeriodeDataViewModel) obj).Periode, StringComparison.Ordinal);
        }

        #endregion // IComparable

        #region Constructor

        public StarPeriodeDataViewModel(StarPeriodeDataModel starPeriode)
        {
            StarPeriode = starPeriode;
        }

        #endregion // Constructor
    }
}