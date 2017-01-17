using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 2, type: TabItemTypeEnum.SpecialsTab)]
    public class VAOntruimenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private string _SelectedFaseNaam;
        private List<string> _ControllerFasen;
        private VAOntruimenFaseViewModel _SelectedVAOntruimenFase;

        #endregion // Fields

        #region Properties

        public string SelectedFaseNaam
        {
            get { return _SelectedFaseNaam; }
            set
            {
                _SelectedFaseNaam = value;
                if(ControllerFasen.Contains(value))
                {
                    if(VAOntruimenFasen.Where(x => x.FaseCyclus == value).Any())
                    {
                        SelectedVAOntruimenFase = VAOntruimenFasen.Where(x => x.FaseCyclus == value).First();
                    }
                    else
                    {
                        SelectedVAOntruimenFase = null;
                    }
                }
                else
                {
                    SelectedVAOntruimenFase = null;
                }
                OnMonitoredPropertyChanged("SelectedVAOntruimenFase");
                OnMonitoredPropertyChanged("SelectedFaseHasVAOntruimen");
                OnMonitoredPropertyChanged("SelectedFaseNaam");
            }
        }

        public VAOntruimenFaseViewModel SelectedVAOntruimenFase
        {
            get { return _SelectedVAOntruimenFase; }
            set
            {
                _SelectedVAOntruimenFase = value;
                OnMonitoredPropertyChanged("SelectedVAOntruimenFase");
            }
        }

        public bool SelectedFaseHasVAOntruimen
        {
            set
            {
                if(value)
                {
                    if (ControllerFasen.Contains(SelectedFaseNaam))
                    {
                        VAOntruimenFaseModel vam = new VAOntruimenFaseModel();
                        vam.FaseCyclus = SelectedFaseNaam;
                        foreach (ConflictModel cm in _Controller.InterSignaalGroep.Conflicten)
                        {
                            if (cm.FaseVan == SelectedFaseNaam && cm.Waarde >= 0)
                            {
                                vam.ConflicterendeFasen.Add(new VAOntruimenNaarFaseModel() { FaseCyclus = cm.FaseNaar, VAOntruimingsTijd = cm.Waarde });
                            }
                        }
                        VAOntruimenFaseViewModel vavm = new VAOntruimenFaseViewModel(vam);
                        foreach (ConflictModel cm in _Controller.InterSignaalGroep.Conflicten)
                        {
                            if (cm.FaseVan == SelectedFaseNaam && cm.Waarde >= 0)
                            {
                                var conf = vavm.ConflicterendeFasen.Where(x => x.FaseCyclus == cm.FaseNaar).First();
                                conf.OntruimingsTijd = cm.Waarde;
                            }
                        }
                        SelectedVAOntruimenFase = vavm;
                        VAOntruimenFasen.Add(vavm);
                    }
                }
                else
                {
                    VAOntruimenFasen.Remove(SelectedVAOntruimenFase);
                    SelectedVAOntruimenFase = null;
                }
                OnMonitoredPropertyChanged("SelectedFaseHasVAOntruimen");
            }
            get
            {
                return SelectedVAOntruimenFase != null;
            }
        }

        public ObservableCollectionAroundList<VAOntruimenFaseViewModel, VAOntruimenFaseModel> VAOntruimenFasen
        {
            get;
            private set;
        }

        public List<string> ControllerFasen
        {
            get { return _ControllerFasen; }
            set
            {
                _ControllerFasen = value;
                OnMonitoredPropertyChanged("ControllerFasen");
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen TabItem overrides

        public override string DisplayName
        {
            get { return "VA ontruimen"; }
        }

        public override void OnSelected()
        {
            string temp = SelectedFaseNaam;
            ControllerFasen = new List<string>();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                ControllerFasen.Add(fcm.Naam);
            }
            if(!string.IsNullOrEmpty(temp) && ControllerFasen.Contains(temp))
            {
                SelectedFaseNaam = temp;
            }
            else if(ControllerFasen.Count > 0)
            {
                SelectedFaseNaam = ControllerFasen.First();
            }
        }

        #endregion // TLCGen TabItem overrides

        #region Constructor

        public VAOntruimenTabViewModel(ControllerModel controller) : base(controller)
        {
            VAOntruimenFasen = new ObservableCollectionAroundList<VAOntruimenFaseViewModel, VAOntruimenFaseModel>(_Controller.VAOntruimenFasen);
        }

        #endregion // Constructor
    }
}
