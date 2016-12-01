using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.ViewModels.Enums;

namespace TLCGen.ViewModels
{
    public class SynchronisatieViewModel : ViewModelBase
    {
        #region Fields

        // Model elements whose data is displayed
        private ConflictModel _Conflict;
        private NaloopModel _Naloop;
        private GelijkstartModel _Gelijkstart;
        private VoorstartModel _Voorstart;
        private MeeaanvraagModel _Meeaanvraag;

        private bool _HasConflict;
        private bool _HasGarantieConflict;
        private bool _HasGelijkstart;
        private bool _HasNaloop;
        private bool _HasVoorstart;
        private bool _HasMeeaanvraag;

        private string _FaseVan;
        private string _FaseNaar;

        private SynchronisatieTypeEnum _DisplayType;
        private int _SynchronisationValue;

        private NaloopViewModel _NaloopVM;
        private MeeaanvraagViewModel _MeeaanvraagVM;

        private Dictionary<SynchronisatieTypeEnum, IInterSignaalGroepElement> _InterSignaalGroepElements;

        #endregion // Fields

        #region Properties

        public NaloopViewModel NaloopVM
        {
            get
            {
                if(_NaloopVM == null)
                    _NaloopVM = new NaloopViewModel(_Naloop);
                return _NaloopVM;
            }
        }

        public MeeaanvraagViewModel MeeaanvraagVM
        {
            get
            {
                if (_MeeaanvraagVM == null)
                    _MeeaanvraagVM = new MeeaanvraagViewModel(_Meeaanvraag);
                return _MeeaanvraagVM;
            }
        }

        public ConflictModel Conflict
        {
            get { return _Conflict; }
            set
            {
                _Conflict = value;
                HasConflict = true;
            }
        }

        public NaloopModel Naloop
        {
            get { return _Naloop; }
            set
            {
                _Naloop = value;
                HasNaloop = true;
            }
        }

        public GelijkstartModel Gelijkstart
        {
            get { return _Gelijkstart; }
            set
            {
                _Gelijkstart = value;
                HasGelijkstart = true;
            }
        }

        public VoorstartModel Voorstart
        {
            get { return _Voorstart; }
            set
            {
                _Voorstart = value;
                HasVoorstart = true;
            }
        }

        public MeeaanvraagModel Meeaanvraag
        {
            get { return _Meeaanvraag; }
            set
            {
                _Meeaanvraag = value;
                _HasMeeaanvraag = true;
            }
        }

        public SynchronisatieTypeEnum DisplayType
        {
            get { return _DisplayType; }
            set
            {
                _DisplayType = value;
                OnPropertyChanged("DisplayType");
                OnPropertyChanged("ConflictValue");
                OnPropertyChanged("IsCoupled");
                OnPropertyChanged("HasNoCoupling");
                OnPropertyChanged("IsEnabled");
            }
        }

        public int SynchronisationValue
        {
            get { return _SynchronisationValue; }
            set
            {
                _SynchronisationValue = value;
                OnPropertyChanged("SynchronisationValue");
            }
        }

        public string FaseVan
        {
            get
            {
                return _FaseVan;
            }
            set
            {
                _FaseVan = value;
                foreach (var elem in _InterSignaalGroepElements)
                {
                    elem.Value.FaseVan = value;
                }
                OnPropertyChanged("FaseVan");
            }
        }
        
        public string FaseNaar
        {
            get
            {
                return _FaseNaar;
            }
            set
            {
                _FaseNaar = value;
                foreach (var elem in _InterSignaalGroepElements)
                {
                    elem.Value.FaseNaar = value;
                }
                OnPropertyChanged("FaseNaar");
            }
        }

        public string ConflictValueNoMessaging
        {
            set
            {
                if(IsCoupled || ReferencesSelf)
                {
                    throw new NotImplementedException();
                }
                switch (DisplayType)
                {
                    case SynchronisatieTypeEnum.Conflict:
                        SetConflictValue(value, false);
                        break;
                    case SynchronisatieTypeEnum.GarantieConflict:
                        SetGarantieConflictValue(value, false);
                        break;
                    default:
                        return;
                }
                OnMonitoredPropertyChanged("ConflictValue");
                OnPropertyChanged("HasConflict");
                OnPropertyChanged("IsEnabled");
            }
        }

