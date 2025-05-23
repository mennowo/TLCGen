﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class TimingsFaseCyclusDataViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        #region Properties
        
        public TimingsFaseCyclusDataModel TimingsFase { get; }

        public string FaseCyclus
        {

            get => TimingsFase.FaseCyclus;
            set
            {
                TimingsFase.FaseCyclus = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public TimingsFaseCyclusTypeEnum ConflictType
        {
            get => TimingsFase.ConflictType;
            set
            {
                TimingsFase.ConflictType = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return TimingsFase;
        }

        #endregion // IViewModelWithItem

        #region IComparable

        public int CompareTo(object obj)
        {
            return string.CompareOrdinal(FaseCyclus, ((TimingsFaseCyclusDataViewModel)obj).FaseCyclus);
        }

        #endregion // IComparable

        #region Constructor

        public TimingsFaseCyclusDataViewModel(TimingsFaseCyclusDataModel timingsFase)
        {
            TimingsFase = timingsFase;
        }

        #endregion // Constructor
    }
}
