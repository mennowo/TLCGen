using System;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class HalfstarOVIngreepViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        #region Fields

        private HalfstarPrioIngreepModel _ovIngreep;

        #endregion // Fields

        #region Properties

        public HalfstarPrioIngreepModel OvIngreep => _ovIngreep;

        public PrioIngreepModel BelongsToOVIngreep { get; set; }

        public string FaseCyclus => BelongsToOVIngreep?.FaseCyclus;

        public int Prioriteit
        {
            get => _ovIngreep.Prioriteit;
            set
            {
                _ovIngreep.Prioriteit = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int GroenNaTXDTijd
        {
            get => _ovIngreep.GroenNaTXDTijd;
            set
            {
                _ovIngreep.GroenNaTXDTijd = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return _ovIngreep;
        }

        #endregion // IViewModelWithItem

        #region IComparable

        public int CompareTo(object other)
        {
            return string.Compare(FaseCyclus, ((HalfstarOVIngreepViewModel)other)?.FaseCyclus, StringComparison.Ordinal);
        }

        #endregion // IComparable

        #region Constructor

        public HalfstarOVIngreepViewModel(HalfstarPrioIngreepModel ovIngreep)
        {
            _ovIngreep = ovIngreep;
        }

        #endregion // Constructor
    }
}