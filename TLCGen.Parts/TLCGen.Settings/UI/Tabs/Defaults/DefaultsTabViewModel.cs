﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace TLCGen.Settings
{
    public class DefaultsTabViewModel : ObservableObject
    {
        #region Fields

        private TLCGenDefaultModel _SelectedFaseCyclusDefault;
        private TLCGenDefaultModel _SelectedDetectorDefault;
        private TLCGenDefaultModel _SelectedOtherDefault;
        private ObservableCollection<TLCGenDefaultModel> _FaseCyclusDefaults;
        private ObservableCollection<TLCGenDefaultModel> _DetectorDefaults;
        private ObservableCollection<TLCGenDefaultModel> _OtherDefaults;

        #endregion // Fields

        #region Properties

        public ObservableCollection<TLCGenDefaultModel> FaseCyclusDefaults
        {
            get
            {
                if (_FaseCyclusDefaults == null)
                {
                    _FaseCyclusDefaults = new ObservableCollection<TLCGenDefaultModel>();
                }
                return _FaseCyclusDefaults;
            }
        }

        public TLCGenDefaultModel SelectedFaseCyclusDefault
        {
            get => _SelectedFaseCyclusDefault;
            set
            {
                _SelectedFaseCyclusDefault = value;
                OnPropertyChanged("SelectedFaseCyclusDefault");
            }
        }

        public ObservableCollection<TLCGenDefaultModel> DetectorDefaults
        {
            get
            {
                if (_DetectorDefaults == null)
                {
                    _DetectorDefaults = new ObservableCollection<TLCGenDefaultModel>();
                }
                return _DetectorDefaults;
            }
        }

        public TLCGenDefaultModel SelectedDetectorDefault
        {
            get => _SelectedDetectorDefault;
            set
            {
                _SelectedDetectorDefault = value;
                OnPropertyChanged("SelectedDetectorDefault");
            }
        }

        public ObservableCollection<TLCGenDefaultModel> OtherDefaults
        {
            get
            {
                if (_OtherDefaults == null)
                {
                    _OtherDefaults = new ObservableCollection<TLCGenDefaultModel>();
                }
                return _OtherDefaults;
            }
        }

        public TLCGenDefaultModel SelectedOtherDefault
        {
            get => _SelectedOtherDefault;
            set
            {
                _SelectedOtherDefault = value;
                OnPropertyChanged("SelectedOtherDefault");
            }
        }

        #endregion // Properties

        #region Private Methods

        private void RefreshDefaults()
        {
            FaseCyclusDefaults.Clear();
            DetectorDefaults.Clear();
            OtherDefaults.Clear();
            SelectedFaseCyclusDefault = null;
            SelectedDetectorDefault = null;
            SelectedOtherDefault = null;
            if (DefaultsProvider.Default.Defaults == null) return;
            foreach (var def in DefaultsProvider.Default.Defaults.Defaults)
            {
                if (def.Category == "FaseCyclus") FaseCyclusDefaults.Add(def);
                if (def.Category == "Detector") DetectorDefaults.Add(def);
                if (def.Category == "Other") OtherDefaults.Add(def);
            }
            if (FaseCyclusDefaults.Count > 0) SelectedFaseCyclusDefault = FaseCyclusDefaults[0];
            if (DetectorDefaults.Count > 0) SelectedDetectorDefault = DetectorDefaults[0];
            if (OtherDefaults.Count > 0) SelectedOtherDefault = OtherDefaults[0];
        }

        #endregion // Private Methods

        #region Constructor

        public DefaultsTabViewModel()
        {
            RefreshDefaults();
            DefaultsProvider.Default.DefaultsChanged += (o, e) =>
            {
                RefreshDefaults();
            };
        }

        #endregion // Constructor
    }
}
