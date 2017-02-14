using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class TLCGenSettingsViewModel : ViewModelBase
    {
        #region Fields

        private object _SelectedFaseCyclus;
        private FaseTypeEnum _SelectedFaseCyclusType;
        private string _SelectedFaseCyclusFunctionality;

        private object _SelectedDetector;
        private DetectorTypeEnum _SelectedDetectorType;
        private string _SelectedDetectorFunctionality;

        private List<string> _FaseCyclusFunctionalities;
        private List<string> _DetectorFunctionalities;

        #endregion // Fields

        #region Properties

        #region Fasen

        public List<string> FaseCyclusFunctionalities
        {
            get { return _FaseCyclusFunctionalities; }
        }

        public string SelectedFaseCyclusFunctionality
        {
            get { return _SelectedFaseCyclusFunctionality; }
            set
            {
                _SelectedFaseCyclusFunctionality = value;
                SetSelectedFaseDefaultsObject(SelectedFaseCyclusType);
                OnPropertyChanged("SelectedFaseCyclusFunctionality");
            }
        }

        public object SelectedFaseCyclus
        {
            get { return _SelectedFaseCyclus; }
            set
            {
                _SelectedFaseCyclus = value;
                OnPropertyChanged("SelectedFaseCyclus");
            }
        }

        public FaseTypeEnum SelectedFaseCyclusType
        {
            get { return _SelectedFaseCyclusType; }
            set
            {
                _SelectedFaseCyclusType = value;
                SetSelectedFaseDefaultsObject(value);
                OnPropertyChanged("SelectedFaseCyclusType");
            }
        }

        public List<FaseCyclusDefaultsModel> Fasen { get; private set; }

        #endregion // Fasen

        #region Detectoren

        public List<string> DetectorFunctionalities
        {
            get { return _DetectorFunctionalities; }
        }

        public string SelectedDetectorFunctionality
        {
            get { return _SelectedDetectorFunctionality; }
            set
            {
                _SelectedDetectorFunctionality = value;
                SetSelectedDetectorDefaultsObject(SelectedDetectorType);
                OnPropertyChanged("SelectedDetectorFunctionality");
            }
        }

        public object SelectedDetector
        {
            get { return _SelectedDetector; }
            set
            {
                _SelectedDetector = value;
                OnPropertyChanged("SelectedDetector");
            }
        }

        public DetectorTypeEnum SelectedDetectorType
        {
            get { return _SelectedDetectorType; }
            set
            {
                _SelectedDetectorType = value;
                SetSelectedDetectorDefaultsObject(SelectedDetectorType);
                OnPropertyChanged("SelectedDetectorType");
            }
        }

        public List<DetectorDefaultsModel> Detectoren { get; private set; }

        #endregion // Detectoren

        #endregion // Properties

        #region Private Methods

        private void SetSelectedFaseDefaultsObject(FaseTypeEnum type)
        {
            if (!string.IsNullOrEmpty(SelectedFaseCyclusFunctionality) && Fasen.Where(x => x.Type == type).Any())
            {
                switch (SelectedFaseCyclusFunctionality)
                {
                    case "Basis":
                        SelectedFaseCyclus = Fasen.Where(x => x.Type == type).First().FaseCyclus;
                        break;
                    case "Module molen":
                        SelectedFaseCyclus = Fasen.Where(x => x.Type == type).First().FaseCyclusModuleData;
                        break;
                    case "Groentijd":
                        SelectedFaseCyclus = Fasen.Where(x => x.Type == type).First().Groentijd;
                        break;
                    case "RoBuGrover":
                        SelectedFaseCyclus = Fasen.Where(x => x.Type == type).First().RoBuGroverFaseCyclusInstellingen;
                        break;
                }
            }
            else
            {
                SelectedFaseCyclus = null;
            }
        }

        private void SetSelectedDetectorDefaultsObject(DetectorTypeEnum type)
        {
            if (!string.IsNullOrEmpty(SelectedDetectorFunctionality) && Detectoren.Where(x => x.Type == type).Any())
            {
                switch (SelectedDetectorFunctionality)
                {
                    case "Basis":
                        SelectedDetector = Detectoren.Where(x => x.Type == type).First().Detector;
                        break;
                    case "RoBuGrover":
                        SelectedDetector = Detectoren.Where(x => x.Type == type).First().RoBuGroverFileDetector;
                        if(SelectedDetector == null)
                        {
                            SelectedDetector = Detectoren.Where(x => x.Type == type).First().RoBuGroverHiaatDetector;
                        }
                        break;
                }
            }
            else
            {
                SelectedDetector = null;
            }
        }

        #endregion // Private Methods

        #region Constructor

        public TLCGenSettingsViewModel()
        {
            Fasen = new List<FaseCyclusDefaultsModel>();
            foreach(var fc in DefaultsProvider.Default.Defaults.Fasen)
            {
                Fasen.Add(fc);
            }
            _FaseCyclusFunctionalities = new List<string>();
            _FaseCyclusFunctionalities.Add("Basis");
            _FaseCyclusFunctionalities.Add("Module molen");
            _FaseCyclusFunctionalities.Add("Groentijd");
            _FaseCyclusFunctionalities.Add("RoBuGrover");
            _SelectedFaseCyclusFunctionality = "Basis";
            SelectedFaseCyclusType = FaseTypeEnum.Auto;

            _DetectorFunctionalities = new List<string>();
            _DetectorFunctionalities.Add("Basis");
            _DetectorFunctionalities.Add("RoBuGrover");
            _SelectedDetectorFunctionality = "Basis";
            Detectoren = new List<DetectorDefaultsModel>();
            foreach (var d in DefaultsProvider.Default.Defaults.Detectoren)
            {
                Detectoren.Add(d);
            }
            SelectedDetectorType = DetectorTypeEnum.Kop;
        }

        #endregion // Constructor
    }
}
