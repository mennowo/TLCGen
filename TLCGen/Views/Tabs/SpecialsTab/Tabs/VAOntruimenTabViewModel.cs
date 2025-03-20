
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;


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
            get => _SelectedFaseNaam;
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
                OnPropertyChanged("SelectedVAOntruimenFase");
                OnPropertyChanged("SelectedFaseHasVAOntruimen");
                OnPropertyChanged("SelectedFaseNaam");
            }
        }

        public VAOntruimenFaseViewModel SelectedVAOntruimenFase
        {
            get => _SelectedVAOntruimenFase;
            set
            {
                _SelectedVAOntruimenFase = value;
                if (_SelectedVAOntruimenFase != null)
                {
                    try
                    {

                        //_SelectedVAOntruimenFase.FaseDetectoren.Clear();
                        //var selfc = Controller.Fasen.Where(x => x.Naam == SelectedFaseNaam).First();
                        //foreach (DetectorModel dm in selfc.Detectoren)
                        //{
                        //    _SelectedVAOntruimenFase.FaseDetectoren.Add(dm.Naam);
                        //}

                        _SelectedVAOntruimenFase.ConflicterendeFasen.Clear();
                        foreach (var cm in _Controller.InterSignaalGroep.Conflicten)
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
                OnPropertyChanged("SelectedVAOntruimenFase");
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
                        var vam = new VAOntruimenFaseModel();
                        vam.FaseCyclus = SelectedFaseNaam;
                        
                        var vavm = new VAOntruimenFaseViewModel(vam);

                        SelectedVAOntruimenFase = vavm;
                        VAOntruimenFasen.Add(vavm);
                    }
                }
                else
                {
                    VAOntruimenFasen.Remove(SelectedVAOntruimenFase);
                    SelectedVAOntruimenFase = null;
                }
                OnPropertyChanged();
            }
            get => SelectedVAOntruimenFase != null;
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

        private void UpdateFasen()
        {
            var temp = SelectedFaseNaam;
            ControllerFasen.Clear();
            foreach (var fcm in _Controller.Fasen)
            {
                ControllerFasen.Add(fcm.Naam);
            }
            if (!string.IsNullOrEmpty(temp) && ControllerFasen.Contains(temp))
            {
                SelectedFaseNaam = temp;
            }
            else if (ControllerFasen.Count > 0)
            {
                SelectedFaseNaam = ControllerFasen.First();
            }
        }

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen TabItem overrides

        public override string DisplayName => "VA ontruimen";

        public override void OnSelected()
        {
            UpdateFasen();
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                if (base.Controller != null)
                {
                    VAOntruimenFasen = new ObservableCollectionAroundList<VAOntruimenFaseViewModel, VAOntruimenFaseModel>(_Controller.VAOntruimenFasen);
                }
                else
                {
                    VAOntruimenFasen = null;
                }
                OnPropertyChanged("VAOntruimenFasen");
            }
        }

        #endregion // TLCGen TabItem overrides

        #region TLCGen Events

        private void OnFasenChanged(object sender, FasenChangedMessage message)
        {
            UpdateFasen();
            VAOntruimenFasen.Rebuild();
        }

        private void OnDetectorenChanged(object sender, DetectorenChangedMessage message)
        {
            VAOntruimenFasen.Rebuild();
        }

        private void OnConflictsChanged(object sender, ConflictsChangedMessage message)
        {
            VAOntruimenFasen.Rebuild();
        }

        #endregion // TLCGen Events

        #region Constructor

        public VAOntruimenTabViewModel() : base()
        {
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessengerEx.Default.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            WeakReferenceMessengerEx.Default.Register<ConflictsChangedMessage>(this, OnConflictsChanged);
        }

        #endregion // Constructor
    }
}
