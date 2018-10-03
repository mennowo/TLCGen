using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using TLCGen.Models.Enumerations;
using TLCGen.Models;
using TLCGen.DataAccess;
using TLCGen.Settings;
using TLCGen.Messaging.Messages;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Helpers;
using TLCGen.Extensions;
using TLCGen.ModelManagement;
using TLCGen.Integrity;

namespace TLCGen.ViewModels
{
    public class FaseCyclusViewModel : ViewModelBase, IComparable
    {
        #region Fields

        private FaseCyclusModel _faseCyclus;
        private ObservableCollection<string> _meeverlengenOpties;
        private List<string> _controllerFasen;
        private string _meeverlengenTypeString;
        private ItemsManagerViewModel<HardMeeverlengenFaseCyclusViewModel, string> _hardMeeverlengenFasenManager;
        private HardMeeverlengenFaseCyclusViewModel _selectedHardMeeverlengenFase;

        #endregion // Fields

        #region Properties

        public FaseCyclusModel FaseCyclus => _faseCyclus;

        public string Naam
        {
            get => _faseCyclus.Naam;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidName(value))
                {
                    if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Fase, value))
                    {
                        string oldname = _faseCyclus.Naam;
                        _faseCyclus.Naam = value;

                        // set new type
                        this.Type = Settings.Utilities.FaseCyclusUtilities.GetFaseTypeFromNaam(value);

                        foreach (var d in FaseCyclus.Detectoren)
                        {
                            string nd = d.Naam.Replace(oldname, value);
                            if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Detector, nd))
                            {
                                var oldD = d.Naam;
                                d.Naam = nd;
                                Messenger.Default.Send(new NameChangingMessage(TLCGenObjectTypeEnum.Detector, oldD, d.Naam));
                            }
                            nd = d.VissimNaam?.Replace(oldname, value);
                            if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Detector, nd))
                            {
                                d.VissimNaam = nd;
                            }
                        }

                        // Notify the messenger
                        Messenger.Default.Send(new NameChangingMessage(TLCGenObjectTypeEnum.Fase, oldname, value));
                    }
                }
                RaisePropertyChanged(string.Empty); // Update all properties
                RaisePropertyChanged<object>(nameof(Naam), broadcast: true);
            }
        }

        public FaseTypeEnum Type
        {
            get => _faseCyclus.Type;
            set
            {
                if (_faseCyclus.Type != value)
                {
                    _faseCyclus.Type = value;

                    // Apply new defaults
                    var iL = FaseCyclus.AantalRijstroken;
                    DefaultsProvider.Default.SetDefaultsOnModel(this.FaseCyclus, this.Type.ToString());
                    if(iL != FaseCyclus.AantalRijstroken)
                    {
                        MessengerInstance.Send(new FaseAantalRijstrokenChangedMessage(FaseCyclus, FaseCyclus.AantalRijstroken));
                    }

                    if (value != FaseTypeEnum.Voetganger && MeeverlengenType == MeeVerlengenTypeEnum.Voetganger)
                    {
                        MeeverlengenType = MeeVerlengenTypeEnum.Default;
                    }
                    SetMeeverlengenOpties();

                    RaisePropertyChanged(string.Empty); // Update all properties
                    RaisePropertyChanged<object>(nameof(Type), broadcast: true);
                }
            }
        }

        public int TFG
        {
            get => _faseCyclus.TFG;
            set
            {
                if (value >= 0 && value >= TGG)
                    _faseCyclus.TFG = value;
                else
                    _faseCyclus.TFG = TGG;
                RaisePropertyChanged<object>(nameof(TFG), broadcast: true);
            }
        }

        public int TGG
        {
            get => _faseCyclus.TGG;
            set
            {
                if (value >= 0 && value >= TGG_min)
                {
                    _faseCyclus.TGG = value;
                }
                else
                {
                    _faseCyclus.TGG = TGG_min;
                }
                if (TFG < _faseCyclus.TGG) TFG = _faseCyclus.TGG;
                RaisePropertyChanged<object>(nameof(TGG), broadcast: true);
            }
        }

        public int TGG_min
        {
            get => _faseCyclus.TGG_min;
            set
            {
                if (value >= 0)
                {
                    _faseCyclus.TGG_min = value;
                    if (TGG < value)
                        TGG = value;
                }
                RaisePropertyChanged<object>(nameof(TGG_min), broadcast: true);
            }
        }

        public int TRG
        {
            get => _faseCyclus.TRG;
            set
            {
                if (value >= 0 && value >= TRG_min)
                {
                    _faseCyclus.TRG = value;
                }
                else
                    _faseCyclus.TRG = TRG_min;
                RaisePropertyChanged<object>(nameof(TRG), broadcast: true);
            }
        }

        public int TRG_min
        {
            get => _faseCyclus.TRG_min;
            set
            {
                if (value >= 0)
                {
                    _faseCyclus.TRG_min = value;
                    if (TRG < value)
                        TRG = value;
                }
                RaisePropertyChanged<object>(nameof(TRG_min), broadcast: true);
            }
        }

        public int TGL
        {
            get => _faseCyclus.TGL;
            set
            {
                if (value >= 0 && value >= TGL_min)
                {
                    _faseCyclus.TGL = value;
                }
                else
                    _faseCyclus.TGL = TGL_min;
                RaisePropertyChanged<object>(nameof(TGL), broadcast: true);
            }
        }

        public int TGL_min
        {
            get => _faseCyclus.TGL_min;
            set
            {
                if (value >= 0)
                {
                    _faseCyclus.TGL_min = value;
                    if (TGL < value)
                        TGL = value;
                }
                RaisePropertyChanged<object>(nameof(TGL_min), broadcast: true);
            }
        }

        public int Kopmax
        {
            get => _faseCyclus.Kopmax;
            set
            {
                if (value >= 0)
                {
                    _faseCyclus.Kopmax = value;
                }
                RaisePropertyChanged<object>(nameof(Kopmax), broadcast: true);
            }
        }

        public int? AantalRijstroken
        {
            get => _faseCyclus.AantalRijstroken;
            set
            {
                if (value >= 0)
                {
                    _faseCyclus.AantalRijstroken = value;
                    foreach (var d in _faseCyclus.Detectoren)
                    {
                        if (d.Rijstrook > value)
                        {
                            d.Rijstrook = value;
                        }
                    }
                }
                RaisePropertyChanged<object>(nameof(AantalRijstroken), broadcast: true);
                MessengerInstance.Send(new FaseAantalRijstrokenChangedMessage(_faseCyclus, _faseCyclus.AantalRijstroken));
            }
        }

        public NooitAltijdAanUitEnum VasteAanvraag
        {
            get => _faseCyclus.VasteAanvraag;
            set
            {
                _faseCyclus.VasteAanvraag = value;
                RaisePropertyChanged<object>(nameof(VasteAanvraag), broadcast: true);
                RaisePropertyChanged(nameof(UitgesteldeVasteAanvraagPossible));
            }
        }

        public NooitAltijdAanUitEnum Wachtgroen
        {
            get => _faseCyclus.Wachtgroen;
            set
            {
                _faseCyclus.Wachtgroen = value;
                RaisePropertyChanged<object>(nameof(Wachtgroen), broadcast: true);
            }
        }

        public NooitAltijdAanUitEnum Meeverlengen
        {
            get => _faseCyclus.Meeverlengen;
            set
            {
                _faseCyclus.Meeverlengen = value;
                RaisePropertyChanged<object>(nameof(Meeverlengen), broadcast: true);
            }
        }

        public MeeVerlengenTypeEnum MeeverlengenType
        {
            get => _faseCyclus.MeeverlengenType;
            set
            {
                _faseCyclus.MeeverlengenType = value;
                RaisePropertyChanged<object>(nameof(MeeverlengenType), broadcast: true);
            }
        }

        public int? MeeverlengenVerschil
        {
            get => _faseCyclus.MeeverlengenVerschil;
            set
            {
                _faseCyclus.MeeverlengenVerschil = value;
                RaisePropertyChanged<object>(nameof(MeeverlengenVerschil), broadcast: true);
            }
        }

        public bool UitgesteldeVasteAanvraag
        {
            get => _faseCyclus.UitgesteldeVasteAanvraag;
            set
            {
                _faseCyclus.UitgesteldeVasteAanvraag = value;
                RaisePropertyChanged<object>(nameof(UitgesteldeVasteAanvraag), broadcast: true);
            }
        }

        public int UitgesteldeVasteAanvraagTijdsDuur
        {
            get => _faseCyclus.UitgesteldeVasteAanvraagTijdsduur;
            set
            {
                _faseCyclus.UitgesteldeVasteAanvraagTijdsduur = value;
                RaisePropertyChanged<object>(nameof(UitgesteldeVasteAanvraagTijdsDuur), broadcast: true);
            }
        }

        public bool UitgesteldeVasteAanvraagPossible => VasteAanvraag != NooitAltijdAanUitEnum.Nooit;

        public bool HiaatKoplusBijDetectieStoring
        {
            get => _faseCyclus.HiaatKoplusBijDetectieStoring;
            set
            {
                _faseCyclus.HiaatKoplusBijDetectieStoring = value;
                if (value && !VervangendHiaatKoplus.HasValue)
                {
                    VervangendHiaatKoplus = 25;
                }
                RaisePropertyChanged<object>(nameof(HiaatKoplusBijDetectieStoring), broadcast: true);
            }
        }

        public bool HasHiaatKoplusBijDetectieStoring => Type != FaseTypeEnum.Voetganger && Type != FaseTypeEnum.Fiets;

        public bool AanvraagBijDetectieStoring
        {
            get => _faseCyclus.AanvraagBijDetectieStoring;
            set
            {
                _faseCyclus.AanvraagBijDetectieStoring = value;
                RaisePropertyChanged<object>(nameof(AanvraagBijDetectieStoring), broadcast: true);
            }
        }

        public bool PercentageGroenBijDetectieStoring
        {
            get => _faseCyclus.PercentageGroenBijDetectieStoring;
            set
            {
                _faseCyclus.PercentageGroenBijDetectieStoring = value;
                if (!PercentageGroenBijStoring.HasValue)
                {
                    PercentageGroenBijStoring = 65;
                }
                RaisePropertyChanged<object>(nameof(PercentageGroenBijDetectieStoring), broadcast: true);
            }
        }

        public bool HasPercentageGroenBijDetectieStoring => Type != FaseTypeEnum.Voetganger;

        public int? VervangendHiaatKoplus
        {
            get => _faseCyclus.VervangendHiaatKoplus;
            set
            {
                _faseCyclus.VervangendHiaatKoplus = value;
                RaisePropertyChanged<object>(nameof(VervangendHiaatKoplus), broadcast: true);
            }
        }

        public int? PercentageGroenBijStoring
        {
            get => _faseCyclus.PercentageGroen;
            set
            {
                _faseCyclus.PercentageGroen = value;
                RaisePropertyChanged<object>(nameof(PercentageGroenBijStoring), broadcast: true);
            }
        }


        public int VeiligheidsGroenMinMG
        {
            get => _faseCyclus.VeiligheidsGroenMinMG;
            set
            {
                _faseCyclus.VeiligheidsGroenMinMG = value;
                RaisePropertyChanged<object>(nameof(VeiligheidsGroenMinMG), broadcast: true);
            }
        }

        public int VeiligheidsGroenTijdsduur
        {
            get => _faseCyclus.VeiligheidsGroenTijdsduur;
            set
            {
                _faseCyclus.VeiligheidsGroenTijdsduur = value;
                RaisePropertyChanged<object>(nameof(VeiligheidsGroenTijdsduur), broadcast: true);
            }
        }


        public int DetectorCount => _faseCyclus.Detectoren.Count;

        public bool HasKopmax
        {
            get
            {
                return _faseCyclus.Detectoren.Any(dvm => dvm.Verlengen != DetectorVerlengenTypeEnum.Geen);
            }
        }

        public bool HasVeiligheidsGroen
        {
            get
            {
                return _faseCyclus.Detectoren.Any(dvm => dvm.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit);
            }
        }

