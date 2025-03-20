using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Models.Enumerations;
using TLCGen.Models;
using TLCGen.DataAccess;
using TLCGen.Settings;
using TLCGen.Messaging.Messages;

using TLCGen.Helpers;
using TLCGen.Extensions;
using TLCGen.ModelManagement;
using TLCGen.Integrity;


namespace TLCGen.ViewModels
{
    public class FaseCyclusViewModel : ObservableObjectEx, IComparable
    {
        #region Fields

        private ObservableCollection<string> _meeverlengenOpties;
        private string _meeverlengenTypeString;
        private ItemsManagerViewModel<HardMeeverlengenFaseCyclusViewModel, string> _hardMeeverlengenFasenManager;
        private HardMeeverlengenFaseCyclusViewModel _selectedHardMeeverlengenFase;

        #endregion // Fields

        #region Properties

        public FaseCyclusModel FaseCyclus { get; }

        public string Naam
        {
            get => FaseCyclus.Naam;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidCName(value))
                {
                    if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Fase, value))
                    {
                        void RenameDetector(DetectorModel detector, string oldFaseCyclusName)
                        {
                            var nd = detector.Naam.Replace(oldFaseCyclusName, value);
                            if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Detector, nd))
                            {
                                var oldD = detector.Naam;
                                detector.Naam = nd;
WeakReferenceMessenger.Default.Send(new NameChangingMessage(TLCGenObjectTypeEnum.Detector, oldD, detector.Naam));
                            }

                            nd = detector.VissimNaam?.Replace(oldFaseCyclusName, value);
                            if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Detector, nd, vissim: true))
                            {
                                detector.VissimNaam = nd;
                            }
                        }

                        var oldname = FaseCyclus.Naam;
                        FaseCyclus.Naam = value;

                        // Notify the messenger
