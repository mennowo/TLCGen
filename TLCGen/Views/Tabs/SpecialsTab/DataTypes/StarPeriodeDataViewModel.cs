using System;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class StarPeriodeDataViewModel : ViewModelBase, IViewModelWithItem, IComparable
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
                RaisePropertyChanged<object>(broadcast: true);
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