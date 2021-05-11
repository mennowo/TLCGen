using System;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class HalfstarOVIngreepViewModel : ViewModelBase, IViewModelWithItem, IComparable
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
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int GroenNaTXDTijd
        {
            get => _ovIngreep.GroenNaTXDTijd;
            set
            {
                _ovIngreep.GroenNaTXDTijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        #endregion // Properties

        #region Commands
        #endregion // Commands

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