using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Settings
{
    public class DefaultsTabViewModel : ViewModelBase
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
            get { return _SelectedFaseCyclusDefault; }
            set
            {
                _SelectedFaseCyclusDefault = value;
                RaisePropertyChanged("SelectedFaseCyclusDefault");
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
            get { return _SelectedDetectorDefault; }
            set
            {
                _SelectedDetectorDefault = value;
                RaisePropertyChanged("SelectedDetectorDefault");
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
            get { return _SelectedOtherDefault; }
            set
            {
                _SelectedOtherDefault = value;
                RaisePropertyChanged("SelectedOtherDefault");
            }
        }

        #endregion // Properties


        #region Private Methods

        #endregion // Private Methods

        #region Constructor

        public DefaultsTabViewModel()
        {
            foreach (var fc in DefaultsProvider.Default.Defaults.Defaults)
            {
                if (fc.Category == "FaseCyclus") FaseCyclusDefaults.Add(fc);
                if (fc.Category == "Detector") DetectorDefaults.Add(fc);
                if (fc.Category == "Other") OtherDefaults.Add(fc);
            }
            if (FaseCyclusDefaults.Count > 0) SelectedFaseCyclusDefault = FaseCyclusDefaults[0];
            if (DetectorDefaults.Count > 0) SelectedDetectorDefault = DetectorDefaults[0];
            if (OtherDefaults.Count > 0) SelectedOtherDefault = OtherDefaults[0];
        }

        #endregion // Constructor
    }
}
