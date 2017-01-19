using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private ObservableCollection<string> _ControllerFasen;
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
                if (_SelectedVAOntruimenFase != null)
                {
                    try
                    {
                        _SelectedVAOntruimenFase.FaseDetectoren.Clear();
                        var selfc = Controller.Fasen.Where(x => x.Naam == SelectedFaseNaam).First();
                        foreach (DetectorModel dm in selfc.Detectoren)
                        {
                            _SelectedVAOntruimenFase.FaseDetectoren.Add(dm.Naam);
                        }

                        _SelectedVAOntruimenFase.ConflicterendeFasen.Clear();
                        foreach (ConflictModel cm in _Controller.InterSignaalGroep.Conflicten)
                        {
                            if (cm.FaseVan == SelectedFaseNaam && cm.Waarde >= 0)
                            {
                                _SelectedVAOntruimenFase.ConflicterendeFasen.Add(cm.FaseNaar, cm.Waarde);
                            }
                        }

                        _SelectedVAOntruimenFase.Refresh();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error in VA ontruimen: " + e.ToString());
                    }
                }
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
                        
                        VAOntruimenFaseViewModel vavm = new VAOntruimenFaseViewModel(vam);

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

        public ObservableCollection<string> ControllerFasen
        {
            get
            {
                if (_ControllerFasen == null)
                {
                    _ControllerFasen = new ObservableCollection<string>();
                }
                return _ControllerFasen;
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
            ControllerFasen.Clear();
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