#warning This seems obsolete?
        public bool OVIngreep
        {
            get => _faseCyclus.OVIngreep;
            set
            {
                _faseCyclus.OVIngreep = value;
                RaisePropertyChanged<object>(nameof(OVIngreep), broadcast: true);
            }
        }

        public bool HDIngreep
        {
            get => _faseCyclus.HDIngreep;
            set
            {
                _faseCyclus.HDIngreep = value;
                RaisePropertyChanged<object>(nameof(HDIngreep), broadcast: true);
            }
        }

        public bool WachttijdVoorspeller
        {
            get => _faseCyclus.WachttijdVoorspeller;
            set
            {
                _faseCyclus.WachttijdVoorspeller = value;
                RaisePropertyChanged<object>(nameof(WachttijdVoorspeller), broadcast: true);
            }
        }

        private ObservableCollectionAroundList<HardMeeverlengenFaseCyclusViewModel, HardMeeverlengenFaseCyclusModel> _hardMeeverlengenFaseCycli;
        public ObservableCollectionAroundList<HardMeeverlengenFaseCyclusViewModel, HardMeeverlengenFaseCyclusModel> HardMeeverlengenFaseCycli
        {
            get => _hardMeeverlengenFaseCycli ?? (_hardMeeverlengenFaseCycli = new ObservableCollectionAroundList<HardMeeverlengenFaseCyclusViewModel, HardMeeverlengenFaseCyclusModel>(FaseCyclus.HardMeeverlengenFaseCycli));
        }

        public HardMeeverlengenFaseCyclusViewModel SelectedHardMeeverlengenFase
        {
            get => _selectedHardMeeverlengenFase;
            set
            {
                _selectedHardMeeverlengenFase = value;
                HardMeeverlengenFasenManager.SelectedItem = value;
                RaisePropertyChanged();
            }
        }

        public ItemsManagerViewModel<HardMeeverlengenFaseCyclusViewModel, string> HardMeeverlengenFasenManager
        {
            get => _hardMeeverlengenFasenManager ?? (
                _hardMeeverlengenFasenManager = new ItemsManagerViewModel<HardMeeverlengenFaseCyclusViewModel, string>(
                    HardMeeverlengenFaseCycli,
                    _controllerFasen,
                    (e) => { return new HardMeeverlengenFaseCyclusViewModel(new HardMeeverlengenFaseCyclusModel() { FaseCyclus = e }); },
                    (e) =>
                    {
                        return e != Naam &&
                               !TLCGenControllerChecker.IsFasenConflicting(DataAccess.TLCGenControllerDataProvider.Default.Controller, Naam, e) &&
                               HardMeeverlengenFaseCycli.All(x => x.FaseCyclus != e);
                    },
                    null,
                    () =>
                    {
                        RaisePropertyChanged<object>(broadcast: true);
                        RaisePropertyChanged("HasHardMeeverlengenFasen");
                    },
                    () =>
                    {
                        RaisePropertyChanged<object>(broadcast: true);
                        RaisePropertyChanged("HasHardMeeverlengenFasen");
                    }));
        }

        public bool HasHardMeeverlengenFasen => HardMeeverlengenFaseCycli.Any();

        public ObservableCollection<string> MeeverlengenOpties => _meeverlengenOpties ?? (_meeverlengenOpties = new ObservableCollection<string>());

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
                else
                {
                    throw new ArgumentOutOfRangeException("MeeverlengenTypeString",
                        "MeeverlengenTypeString was set to value that is not defined for MeeverlengenType");
                }

                RaisePropertyChanged("MeeverlengenTypeString");
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
            else
            {
                string myName = Naam;
                string hisName = fcvm.Naam;
                if (myName.Length < hisName.Length) myName = myName.PadLeft(hisName.Length, '0');
                else if (hisName.Length < myName.Length) hisName = hisName.PadLeft(myName.Length, '0');
                return myName.CompareTo(hisName);
            }
        }

        #endregion // IComparable

        #region Public methods

        public void UpdateHasKopmax()
        {
            RaisePropertyChanged("HasKopmax");
        }

        public void UpdateHasVeiligheidsGroen()
        {
            RaisePropertyChanged("HasVeiligheidsGroen");
        }

        #endregion // Public methods

        #region Private Methods

        private void SetMeeverlengenOpties()
        {
            MeeverlengenOpties.Clear();
            MeeverlengenOpties.Add(MeeVerlengenTypeEnum.Default.GetDescription());
            MeeverlengenOpties.Add(MeeVerlengenTypeEnum.To.GetDescription());
            MeeverlengenOpties.Add(MeeVerlengenTypeEnum.MKTo.GetDescription());
            if (_faseCyclus.Type == FaseTypeEnum.Voetganger)
                MeeverlengenOpties.Add(MeeVerlengenTypeEnum.Voetganger.GetDescription());

            _meeverlengenTypeString = MeeverlengenType.GetDescription();
            RaisePropertyChanged("MeeverlengenTypeString");
        }

        #endregion // Private Methods

        #region TLCGen events

        private void OnFasenChanged(FasenChangedMessage msg)
        {
            _hardMeeverlengenFasenManager = null;
            _controllerFasen = new List<string>();
            foreach (var fc in TLCGenControllerDataProvider.Default.Controller.Fasen)
            {
                _controllerFasen.Add(fc.Naam);
            }
            RaisePropertyChanged(nameof(HardMeeverlengenFasenManager));
        }

        #endregion // TLCGen events

        #region Constructor

        public FaseCyclusViewModel(FaseCyclusModel fasecyclus)
        {
            _faseCyclus = fasecyclus;

            SetMeeverlengenOpties();

            _controllerFasen = new List<string>();
            foreach (var fc in TLCGenControllerDataProvider.Default.Controller.Fasen)
            {
                _controllerFasen.Add(fc.Naam);
            }
        }

        #endregion // Constructor
    }
}
