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
        private Dictionary<string, string> _ControllerFasen;
        private VAOntruimenFaseViewModel _SelectedVAOntruimenFase;
        private bool _SelectedFaseHasVAOntruimen;

        private RelayCommand _AddVAOntruimenFaseCommand;
        private RelayCommand _RemoveVAOntruimenFaseCommand;

        #endregion // Fields

        #region Properties

        public string SelectedFaseNaam
        {
            get { return _SelectedFaseNaam; }
            set
            {
                _SelectedFaseNaam = value;
                string def;
                if(ControllerFasen.TryGetValue(value, out def))
                {
                    if(VAOntruimenFasen.Where(x => x.FaseCyclus == def).Any())
                    {
                        SelectedVAOntruimenFase = VAOntruimenFasen.Where(x => x.FaseCyclus == def).First();
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
                    string def;
                    if (ControllerFasen.TryGetValue(SelectedFaseNaam, out def))
                    {
                        VAOntruimenFaseModel vam = new VAOntruimenFaseModel();
                        vam.FaseCyclus = def;
                        foreach (ConflictModel cm in _Controller.InterSignaalGroep.Conflicten)
                        {
                            if (cm.FaseVan == def && cm.Waarde >= 0)
                            {
                                vam.ConflicterendeFasen.Add(new VAOntruimenNaarFaseModel() { FaseCyclus = cm.FaseNaar, VAOntruimingsTijd = cm.Waarde });
                            }
                        }
                        VAOntruimenFaseViewModel vavm = new VAOntruimenFaseViewModel(vam);
                        foreach (ConflictModel cm in _Controller.InterSignaalGroep.Conflicten)
                        {
                            if (cm.FaseVan == def && cm.Waarde >= 0)
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

        public Dictionary<string, string> ControllerFasen
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

        public ICommand AddVAOntruimenFaseCommand
        {
            get
            {
                if (_AddVAOntruimenFaseCommand == null)
                {
                    _AddVAOntruimenFaseCommand = new RelayCommand(AddNewVAOntruimenFaseCommand_Executed, AddNewVAOntruimenFaseCommand_CanExecute);
                }
                return _AddVAOntruimenFaseCommand;
            }
        }

        public ICommand RemoveVAOntruimenFaseCommand
        {
            get
            {
                if (_RemoveVAOntruimenFaseCommand == null)
                {
                    _RemoveVAOntruimenFaseCommand = new RelayCommand(RemoveVAOntruimenFaseCommand_Executed, RemoveVAOntruimenFaseCommand_CanExecute);
                }
                return _RemoveVAOntruimenFaseCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewVAOntruimenFaseCommand_Executed(object prm)
        {
            
        }

        bool AddNewVAOntruimenFaseCommand_CanExecute(object prm)
        {
            return true;
        }

        void RemoveVAOntruimenFaseCommand_Executed(object prm)
        {
            VAOntruimenFasen.Remove(SelectedVAOntruimenFase);
            SelectedVAOntruimenFase = null;
        }

        bool RemoveVAOntruimenFaseCommand_CanExecute(object prm)
        {
            return SelectedVAOntruimenFase != null;
        }

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
            ControllerFasen = new Dictionary<string, string>();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                ControllerFasen.Add(fcm.Naam, fcm.Define);
            }
            string notused;
            if(!string.IsNullOrEmpty(temp) && ControllerFasen.TryGetValue(temp, out notused))
            {
                SelectedFaseNaam = temp;
            }
            else if(ControllerFasen.Count > 0)
            {
                SelectedFaseNaam = ControllerFasen.First().Key;
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
