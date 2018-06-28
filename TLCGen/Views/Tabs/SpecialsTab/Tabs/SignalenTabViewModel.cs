using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 3, type: TabItemTypeEnum.SpecialsTab)]
    public class SignalenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private WaarschuwingsGroepViewModel _SelectedWaarschuwingsGroep;
        private RatelTikkerViewModel _SelectedRatelTikker;

        private ObservableCollection<string> _ControllerFasen;
        private ObservableCollection<string> _SelectableRatelTikkerFasen;
        private List<string> _ControllerDetectoren;
        private string _SelectedRatelTikkerFaseToAdd;

        private RelayCommand _AddWaarschuwingsGroepCommand;
        private RelayCommand _RemoveWaarschuwingsGroepCommand;
        private RelayCommand _AddRatelTikkerCommand;
        private RelayCommand _RemoveRatelTikkerCommand;

        #endregion // Fields

        #region Properties

        public WaarschuwingsGroepViewModel SelectedWaarschuwingsGroep
        {
            get => _SelectedWaarschuwingsGroep;
	        set
            {
                _SelectedWaarschuwingsGroep = value;
                RaisePropertyChanged();
            }
        }

        public RatelTikkerViewModel SelectedRatelTikker
        {
            get => _SelectedRatelTikker;
	        set
            {
                _SelectedRatelTikker = value;
                RaisePropertyChanged();
                UpdateSelectables();
            }
        }

        public ObservableCollectionAroundList<WaarschuwingsGroepViewModel, WaarschuwingsGroepModel> WaarschuwingsGroepen
        {
            get;
            private set;
        }

        public ObservableCollectionAroundList<RatelTikkerViewModel, RatelTikkerModel> RatelTikkers
        {
            get;
            private set;
        }

        public ObservableCollection<string> ControllerFasen => _ControllerFasen ?? (_ControllerFasen = new ObservableCollection<string>());

	    public ObservableCollection<string> SelectableRatelTikkerFasen => _SelectableRatelTikkerFasen ?? (_SelectableRatelTikkerFasen = new ObservableCollection<string>());

	    public string SelectedRatelTikkerFaseToAdd
        {
            get => _SelectedRatelTikkerFaseToAdd;
	        set
            {
                _SelectedRatelTikkerFaseToAdd = value;
                RaisePropertyChanged();
            }
        }

        public bool DimUitgangPerTikker
        {
            get => _Controller.Signalen.DimUitgangPerTikker;
            set
            {
                _Controller.Signalen.DimUitgangPerTikker = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddWaarschuwingsGroepCommand => _AddWaarschuwingsGroepCommand ?? (_AddWaarschuwingsGroepCommand =
	                                                        new RelayCommand(AddWaarschuwingsGroepCommand_Executed, AddWaarschuwingsGroepCommand_CanExecute));

	    public ICommand RemoveWaarschuwingsGroepCommand => _RemoveWaarschuwingsGroepCommand ?? (_RemoveWaarschuwingsGroepCommand =
		                                                       new RelayCommand(RemoveWaarschuwingsGroepCommand_Executed, RemoveWaarschuwingsGroepCommand_CanExecute));

	    public ICommand AddRatelTikkerCommand => _AddRatelTikkerCommand ?? (_AddRatelTikkerCommand =
		                                             new RelayCommand(AddRatelTikkerCommand_Executed, AddRatelTikkerCommand_CanExecute));

	    public ICommand RemoveRatelTikkerCommand => _RemoveRatelTikkerCommand ?? (_RemoveRatelTikkerCommand =
		                                                new RelayCommand(RemoveRatelTikkerCommand_Executed, RemoveRatelTikkerCommand_CanExecute));

	    #endregion // Commands

        #region Command functionality

        void AddWaarschuwingsGroepCommand_Executed(object prm)
        {
            var grm = new WaarschuwingsGroepModel();
            int i = WaarschuwingsGroepen.Count + 1;
            grm.Naam = "groep" + i;
            while (!Integrity.TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, grm.Naam))
            {
                ++i;
                grm.Naam = "groep" + i;
            }
            var grvm = new WaarschuwingsGroepViewModel(grm);
            WaarschuwingsGroepen.Add(grvm);
            SelectedWaarschuwingsGroep = grvm;
            Messenger.Default.Send(new ControllerDataChangedMessage());
            UpdateSelectables();

            if (_Controller.PeriodenData.Perioden.All(x => x.Type != PeriodeTypeEnum.BellenActief))
            {
                AddPeriodToModel(PeriodeTypeEnum.BellenActief, "bel");
            }
            if (_Controller.PeriodenData.Perioden.All(x => x.Type != PeriodeTypeEnum.BellenDimmen))
            {
                AddPeriodToModel(PeriodeTypeEnum.BellenDimmen, "beldim");
            }
        }

        bool AddWaarschuwingsGroepCommand_CanExecute(object prm)
        {
            return true;
        }

        void RemoveWaarschuwingsGroepCommand_Executed(object prm)
        {
            int id = WaarschuwingsGroepen.IndexOf(SelectedWaarschuwingsGroep);
            WaarschuwingsGroepen.Remove(SelectedWaarschuwingsGroep);
            SelectedWaarschuwingsGroep = null;
            if(WaarschuwingsGroepen.Count > 0)
            {
                id = id < 0 ? 0 : id;
                id = id >= WaarschuwingsGroepen.Count ? WaarschuwingsGroepen.Count - 1 : id;
                SelectedWaarschuwingsGroep = WaarschuwingsGroepen[id];
            }
            Messenger.Default.Send(new ControllerDataChangedMessage());
            UpdateSelectables();
        }

        bool RemoveWaarschuwingsGroepCommand_CanExecute(object prm)
        {
            return SelectedWaarschuwingsGroep != null;
        }

        void AddRatelTikkerCommand_Executed(object prm)
        {
            int id = SelectableRatelTikkerFasen.IndexOf(SelectedRatelTikkerFaseToAdd);
            var rtm = new RatelTikkerModel()
            {
                FaseCyclus = SelectedRatelTikkerFaseToAdd
            };
            foreach (var fc in _Controller.Fasen)
            {
                if(fc.Naam == SelectedRatelTikkerFaseToAdd)
                {
                    foreach(var d in fc.Detectoren)
                    {
                        if(d.Type == Models.Enumerations.DetectorTypeEnum.Knop ||
                           d.Type == Models.Enumerations.DetectorTypeEnum.KnopBinnen ||
                           d.Type == Models.Enumerations.DetectorTypeEnum.KnopBuiten)
                        rtm.Detectoren.Add(new RatelTikkerDetectorModel { Detector = d.Naam });
                    }
                }
            }
            var rtvm = new RatelTikkerViewModel(rtm);
            RatelTikkers.Add(rtvm);
            SelectedRatelTikker = rtvm;
            Messenger.Default.Send(new ControllerDataChangedMessage());
            UpdateSelectables();
            if (SelectableRatelTikkerFasen.Count > 0)
            {
                id = id < 0 ? 0 : id;
                id = id >= SelectableRatelTikkerFasen.Count ? SelectableRatelTikkerFasen.Count - 1 : id;
                SelectedRatelTikkerFaseToAdd = SelectableRatelTikkerFasen[id];
            }

	        if (_Controller.PeriodenData.Perioden.All(x => x.Type != PeriodeTypeEnum.RateltikkersAanvraag))
	        {
		       AddPeriodToModel(PeriodeTypeEnum.RateltikkersAanvraag, "rtaanvr");
			}
	        if (_Controller.PeriodenData.Perioden.All(x => x.Type != PeriodeTypeEnum.RateltikkersAltijd))
	        {
		        AddPeriodToModel(PeriodeTypeEnum.RateltikkersAltijd, "rtaltijd");
	        }
	        if (_Controller.PeriodenData.Perioden.All(x => x.Type != PeriodeTypeEnum.RateltikkersDimmen))
	        {
		        AddPeriodToModel(PeriodeTypeEnum.RateltikkersDimmen, "rtdimmen");
	        }
		}

        bool AddRatelTikkerCommand_CanExecute(object prm)
        {
            return true;
        }

        void RemoveRatelTikkerCommand_Executed(object prm)
        {
            int id = RatelTikkers.IndexOf(SelectedRatelTikker);
            int id2 = SelectableRatelTikkerFasen.IndexOf(SelectedRatelTikkerFaseToAdd);
            RatelTikkers.Remove(SelectedRatelTikker);
            UpdateSelectables();
            SelectedRatelTikker = null;
            Messenger.Default.Send(new ControllerDataChangedMessage());
            if (RatelTikkers.Count > 0)
            {
                id = id < 0 ? 0 : id;
                id = id >= RatelTikkers.Count ? RatelTikkers.Count - 1 : id;
                SelectedRatelTikker = RatelTikkers[id];
            }
            if (SelectableRatelTikkerFasen.Count > 0)
            {
                id2 = id2 < 0 ? 0 : id2;
                id2 = id2 >= SelectableRatelTikkerFasen.Count ? SelectableRatelTikkerFasen.Count - 1 : id2;
                SelectedRatelTikkerFaseToAdd = SelectableRatelTikkerFasen[id2];
            }
        }

        bool RemoveRatelTikkerCommand_CanExecute(object prm)
        {
            return SelectedRatelTikker != null;
        }

        #endregion // Command functionality

        #region Private methods

	    private void AddPeriodToModel(PeriodeTypeEnum type, string name)
	    {
			var p = new PeriodeModel { Type = type };
		    DefaultsProvider.Default.SetDefaultsOnModel(p, p.Type.ToString(), null, false);
            var newname = name;
		    var i = 0;
            while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Detector, newname))
            {
			    newname = name + (i++);
		    }
		    p.Naam = newname;
		    _Controller.PeriodenData.Perioden.Add(p);
		}

        private void UpdateSelectables()
        {
            ControllerFasen.Clear();
            foreach (var fcm in _Controller.Fasen)
            {
                ControllerFasen.Add(fcm.Naam);
            }

            _ControllerDetectoren = new List<string>();
            foreach (var fcm in _Controller.Fasen)
            {
                foreach (var dm in fcm.Detectoren)
                {
                    _ControllerDetectoren.Add(dm.Naam);
                }
            }
            foreach (var dm in _Controller.Detectoren)
            {
                _ControllerDetectoren.Add(dm.Naam);
            }

            string tempfc = SelectedRatelTikkerFaseToAdd;
            SelectableRatelTikkerFasen.Clear();
            if (ControllerFasen.Count > 0 && RatelTikkers.Count > 0)
            {
                foreach (var fc in ControllerFasen)
                {
                    if (RatelTikkers.All(x => x.FaseCyclus != fc))
                    {
                        SelectableRatelTikkerFasen.Add(fc);
                    }
                }
            }
            else
            {
                foreach (var fc in ControllerFasen)
                {
                    SelectableRatelTikkerFasen.Add(fc);
                }
            }
            if (_SelectableRatelTikkerFasen.Count > 0)
            {
                SelectedRatelTikkerFaseToAdd = tempfc ?? SelectableRatelTikkerFasen[0];
            }
            else
            {
                SelectedRatelTikkerFaseToAdd = null;
            }
        }

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen TabItem overrides

        public override string DisplayName => "Signalen";

	    public override void OnSelected()
        {
            UpdateSelectables();
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

	        set
            {
                base.Controller = value;
                if (base.Controller != null)
                {
                    WaarschuwingsGroepen = new ObservableCollectionAroundList<WaarschuwingsGroepViewModel, WaarschuwingsGroepModel>(_Controller.Signalen.WaarschuwingsGroepen);
                    RatelTikkers = new ObservableCollectionAroundList<RatelTikkerViewModel, RatelTikkerModel>(_Controller.Signalen.Rateltikkers);
                }
                else
                {
                    WaarschuwingsGroepen = null;
                    RatelTikkers = null;
                }
                RaisePropertyChanged(nameof(WaarschuwingsGroepen));
                RaisePropertyChanged(nameof(RatelTikkers));
            }
        }

        #endregion // TLCGen TabItem overrides

        #region TLCGen Events

        private void OnFasenChanged(FasenChangedMessage message)
        {
            UpdateSelectables();
            WaarschuwingsGroepen.Rebuild();
            RatelTikkers.Rebuild();
        }

        private void OnDetectorenChanged(DetectorenChangedMessage message)
        {
            UpdateSelectables();
            WaarschuwingsGroepen.Rebuild();
            RatelTikkers.Rebuild();
        }

        #endregion // TLCGen Events

        #region Constructor

        public SignalenTabViewModel()
        {
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<DetectorenChangedMessage>(OnDetectorenChanged));
        }

        #endregion // Constructor
    }
}
