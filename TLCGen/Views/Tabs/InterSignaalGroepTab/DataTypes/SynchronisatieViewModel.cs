using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
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
        private LateReleaseModel _LateRelease;

        private bool _HasConflict;
        private bool _HasGarantieConflict;
        private bool _HasGelijkstart;
        private bool _HasNaloop;
        private bool _HasVoorstart;
        private bool _HasMeeaanvraag;
        private bool _HasLateRelease;

        private string _FaseVan;
        private string _FaseNaar;

        private IntersignaalGroepTypeEnum _DisplayType;
        private int _SynchronisationValue;

        private NaloopViewModel _NaloopVM;
        private MeeaanvraagViewModel _MeeaanvraagVM;

        private Dictionary<IntersignaalGroepTypeEnum, IInterSignaalGroepElement> _InterSignaalGroepElements;

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
                HasConflict = _Conflict != null;
            }
        }

        public NaloopModel Naloop
        {
            get { return _Naloop; }
            set
            {
                _Naloop = value;
                HasNaloop = _Naloop != null;
            }
        }

        public GelijkstartModel Gelijkstart
        {
            get { return _Gelijkstart; }
            set
            {
                _Gelijkstart = value;
                HasGelijkstart = _Gelijkstart != null;
            }
        }

        public VoorstartModel Voorstart
        {
            get { return _Voorstart; }
            set
            {
                _Voorstart = value;
                HasVoorstart = _Voorstart != null;
            }
        }

        public MeeaanvraagModel Meeaanvraag
        {
            get { return _Meeaanvraag; }
            set
            {
                _Meeaanvraag = value;
                _HasMeeaanvraag = _Meeaanvraag != null;
            }
        }

        public LateReleaseModel LateRelease
        {
            get { return _LateRelease; }
            set
            {
                _LateRelease = value;
                _HasLateRelease = _LateRelease != null;
            }
        }

        public IntersignaalGroepTypeEnum DisplayType
        {
            get { return _DisplayType; }
            set
            {
                _DisplayType = value;
                RaisePropertyChanged("DisplayType");
                RaisePropertyChanged("ConflictValue");
                RaisePropertyChanged("IsCoupled");
                RaisePropertyChanged("HasNoCoupling");
                RaisePropertyChanged("IsEnabled");
            }
        }

        public int SynchronisationValue
        {
            get { return _SynchronisationValue; }
            set
            {
                _SynchronisationValue = value;
                RaisePropertyChanged("SynchronisationValue");
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
                RaisePropertyChanged("FaseVan");
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
                RaisePropertyChanged("FaseNaar");
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
                    case IntersignaalGroepTypeEnum.Conflict:
                        SetConflictValue(value, false);
                        break;
                    case IntersignaalGroepTypeEnum.GarantieConflict:
                        SetGarantieConflictValue(value, false);
                        break;
                    default:
                        return;
                }
                RaisePropertyChanged<object>("ConflictValue", broadcast: true);
                RaisePropertyChanged("HasConflict");
                RaisePropertyChanged("IsEnabled");
            }
        }

        public string ConflictValue
        {
            get
            {
                switch (DisplayType)
                {
                    case IntersignaalGroepTypeEnum.Conflict:
                        return GetConflictValue();
                    case IntersignaalGroepTypeEnum.GarantieConflict:
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
                    case IntersignaalGroepTypeEnum.Conflict:
                        SetConflictValue(value, true);
                        break;
                    case IntersignaalGroepTypeEnum.GarantieConflict:
                        SetGarantieConflictValue(value, true);
                        break;
                    default:
                        return;
                }
                RaisePropertyChanged<object>("ConflictValue", broadcast: true);
                RaisePropertyChanged("HasConflict");
                RaisePropertyChanged("IsEnabled");
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
                    case IntersignaalGroepTypeEnum.Gelijkstart:
                        HasGelijkstart = value;
                        break;
                    case IntersignaalGroepTypeEnum.Voorstart:
                        HasVoorstart = value;
                        break;
                    case IntersignaalGroepTypeEnum.Naloop:
                        HasNaloop = value;
                        break;
                    case IntersignaalGroepTypeEnum.Meeaanvraag:
                        HasMeeaanvraag = value;
                        break;
                    case IntersignaalGroepTypeEnum.LateRelease:
                        HasLateRelease = value;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                RaisePropertyChanged<object>("IsCoupled", broadcast: true);
                RaisePropertyChanged("HasNoCoupling");
            }
        }

        public bool IsCoupled
        {
            get
            {
                switch (DisplayType)
                {
                    case IntersignaalGroepTypeEnum.Gelijkstart:
                        return HasGelijkstart;
                    case IntersignaalGroepTypeEnum.Voorstart:
                        return HasVoorstart;
                    case IntersignaalGroepTypeEnum.Naloop:
                        return HasNaloop;
                    case IntersignaalGroepTypeEnum.Meeaanvraag:
                        return HasMeeaanvraag;
                    case IntersignaalGroepTypeEnum.LateRelease:
                        return HasLateRelease;
                    default:
                        return HasGelijkstart || HasVoorstart || HasNaloop || HasMeeaanvraag || HasLateRelease;
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
                    case IntersignaalGroepTypeEnum.Gelijkstart:
                        HasGelijkstart = value;
                        Messenger.Default.Send(new InterSignaalGroepChangedMessage(this.FaseVan, this.FaseNaar, this.Gelijkstart, value));
                        break;
                    case IntersignaalGroepTypeEnum.Voorstart:
                        HasVoorstart = value;
                        Messenger.Default.Send(new InterSignaalGroepChangedMessage(this.FaseVan, this.FaseNaar, this.Voorstart, value));
                        break;
                    case IntersignaalGroepTypeEnum.Naloop:
                        HasNaloop = value;
                        Messenger.Default.Send(new InterSignaalGroepChangedMessage(this.FaseVan, this.FaseNaar, this.Naloop, value));
                        break;
                    case IntersignaalGroepTypeEnum.Meeaanvraag:
                        HasMeeaanvraag = value;
                        Messenger.Default.Send(new InterSignaalGroepChangedMessage(this.FaseVan, this.FaseNaar, this.Meeaanvraag, value));
                        break;
                    case IntersignaalGroepTypeEnum.LateRelease:
                        HasLateRelease = value;
                        Messenger.Default.Send(new InterSignaalGroepChangedMessage(this.FaseVan, this.FaseNaar, this.LateRelease, value));
                        break;
                    default:
                        throw new NotImplementedException();
                }
                RaisePropertyChanged<object>("IsCoupled", broadcast: true);
                RaisePropertyChanged("HasNoCoupling");
            }
        }

        public bool HasConflict
        {
            get { return _HasConflict; }
            set
            {
                _HasConflict = value;
                RaisePropertyChanged("HasConflict");
                RaisePropertyChanged("IsEnabled");
            }
        }

        public bool HasGarantieConflict
        {
            get { return _HasGarantieConflict; }
            set
            {
                _HasGarantieConflict = value;
                RaisePropertyChanged("HasGarantieConflict");
            }
        }

        public bool HasGelijkstart
        {
            get { return _HasGelijkstart; }
            set
            {
                _HasGelijkstart = value;
                RaisePropertyChanged("HasGelijkstart");
                RaisePropertyChanged("IsCoupled");
            }
        }

        public bool HasNaloop
        {
            get { return _HasNaloop; }
            set
            {
                _HasNaloop = value;
                RaisePropertyChanged("HasNaloop");
                RaisePropertyChanged("IsCoupled");
            }
        }

        public bool HasVoorstart
        {
            get { return _HasVoorstart; }
            set
            {
                _HasVoorstart = value;
                RaisePropertyChanged("HasVoorstart");
                RaisePropertyChanged("IsCoupled");
            }
        }

        public bool HasMeeaanvraag
        {
            get { return _HasMeeaanvraag; }
            set
            {
                _HasMeeaanvraag = value;
                RaisePropertyChanged("HasMeeaanvraag");
                RaisePropertyChanged("IsCoupled");
            }
        }

        public bool HasLateRelease
        {
            get { return _HasLateRelease; }
            set
            {
                _HasLateRelease = value;
                RaisePropertyChanged("HasLateRelease");
                RaisePropertyChanged("IsCoupled");
            }
        }

        private bool _HasOppositeVoorstart;
        public bool HasOppositeVoorstart
        {
            get { return _HasOppositeVoorstart; }
            set
            {
                _HasOppositeVoorstart = value;
                RaisePropertyChanged("HasOppositeVoorstart");
                RaisePropertyChanged("IsEnabled");
            }
        }

        private bool _HasOppositeNaloop;
        public bool HasOppositeNaloop
        {
            get { return _HasOppositeNaloop; }
            set
            {
                _HasOppositeNaloop = value;
                RaisePropertyChanged("HasOppositeNaloop");
                RaisePropertyChanged("IsEnabled");
            }
        }

        private bool _HasOppositeMeeaanvraag;
        public bool HasOppositeMeeaanvraag
        {
            get { return _HasOppositeMeeaanvraag; }
            set
            {
                _HasOppositeMeeaanvraag = value;
                RaisePropertyChanged("HasOppositeMeeaanvraag");
                RaisePropertyChanged("IsEnabled");
            }
        }

        private bool _HasOppositeLateRelease;
        public bool HasOppositeLateRelease
        {
            get { return _HasOppositeLateRelease; }
            set
            {
                _HasOppositeLateRelease = value;
                RaisePropertyChanged("HasOppositeLateRelease");
                RaisePropertyChanged("IsEnabled");
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
                    case IntersignaalGroepTypeEnum.Conflict:
                    case IntersignaalGroepTypeEnum.GarantieConflict:
                        return HasNoCoupling && !HasOppositeVoorstart && !HasOppositeNaloop && !HasOppositeMeeaanvraag;
                    case IntersignaalGroepTypeEnum.Gelijkstart:
                        return !HasConflict && !HasGarantieConflict && !HasVoorstart && !HasOppositeVoorstart && !HasLateRelease && !HasOppositeLateRelease;
                    case IntersignaalGroepTypeEnum.Voorstart:
                        return !HasConflict && !HasGarantieConflict && !HasGelijkstart && !HasOppositeVoorstart;
                    case IntersignaalGroepTypeEnum.LateRelease:
                        return !HasConflict && !HasGarantieConflict && !HasGelijkstart && !HasOppositeLateRelease;
                    case IntersignaalGroepTypeEnum.Naloop:
                    case IntersignaalGroepTypeEnum.Meeaanvraag:
                        return !HasConflict && !HasGarantieConflict;
                }
                return false;
            }
        }

        public bool HasNoCoupling
        {
            get { return !(HasVoorstart || HasGelijkstart || HasNaloop || HasOppositeVoorstart || HasMeeaanvraag || HasLateRelease); }
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
                Messenger.Default.Send(new InterSignaalGroepChangedMessage(this.FaseVan, this.FaseNaar, this.Conflict));
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
                Messenger.Default.Send(new InterSignaalGroepChangedMessage(this.FaseVan, this.FaseNaar, this.Conflict));
            }

        }

        private void SetSynchValue<T>(string property, T val)
        {
            PropertyInfo propertyInfo;
            switch (DisplayType)
            {
                case IntersignaalGroepTypeEnum.Conflict:
                case IntersignaalGroepTypeEnum.GarantieConflict:
                    propertyInfo = _Conflict.GetType().GetProperty(property);
                    propertyInfo.SetValue(_Conflict, val);
                    break;
                case IntersignaalGroepTypeEnum.Gelijkstart:
                    propertyInfo = _Gelijkstart.GetType().GetProperty(property);
                    propertyInfo.SetValue(_Gelijkstart, val);
                    break;
                case IntersignaalGroepTypeEnum.Naloop:
                    propertyInfo = _Naloop.GetType().GetProperty(property);
                    propertyInfo.SetValue(_Naloop, val);
                    break;
                case IntersignaalGroepTypeEnum.Voorstart:
                    propertyInfo = _Voorstart.GetType().GetProperty(property);
                    propertyInfo.SetValue(_Voorstart, val);
                    break;
                case IntersignaalGroepTypeEnum.LateRelease:
                    propertyInfo = _LateRelease.GetType().GetProperty(property);
                    propertyInfo.SetValue(_LateRelease, val);
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
                case IntersignaalGroepTypeEnum.Conflict:
                case IntersignaalGroepTypeEnum.GarantieConflict:
                    propertyInfo = _Conflict.GetType().GetProperty(property);
                    returnval = (T)propertyInfo.GetValue(_Conflict);
                    break;
                case IntersignaalGroepTypeEnum.Gelijkstart:
                    propertyInfo = _Gelijkstart.GetType().GetProperty(property);
                    returnval = (T)propertyInfo.GetValue(_Gelijkstart);
                    break;
                case IntersignaalGroepTypeEnum.Naloop:
                    propertyInfo = _Naloop.GetType().GetProperty(property);
                    returnval = (T)propertyInfo.GetValue(_Naloop);
                    break;
                case IntersignaalGroepTypeEnum.Voorstart:
                    propertyInfo = _Voorstart.GetType().GetProperty(property);
                    returnval = (T)propertyInfo.GetValue(_Voorstart);
                    break;
                case IntersignaalGroepTypeEnum.LateRelease:
                    propertyInfo = _LateRelease.GetType().GetProperty(property);
                    returnval = (T)propertyInfo.GetValue(_LateRelease);
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
            if ((HasConflict || HasGarantieConflict) && (HasVoorstart || HasNaloop || HasGelijkstart || HasMeeaanvraag | HasLateRelease))
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
            _LateRelease = new LateReleaseModel();

            _InterSignaalGroepElements = new Dictionary<IntersignaalGroepTypeEnum, IInterSignaalGroepElement>();
            _InterSignaalGroepElements.Add(IntersignaalGroepTypeEnum.Conflict, _Conflict as IInterSignaalGroepElement);
            _InterSignaalGroepElements.Add(IntersignaalGroepTypeEnum.GarantieConflict, _Conflict as IInterSignaalGroepElement);
            _InterSignaalGroepElements.Add(IntersignaalGroepTypeEnum.Gelijkstart, _Gelijkstart as IInterSignaalGroepElement);
            _InterSignaalGroepElements.Add(IntersignaalGroepTypeEnum.Voorstart, _Voorstart as IInterSignaalGroepElement);
            _InterSignaalGroepElements.Add(IntersignaalGroepTypeEnum.Naloop, _Naloop as IInterSignaalGroepElement);
            _InterSignaalGroepElements.Add(IntersignaalGroepTypeEnum.Meeaanvraag, _Meeaanvraag as IInterSignaalGroepElement);
            _InterSignaalGroepElements.Add(IntersignaalGroepTypeEnum.LateRelease, _LateRelease as IInterSignaalGroepElement);

            ReferencesSelf = referencetoself;
            if (ReferencesSelf) _Conflict.Waarde = -5;
            else _Conflict.Waarde = -1;
        }

        #endregion // Constructor
    }
}
