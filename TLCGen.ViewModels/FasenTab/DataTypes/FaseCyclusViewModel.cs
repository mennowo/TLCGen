using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                OnMonitoredPropertyChanged(null); // Update all properties
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
                    //SettingsProvider.Default.ApplyDefaultFaseCyclusSettings(this.FaseCyclus, this.Type);

                    // Set default maxgroentijd
#warning TODO
                    //foreach (MaxGroentijdenSetViewModel mgsvm in _Controller.MaxGroentijdenSets)
                    //{
                    //    foreach (MaxGroentijdViewModel mgvm in mgsvm.MaxGroentijdenSetList)
                    //    {
                    //        if (mgvm.FaseCyclus == this.Define)
                    //            mgvm.Waarde = Settings.Utilities.FaseCyclusUtilities.GetFaseDefaultMaxGroenTijd(value);
                    //    }
                    //}

                    OnMonitoredPropertyChanged("Type");
                    if(value != FaseTypeEnum.Voetganger && MeeverlengenType == MeeVerlengenTypeEnum.Voetganger)
                    {
                        MeeverlengenType = MeeVerlengenTypeEnum.Default;
                    }
                    SetMeeverlengenOpties();

                    OnMonitoredPropertyChanged(null); // Update all properties
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
                OnMonitoredPropertyChanged("TFG");
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
                OnMonitoredPropertyChanged("TGG");
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
                OnMonitoredPropertyChanged("TGG_min");
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
                OnMonitoredPropertyChanged("TRG");
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
                OnMonitoredPropertyChanged("TRG_min");
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
                OnMonitoredPropertyChanged("TGL");
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
                OnMonitoredPropertyChanged("TGL_min");
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
                OnMonitoredPropertyChanged("Kopmax");
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
                OnMonitoredPropertyChanged("AantalRijstroken");
            }
        }

        public NooitAltijdAanUitEnum VasteAanvraag
        {
            get { return _FaseCyclus.VasteAanvraag; }
            set
            {
                _FaseCyclus.VasteAanvraag = value;
                OnMonitoredPropertyChanged("VasteAanvraag");
            }
        }

        public NooitAltijdAanUitEnum Wachtgroen
        {
            get { return _FaseCyclus.Wachtgroen; }
            set
            {
                _FaseCyclus.Wachtgroen = value;
                OnMonitoredPropertyChanged("Wachtgroen");
            }
        }

        public NooitAltijdAanUitEnum Meeverlengen
        {
            get { return _FaseCyclus.Meeverlengen; }
            set
            {
                _FaseCyclus.Meeverlengen = value;
                OnMonitoredPropertyChanged("Meeverlengen");
            }
        }

        public MeeVerlengenTypeEnum MeeverlengenType
        {
            get { return _FaseCyclus.MeeverlengenType; }
            set
            {
                _FaseCyclus.MeeverlengenType = value;
                OnMonitoredPropertyChanged("MeeverlengenType");
            }
        }

        public int? MeeverlengenVerschil
        {
            get { return _FaseCyclus.MeeverlengenVerschil; }
            set
            {
                _FaseCyclus.MeeverlengenVerschil = value;
                OnMonitoredPropertyChanged("MeeverlengenVerschil");
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
                OnMonitoredPropertyChanged("HiaatKoplusBijDetectieStoring");
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
                OnMonitoredPropertyChanged("AanvraagBijDetectieStoring");
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
                OnMonitoredPropertyChanged("PercentageGroenBijDetectieStoring");
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
                OnMonitoredPropertyChanged("VervangendHiaatKoplus");
            }
        }

        public int? PercentageGroenBijStoring
        {
            get { return _FaseCyclus.PercentageGroen; }
            set
            {
                _FaseCyclus.PercentageGroen = value;
                OnMonitoredPropertyChanged("PercentageGroenBijStoring");
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
                    if (dvm.Type == DetectorTypeEnum.Kop)
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
                OnMonitoredPropertyChanged("OVIngreep");
            }
        }

        public bool HDIngreep
        {
            get { return _FaseCyclus.HDIngreep; }
            set
            {
                _FaseCyclus.HDIngreep = value;
                OnMonitoredPropertyChanged("HDIngreep");
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

                OnPropertyChanged("MeeverlengenTypeString");
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
            OnPropertyChanged("HasKopmax");
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
            OnPropertyChanged("MeeverlengenTypeString");
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
