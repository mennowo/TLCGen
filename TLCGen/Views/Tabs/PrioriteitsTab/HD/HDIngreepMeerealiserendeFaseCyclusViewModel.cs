﻿using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class HDIngreepMeerealiserendeFaseCyclusViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private HDIngreepMeerealiserendeFaseCyclusModel _FaseCyclus;

        #endregion // Fields

        #region Properties

        public HDIngreepMeerealiserendeFaseCyclusModel FaseCyclus
        {
            get => _FaseCyclus;
            set
            {
                _FaseCyclus = value;
                RaisePropertyChanged<object>(nameof(FaseCyclus), broadcast: true);
            }
        }

        public string Fase
        {
            get => _FaseCyclus.FaseCyclus;
            set
            {
                _FaseCyclus.FaseCyclus = value;
                RaisePropertyChanged<object>(nameof(Fase), broadcast: true);
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return _FaseCyclus;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public HDIngreepMeerealiserendeFaseCyclusViewModel(HDIngreepMeerealiserendeFaseCyclusModel fase)
        {
            _FaseCyclus = fase;
        }

        #endregion // Constructor
    }
}
