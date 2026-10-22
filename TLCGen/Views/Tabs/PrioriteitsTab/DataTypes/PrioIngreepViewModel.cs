using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TLCGen.Controls;
using TLCGen.Dependencies.Providers;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    // Prio ingreep view-model, split over partial files per section of PrioIngreepView.xaml:
    // - PrioIngreepViewModel.cs                 fields, model, vehicle type, name, meldingen, wissels, constructor
    // - PrioIngreepViewModel.Koplus.cs          koplus and versnelde inmelding (Algemeen)
    // - PrioIngreepViewModel.Tijden.cs          rijtijden, groenbewaking, blokkeer- en roodtijden (Tijden)
    // - PrioIngreepViewModel.Opties.cs          prioriteitsopties, geconditioneerde prio, wagennummer, sirene, inmelding ook door
    // - PrioIngreepViewModel.LijnNummers.cs     lijnnummers and ritcategorie
    // - PrioIngreepViewModel.Perioden.cs        gerelateerde perioden
    // - PrioIngreepViewModel.Meerealiserend.cs  meerealiserende ingrepen
    // Rules for the model (melding types, RIS roles, dummy KAR detectors) live in PrioIngreepModelSync.
    public partial class PrioIngreepViewModel : PrioItemViewModel
    {
        private OVIngreepLijnNummerViewModel _selectedLijnNummer;
        private string _newLijnNummer;
        private RelayCommand _addLijnNummerCommand;
        private RelayCommand _add10LijnNummersCommand;
        private RelayCommand _removeLijnNummerCommand;
        private RelayCommand _removeIngreepCommand;
        private readonly FaseCyclusWithPrioViewModel _parentIngreep;
        private ObservableCollectionAroundList<OVIngreepLijnNummerViewModel, OVIngreepLijnNummerModel> _lijnNummers;
        private PrioIngreepWisselDataViewModel _wisselData;
        private PrioIngreepMeerealiserendeIngreepViewModel _SelectedMeerealiserendeIngreep;
        private ObservableCollection<string> _Fasen;
        private string _SelectedFase;

        [Browsable(false)]
        public PrioIngreepModel PrioIngreep { get; set; }

        [Browsable(false)]
        public string FaseCyclus => PrioIngreep.FaseCyclus;

        private ObservableCollection<PrioIngreepMeldingenListViewModel> _meldingenLists;
        private ObservableCollectionAroundList<OVIngreepPeriodeViewModel, OVIngreepPeriodeModel> _gerelateerdePerioden;
        private OVIngreepPeriodeViewModel _selectedPeriode;
        private ItemsManagerViewModel<OVIngreepPeriodeViewModel, string> _gerelateerdePeriodenManager;
        public ObservableCollection<PrioIngreepMeldingenListViewModel> MeldingenLists => _meldingenLists ??= new ObservableCollection<PrioIngreepMeldingenListViewModel>();

        [Category("Algemene opties")]
        [Description("Type voertuig")]
        public PrioIngreepVoertuigTypeEnum Type
        {
            get => PrioIngreep.Type;
            set
            {
                // the model owns the rules (allowed melding types, RIS roles, dummy KAR detectors); the
                // melding view-models then follow the model
                PrioIngreepModelSync.ApplyVehicleTypeChange(PrioIngreep, value, ControllerAccessProvider.Default.Controller.Data.CCOLVersie);
                foreach (var melding in MeldingenLists.SelectMany(x => x.Meldingen))
                {
                    melding.RefreshAvailableTypes();
                    melding.RISViewModel.UpdateRoles();
                }

                OnPropertyChanged(nameof(Type), broadcast: true);
                OnPropertyChanged(nameof(IsTypeBus));
                OnPropertyChanged(nameof(IsTypeBicycle));
                OnPropertyChanged(nameof(IsTypeTram));
                OnPropertyChanged(nameof(IsTypeTruck));
                OnPropertyChanged(nameof(IsTypeHulpdienst));
                OnPropertyChanged(nameof(IsNotTypeHulpdienst));
                OnPropertyChanged(nameof(ShowLijnnummer));

                // values only: copying reference properties would replace MeldingenData etc. with the defaults' instances
                DefaultsProvider.Default.SetDefaultsOnModel(PrioIngreep, PrioIngreep.Type.ToString(), null, true);
            }
        }

        [Browsable(false)]
        public bool IsTypeBus => Type == PrioIngreepVoertuigTypeEnum.Bus;
        [Browsable(false)]
        public bool IsTypeBicycle => Type == PrioIngreepVoertuigTypeEnum.Fiets;
        [Browsable(false)]
        public bool IsTypeTram => Type == PrioIngreepVoertuigTypeEnum.Tram;
        [Browsable(false)]
        public bool IsTypeTruck => Type == PrioIngreepVoertuigTypeEnum.Vrachtwagen;
        [Browsable(false)]
        public bool IsTypeHulpdienst => Type == PrioIngreepVoertuigTypeEnum.Hulpdienst;
        [Browsable(false)]
        public bool IsNotTypeHulpdienst => Type != PrioIngreepVoertuigTypeEnum.Hulpdienst;
        [Browsable(false)]
        public bool ShowLijnnummer => Type != PrioIngreepVoertuigTypeEnum.Hulpdienst &&
                                      Type != PrioIngreepVoertuigTypeEnum.Fiets &&
                                      Type != PrioIngreepVoertuigTypeEnum.Vrachtwagen;

        [Browsable(false)]
        public string DisplayName => PrioIngreep.DisplayName;

        public string Naam
        {
            get => PrioIngreep.Naam;
            set
            {
                var oldName = PrioIngreep.Naam;
                var newName = PrioIngreep.FaseCyclus + value;
                if (string.IsNullOrWhiteSpace(value))
                {
                    PrioIngreep.Naam = oldName;
                    OnPropertyChanged(nameof(Naam), broadcast: true);
                    OnPropertyChanged(nameof(DisplayName));
                    return;
                }
                if (NameSyntaxChecker.IsValidCName(value) &&
                    Integrity.TLCGenIntegrityChecker.IsElementNaamUnique(
                    DataAccess.TLCGenControllerDataProvider.Default.Controller, newName,
                    TLCGenObjectTypeEnum.PrioriteitsIngreep))
                {
                    PrioIngreepModelSync.RenameIngreep(PrioIngreep, value);
                    OnPropertyChanged(nameof(Naam), broadcast: true);
                }
                else
                {
                    PrioIngreep.Naam = oldName;
                }
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        [Browsable(false)]
        public bool HasKAR => PrioIngreep.HasPrioIngreepKAR();

        [Browsable(false)]
        public bool HasVecom => PrioIngreep.HasOVIngreepVecom();

        public string Description => PrioIngreep.DisplayName;

        public PrioIngreepWisselDataViewModel WisselData =>
            _wisselData ??= new PrioIngreepWisselDataViewModel(PrioIngreep.MeldingenData);

        public ICommand RemoveIngreepCommand => _removeIngreepCommand ??= new RelayCommand(() =>
            {
                _parentIngreep.Ingrepen.Remove(this);
                WeakReferenceMessengerEx.Default.Send(new PrioIngreepMeldingChangedMessage(PrioIngreep.FaseCyclus, null, true));
            });

        public void SetRisRoles(PrioIngreepInUitMeldingViewModel ingreepMelding)
        {
            PrioIngreepModelSync.SetRisRoles(ingreepMelding.PrioIngreepInUitMelding, Type);
            ingreepMelding.RISViewModel.UpdateRoles();
        }

        private void OnNameChanged(object sender, NameChangedMessage nmsg)
        {
            OnPropertyChanged(nameof(Naam));
        }

        public PrioIngreepViewModel(PrioIngreepModel ovingreep, FaseCyclusWithPrioViewModel parentIngreep)
        {
            PrioIngreep = ovingreep;
            _parentIngreep = parentIngreep;

            WeakReferenceMessengerEx.Default.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
            WeakReferenceMessengerEx.Default.Register<PeriodenChangedMessage>(this, OnPeriodenChanged);
            WeakReferenceMessengerEx.Default.Register<CCOLVersionChangedMessage>(this, OnCCOLVersionChanged);
            WeakReferenceMessengerEx.Default.Register<PrioIngrepenChangedMessage>(this, OnPrioIngrepenChanged);
            Detectoren = new ObservableCollection<string>();
            OnDetectorenChanged(null, null);

            MeldingenLists.Add(new PrioIngreepMeldingenListViewModel("Inmeldingen", PrioIngreepInUitMeldingTypeEnum.Inmelding, ovingreep.MeldingenData, this));
            MeldingenLists.Add(new PrioIngreepMeldingenListViewModel("Uitmeldingen", PrioIngreepInUitMeldingTypeEnum.Uitmelding, ovingreep.MeldingenData, this));

            MeerealiserendeIngrepen = new ObservableCollectionAroundList<PrioIngreepMeerealiserendeIngreepViewModel, PrioIngreepMeerealiserendeIngreepModel>(ovingreep.MeerealiserendeIngrepen);

            BuildFasenList();
            BuildIngrepenList();
        }

        private void OnCCOLVersionChanged(object sender, CCOLVersionChangedMessage obj)
        {
            PrioIngreepModelSync.ApplyAllowedMeldingTypes(PrioIngreep, ControllerAccessProvider.Default.Controller.Data.CCOLVersie);
            foreach (var m in MeldingenLists.SelectMany(x => x.Meldingen)) m.RefreshAvailableTypes();
        }
    }
}