WeakReferenceMessenger.Default.Send(new NameChangingMessage(TLCGenObjectTypeEnum.Fase, oldname, value));

                        // set new type
                        Type = Settings.Utilities.FaseCyclusUtilities.GetFaseTypeFromNaam(value);

                        foreach (var d in FaseCyclus.Detectoren)
                        {
                            RenameDetector(d, oldname);
                        }

                        if (TLCGenControllerDataProvider.Default.Controller != null)
                        {
                            var prios = TLCGenControllerDataProvider.Default.Controller.PrioData.PrioIngrepen.Where(x =>
                                x.FaseCyclus == FaseCyclus.Naam);
                            var hds = TLCGenControllerDataProvider.Default.Controller.PrioData.HDIngrepen.Where(x =>
                                x.FaseCyclus == FaseCyclus.Naam);
                            foreach (var prio in prios)
                            {
                                foreach (var melding in prio.MeldingenData.Inmeldingen.Where(x =>
                                             x.DummyKARMelding != null))
                                    RenameDetector(melding.DummyKARMelding, oldname);
                                foreach (var melding in prio.MeldingenData.Uitmeldingen.Where(x =>
                                             x.DummyKARMelding != null))
                                    RenameDetector(melding.DummyKARMelding, oldname);
                            }

                            foreach (var hd in hds)
                            {
                                RenameDetector(hd.DummyKARInmelding, oldname);
                                RenameDetector(hd.DummyKARUitmelding, oldname);
                            }
                        }
                    }
                }
                OnPropertyChanged(string.Empty); // Update all properties
                OnPropertyChanged(nameof(Naam), broadcast: true);
            }
        }

        public FaseTypeEnum Type
        {
            get => FaseCyclus.Type;
            set
            {
                if (FaseCyclus.Type != value)
                {
                    var old = FaseCyclus.Type;
                    FaseCyclus.Type = value;

                    // Apply new defaults
                    var iL = FaseCyclus.AantalRijstroken;
                    DefaultsProvider.Default.SetDefaultsOnModel(this.FaseCyclus, this.Type.ToString());
                    if (iL != FaseCyclus.AantalRijstroken)
                    {
                        WeakReferenceMessenger.Default.Send(new FaseAantalRijstrokenChangedMessage(FaseCyclus, FaseCyclus.AantalRijstroken));
                    }

                    if (value != FaseTypeEnum.Voetganger && MeeverlengenType == MeeVerlengenTypeEnum.Voetganger)
                    {
                        MeeverlengenType = MeeVerlengenTypeEnum.Default;
                    }
                    SetMeeverlengenOpties();

                    OnPropertyChanged(string.Empty); // Update all properties
                    OnPropertyChanged(nameof(Type), broadcast: true);

                    WeakReferenceMessenger.Default.Send(new FaseTypeChangedMessage(FaseCyclus, old, value));
                }
            }
        }

        public int TFG
        {
            get => FaseCyclus.TFG;
            set
            {
                if (value >= 0 && value >= TGG)
                    FaseCyclus.TFG = value;
                else
                    FaseCyclus.TFG = TGG;
                OnPropertyChanged(nameof(TFG), broadcast: true);
            }
        }

        public int TGG
        {
            get => FaseCyclus.TGG;
            set
            {
                if (value >= 0 && value >= TGG_min)
                {
                    FaseCyclus.TGG = value;
                }
                else
                {
                    FaseCyclus.TGG = TGG_min;
                }
                if (TFG < FaseCyclus.TGG) TFG = FaseCyclus.TGG;
                OnPropertyChanged(nameof(TGG), broadcast: true);
            }
        }

        public int TGG_min
        {
            get => FaseCyclus.TGG_min;
            set
            {
                if (value >= 0)
                {
                    FaseCyclus.TGG_min = value;
                    if (TGG < value)
                        TGG = value;
                }
                OnPropertyChanged(nameof(TGG_min), broadcast: true);
            }
        }

        public int TRG
        {
            get => FaseCyclus.TRG;
            set
            {
                if (value >= 0 && value >= TRG_min)
                {
                    FaseCyclus.TRG = value;
                }
                else
                    FaseCyclus.TRG = TRG_min;
                OnPropertyChanged(nameof(TRG), broadcast: true);
            }
        }

        public int TRG_min
        {
            get => FaseCyclus.TRG_min;
            set
            {
                if (value >= 0)
                {
                    FaseCyclus.TRG_min = value;
                    if (TRG < value)
                        TRG = value;
                }
                OnPropertyChanged(nameof(TRG_min), broadcast: true);
            }
        }

        public int TGL
        {
            get => FaseCyclus.TGL;
            set
            {
                if (value >= 0 && (value >= TGL_min || ControllerAccessProvider.Default.Controller.Data.Intergroen))
                {
                    FaseCyclus.TGL = value;
                    if (ControllerAccessProvider.Default.Controller.Data.Intergroen)
                    {
                        FaseCyclus.TGL_min = value;
                        OnPropertyChanged(nameof(TGL_min), broadcast: true);
                    }
                }
                else
                {
                    FaseCyclus.TGL = TGL_min;
                }
                OnPropertyChanged(nameof(TGL), broadcast: true);
            }
        }

        public int TGL_min
        {
            get => FaseCyclus.TGL_min;
            set
            {
                if (value >= 0)
                {
                    FaseCyclus.TGL_min = value;
                    if (TGL < value) TGL = value;
                }
                OnPropertyChanged(nameof(TGL_min), broadcast: true);
            }
        }

        public int Kopmax
        {
            get => FaseCyclus.Kopmax;
            set
            {
                if (value >= 0)
                {
                    FaseCyclus.Kopmax = value;
                }
                OnPropertyChanged(nameof(Kopmax), broadcast: true);
            }
        }

        public int? AantalRijstroken
        {
            get => FaseCyclus.AantalRijstroken;
            set
            {
                if (value >= 0)
                {
                    FaseCyclus.AantalRijstroken = value;
                    foreach (var d in FaseCyclus.Detectoren)
                    {
                        if (d.Rijstrook > value)
                        {
                            d.Rijstrook = value;
                        }
                    }
                }
                OnPropertyChanged(nameof(ToepassenMK2Enabled));
                OnPropertyChanged(nameof(AantalRijstroken), broadcast: true);
                WeakReferenceMessenger.Default.Send(new FaseAantalRijstrokenChangedMessage(FaseCyclus, FaseCyclus.AantalRijstroken));
            }
        }

        public NooitAltijdAanUitEnum VasteAanvraag
        {
            get => FaseCyclus.VasteAanvraag;
            set
            {
                FaseCyclus.VasteAanvraag = value;
                OnPropertyChanged(nameof(VasteAanvraag), broadcast: true);
                OnPropertyChanged(nameof(UitgesteldeVasteAanvraagPossible));
            }
        }

        public NooitAltijdAanUitEnum Wachtgroen
        {
            get => FaseCyclus.Wachtgroen;
            set
            {
                FaseCyclus.Wachtgroen = value;
                OnPropertyChanged(nameof(Wachtgroen), broadcast: true);
            }
        }

        public NooitAltijdAanUitEnum Meeverlengen
        {
            get => FaseCyclus.Meeverlengen;
            set
            {
                FaseCyclus.Meeverlengen = value;
                OnPropertyChanged(nameof(Meeverlengen), broadcast: true);
            }
        }

        public MeeVerlengenTypeEnum MeeverlengenType
        {
            get => FaseCyclus.MeeverlengenType;
            set
            {
                FaseCyclus.MeeverlengenType = value;
                OnPropertyChanged(nameof(MeeverlengenType), broadcast: true);
            }
        }

        public AlternatieveRuimteTypeEnum AlternatieveRuimteType
        {
            get => FaseCyclus.AlternatieveRuimteType;
            set
            {
                FaseCyclus.AlternatieveRuimteType = value;
                OnPropertyChanged(nameof(AlternatieveRuimteType), broadcast: true);
            }
        }

        public int? MeeverlengenVerschil
        {
            get => FaseCyclus.MeeverlengenVerschil;
            set
            {
                FaseCyclus.MeeverlengenVerschil = value;
                OnPropertyChanged(nameof(MeeverlengenVerschil), broadcast: true);
            }
        }

        public bool UitgesteldeVasteAanvraag
        {
            get => FaseCyclus.UitgesteldeVasteAanvraag;
            set
            {
                FaseCyclus.UitgesteldeVasteAanvraag = value;
                OnPropertyChanged(nameof(UitgesteldeVasteAanvraag), broadcast: true);
            }
        }

        public int UitgesteldeVasteAanvraagTijdsDuur
        {
            get => FaseCyclus.UitgesteldeVasteAanvraagTijdsduur;
            set
            {
                FaseCyclus.UitgesteldeVasteAanvraagTijdsduur = value;
                OnPropertyChanged(nameof(UitgesteldeVasteAanvraagTijdsDuur), broadcast: true);
            }
        }

        public bool UitgesteldeVasteAanvraagPossible => VasteAanvraag != NooitAltijdAanUitEnum.Nooit;

        public bool HiaatKoplusBijDetectieStoring
        {
            get => FaseCyclus.HiaatKoplusBijDetectieStoring;
            set
            {
                FaseCyclus.HiaatKoplusBijDetectieStoring = value;
                if (value && !VervangendHiaatKoplus.HasValue)
                {
                    VervangendHiaatKoplus = 25;
                }
                OnPropertyChanged(nameof(HiaatKoplusBijDetectieStoring), broadcast: true);
            }
        }

        public bool HasHiaatKoplusBijDetectieStoring => Type != FaseTypeEnum.Voetganger && Type != FaseTypeEnum.Fiets;

        public bool AanvraagBijDetectieStoring
        {
            get => FaseCyclus.AanvraagBijDetectieStoring;
            set
            {
                FaseCyclus.AanvraagBijDetectieStoring = value;
                OnPropertyChanged(nameof(AanvraagBijDetectieStoring), broadcast: true);
                OnPropertyChanged(nameof(AanvraagBijDetectieStoringKoplusKnopVisible));
                OnPropertyChanged(nameof(AanvraagBijDetectieStoringVertraagdVisible));
            }
        }

        public bool AanvraagBijDetectieStoringVertraagdVisible => AanvraagBijDetectieStoring;

        public bool AanvraagBijDetectieStoringVertraagd
        {
            get => FaseCyclus.AanvraagBijDetectieStoringVertraagd;
            set
            {
                FaseCyclus.AanvraagBijDetectieStoringVertraagd = value;
                OnPropertyChanged(nameof(AanvraagBijDetectieStoringVertraagd), broadcast: true);
            }
        }

        public bool AanvraagBijDetectieStoringVertragingVisible => AanvraagBijDetectieStoringVertraagd;

        public int AanvraagBijDetectieStoringVertraging
        {
            get => FaseCyclus.AanvraagBijDetectieStoringVertraging;
            set
            {
                FaseCyclus.AanvraagBijDetectieStoringVertraging = value;
                OnPropertyChanged(nameof(AanvraagBijDetectieStoringVertraging), broadcast: true);
            }
        }

        public bool AanvraagBijDetectieStoringKoplusKnopVisible =>
            AanvraagBijDetectieStoring &&
            FaseCyclus.Detectoren.Any(x => x.Type == DetectorTypeEnum.Kop) &&
            FaseCyclus.Detectoren.Any(x => x.Type == DetectorTypeEnum.Knop);

        public bool AanvraagBijDetectieStoringKoplusKnop
        {
            get => FaseCyclus.AanvraagBijDetectieStoringKoplusKnop;
            set
            {
                FaseCyclus.AanvraagBijDetectieStoringKoplusKnop = value;
                OnPropertyChanged(nameof(AanvraagBijDetectieStoringKoplusKnop), broadcast: true);
            }
        }

        public bool AanvraagBijDetectieStoringKopLangVisible =>
            AanvraagBijDetectieStoring &&
            FaseCyclus.Detectoren.Any(x => x.Type == DetectorTypeEnum.Kop) &&
            FaseCyclus.Detectoren.Any(x => x.Type == DetectorTypeEnum.Lang);

        public bool AanvraagBijDetectieStoringKopLang
        {
            get => FaseCyclus.AanvraagBijDetectieStoringKopLang;
            set
            {
                FaseCyclus.AanvraagBijDetectieStoringKopLang = value;
                OnPropertyChanged(nameof(AanvraagBijDetectieStoringKopLang), broadcast: true);
            }
        }

        public bool PercentageGroenBijDetectieStoring
        {
            get => FaseCyclus.PercentageGroenBijDetectieStoring;
            set
            {
                FaseCyclus.PercentageGroenBijDetectieStoring = value;
                if (!PercentageGroenBijStoring.HasValue)
                {
                    PercentageGroenBijStoring = 65;
                }
                OnPropertyChanged(nameof(PercentageGroenBijDetectieStoring), broadcast: true);
            }
        }

        public bool HasPercentageGroenBijDetectieStoring => Type != FaseTypeEnum.Voetganger;

        public int? VervangendHiaatKoplus
        {
            get => FaseCyclus.VervangendHiaatKoplus;
            set
            {
                FaseCyclus.VervangendHiaatKoplus = value;
                OnPropertyChanged(nameof(VervangendHiaatKoplus), broadcast: true);
            }
        }

        public int? PercentageGroenBijStoring
        {
            get => FaseCyclus.PercentageGroen;
            set
            {
                FaseCyclus.PercentageGroen = value;
                OnPropertyChanged(nameof(PercentageGroenBijStoring), broadcast: true);
            }
        }

        public int VeiligheidsGroenMaximaal
        {
            get => FaseCyclus.VeiligheidsGroenMaximaal;
            set
            {
                FaseCyclus.VeiligheidsGroenMaximaal = value;
                OnPropertyChanged(nameof(VeiligheidsGroenMaximaal), broadcast: true);
            }
        }

        public bool ToepassenMK2Enabled => true; //AantalRijstroken > 1;

        public bool ToepassenMK2
        {
            get => FaseCyclus.ToepassenMK2;
            set
            {
                FaseCyclus.ToepassenMK2 = value;
                OnPropertyChanged(nameof(ToepassenMK2), broadcast: true);
            }
        }

        public int DetectorCount => FaseCyclus.Detectoren.Count;

        public bool HasKopmax
        {
            get
            {
                return FaseCyclus.Detectoren.Any(dvm => dvm.Verlengen != DetectorVerlengenTypeEnum.Geen);
            }
        }

        public bool HasVeiligheidsGroen
        {
            get
            {
                return FaseCyclus.Detectoren.Any(dvm => dvm.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit);
            }
        }

        public bool HDIngreep
        {
            get => FaseCyclus.HDIngreep;
            set
            {
                FaseCyclus.HDIngreep = value;
                OnPropertyChanged(nameof(HDIngreep), broadcast: true);
            }
        }

        public bool WachttijdVoorspeller
        {
            get => FaseCyclus.WachttijdVoorspeller;
            set
            {
                FaseCyclus.WachttijdVoorspeller = value;
                OnPropertyChanged(nameof(WachttijdVoorspeller), broadcast: true);
                TLCGenModelManager.Default.UpdateControllerAlerts();
            }
        }

        private ObservableCollectionAroundList<HardMeeverlengenFaseCyclusViewModel, HardMeeverlengenFaseCyclusModel> _hardMeeverlengenFaseCycli;
        private ObservableCollection<string> _alternatieveRuimteOpties;
        private string _alternatieveRuimteTypeString;
        public ObservableCollectionAroundList<HardMeeverlengenFaseCyclusViewModel, HardMeeverlengenFaseCyclusModel> HardMeeverlengenFaseCycli => _hardMeeverlengenFaseCycli ??= new ObservableCollectionAroundList<HardMeeverlengenFaseCyclusViewModel, HardMeeverlengenFaseCyclusModel>(FaseCyclus.HardMeeverlengenFaseCycli);

        public HardMeeverlengenFaseCyclusViewModel SelectedHardMeeverlengenFase
        {
            get => _selectedHardMeeverlengenFase;
            set
            {
                _selectedHardMeeverlengenFase = value;
                HardMeeverlengenFasenManager.SelectedItem = value;
                OnPropertyChanged();
            }
        }

        public ItemsManagerViewModel<HardMeeverlengenFaseCyclusViewModel, string> HardMeeverlengenFasenManager =>
            _hardMeeverlengenFasenManager ??= new ItemsManagerViewModel<HardMeeverlengenFaseCyclusViewModel, string>(
                HardMeeverlengenFaseCycli,
                ControllerAccessProvider.Default.AllSignalGroupStrings,
                e => new HardMeeverlengenFaseCyclusViewModel(new HardMeeverlengenFaseCyclusModel() { FaseCyclus = e, AanUit = true }),
                e => e != Naam &&
                     !TLCGenControllerChecker.IsFasenConflicting(TLCGenControllerDataProvider.Default.Controller, Naam, e) &&
                     HardMeeverlengenFaseCycli.All(x => x.FaseCyclus != e),
                null,
                UpdateHardMeeVerlengenItemsChanged,
                UpdateHardMeeVerlengenItemsChanged);

        private void UpdateHardMeeVerlengenItemsChanged()
        {
            OnPropertyChanged(broadcast: true);
            OnPropertyChanged(nameof(HasHardMeeverlengenFasen));
        }

        public bool HasHardMeeverlengenFasen => HardMeeverlengenFaseCycli.Any();

        public ObservableCollection<string> MeeverlengenOpties => _meeverlengenOpties ??= new ObservableCollection<string>();

        public string MeeverlengenTypeString
        {
            get => _meeverlengenTypeString;
            set
            {
                _meeverlengenTypeString = value;
                if (value == MeeVerlengenTypeEnum.Default.GetDescription())
                {
                    MeeverlengenType = MeeVerlengenTypeEnum.Default;
                }
                else if (value == MeeVerlengenTypeEnum.To.GetDescription())
                {
                    MeeverlengenType = MeeVerlengenTypeEnum.To;
                }
                else if (value == MeeVerlengenTypeEnum.MKTo.GetDescription())
                {
                    MeeverlengenType = MeeVerlengenTypeEnum.MKTo;
                }
                else if (value == MeeVerlengenTypeEnum.Voetganger.GetDescription())
                {
                    MeeverlengenType = MeeVerlengenTypeEnum.Voetganger;
                }
                else if (value == MeeVerlengenTypeEnum.DefaultCCOL.GetDescription())
                {
                    MeeverlengenType = MeeVerlengenTypeEnum.DefaultCCOL;
                }
                else if (value == MeeVerlengenTypeEnum.ToCCOL.GetDescription())
                {
                    MeeverlengenType = MeeVerlengenTypeEnum.ToCCOL;
                }
                else if (value == MeeVerlengenTypeEnum.MKToCCOL.GetDescription())
                {
                    MeeverlengenType = MeeVerlengenTypeEnum.MKToCCOL;
                }
                else if (value == MeeVerlengenTypeEnum.MaatgevendGroen.GetDescription())
                {
                    MeeverlengenType = MeeVerlengenTypeEnum.MaatgevendGroen;
                }
                else if (value == MeeVerlengenTypeEnum.Default2.GetDescription())
                {
                    MeeverlengenType = MeeVerlengenTypeEnum.Default2;
                }
                else if (value == MeeVerlengenTypeEnum.MKTo2.GetDescription())
                {
                    MeeverlengenType = MeeVerlengenTypeEnum.MKTo2;
                }
                else if (value == MeeVerlengenTypeEnum.To2.GetDescription())
                {
                    MeeverlengenType = MeeVerlengenTypeEnum.To2;
                }
                else if (value == MeeVerlengenTypeEnum.Voetganger2.GetDescription())
                {
                    MeeverlengenType = MeeVerlengenTypeEnum.Voetganger2;
                }
                else if (value != null)
                {
                    throw new ArgumentOutOfRangeException(nameof(MeeverlengenTypeString),
                        "MeeverlengenTypeString was set to value that is not defined for MeeverlengenType");
                }

                OnPropertyChanged();
            }
        }
        
        public ObservableCollection<string> AlternatieveRuimteOpties => _alternatieveRuimteOpties ??= new ObservableCollection<string>();

        public string AlternatieveRuimteTypeString
        {
            get => _alternatieveRuimteTypeString;
            set
            {
                _alternatieveRuimteTypeString = value;
                if (value == AlternatieveRuimteTypeEnum.MaxTarToTig.GetDescription())
                {
                    AlternatieveRuimteType = AlternatieveRuimteTypeEnum.MaxTarToTig;
                }
                else if (value == AlternatieveRuimteTypeEnum.MaxTar.GetDescription())
                {
                    AlternatieveRuimteType = AlternatieveRuimteTypeEnum.MaxTar;
                }
                else if (value == AlternatieveRuimteTypeEnum.RealRuimte.GetDescription())
                {
                    AlternatieveRuimteType = AlternatieveRuimteTypeEnum.RealRuimte;
                }
                else if (value != null)
                {
                    throw new ArgumentOutOfRangeException(nameof(MeeverlengenTypeString),
                        "MeeverlengenTypeString was set to value that is not defined for MeeverlengenType");
                }

                OnPropertyChanged();
            }
        }

        public bool MeeverlengenTypeInstelbaarOpStraat
        {
            get => FaseCyclus.MeeverlengenTypeInstelbaarOpStraat;
            set
            {
                FaseCyclus.MeeverlengenTypeInstelbaarOpStraat = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool SchoolIngreepEnabled => FaseCyclus.Detectoren.Any(x => x.Type == DetectorTypeEnum.KnopBinnen || x.Type == DetectorTypeEnum.KnopBuiten);

        public bool SchoolIngreepActive => SchoolIngreep != NooitAltijdAanUitEnum.Nooit;

        public NooitAltijdAanUitEnum SchoolIngreep
        {
            get => FaseCyclus.SchoolIngreep;
            set
            {
                FaseCyclus.SchoolIngreep = value;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(SchoolIngreepActive));
            }
        }

        public int SchoolIngreepMaximumGroen
        {
            get => FaseCyclus.SchoolIngreepMaximumGroen;
            set
            {
                FaseCyclus.SchoolIngreepMaximumGroen = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int SchoolIngreepBezetTijd
        {
            get => FaseCyclus.SchoolIngreepBezetTijd;
            set
            {
                FaseCyclus.SchoolIngreepBezetTijd = value;
                OnPropertyChanged(broadcast: true);
            }
        }
        public bool SeniorenIngreepEnabled => FaseCyclus.Detectoren.Any(x => x.Type == DetectorTypeEnum.KnopBinnen || x.Type == DetectorTypeEnum.KnopBuiten);

        public bool SeniorenIngreepActive => SeniorenIngreep != NooitAltijdAanUitEnum.Nooit;

        public NooitAltijdAanUitEnum SeniorenIngreep
        {
            get => FaseCyclus.SeniorenIngreep;
            set
            {
                FaseCyclus.SeniorenIngreep = value;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(SeniorenIngreepActive));
            }
        }

        public int SeniorenIngreepExtraGroenPercentage
        {
            get => FaseCyclus.SeniorenIngreepExtraGroenPercentage;
            set
            {
                FaseCyclus.SeniorenIngreepExtraGroenPercentage = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int SeniorenIngreepBezetTijd
        {
            get => FaseCyclus.SeniorenIngreepBezetTijd;
            set
            {
                FaseCyclus.SeniorenIngreepBezetTijd = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        #endregion // Properties

        #region Overrides

        public override string ToString()
        {
            return Naam;
        }

        #endregion // Overrides

        #region IComparable

        public int CompareTo(object obj)
        {
            if (!(obj is FaseCyclusViewModel fcvm))
                throw new InvalidCastException();
            return TLCGenIntegrityChecker.CompareSignalGroups(this.Naam, fcvm.Naam);
        }

        #endregion // IComparable

        #region Public methods

        public void UpdateHasKopmax()
        {
            OnPropertyChanged(nameof(HasKopmax));
        }

        public void UpdateHasVeiligheidsGroen()
        {
            OnPropertyChanged(nameof(HasVeiligheidsGroen));
        }

        #endregion // Public methods

        #region Private Methods

        private void SetMeeverlengenOpties()
        {
            MeeverlengenOpties.Clear();
            MeeverlengenOpties.Add(MeeVerlengenTypeEnum.Default.GetDescription());
            MeeverlengenOpties.Add(MeeVerlengenTypeEnum.To.GetDescription());
            MeeverlengenOpties.Add(MeeVerlengenTypeEnum.MKTo.GetDescription());
            if (FaseCyclus.Type == FaseTypeEnum.Voetganger)
            {
                MeeverlengenOpties.Add(MeeVerlengenTypeEnum.Voetganger.GetDescription());
                MeeverlengenOpties.Add(MeeVerlengenTypeEnum.Voetganger2.GetDescription());
            }
            MeeverlengenOpties.Add(MeeVerlengenTypeEnum.DefaultCCOL.GetDescription());
            MeeverlengenOpties.Add(MeeVerlengenTypeEnum.ToCCOL.GetDescription());
            MeeverlengenOpties.Add(MeeVerlengenTypeEnum.MKToCCOL.GetDescription());
            if (TLCGenControllerDataProvider.Default.Controller?.Data.SynchronisatiesType == SynchronisatiesTypeEnum.RealFunc)
            {
                MeeverlengenOpties.Add(MeeVerlengenTypeEnum.MaatgevendGroen.GetDescription());
            }
            MeeverlengenOpties.Add(MeeVerlengenTypeEnum.Default2.GetDescription());
            MeeverlengenOpties.Add(MeeVerlengenTypeEnum.To2.GetDescription());
            MeeverlengenOpties.Add(MeeVerlengenTypeEnum.MKTo2.GetDescription());

            _meeverlengenTypeString = MeeverlengenType.GetDescription();
            OnPropertyChanged(nameof(MeeverlengenTypeString));
        }

        private void SetAlternatieveRuimteTypeOpties()
        {
            AlternatieveRuimteOpties.Clear();
            AlternatieveRuimteOpties.Add(AlternatieveRuimteTypeEnum.MaxTarToTig.GetDescription());
            AlternatieveRuimteOpties.Add(AlternatieveRuimteTypeEnum.MaxTar.GetDescription());
            if (TLCGenControllerDataProvider.Default.Controller?.Data.SynchronisatiesType == SynchronisatiesTypeEnum.RealFunc)
            {
                AlternatieveRuimteOpties.Add(AlternatieveRuimteTypeEnum.RealRuimte.GetDescription());
            }

            _alternatieveRuimteTypeString = AlternatieveRuimteType.GetDescription();
            OnPropertyChanged(nameof(AlternatieveRuimteTypeString));
        }

        #endregion // Private Methods

        #region TLCGen events

        private void OnFasenChanged(object sender, FasenChangedMessage msg)
        {
            _hardMeeverlengenFasenManager?.Refresh();
        }

        private void OnNameChanged(object sender, NameChangedMessage obj)
        {
            _hardMeeverlengenFasenManager?.Refresh();
        }

        private void OnSynchronisatiesTypeChanged(object sender, SynchronisatiesTypeChangedMessage obj)
        {
            SetMeeverlengenOpties();
            SetAlternatieveRuimteTypeOpties();
        }

        #endregion // TLCGen events

        #region Constructor

        public FaseCyclusViewModel(FaseCyclusModel fasecyclus)
        {
            FaseCyclus = fasecyclus;
            SetMeeverlengenOpties();
            SetAlternatieveRuimteTypeOpties();

            WeakReferenceMessenger.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessenger.Default.Register<NameChangedMessage>(this, OnNameChanged);
            WeakReferenceMessenger.Default.Register<SynchronisatiesTypeChangedMessage>(this, OnSynchronisatiesTypeChanged);
        }

        #endregion // Constructor
    }
}
