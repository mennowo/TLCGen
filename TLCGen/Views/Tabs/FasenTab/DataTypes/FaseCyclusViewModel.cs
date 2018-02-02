using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TLCGen.Models.Enumerations;
using TLCGen.Models;
using TLCGen.DataAccess;
using TLCGen.Settings;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Helpers;
using TLCGen.Extensions;

namespace TLCGen.ViewModels
{
    public class FaseCyclusViewModel : ViewModelBase, IComparable
    {
        #region Fields

        private FaseCyclusModel _FaseCyclus;
        private ObservableCollection<string> _MeeverlengenOpties;
        private string _MeeverlengenTypeString;
        #endregion // Fields

        #region Properties

        public FaseCyclusModel FaseCyclus => _FaseCyclus;

        public string Naam
        {
            get => _FaseCyclus.Naam;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidName(value))
                {
                    var message = new IsElementIdentifierUniqueRequest(value, ElementIdentifierType.Naam);
                    Messenger.Default.Send(message);
                    if (message.Handled && message.IsUnique)
                    {
                        string oldname = _FaseCyclus.Naam;
                        _FaseCyclus.Naam = value;

                        // set new type
                        this.Type = Settings.Utilities.FaseCyclusUtilities.GetFaseTypeFromNaam(value);

                        foreach(var d in FaseCyclus.Detectoren)
                        {
                            string nd = d.Naam.Replace(oldname, value);
                            var _message = new IsElementIdentifierUniqueRequest(nd, ElementIdentifierType.Naam);
                            Messenger.Default.Send(_message);
                            if (_message.Handled && _message.IsUnique)
                            {
                                var oldD = d.Naam;
                                d.Naam = nd;
                                Messenger.Default.Send(new NameChangedMessage(oldD, d.Naam));
                            }
                            nd = d.VissimNaam.Replace(oldname, value);
                            _message = new IsElementIdentifierUniqueRequest(nd, ElementIdentifierType.VissimNaam);
                            Messenger.Default.Send(_message);
                            if (_message.Handled && _message.IsUnique)
                            {
                                d.VissimNaam = nd;
                            }
                        }

                        // Notify the messenger
                        Messenger.Default.Send(new NameChangedMessage(oldname, value));
                    }
                }
                RaisePropertyChanged(string.Empty); // Update all properties
                RaisePropertyChanged<object>(nameof(Naam), broadcast: true);
            }
        }

        public FaseTypeEnum Type
        {
            get => _FaseCyclus.Type;
            set
            {
                if (_FaseCyclus.Type != value)
                {
                    _FaseCyclus.Type = value;

                    // Apply new defaults
                    DefaultsProvider.Default.SetDefaultsOnModel(this.FaseCyclus, this.Type.ToString());
                    
                    if(value != FaseTypeEnum.Voetganger && MeeverlengenType == MeeVerlengenTypeEnum.Voetganger)
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
            get => _FaseCyclus.TFG;
            set
            {
                if (value >= 0 && value >= TGG)
                    _FaseCyclus.TFG = value;
                else
                    _FaseCyclus.TFG = TGG;
                RaisePropertyChanged<object>(nameof(TFG), broadcast: true);
            }
        }

        public int TGG
        {
            get => _FaseCyclus.TGG;
            set
            {
                if (value >= 0 && value >= TGG_min)
                {
                    _FaseCyclus.TGG = value;
                }
                else
                {
                    _FaseCyclus.TGG = TGG_min;
                }
                if (TFG < _FaseCyclus.TGG) TFG = _FaseCyclus.TGG;
                RaisePropertyChanged<object>(nameof(TGG), broadcast: true);
            }
        }

        public int TGG_min
        {
            get => _FaseCyclus.TGG_min;
            set
            {
                if (value >= 0)
                {
                    _FaseCyclus.TGG_min = value;
                    if (TGG < value)
                        TGG = value;
                }
                RaisePropertyChanged<object>(nameof(TGG_min), broadcast: true);
            }
        }

        public int TRG
        {
            get => _FaseCyclus.TRG;
            set
            {
                if (value >= 0 && value >= TRG_min)
                {
                    _FaseCyclus.TRG = value;
                }
                else
                    _FaseCyclus.TRG = TRG_min;
                RaisePropertyChanged<object>(nameof(TRG), broadcast: true);
            }
        }

        public int TRG_min
        {
            get => _FaseCyclus.TRG_min;
            set
            {
                if (value >= 0)
                {
                    _FaseCyclus.TRG_min = value;
                    if (TRG < value)
                        TRG = value;
                }
                RaisePropertyChanged<object>(nameof(TRG_min), broadcast: true);
            }
        }

        public int TGL
        {
            get => _FaseCyclus.TGL;
            set
            {
                if (value >= 0 && value >= TGL_min)
                {
                    _FaseCyclus.TGL = value;
                }
                else
                    _FaseCyclus.TGL = TGL_min;
                RaisePropertyChanged<object>(nameof(TGL), broadcast: true);
            }
        }

        public int TGL_min
        {
            get => _FaseCyclus.TGL_min;
            set
            {
                if (value >= 0)
                {
                    _FaseCyclus.TGL_min = value;
                    if (TGL < value)
                        TGL = value;
                }
                RaisePropertyChanged<object>(nameof(TGL_min), broadcast: true);
            }
        }

        public int Kopmax
        {
            get => _FaseCyclus.Kopmax;
            set
            {
                if (value >= 0)
                {
                    _FaseCyclus.Kopmax = value;
                }
                RaisePropertyChanged<object>(nameof(Kopmax), broadcast: true);
            }
        }

        public int? AantalRijstroken
        {
            get => _FaseCyclus.AantalRijstroken;
            set
            {
                if (value >= 0)
                {
                    _FaseCyclus.AantalRijstroken = value;
                }
                RaisePropertyChanged<object>(nameof(AantalRijstroken), broadcast: true);
            }
        }

        public NooitAltijdAanUitEnum VasteAanvraag
        {
            get => _FaseCyclus.VasteAanvraag;
            set
            {
                _FaseCyclus.VasteAanvraag = value;
                RaisePropertyChanged<object>(nameof(VasteAanvraag), broadcast: true);
                RaisePropertyChanged(nameof(UitgesteldeVasteAanvraagPossible));
            }
        }

        public NooitAltijdAanUitEnum Wachtgroen
        {
            get => _FaseCyclus.Wachtgroen;
            set
            {
                _FaseCyclus.Wachtgroen = value;
                RaisePropertyChanged<object>(nameof(Wachtgroen), broadcast: true);
            }
        }

        public NooitAltijdAanUitEnum Meeverlengen
        {
            get => _FaseCyclus.Meeverlengen;
            set
            {
                _FaseCyclus.Meeverlengen = value;
                RaisePropertyChanged<object>(nameof(Meeverlengen), broadcast: true);
            }
        }

        public MeeVerlengenTypeEnum MeeverlengenType
        {
            get => _FaseCyclus.MeeverlengenType;
            set
            {
                _FaseCyclus.MeeverlengenType = value;
                RaisePropertyChanged<object>(nameof(MeeverlengenType), broadcast: true);
            }
        }

        public int? MeeverlengenVerschil
        {
            get => _FaseCyclus.MeeverlengenVerschil;
            set
            {
                _FaseCyclus.MeeverlengenVerschil = value;
                RaisePropertyChanged<object>(nameof(MeeverlengenVerschil), broadcast: true);
            }
        }

        public bool UitgesteldeVasteAanvraag
        {
            get => _FaseCyclus.UitgesteldeVasteAanvraag;
            set
            {
                _FaseCyclus.UitgesteldeVasteAanvraag = value;
                RaisePropertyChanged<object>(nameof(UitgesteldeVasteAanvraag), broadcast: true);
            }
        }
        
        public int UitgesteldeVasteAanvraagTijdsDuur
        {
            get => _FaseCyclus.UitgesteldeVasteAanvraagTijdsduur;
            set
            {
                _FaseCyclus.UitgesteldeVasteAanvraagTijdsduur = value;
                RaisePropertyChanged<object>(nameof(UitgesteldeVasteAanvraagTijdsDuur), broadcast: true);
            }
        }

        public bool UitgesteldeVasteAanvraagPossible => VasteAanvraag != NooitAltijdAanUitEnum.Nooit;

        public bool HiaatKoplusBijDetectieStoring
        {
            get => _FaseCyclus.HiaatKoplusBijDetectieStoring;
            set
            {
                _FaseCyclus.HiaatKoplusBijDetectieStoring = value;
                if(value && !VervangendHiaatKoplus.HasValue)
                {
                    VervangendHiaatKoplus = 25;
                }
                RaisePropertyChanged<object>(nameof(HiaatKoplusBijDetectieStoring), broadcast: true);
            }
        }

        public bool HasHiaatKoplusBijDetectieStoring => Type != FaseTypeEnum.Voetganger && Type != FaseTypeEnum.Fiets;

        public bool AanvraagBijDetectieStoring
        {
            get => _FaseCyclus.AanvraagBijDetectieStoring;
            set
            {
                _FaseCyclus.AanvraagBijDetectieStoring = value;
                RaisePropertyChanged<object>(nameof(AanvraagBijDetectieStoring), broadcast: true);
            }
        }

        public bool PercentageGroenBijDetectieStoring
        {
            get => _FaseCyclus.PercentageGroenBijDetectieStoring;
            set
            {
                _FaseCyclus.PercentageGroenBijDetectieStoring = value;
                if(!PercentageGroenBijStoring.HasValue)
                {
                    PercentageGroenBijStoring = 65;
                }
                RaisePropertyChanged<object>(nameof(PercentageGroenBijDetectieStoring), broadcast: true);
            }
        }

        public bool HasPercentageGroenBijDetectieStoring => Type != FaseTypeEnum.Voetganger;

        public int? VervangendHiaatKoplus
        {
            get => _FaseCyclus.VervangendHiaatKoplus;
            set
            {
                _FaseCyclus.VervangendHiaatKoplus = value;
                RaisePropertyChanged<object>(nameof(VervangendHiaatKoplus), broadcast: true);
            }
        }

        public int? PercentageGroenBijStoring
        {
            get => _FaseCyclus.PercentageGroen;
            set
            {
                _FaseCyclus.PercentageGroen = value;
                RaisePropertyChanged<object>(nameof(PercentageGroenBijStoring), broadcast: true);
            }
        }


        public int VeiligheidsGroenMinMG
        {
            get => _FaseCyclus.VeiligheidsGroenMinMG;
            set
            {
                _FaseCyclus.VeiligheidsGroenMinMG = value;
                RaisePropertyChanged<object>(nameof(VeiligheidsGroenMinMG), broadcast: true);
            }
        }

        public int VeiligheidsGroenTijdsduur
        {
            get => _FaseCyclus.VeiligheidsGroenTijdsduur;
            set
            {
                _FaseCyclus.VeiligheidsGroenTijdsduur = value;
                RaisePropertyChanged<object>(nameof(VeiligheidsGroenTijdsduur), broadcast: true);
            }
        }


        public int DetectorCount => _FaseCyclus.Detectoren.Count;

        public bool HasKopmax
        {
            get
            {
                return _FaseCyclus.Detectoren.Any(dvm => dvm.Verlengen != DetectorVerlengenTypeEnum.Geen);
            }
        }

        public bool HasVeiligheidsGroen
        {
            get
            {
                return _FaseCyclus.Detectoren.Any(dvm => dvm.VeiligheidsGroen != NooitAltijdAanUitEnum.Nooit);
            }
        }

        public bool OVIngreep
        {
            get => _FaseCyclus.OVIngreep;
            set
            {
                _FaseCyclus.OVIngreep = value;
                RaisePropertyChanged<object>(nameof(OVIngreep), broadcast: true);
            }
        }

        public bool HDIngreep
        {
            get => _FaseCyclus.HDIngreep;
            set
            {
                _FaseCyclus.HDIngreep = value;
                RaisePropertyChanged<object>(nameof(HDIngreep), broadcast: true);
            }
        }

        public ObservableCollection<string> MeeverlengenOpties => _MeeverlengenOpties ?? (_MeeverlengenOpties = new ObservableCollection<string>());

        public string MeeverlengenTypeString
        {
            get => _MeeverlengenTypeString;
            set
            {
                _MeeverlengenTypeString = value;
                if(value == MeeVerlengenTypeEnum.Default.GetDescription())
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
            if (_FaseCyclus.Type == FaseTypeEnum.Voetganger)
                MeeverlengenOpties.Add(MeeVerlengenTypeEnum.Voetganger.GetDescription());

            _MeeverlengenTypeString = MeeverlengenType.GetDescription();
            RaisePropertyChanged("MeeverlengenTypeString");
        }
        #endregion // Private Methods

        #region Constructor

        public FaseCyclusViewModel(FaseCyclusModel fasecyclus)
        {
            _FaseCyclus = fasecyclus;

            SetMeeverlengenOpties();
        }

        #endregion // Constructor
    }
}
