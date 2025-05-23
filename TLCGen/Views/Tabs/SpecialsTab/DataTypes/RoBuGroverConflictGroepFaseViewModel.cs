﻿using System;
using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RoBuGroverConflictGroepFaseViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        #region Fields

        private RoBuGroverConflictGroepFaseModel _Fase;

        #endregion // Fields

        #region Properties

        public string FaseCyclus
        {
            get => _Fase.FaseCyclus;
            set
            {
                _Fase.FaseCyclus = value;
                OnPropertyChanged(nameof(FaseCyclus), broadcast: true);
            }
        }

        #endregion Properties

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public Methods

        #region Overrides

        public override string ToString()
        {
            return FaseCyclus;
        }

        #endregion // Overrides

        #region IViewModelWithItem

        public object GetItem()
        {
            return _Fase;
        }

        #endregion // IViewModelWithItem

        #region IComparable

        public int CompareTo(object obj)
        {
            if(obj is RoBuGroverConflictGroepFaseViewModel)
            {
                return this.FaseCyclus.CompareTo(((RoBuGroverConflictGroepFaseViewModel)obj).FaseCyclus);
            }
            return 0;
        }

        #endregion // IComparable

        #region Constructor

        public RoBuGroverConflictGroepFaseViewModel(RoBuGroverConflictGroepFaseModel fase)
        {
            _Fase = fase;
        }

        #endregion // Constructor
    }
}
