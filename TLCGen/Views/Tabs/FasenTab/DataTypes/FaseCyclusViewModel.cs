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

        public FaseCyclusModel FaseCyclus
        {
            get { return _FaseCyclus; }
        }

        public string Naam
        {
            get { return _FaseCyclus.Naam; }
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
                                d.Naam = nd;
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
                RaisePropertyChanged<object>(null, broadcast: true); // Update all properties
            }
        }

        public FaseTypeEnum Type
        {
            get { return _FaseCyclus.Type; }
            set
            {
                if (_FaseCyclus.Type != value)
                {
                    _FaseCyclus.Type = value;

                    // Apply new defaults
                    DefaultsProvider.Default.SetDefaultsOnModel(this.FaseCyclus, this.Type.ToString());
                    
                    RaisePropertyChanged<object>("Type", broadcast: true);
                    if(value != FaseTypeEnum.Voetganger && MeeverlengenType == MeeVerlengenTypeEnum.Voetganger)
                    {
                        MeeverlengenType = MeeVerlengenTypeEnum.Default;
                    }
                    SetMeeverlengenOpties();

                    RaisePropertyChanged<object>(null, broadcast: true); // Update all properties
                }
            }
        }

        public int TFG
        {
            get { return _FaseCyclus.TFG; }
            set
            {
                if (value >= 0 && value >= TGG)
                    _FaseCyclus.TFG = value;
                else
                    _FaseCyclus.TFG = TGG;
                RaisePropertyChanged<object>("TFG", broadcast: true);
            }
        }

        public int TGG
        {
            get { return _FaseCyclus.TGG; }
            set
            {
                if (value >= 0 && value >= TGG_min)
                {
                    _FaseCyclus.TGG = value;
                    if (TFG < value)
                        TFG = value;
                }
                else if (value >= 0)
                    _FaseCyclus.TGG = TGG_min;
                RaisePropertyChanged<object>("TGG", broadcast: true);
            }
        }

        public int TGG_min
        {
            get { return _FaseCyclus.TGG_min; }
            set
            {
                if (value >= 0)
                {
                    _FaseCyclus.TGG_min = value;
                    if (TGG < value)
                        TGG = value;
                }
                RaisePropertyChanged<object>("TGG_min", broadcast: true);
            }
        }

        public int TRG
        {
            get { return _FaseCyclus.TRG; }
            set
            {
                if (value >= 0 && value >= TRG_min)
                {
                    _FaseCyclus.TRG = value;
                }
                else if (value >= 0)
                    _FaseCyclus.TGG = TRG_min;
                RaisePropertyChanged<object>("TRG", broadcast: true);
            }
        }

        public int TRG_min
        {
            get { return _FaseCyclus.TRG_min; }
            set
            {
                if (value >= 0)
                {
                    _FaseCyclus.TRG_min = value;
                    if (TRG < value)
                        TRG = value;
                }
                RaisePropertyChanged<object>("TRG_min", broadcast: true);
            }
        }

        public int TGL
        {
            get { return _FaseCyclus.TGL; }
            set
            {
                if (value >= 0 && value >= TGL_min)
                {
                    _FaseCyclus.TGL = value;
                }
                else if (value >= 0)
                    _FaseCyclus.TGG = TGL_min;
                RaisePropertyChanged<object>("TGL", broadcast: true);
            }
        }

        public int TGL_min
        {
            get { return _FaseCyclus.TGL_min; }
            set
            {
                if (value >= 0)
                {
                    _FaseCyclus.TGL_min = value;
                    if (TGL < value)
                        TGL = value;
                }
                RaisePropertyChanged<object>("TGL_min", broadcast: true);
            }
        }

        public int Kopmax
        {
            get { return _FaseCyclus.Kopmax; }
            set
            {
                if (value >= 0)
                {
                    _FaseCyclus.Kopmax = value;
                }
                RaisePropertyChanged<object>("Kopmax", broadcast: true);
            }
        }

        public int? AantalRijstroken
        {
            get { return _FaseCyclus.AantalRijstroken; }
            set
            {
                if (value >= 0)
                {
                    _FaseCyclus.AantalRijstroken = value;
                }
                RaisePropertyChanged<object>("AantalRijstroken", broadcast: true);
            }
        }

        public NooitAltijdAanUitEnum VasteAanvraag
        {
            get { return _FaseCyclus.VasteAanvraag; }
            set
            {
                _FaseCyclus.VasteAanvraag = value;
                RaisePropertyChanged<object>("VasteAanvraag", broadcast: true);
            }
        }

        public NooitAltijdAanUitEnum Wachtgroen
        {
            get { return _FaseCyclus.Wachtgroen; }
            set
            {
                _FaseCyclus.Wachtgroen = value;
                RaisePropertyChanged<object>("Wachtgroen", broadcast: true);
            }
        }

        public NooitAltijdAanUitEnum Meeverlengen
        {
            get { return _FaseCyclus.Meeverlengen; }
            set
            {
                _FaseCyclus.Meeverlengen = value;
                RaisePropertyChanged<object>("Meeverlengen", broadcast: true);
            }
        }

        public MeeVerlengenTypeEnum MeeverlengenType
        {
            get { return _FaseCyclus.MeeverlengenType; }
            set
            {
                _FaseCyclus.MeeverlengenType = value;
                RaisePropertyChanged<object>("MeeverlengenType", broadcast: true);
            }
        }

        public int? MeeverlengenVerschil
        {
            get { return _FaseCyclus.MeeverlengenVerschil; }
            set
            {
                _FaseCyclus.MeeverlengenVerschil = value;
                RaisePropertyChanged<object>("MeeverlengenVerschil", broadcast: true);
            }
        }

        public bool HiaatKoplusBijDetectieStoring
        {
            get { return _FaseCyclus.HiaatKoplusBijDetectieStoring; }
            set
            {
                _FaseCyclus.HiaatKoplusBijDetectieStoring = value;
                if(value && !VervangendHiaatKoplus.HasValue)
                {
                    VervangendHiaatKoplus = 25;
                }
                RaisePropertyChanged<object>("HiaatKoplusBijDetectieStoring", broadcast: true);
            }
        }

        public bool HasHiaatKoplusBijDetectieStoring
        {
            get
            {
                return Type != FaseTypeEnum.Voetganger && Type != FaseTypeEnum.Fiets;
            }
        }

        public bool AanvraagBijDetectieStoring
        {
            get { return _FaseCyclus.AanvraagBijDetectieStoring; }
            set
            {
                _FaseCyclus.AanvraagBijDetectieStoring = value;
                RaisePropertyChanged<object>("AanvraagBijDetectieStoring", broadcast: true);
            }
        }

        public bool PercentageGroenBijDetectieStoring
        {
            get { return _FaseCyclus.PercentageGroenBijDetectieStoring; }
            set
            {
                _FaseCyclus.PercentageGroenBijDetectieStoring = value;
                if(!PercentageGroenBijStoring.HasValue)
                {
                    PercentageGroenBijStoring = 65;
                }
                RaisePropertyChanged<object>("PercentageGroenBijDetectieStoring", broadcast: true);
            }
        }

        public bool HasPercentageGroenBijDetectieStoring
        {
            get
            {
                return Type != FaseTypeEnum.Voetganger;
            }
        }

        public int? VervangendHiaatKoplus
        {
            get { return _FaseCyclus.VervangendHiaatKoplus; }
            set
            {
                _FaseCyclus.VervangendHiaatKoplus = value;
                RaisePropertyChanged<object>("VervangendHiaatKoplus", broadcast: true);
            }
        }

        public int? PercentageGroenBijStoring
        {
            get { return _FaseCyclus.PercentageGroen; }
            set
            {
                _FaseCyclus.PercentageGroen = value;
                RaisePropertyChanged<object>("PercentageGroenBijStoring", broadcast: true);
            }
        }

        public int DetectorCount
        {
            get
            {
                return _FaseCyclus.Detectoren.Count;
            }
        }

        public bool HasKopmax
        {
            get
            {
                foreach(DetectorModel dvm in _FaseCyclus.Detectoren)
                {
                    if (dvm.Verlengen != DetectorVerlengenTypeEnum.Geen)
                        return true;
                }
                return false;
            }
        }

        public bool OVIngreep
        {
            get { return _FaseCyclus.OVIngreep; }
            set
            {
                _FaseCyclus.OVIngreep = value;
                RaisePropertyChanged<object>("OVIngreep", broadcast: true);
            }
        }

        public bool HDIngreep
        {
            get { return _FaseCyclus.HDIngreep; }
            set
            {
                _FaseCyclus.HDIngreep = value;
                RaisePropertyChanged<object>("HDIngreep", broadcast: true);
            }
        }

        public ObservableCollection<string> MeeverlengenOpties
        {
            get
            {
                if (_MeeverlengenOpties == null)
                {
                    _MeeverlengenOpties = new ObservableCollection<string>();
                }
                return _MeeverlengenOpties;
            }
        }

        public string MeeverlengenTypeString
        {
            get { return _MeeverlengenTypeString; }
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
            FaseCyclusViewModel fcvm = obj as FaseCyclusViewModel;
            if (fcvm == null)
                throw new NotImplementedException();
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
