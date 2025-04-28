using System;
using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class FaseCyclusAlternatiefPerBlokViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
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
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged("");
            }
        }

        public bool ML1
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b000000001) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b000000001;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b000000001;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML2
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b000000010) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b000000010;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b000000010;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML3
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b000000100) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b000000100;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b000000100;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML4
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b000001000) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b000001000;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b000001000;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML5
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b000010000) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b000010000;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b000010000;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML6
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b000100000) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b000100000;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b000100000;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML7
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b001000000) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b001000000;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b001000000;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML8
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b010000000) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b010000000;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b010000000;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(BitWiseBlokAlternatief));
            }
        }

        public bool ML9
        {
            get => (_faseCyclus.BitWiseBlokAlternatief & 0b100000000) > 0;
            set
            {
                if (value) _faseCyclus.BitWiseBlokAlternatief |= 0b100000000;
                else _faseCyclus.BitWiseBlokAlternatief &= ~0b100000000;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(BitWiseBlokAlternatief));
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