using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Settings
{
    public class FasenDefaultsTabViewModel : ViewModelBase
    {
        #region Fields

        private object _SelectedFaseCyclus;
        private FaseTypeEnum _SelectedFaseCyclusType;
        private string _SelectedFaseCyclusFunctionality;

        private List<string> _FaseCyclusFunctionalities;

        #endregion // Fields

        #region Properties

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
                RaisePropertyChanged("SelectedFaseCyclusFunctionality");
            }
        }

        public object SelectedFaseCyclus
        {
            get { return _SelectedFaseCyclus; }
            set
            {
                _SelectedFaseCyclus = value;
                RaisePropertyChanged("SelectedFaseCyclus");
            }
        }

        public FaseTypeEnum SelectedFaseCyclusType
        {
            get { return _SelectedFaseCyclusType; }
            set
            {
                _SelectedFaseCyclusType = value;
                SetSelectedFaseDefaultsObject(value);
                RaisePropertyChanged("SelectedFaseCyclusType");
            }
        }

        public List<FaseCyclusDefaultsModel> Fasen { get; private set; }

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

        #endregion // Private Methods

        #region Constructor

        public FasenDefaultsTabViewModel()
        {
            Fasen = new List<FaseCyclusDefaultsModel>();
            foreach (var fc in DefaultsProvider.Default.Defaults.Fasen)
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


        }

        #endregion // Constructor
    }
}
