﻿
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
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
                OnPropertyChanged();
                _RemoveWaarschuwingsGroepCommand?.NotifyCanExecuteChanged();
            }
        }

        public RatelTikkerViewModel SelectedRatelTikker
        {
            get => _SelectedRatelTikker;
	        set
            {
                _SelectedRatelTikker = value;
                OnPropertyChanged();
                UpdateSelectables();
                _RemoveRatelTikkerCommand?.NotifyCanExecuteChanged();
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

        public ObservableCollection<RatelTikkerViewModel> RatelTikkersBewaakt { get; } = new ObservableCollection<RatelTikkerViewModel>();

        public ObservableCollection<string> ControllerFasen => _ControllerFasen ?? (_ControllerFasen = new ObservableCollection<string>());

	    public ObservableCollection<string> SelectableRatelTikkerFasen => _SelectableRatelTikkerFasen ?? (_SelectableRatelTikkerFasen = new ObservableCollection<string>());

	    public string SelectedRatelTikkerFaseToAdd
        {
            get => _SelectedRatelTikkerFaseToAdd;
	        set
            {
                _SelectedRatelTikkerFaseToAdd = value;
                OnPropertyChanged();
            }
        }

        public bool DimUitgangPerTikker
        {
            get => _Controller.Signalen.DimUitgangPerTikker;
            set
            {
                _Controller.Signalen.DimUitgangPerTikker = value;
                foreach (var rt in _Controller.Signalen.Rateltikkers)
                {
                    rt.DimmenPerUitgang = value;
                    if (rt.DimUitgangBitmapData == null) rt.DimUitgangBitmapData = new BitmapCoordinatenDataModel();
                }
                if (!value) DimmingNiveauVanuitApplicatie = false;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool DimmingNiveauVanuitApplicatie
        {
            get => _Controller.Signalen.DimmingNiveauVanuitApplicatie;
            set
            {
                _Controller.Signalen.DimmingNiveauVanuitApplicatie = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddWaarschuwingsGroepCommand => _AddWaarschuwingsGroepCommand ??= new RelayCommand(() =>
        {
            var grm = new WaarschuwingsGroepModel();
            var i = WaarschuwingsGroepen.Count + 1;
            grm.Naam = "groep" + i;
            while (!Integrity.TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, grm.Naam, TLCGenObjectTypeEnum.WaarschuwingsGroep))
            {
                ++i;
                grm.Naam = "groep" + i;
            }
            var grvm = new WaarschuwingsGroepViewModel(grm);
            WaarschuwingsGroepen.Add(grvm);
            SelectedWaarschuwingsGroep = grvm;
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
            UpdateSelectables();

            if (_Controller.PeriodenData.Perioden.All(x => x.Type != PeriodeTypeEnum.BellenActief))
            {
                AddPeriodToModel(PeriodeTypeEnum.BellenActief, "bel");
            }
            if (_Controller.PeriodenData.Perioden.All(x => x.Type != PeriodeTypeEnum.BellenDimmen))
            {
                AddPeriodToModel(PeriodeTypeEnum.BellenDimmen, "beldim");
            }
            
            WeakReferenceMessengerEx.Default.Send(new ModelManagerMessageBase());
        });

	    public ICommand RemoveWaarschuwingsGroepCommand => _RemoveWaarschuwingsGroepCommand ??= new RelayCommand(() =>
        {
            var id = WaarschuwingsGroepen.IndexOf(SelectedWaarschuwingsGroep);
            WaarschuwingsGroepen.Remove(SelectedWaarschuwingsGroep);
            SelectedWaarschuwingsGroep = null;
            if(WaarschuwingsGroepen.Count > 0)
            {
                id = id < 0 ? 0 : id;
                id = id >= WaarschuwingsGroepen.Count ? WaarschuwingsGroepen.Count - 1 : id;
                SelectedWaarschuwingsGroep = WaarschuwingsGroepen[id];
            }
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
            WeakReferenceMessengerEx.Default.Send(new ModelManagerMessageBase());
            UpdateSelectables();
        }, () => SelectedWaarschuwingsGroep != null);

	    public ICommand AddRatelTikkerCommand => _AddRatelTikkerCommand ??= new RelayCommand(() =>
        {
            var id = SelectableRatelTikkerFasen.IndexOf(SelectedRatelTikkerFaseToAdd);
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
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
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
            
            RatelTikkers.BubbleSort();

            WeakReferenceMessengerEx.Default.Send(new ModelManagerMessageBase());
        });

	    public ICommand RemoveRatelTikkerCommand => _RemoveRatelTikkerCommand ??= new RelayCommand(() =>
        {
            var id = RatelTikkers.IndexOf(SelectedRatelTikker);
            var id2 = SelectableRatelTikkerFasen.IndexOf(SelectedRatelTikkerFaseToAdd);
            RatelTikkers.Remove(SelectedRatelTikker);
            if (RatelTikkersBewaakt.Contains(SelectedRatelTikker))
            {
                RatelTikkersBewaakt.Remove(SelectedRatelTikker);
            }
            UpdateSelectables();
            SelectedRatelTikker = null;
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
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
            WeakReferenceMessengerEx.Default.Send(new ModelManagerMessageBase());
        }, () => SelectedRatelTikker != null);

	    #endregion // Commands

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

            var tempfc = SelectedRatelTikkerFaseToAdd;
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

        private void UpdateBewaakteRatelTikkers()
        {
            RatelTikkersBewaakt.Clear();
            foreach(var rt in RatelTikkers.Where(x => x.Type == RateltikkerTypeEnum.HoeflakeBewaakt))
            {
                RatelTikkersBewaakt.Add(rt);
            }
        }

        private void SortTikkers()
        {
            if (!RatelTikkers.IsSorted())
            {
                RatelTikkers.BubbleSort();
            }
            if (!RatelTikkersBewaakt.IsSorted())
            {
                RatelTikkersBewaakt.BubbleSort();
            }
        }

        private void RebuildTikkers()
        {
            RatelTikkers.Rebuild();
            UpdateBewaakteRatelTikkers();
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
                    UpdateBewaakteRatelTikkers();
                    SortTikkers();
                }
                else
                {
                    WaarschuwingsGroepen = null;
                    RatelTikkers = null;
                }
                OnPropertyChanged(nameof(WaarschuwingsGroepen));
                OnPropertyChanged(nameof(RatelTikkers));
            }
        }

        #endregion // TLCGen TabItem overrides

        #region TLCGen Events

        private void OnFasenChanged(object sender, FasenChangedMessage message)
        {
            UpdateSelectables();
            WaarschuwingsGroepen.Rebuild();
            RebuildTikkers();
            SortTikkers();
        }

        private void OnDetectorenChanged(object sender, DetectorenChangedMessage message)
        {
            UpdateSelectables();
            WaarschuwingsGroepen.Rebuild();
            RebuildTikkers();
        }

        private void OnRatelTikkerTypeChanged(object sender, RatelTikkerTypeChangedMessage obj)
        {
            UpdateBewaakteRatelTikkers();
        }

        private void OnNameChanged(object sender, NameChangedMessage obj)
        {
            SortTikkers();
        }

        #endregion // TLCGen Events

        #region Constructor

        public SignalenTabViewModel()
        {
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessengerEx.Default.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
            WeakReferenceMessengerEx.Default.Register<RatelTikkerTypeChangedMessage>(this, OnRatelTikkerTypeChanged);
        }

        #endregion // Constructor
    }
}
