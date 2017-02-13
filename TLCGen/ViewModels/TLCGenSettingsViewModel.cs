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

        private FaseCyclusModel _SelectedFaseCyclus;
        private FaseTypeEnum _SelectedFaseCyclusType;
        private DetectorModel _SelectedDetector;
        private DetectorTypeEnum _SelectedDetectorType;

        #endregion // Fields

        #region Properties

        public FaseCyclusModel SelectedFaseCyclus
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
                if (Fasen.Where(x => x.Type == value).Any())
                {
                    SelectedFaseCyclus = Fasen.Where(x => x.Type == value).First();
                }
                OnPropertyChanged("SelectedFaseCyclusType");
            }
        }

        public DetectorModel SelectedDetector
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
                if (Detectoren.Where(x => x.Type == value).Any())
                {
                    SelectedDetector = Detectoren.Where(x => x.Type == value).First();
                }
                OnPropertyChanged("SelectedDetectorType");
            }
        }

        public List<FaseCyclusModel> Fasen { get; private set; }
        public List<DetectorModel> Detectoren { get; private set; }

        #endregion // Properties

        #region Constructor

        public TLCGenSettingsViewModel()
        {
            Fasen = new List<FaseCyclusModel>();
            foreach(var fc in DefaultsProvider.Default.Defaults.Fasen)
            {
                Fasen.Add(fc);
            }
            Detectoren = new List<DetectorModel>();
            foreach (var d in DefaultsProvider.Default.Defaults.Detectoren)
            {
                Detectoren.Add(d);
            }
        }

        #endregion // Constructor
    }
}