        public string ConflictValue
        {
            get
            {
                switch (DisplayType)
                {
                    case SynchronisatieTypeEnum.Conflict:
                        return GetConflictValue();
                    case SynchronisatieTypeEnum.GarantieConflict:
                        return GetGarantieConflictValue();
                    default:
                        return "";
                }
            }
            set
            {
                if(IsCoupled || ReferencesSelf || !IsEnabled)
                {
                    throw new NotImplementedException();
                }
                switch (DisplayType)
                {
                    case SynchronisatieTypeEnum.Conflict:
                        SetConflictValue(value, true);
                        break;
                    case SynchronisatieTypeEnum.GarantieConflict:
                        SetGarantieConflictValue(value, true);
                        break;
                    default:
                        return;
                }
                OnMonitoredPropertyChanged("ConflictValue");
                OnPropertyChanged("HasConflict");
                OnPropertyChanged("IsEnabled");
            }
        }

        public bool IsCoupledNoMessaging
        {
            set
            {
                if (HasConflict)
                {
                    throw new NotImplementedException();
                }
                switch (DisplayType)
                {
                    case SynchronisatieTypeEnum.Gelijkstart:
                        HasGelijkstart = value;
                        break;
                    case SynchronisatieTypeEnum.Voorstart:
                        HasVoorstart = value;
                        break;
                    case SynchronisatieTypeEnum.Naloop:
                        HasNaloop = value;
                        break;
                    case SynchronisatieTypeEnum.Meeaanvraag:
                        HasMeeaanvraag = value;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                OnMonitoredPropertyChanged("IsCoupled");
                OnPropertyChanged("HasNoCoupling");
            }
        }

        public bool IsCoupled
        {
            get
            {
                switch (DisplayType)
                {
                    case SynchronisatieTypeEnum.Gelijkstart:
                        return HasGelijkstart;
                    case SynchronisatieTypeEnum.Voorstart:
                        return HasVoorstart;
                    case SynchronisatieTypeEnum.Naloop:
                        return HasNaloop;
                    case SynchronisatieTypeEnum.Meeaanvraag:
                        return HasMeeaanvraag;
                    default:
                        return HasGelijkstart || HasVoorstart || HasNaloop || HasMeeaanvraag;
                }
            }
            set
            {
                if(HasConflict || ReferencesSelf || !IsEnabled)
                {
                    throw new NotImplementedException();
                }
                switch (DisplayType)
                {
                    case SynchronisatieTypeEnum.Gelijkstart:
                        HasGelijkstart = value;
                        MessageManager.Instance.Send(new InterSignaalGroepChangedMessage(this.FaseVan, this.FaseNaar, this.Gelijkstart, value));
                        break;
                    case SynchronisatieTypeEnum.Voorstart:
                        HasVoorstart = value;
                        MessageManager.Instance.Send(new InterSignaalGroepChangedMessage(this.FaseVan, this.FaseNaar, this.Voorstart, value));
                        break;
                    case SynchronisatieTypeEnum.Naloop:
                        HasNaloop = value;
                        MessageManager.Instance.Send(new InterSignaalGroepChangedMessage(this.FaseVan, this.FaseNaar, this.Naloop, value));
                        break;
                    case SynchronisatieTypeEnum.Meeaanvraag:
                        HasMeeaanvraag = value;
                        MessageManager.Instance.Send(new InterSignaalGroepChangedMessage(this.FaseVan, this.FaseNaar, this.Naloop, value));
                        break;
                    default:
                        throw new NotImplementedException();
                }
                OnMonitoredPropertyChanged("IsCoupled");
                OnPropertyChanged("HasNoCoupling");
            }
        }

        public bool HasConflict
        {
            get { return _HasConflict; }
            set
            {
                _HasConflict = value;
                OnPropertyChanged("HasConflict");
                OnPropertyChanged("IsEnabled");
            }
        }

        public bool HasGarantieConflict
        {
            get { return _HasGarantieConflict; }
            set
            {
                _HasGarantieConflict = value;
                OnPropertyChanged("HasGarantieConflict");
            }
        }

        public bool HasGelijkstart
        {
            get { return _HasGelijkstart; }
            set
            {
                _HasGelijkstart = value;
                OnPropertyChanged("HasGelijkstart");
                OnPropertyChanged("IsCoupled");
            }
        }

        public bool HasNaloop
        {
            get { return _HasNaloop; }
            set
            {
                _HasNaloop = value;
                OnPropertyChanged("HasNaloop");
                OnPropertyChanged("IsCoupled");
            }
        }

        public bool HasVoorstart
        {
            get { return _HasVoorstart; }
            set
            {
                _HasVoorstart = value;
                OnPropertyChanged("HasVoorstart");
                OnPropertyChanged("IsCoupled");
            }
        }

        public bool HasMeeaanvraag
        {
            get { return _HasMeeaanvraag; }
            set
            {
                _HasMeeaanvraag = value;
                OnPropertyChanged("HasMeeaanvraag");
                OnPropertyChanged("IsCoupled");
            }
        }

        private bool _HasOppositeVoorstart;
        public bool HasOppositeVoorstart
        {
            get { return _HasOppositeVoorstart; }
            set
            {
                _HasOppositeVoorstart = value;
                OnPropertyChanged("HasOppositeVoorstart");
                OnPropertyChanged("IsEnabled");
            }
        }

        private bool _HasOppositeNaloop;
        public bool HasOppositeNaloop
        {
            get { return _HasOppositeNaloop; }
            set
            {
                _HasOppositeNaloop = value;
                OnPropertyChanged("HasOppositeNaloop");
                OnPropertyChanged("IsEnabled");
            }
        }

        private bool _HasOppositeMeeaanvraag;
        public bool HasOppositeMeeaanvraag
        {
            get { return _HasOppositeMeeaanvraag; }
            set
            {
                _HasOppositeMeeaanvraag = value;
                OnPropertyChanged("HasOppositeMeeaanvraag");
                OnPropertyChanged("IsEnabled");
            }
        }

        public bool IsEnabled
        {
            get
            {
                if(ReferencesSelf)
                {
                    return false;
                }
                switch(DisplayType)
                {
                    case SynchronisatieTypeEnum.Conflict:
                    case SynchronisatieTypeEnum.GarantieConflict:
                        return HasNoCoupling && !HasOppositeVoorstart && !HasOppositeNaloop && !HasOppositeMeeaanvraag;
                    case SynchronisatieTypeEnum.Gelijkstart:
                        return !HasConflict && !HasGarantieConflict && !HasVoorstart && !HasOppositeVoorstart;
                    case SynchronisatieTypeEnum.Voorstart:
                        return !HasConflict && !HasGarantieConflict && !HasGelijkstart && !HasOppositeVoorstart;
                    case SynchronisatieTypeEnum.Naloop:
                    case SynchronisatieTypeEnum.Meeaanvraag:
                        return !HasConflict && !HasGarantieConflict;
                }
                return false;
            }
        }

        public bool HasNoCoupling
        {
            get { return !(HasVoorstart || HasGelijkstart || HasNaloop || HasOppositeVoorstart || HasMeeaanvraag); }
        }

        public bool ReferencesSelf
        {
            get; private set;
        }

        public bool NotReferencingSelf
        {
            get { return !ReferencesSelf; }
        }

        public bool AllowCoupling
        {
            get { return !ReferencesSelf && !HasConflict && !HasGarantieConflict; }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        public string GetConflictValue()
        {
            if(_Conflict.GarantieWaarde != null && _Conflict.GarantieWaarde >= 0)
            {
                if(_Conflict.Waarde == -1)
                {
                    return "*";
                }
            }
            switch (_Conflict.Waarde)
            {
                case -1:
                    return "";
                case -2:
                    return "FK";
                case -3:
                    return "GK";
                case -4:
                    return "GKL";
                case -5:
                    return "";
                case -6:
                    return "*";
                default:
                    return _Conflict.Waarde.ToString();
            }
        }

        private void SetConflictValue(string value, bool sendmessage)
        {
            HasConflict = true;
            switch (value)
            {
                case "FK":
                    _Conflict.Waarde = -2;
                    break;
                case "GK":
                    _Conflict.Waarde = -3;
                    break;
                case "GKL":
                    _Conflict.Waarde = -4;
                    break;
                case "*":
                    _Conflict.Waarde = -6;
                    break;
                default:
                    int confval;
                    if (Int32.TryParse(value, out confval))
                    {
                        if (_Conflict.GarantieWaarde != null && _Conflict.GarantieWaarde >= 0)
                        {
                            if (confval >= _Conflict.GarantieWaarde)
                                _Conflict.Waarde = confval;
                            else
                                _Conflict.Waarde = (int)_Conflict.GarantieWaarde;
                        }
                        else
                        {
                            _Conflict.Waarde = confval;
                        }
                    }
                    // Ignore false data
                    else
                    {
                        _Conflict.Waarde = -1;
                        HasConflict = false;
                    }
                    break;
            }

            if (sendmessage)
            {
                MessageManager.Instance.Send(new InterSignaalGroepChangedMessage(this.FaseVan, this.FaseNaar, this.Conflict));
            }
        }

        public string GetGarantieConflictValue()
        {
            if(_Conflict.Waarde >= 0)
            {
                switch (_Conflict.GarantieWaarde)
                {
                    case null:
                    case -1:
                        return "*";
                    case -5:
                        return "X";
                    case -6:
                        return "*";
                    default:
                        return _Conflict.GarantieWaarde.ToString();
                }
            }
            else if (_Conflict.GarantieWaarde != -1)
            {
                switch (_Conflict.GarantieWaarde)
                {
                    case -1:
                        return "";
                    case -5:
                        return "X";
                    case -6:
                        return "*";
                    default:
                        return _Conflict.GarantieWaarde.ToString();
                }
            }
            else
            {
                return "";
            }
        }

        private void SetGarantieConflictValue(string value, bool sendmessage)
        {
            int confval;
            HasGarantieConflict = true;

            switch (value)
            {
                case "*":
                    _Conflict.GarantieWaarde = -6;
                    break;
                default:
                    if (Int32.TryParse(value, out confval))
                    {
                        _Conflict.GarantieWaarde = confval;
                    }
                    // Ignore false data
                    else
                    {
                        _Conflict.GarantieWaarde = -1;
                        HasGarantieConflict = false;
                    }
                    break;
            }

            if(sendmessage)
            {
                MessageManager.Instance.Send(new InterSignaalGroepChangedMessage(this.FaseVan, this.FaseNaar, this.Conflict));
            }

        }

        private void SetSynchValue<T>(string property, T val)
        {
            PropertyInfo propertyInfo;
            switch (DisplayType)
            {
                case SynchronisatieTypeEnum.Conflict:
                case SynchronisatieTypeEnum.GarantieConflict:
                    propertyInfo = _Conflict.GetType().GetProperty(property);
                    propertyInfo.SetValue(_Conflict, val);
                    break;
                case SynchronisatieTypeEnum.Gelijkstart:
                    propertyInfo = _Gelijkstart.GetType().GetProperty(property);
                    propertyInfo.SetValue(_Conflict, val);
                    break;
                case SynchronisatieTypeEnum.Naloop:
                    propertyInfo = _Naloop.GetType().GetProperty(property);
                    propertyInfo.SetValue(_Conflict, val);
                    break;
                case SynchronisatieTypeEnum.Voorstart:
                    propertyInfo = _Voorstart.GetType().GetProperty(property);
                    propertyInfo.SetValue(_Conflict, val);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private T GetSynchValue<T>(string property)
        {
            PropertyInfo propertyInfo;
            T returnval;
            switch (DisplayType)
            {
                case SynchronisatieTypeEnum.Conflict:
                case SynchronisatieTypeEnum.GarantieConflict:
                    propertyInfo = _Conflict.GetType().GetProperty(property);
                    returnval = (T)propertyInfo.GetValue(_Conflict);
                    break;
                case SynchronisatieTypeEnum.Gelijkstart:
                    propertyInfo = _Gelijkstart.GetType().GetProperty(property);
                    returnval = (T)propertyInfo.GetValue(_Gelijkstart);
                    break;
                case SynchronisatieTypeEnum.Naloop:
                    propertyInfo = _Naloop.GetType().GetProperty(property);
                    returnval = (T)propertyInfo.GetValue(_Naloop);
                    break;
                case SynchronisatieTypeEnum.Voorstart:
                    propertyInfo = _Voorstart.GetType().GetProperty(property);
                    returnval = (T)propertyInfo.GetValue(_Voorstart);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return returnval;
        }

        #endregion // Private methods

        #region Public methods

        public bool IsOK()
        {
            if ((HasConflict || HasGarantieConflict) && (HasVoorstart || HasNaloop || HasGelijkstart || HasMeeaanvraag))
                return false;
            if (HasVoorstart && HasGelijkstart)
                return false;
            return true;
        }

        #endregion // Public methods

        #region Constructor

        public SynchronisatieViewModel(bool referencetoself = false)
        {
            _Conflict = new ConflictModel();
            _Gelijkstart = new GelijkstartModel();
            _Voorstart = new VoorstartModel();
            _Naloop = new NaloopModel();
            _Meeaanvraag = new MeeaanvraagModel();

            _InterSignaalGroepElements = new Dictionary<SynchronisatieTypeEnum, IInterSignaalGroepElement>();
            _InterSignaalGroepElements.Add(SynchronisatieTypeEnum.Conflict, _Conflict as IInterSignaalGroepElement);
            _InterSignaalGroepElements.Add(SynchronisatieTypeEnum.GarantieConflict, _Conflict as IInterSignaalGroepElement);
            _InterSignaalGroepElements.Add(SynchronisatieTypeEnum.Gelijkstart, _Gelijkstart as IInterSignaalGroepElement);
            _InterSignaalGroepElements.Add(SynchronisatieTypeEnum.Voorstart, _Voorstart as IInterSignaalGroepElement);
            _InterSignaalGroepElements.Add(SynchronisatieTypeEnum.Naloop, _Naloop as IInterSignaalGroepElement);
            _InterSignaalGroepElements.Add(SynchronisatieTypeEnum.Meeaanvraag, _Meeaanvraag as IInterSignaalGroepElement);

            ReferencesSelf = referencetoself;
            if (ReferencesSelf) _Conflict.Waarde = -5;
            else _Conflict.Waarde = -1;
        }

        #endregion // Constructor
    }
}
