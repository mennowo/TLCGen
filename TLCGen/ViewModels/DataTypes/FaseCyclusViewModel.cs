using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;
using TLCGen.Models;
using TLCGen.DataAccess;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class FaseCyclusViewModel : ViewModelBase, IComparable
    {
        #region Fields

        private FaseCyclusModel _FaseCyclus;
        private ObservableCollection<DetectorViewModel> _Detectoren;
        private ObservableCollection<ConflictViewModel> _Conflicten;
        private ControllerViewModel _ControllerVM;

        #endregion // Fields

        #region Properties

        public FaseCyclusModel FaseCyclus
        {
            get { return _FaseCyclus; }
        }

        public string Naam
        {
            get { return _FaseCyclus.Naam; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && _ControllerVM.IsElementNaamUnique(value))
                {
                    _FaseCyclus.Naam = value;
                }
                OnMonitoredPropertyChanged("Naam", _ControllerVM);
            }
        }

        public string Define
        {
            get { return _FaseCyclus.Define; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && _ControllerVM.IsElementDefineUnique(value))
                {
                    string oldname = _FaseCyclus.Define;
                    _FaseCyclus.Naam = value.Replace(SettingsProvider.GetFaseCyclusDefinePrefix(), "");
                    _FaseCyclus.Define = value;
                    foreach (ConflictViewModel cvm in Conflicten)
                    {
                        cvm.FaseVan = value;
                    }
                    _ControllerVM.ChangeFaseDefine(this, oldname);

                    // set new type
                    this.Type = Settings.Utilities.FaseCyclusUtilities.GetFaseTypeFromDefine(value);
                    _ControllerVM.HasChangedFasen = true;
                }
                OnMonitoredPropertyChanged(null, _ControllerVM); // Update all properties
            }
        }

        public FaseTypeEnum Type
        {
            get { return _FaseCyclus.Type; }
            set
            {
                if (_FaseCyclus.Type != value)
                {
                    _FaseCyclus.Type = value;

                    // Apply new defaults
                    SettingsProvider.ApplyDefaultFaseCyclusSettings(this.FaseCyclus, this.Type);

                    // Set default maxgroentijd
                    foreach (MaxGroentijdenSetViewModel mgsvm in _ControllerVM.MaxGroentijdenSets)
                    {
                        foreach (MaxGroentijdViewModel mgvm in mgsvm.MaxGroentijdenSetList)
                        {
                            if (mgvm.FaseCyclus == this.Define)
                                mgvm.Waarde = Settings.Utilities.FaseCyclusUtilities.GetFaseDefaultMaxGroenTijd(value);
                        }
                    }

                    OnMonitoredPropertyChanged(null, _ControllerVM); // Update all properties
                    _ControllerVM.SetAllSelectedElementsValue(this, "Type");
                }
            }
        }

        public int TFG
        {
            get { return _FaseCyclus.TFG; }
            set
            {
                if (value >= 0 && value >= TGG)
                    _FaseCyclus.TFG = value;
                else
                    _FaseCyclus.TFG = TGG;
                OnMonitoredPropertyChanged("TFG", _ControllerVM);
                _ControllerVM.SetAllSelectedElementsValue(this, "TFG");
            }
        }

        public int TGG
        {
            get { return _FaseCyclus.TGG; }
            set
            {
                if (value >= 0 && value >= TGG_min)
                {
                    _FaseCyclus.TGG = value;
                    if (TFG < value)
                        TFG = value;
                }
                else if (value >= 0)
                    _FaseCyclus.TGG = TGG_min;
                OnMonitoredPropertyChanged("TGG", _ControllerVM);
                _ControllerVM.SetAllSelectedElementsValue(this, "TGG");
            }
        }

        public int TGG_min
        {
            get { return _FaseCyclus.TGG_min; }
            set
            {
                if (value >= 0)
                {
                    _FaseCyclus.TGG_min = value;
                    if (TGG < value)
                        TGG = value;
                }
                OnMonitoredPropertyChanged("TGG_min", _ControllerVM);
                _ControllerVM.SetAllSelectedElementsValue(this, "TGG_min");
            }
        }

        public int TRG
        {
            get { return _FaseCyclus.TRG; }
            set
            {
                if (value >= 0 && value >= TRG_min)
                {
                    _FaseCyclus.TRG = value;
                }
                else if (value >= 0)
                    _FaseCyclus.TGG = TRG_min;
                OnMonitoredPropertyChanged("TRG", _ControllerVM);
                _ControllerVM.SetAllSelectedElementsValue(this, "TRG");
            }
        }

        public int TRG_min
        {
            get { return _FaseCyclus.TRG_min; }
            set
            {
                if (value >= 0)
                {
                    _FaseCyclus.TRG_min = value;
                    if (TRG < value)
                        TRG = value;
                }
                OnMonitoredPropertyChanged("TRG_min", _ControllerVM);
                _ControllerVM.SetAllSelectedElementsValue(this, "TRG_min");
            }
        }

        public int TGL
        {
            get { return _FaseCyclus.TGL; }
            set
            {
                if (value >= 0 && value >= TGL_min)
                {
                    _FaseCyclus.TGL = value;
                }
                else if (value >= 0)
                    _FaseCyclus.TGG = TGL_min;
                OnMonitoredPropertyChanged("TGL", _ControllerVM);
                _ControllerVM.SetAllSelectedElementsValue(this, "TGL");
            }
        }

        public int TGL_min
        {
            get { return _FaseCyclus.TGL_min; }
            set
            {
                if (value >= 0)
                {
                    _FaseCyclus.TGL_min = value;
                    if (TGL < value)
                        TGL = value;
                }
                OnMonitoredPropertyChanged("TGL_min", _ControllerVM);
                _ControllerVM.SetAllSelectedElementsValue(this, "TGL_min");
            }
        }

        public int Kopmax
        {
            get { return _FaseCyclus.Kopmax; }
            set
            {
                if (value >= 0)
                {
                    _FaseCyclus.Kopmax = value;
                }
                OnMonitoredPropertyChanged("Kopmax", _ControllerVM);
                _ControllerVM.SetAllSelectedElementsValue(this, "Kopmax");
            }
        }

        public NooitAltijdAanUitEnum VasteAanvraag
        {
            get { return _FaseCyclus.VasteAanvraag; }
            set
            {
                _FaseCyclus.VasteAanvraag = value;
                OnMonitoredPropertyChanged("VasteAanvraag", _ControllerVM);
                _ControllerVM.SetAllSelectedElementsValue(this, "VasteAanvraag");
            }
        }

        public NooitAltijdAanUitEnum Wachtgroen
        {
            get { return _FaseCyclus.Wachtgroen; }
            set
            {
                _FaseCyclus.Wachtgroen = value;
                OnMonitoredPropertyChanged("Wachtgroen", _ControllerVM);
                _ControllerVM.SetAllSelectedElementsValue(this, "Wachtgroen");
            }
        }

        public NooitAltijdAanUitEnum Meeverlengen
        {
            get { return _FaseCyclus.Meeverlengen; }
            set
            {
                _FaseCyclus.Meeverlengen = value;
                OnMonitoredPropertyChanged("Meeverlengen", _ControllerVM);
                _ControllerVM.SetAllSelectedElementsValue(this, "Meeverlengen");
            }
        }

        public ObservableCollection<DetectorViewModel> Detectoren
        {
            get
            {
                if(_Detectoren == null)
                {
                    _Detectoren = new ObservableCollection<DetectorViewModel>();
                }
                return _Detectoren;
            }
        }

        public ObservableCollection<ConflictViewModel> Conflicten
        {
            get
            {
                if (_Conflicten == null)
                {
                    _Conflicten = new ObservableCollection<ConflictViewModel>();
                }
                return _Conflicten;
            }
        }

        public int DetectorCount
        {
            get
            {
                return Detectoren.Count;
            }
        }

        public bool HasKopmax
        {
            get
            {
                foreach(DetectorViewModel dvm in Detectoren)
                {
                    if (dvm.Type == DetectorTypeEnum.Kop)
                        return true;
                }
                return false;
            }
        }

        #endregion // Properties

        #region Collection Changed

        private void _Detectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (DetectorViewModel detvm in e.NewItems)
                {
                    _FaseCyclus.Detectoren.Add(detvm.Detector);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (DetectorViewModel detvm in e.OldItems)
                {
                    _FaseCyclus.Detectoren.Remove(detvm.Detector);
                }
            }
            _ControllerVM.HasChanged = true;
            OnPropertyChanged("HasKopmax");
            OnPropertyChanged("DetectorCount");
        }

        private void _Conflicten_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (ConflictViewModel cvm in e.NewItems)
                {
                    _FaseCyclus.Conflicten.Add(cvm.Conflict);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (ConflictViewModel cvm in e.OldItems)
                {
                    _FaseCyclus.Conflicten.Remove(cvm.Conflict);
                }
            }
            _ControllerVM.HasChanged = true;
        }

        #endregion // Collection Changed

        #region Overrides

        public override string ToString()
        {
            return Naam;
        }

        #endregion // Overrides

        #region IComparable

        public int CompareTo(object obj)
        {
            FaseCyclusViewModel fcvm = obj as FaseCyclusViewModel;
            if (fcvm == null)
                throw new NotImplementedException();
            else
            {
                string myName = Naam;
                string hisName = fcvm.Naam;
                if (myName.Length < hisName.Length) myName = myName.PadLeft(hisName.Length, '0');
                else if (hisName.Length < myName.Length) hisName = hisName.PadLeft(myName.Length, '0');
                return myName.CompareTo(hisName);
            }
        }

        #endregion // IComparable

        #region Public methods

        /// <summary>
        /// Updates the Define member of the model based on member Name
        /// </summary>
        public void UpdateModelDefine()
        {
            _FaseCyclus.Define = SettingsProvider.GetFaseCyclusDefinePrefix() + _FaseCyclus.Naam;
        }

        public void UpdateHasKopmax()
        {
            OnPropertyChanged("HasKopmax");
        }

        #endregion // Public methods

        #region Constructor

        public FaseCyclusViewModel(ControllerViewModel controllervm, FaseCyclusModel fasecyclus)
        {
            _ControllerVM = controllervm;
            _FaseCyclus = fasecyclus;

            // Add data from the Model to the ViewModel structure
            foreach (DetectorModel dm in _FaseCyclus.Detectoren)
            {
                DetectorViewModel dvm = new DetectorViewModel(_ControllerVM, dm);
                dvm.FaseVM = this;
                Detectoren.Add(dvm);
            }
            foreach (ConflictModel cm in _FaseCyclus.Conflicten)
            {
                ConflictViewModel cvm = new ConflictViewModel(_ControllerVM, cm);
                Conflicten.Add(cvm);
            }

            Detectoren.CollectionChanged += _Detectoren_CollectionChanged;
            Conflicten.CollectionChanged += _Conflicten_CollectionChanged;
        }

        #endregion // Constructor
    }
}
