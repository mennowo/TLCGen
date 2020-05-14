using System;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class FaseCyclusAlternatiefPerBlokViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        #region Fields

        private readonly FaseCyclusAlternatiefPerBlokModel _faseCyclus;
        
        #endregion // Fields

        #region Properties

        public string FaseCyclus
        {
            get => _faseCyclus.FaseCyclus;
            set => _faseCyclus.FaseCyclus = value;
        }

        public int BitWiseBlokAlternatief
        {
            get => _faseCyclus.BitWiseBlokAlternatief;
            set
            {
                _faseCyclus.BitWiseBlokAlternatief = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged("");
            }
        }

        public bool ML1
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b000000001) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b000000001;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b000000001;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML2
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b000000010) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b000000010;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b000000010;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML3
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b000000100) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b000000100;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b000000100;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML4
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b000001000) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b000001000;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b000001000;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML5
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b000010000) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b000010000;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b000010000;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML6
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b000100000) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b000100000;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b000100000;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML7
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b001000000) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b001000000;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b001000000;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML8
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b010000000) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b010000000;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b010000000;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML9
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b100000000) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b100000000;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b100000000;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return _faseCyclus;
        }

        #endregion // IViewModelWithItem

        #region IComparable

        public int CompareTo(object obj)
        {
            var other = obj as FaseCyclusAlternatiefPerBlokViewModel;
            return other != null ? string.Compare(FaseCyclus, other.FaseCyclus, StringComparison.Ordinal) : 0;
        }
        
        #endregion // IComparable

        #region Constructor

        public FaseCyclusAlternatiefPerBlokViewModel(FaseCyclusAlternatiefPerBlokModel faseCyclus)
        {
            _faseCyclus = faseCyclus;
        }

        #endregion // Constructor
    }
}