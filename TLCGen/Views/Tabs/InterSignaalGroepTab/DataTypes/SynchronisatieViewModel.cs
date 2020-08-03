using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Reflection;
using GalaSoft.MvvmLight;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.ViewModels.Enums;
using System.Windows.Media;

namespace TLCGen.ViewModels
{
    public class SynchronisatieViewModel : ViewModelBase
    {
        #region Fields

        // Model elements whose data is displayed
        private ConflictModel _conflict;
        private NaloopModel _naloop;
        private GelijkstartModel _gelijkstart;
        private VoorstartModel _voorstart;
        private MeeaanvraagModel _meeaanvraag;
        private LateReleaseModel _lateRelease;

        private bool _hasConflict;
        private bool _hasGarantieConflict;
        private bool _hasGelijkstart;
        private bool _hasNaloop;
        private bool _hasVoorstart;
        private bool _hasMeeaanvraag;
        private bool _hasLateRelease;

        private string _faseVan;
        private string _faseNaar;

        private IntersignaalGroepTypeEnum _displayType;

        private NaloopViewModel _naloopVm;
        private MeeaanvraagViewModel _meeaanvraagVm;
        private VoorstartViewModel _voorstartVm;
	    private GelijkstartViewModel _gelijkstartVm;
	    private LateReleaseViewModel _lateReleaseVm;

        private Dictionary<IntersignaalGroepTypeEnum, IInterSignaalGroepElement> _interSignaalGroepElements;

        #endregion // Fields

        #region Properties

        public object SelectedObject
        {
            get
            {
                switch (DisplayType)
                {
                    case IntersignaalGroepTypeEnum.Conflict:
                        break;
                    case IntersignaalGroepTypeEnum.GarantieConflict:
                        break;
                    case IntersignaalGroepTypeEnum.Gelijkstart:
                        return GelijkstartVm;
                    case IntersignaalGroepTypeEnum.Voorstart:
                        return VoorstartVm;
                    case IntersignaalGroepTypeEnum.Naloop:
                        return NaloopVm;
                    case IntersignaalGroepTypeEnum.Meeaanvraag:
                        return MeeaanvraagVm;
                    case IntersignaalGroepTypeEnum.LateRelease:
                        return LateReleaseVm;
                    case IntersignaalGroepTypeEnum.SomeConflict:
                        break;
                    case IntersignaalGroepTypeEnum.SomeSynchronisatie:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return null;
            }
        }

        public bool ShowIsCoupled =>
            DisplayType == IntersignaalGroepTypeEnum.Gelijkstart
            || DisplayType == IntersignaalGroepTypeEnum.LateRelease
            || DisplayType == IntersignaalGroepTypeEnum.Meeaanvraag
            || DisplayType == IntersignaalGroepTypeEnum.Naloop
            || DisplayType == IntersignaalGroepTypeEnum.LateRelease
            || DisplayType == IntersignaalGroepTypeEnum.Voorstart;

        public NaloopViewModel NaloopVm => _naloopVm ?? (_naloopVm = new NaloopViewModel(_naloop));

	    public MeeaanvraagViewModel MeeaanvraagVm => _meeaanvraagVm ?? (_meeaanvraagVm = new MeeaanvraagViewModel(_meeaanvraag));
        
	    public VoorstartViewModel VoorstartVm => _voorstartVm ?? (_voorstartVm = new VoorstartViewModel(_voorstart));
	    
	    public GelijkstartViewModel GelijkstartVm => _gelijkstartVm ?? (_gelijkstartVm = new GelijkstartViewModel(_gelijkstart));

        public LateReleaseViewModel LateReleaseVm => _lateReleaseVm ?? (_lateReleaseVm = new LateReleaseViewModel(_lateRelease));

	    public ConflictModel Conflict
        {
            get => _conflict;
		    set
            {
                _conflict = value;
                HasConflict = _conflict != null;
            }
        }

        public NaloopModel Naloop
        {
            get => _naloop;
	        set
            {
                _naloop = value;
                HasNaloop = _naloop != null;
            }
        }

        public GelijkstartModel Gelijkstart
        {
            get => _gelijkstart;
	        set
            {
                _gelijkstart = value;
                HasGelijkstart = _gelijkstart != null;
            }
        }

        public VoorstartModel Voorstart
        {
            get => _voorstart;
	        set
            {
                _voorstart = value;
                HasVoorstart = _voorstart != null;
            }
        }

        public MeeaanvraagModel Meeaanvraag
        {
            get => _meeaanvraag;
	        set
            {
                _meeaanvraag = value;
                _hasMeeaanvraag = _meeaanvraag != null;
            }
        }

        public LateReleaseModel LateRelease
        {
            get => _lateRelease;
	        set
            {
                _lateRelease = value;
                _hasLateRelease = _lateRelease != null;
            }
        }

        public IntersignaalGroepTypeEnum DisplayType
        {
            get => _displayType;
	        set
            {
                _displayType = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ConflictValue));
                RaisePropertyChanged(nameof(IsCoupled));
                RaisePropertyChanged(nameof(HasNoCoupling));
                RaisePropertyChanged(nameof(IsEnabled));
                RaisePropertyChanged(nameof(ConflictForeground));
                RaisePropertyChanged(nameof(ConflictBackground));
                RaisePropertyChanged(nameof(SynchronisatieIndicatorBrush));
                RaisePropertyChanged(nameof(DisplayTypeTimings));
                RaisePropertyChanged(nameof(SelectedObject));
                RaisePropertyChanged(nameof(ShowIsCoupled));
            }
        }

        public bool DisplayTypeTimings => 
            DisplayType == IntersignaalGroepTypeEnum.Conflict ||
            DisplayType == IntersignaalGroepTypeEnum.GarantieConflict;
        
        public string FaseVan
        {
            get => _faseVan;
            set
            {
                _faseVan = value;
                foreach (var elem in _interSignaalGroepElements)
                {
                    elem.Value.FaseVan = value;
                }
                RaisePropertyChanged();
            }
        }
        
        public string FaseNaar
        {
            get => _faseNaar;
            set
            {
                _faseNaar = value;
                foreach (var elem in _interSignaalGroepElements)
                {
                    elem.Value.FaseNaar = value;
                }
                RaisePropertyChanged();
            }
        }

        public string ConflictValueNoMessaging
        {
            set
            {
                if(IsCoupled || ReferencesSelf)
                {
                    throw new InvalidOperationException("Coupled or self-referenced synch cell cannot have a conflict value");
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
                RaisePropertyChanged<object>(nameof(ConflictValue), broadcast: true);
                RaisePropertyChanged(nameof(HasConflict));
                RaisePropertyChanged(nameof(IsEnabled));
                RaisePropertyChanged(nameof(ConflictBackground));
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
                if(ReferencesSelf || !IsEnabled)
                {
					throw new InvalidOperationException("Coupled or self-referenced synch cell cannot have a conflict value");
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
                RaisePropertyChanged<object>(nameof(ConflictValue), broadcast: true);
                RaisePropertyChanged(nameof(HasConflict));
                RaisePropertyChanged(nameof(IsEnabled));
                RaisePropertyChanged(nameof(ConflictBackground));
            }
        }

        public bool IsCoupledNoMessaging
        {
            set
            {
                if (HasConflict)
                {
					throw new InvalidOperationException("Cell with conflict cannot have a synch value");
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
                        throw new ArgumentOutOfRangeException();
                }
                RaisePropertyChanged<object>(nameof(IsCoupled), broadcast: true);
                RaisePropertyChanged(nameof(HasNoCoupling));
                RaisePropertyChanged(nameof(SynchronisatieIndicatorBrush));
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
					throw new InvalidOperationException("Cell with conflict or self-reference cannot have a synch value");
				}
                switch (DisplayType)
                {
                    case IntersignaalGroepTypeEnum.Gelijkstart:
                        HasGelijkstart = value;
                        Messenger.Default.Send(new InterSignaalGroepChangedMessage(FaseVan, FaseNaar, Gelijkstart, value));
                        break;
                    case IntersignaalGroepTypeEnum.Voorstart:
                        HasVoorstart = value;
                        Messenger.Default.Send(new InterSignaalGroepChangedMessage(FaseVan, FaseNaar, Voorstart, value));
                        break;
                    case IntersignaalGroepTypeEnum.Naloop:
                        HasNaloop = value;
                        Messenger.Default.Send(new InterSignaalGroepChangedMessage(FaseVan, FaseNaar, Naloop, value));
                        break;
                    case IntersignaalGroepTypeEnum.Meeaanvraag:
                        HasMeeaanvraag = value;
                        Messenger.Default.Send(new InterSignaalGroepChangedMessage(FaseVan, FaseNaar, Meeaanvraag, value));
                        break;
                    case IntersignaalGroepTypeEnum.LateRelease:
                        HasLateRelease = value;
                        Messenger.Default.Send(new InterSignaalGroepChangedMessage(FaseVan, FaseNaar, LateRelease, value));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                RaisePropertyChanged<object>(nameof(IsCoupled), broadcast: true);
                RaisePropertyChanged(nameof(HasNoCoupling));
                RaisePropertyChanged(nameof(SynchronisatieIndicatorBrush));
            }
        }

        public bool HasConflict
        {
            get => _hasConflict;
            set
            {
                _hasConflict = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }

        public bool HasGarantieConflict
        {
            get => _hasGarantieConflict;
            set
            {
                _hasGarantieConflict = value;
                RaisePropertyChanged();
            }
        }

        public bool HasGelijkstart
        {
            get => _hasGelijkstart;
            set
            {
                _hasGelijkstart = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsCoupled));
            }
        }

        public bool HasNaloop
        {
            get => _hasNaloop;
            set
            {
                _hasNaloop = value;
                RaisePropertyChanged(nameof(IsCoupled));
                RaisePropertyChanged($"IsCoupled");
            }
        }

        public bool HasVoorstart
        {
            get => _hasVoorstart;
            set
            {
                _hasVoorstart = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsCoupled));
            }
        }

        public bool HasMeeaanvraag
        {
            get => _hasMeeaanvraag;
            set
            {
                _hasMeeaanvraag = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsCoupled));
            }
        }

        public bool HasLateRelease
        {
            get => _hasLateRelease;
            set
            {
                _hasLateRelease = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsCoupled));
            }
        }

        private bool _hasOppositeVoorstart;
        public bool HasOppositeVoorstart
        {
            get => _hasOppositeVoorstart;
            set
            {
                _hasOppositeVoorstart = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsEnabled));
                RaisePropertyChanged(nameof(ConflictBackground));
                RaisePropertyChanged(nameof(SynchronisatieIndicatorBrush));
            }
        }

        private bool _hasOppositeNaloop;
        public bool HasOppositeNaloop
        {
            get => _hasOppositeNaloop;
            set
            {
                _hasOppositeNaloop = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsEnabled");
            }
        }

        private bool _hasOppositeMeeaanvraag;
        public bool HasOppositeMeeaanvraag
        {
            get => _hasOppositeMeeaanvraag;
            set
            {
                _hasOppositeMeeaanvraag = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsEnabled");
            }
        }

        private bool _hasOppositeLateRelease;

        public bool HasOppositeLateRelease
        {
            get => _hasOppositeLateRelease;
            set
            {
                _hasOppositeLateRelease = value;
                RaisePropertyChanged();
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
                        return !HasConflict && !HasGarantieConflict && !HasGelijkstart && !HasOppositeVoorstart && !HasLateRelease && !HasOppositeLateRelease;
                    case IntersignaalGroepTypeEnum.LateRelease:
                        return !HasConflict && !HasGarantieConflict && !HasGelijkstart && !HasOppositeLateRelease && !HasVoorstart && !HasOppositeVoorstart;
                    case IntersignaalGroepTypeEnum.Naloop:
                    case IntersignaalGroepTypeEnum.Meeaanvraag:
                        return !HasConflict && !HasGarantieConflict;
                }
                return false;
            }
        }

        public bool HasNoCoupling => !(HasVoorstart || HasGelijkstart || HasNaloop || HasOppositeVoorstart || HasMeeaanvraag || HasLateRelease);

        public bool ReferencesSelf
        {
            get; private set;
        }

        public bool NotReferencingSelf => !ReferencesSelf;

        public bool AllowCoupling => !ReferencesSelf && !HasConflict && !HasGarantieConflict;

        public Brush SynchronisatieIndicatorBrush
        {
            get
            {
                if (ReferencesSelf) return null;
                if (!IsEnabled) return Brushes.LightGray;
                if (IsCoupled) return Brushes.LightGreen;
                return Brushes.Transparent;
            }
        }

        public Brush ConflictForeground
        {
            get
            {
                switch (DisplayType)
                {
                    case IntersignaalGroepTypeEnum.Conflict:
                    case IntersignaalGroepTypeEnum.GarantieConflict:
                        return Brushes.Black;
                    default:
                        return Brushes.Gray;
                }
            }
        }

        public Brush ConflictBackground
        {
            get
            {
                if (ReferencesSelf) return Brushes.DarkGray;
                if (!IsEnabled) return Brushes.LightGray;
                switch (DisplayType)
                {
                    case IntersignaalGroepTypeEnum.Conflict:
                    case IntersignaalGroepTypeEnum.GarantieConflict:
                        if (!string.IsNullOrEmpty(ConflictValue))
                        {
                            if (!Int32.TryParse(ConflictValue, out var i))
                            {
                                switch (ConflictValue)
                                {
                                    case "*":
                                        return Brushes.OrangeRed;
                                    case "FK":
                                        return Brushes.LightYellow;
                                    case "GK":
                                        return Brushes.DarkSeaGreen;
                                    case "GKL":
                                        return Brushes.MediumAquamarine;
                                    default:
                                        return Brushes.OrangeRed;
                                }
                            }
                            else
                                return Brushes.LightBlue;
                        }
                        return null;
                    case IntersignaalGroepTypeEnum.Gelijkstart:
                    case IntersignaalGroepTypeEnum.Naloop:
                    case IntersignaalGroepTypeEnum.Voorstart:
                    case IntersignaalGroepTypeEnum.Meeaanvraag:
                        if (IsEnabled)
                            return null;
                        else
                            return Brushes.LightGray;
                    default:
                        return null;
                }
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        public string GetConflictValue()
        {
            if(_conflict.GarantieWaarde != null && _conflict.GarantieWaarde >= 0)
            {
                if(_conflict.Waarde == -1)
                {
                    return "*";
                }
            }
            switch (_conflict.Waarde)
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
                    return _conflict.Waarde.ToString();
            }
        }

        private void SetConflictValue(string value, bool sendmessage)
        {
            HasConflict = true;
            switch (value)
            {
                case "FK":
                    _conflict.Waarde = -2;
                    break;
                case "GK":
                    _conflict.Waarde = -3;
                    break;
                case "GKL":
                    _conflict.Waarde = -4;
                    break;
                case "*":
                    _conflict.Waarde = -6;
                    break;
                default:
                    int confval;
                    if (Int32.TryParse(value, out confval))
                    {
                        if (_conflict.GarantieWaarde != null && _conflict.GarantieWaarde >= 0)
                        {
                            if (confval >= _conflict.GarantieWaarde)
                                _conflict.Waarde = confval;
                            else
                                _conflict.Waarde = (int)_conflict.GarantieWaarde;
                        }
                        else
                        {
                            _conflict.Waarde = confval;
                        }
                    }
                    // Ignore false data
                    else
                    {
                        _conflict.Waarde = -1;
                        HasConflict = false;
                    }
                    break;
            }

            if (sendmessage)
            {
                Messenger.Default.Send(new InterSignaalGroepChangedMessage(FaseVan, FaseNaar, Conflict));
            }
        }

        public string GetGarantieConflictValue()
        {
            if(_conflict.Waarde >= 0)
            {
                switch (_conflict.GarantieWaarde)
                {
                    case null:
                    case -1:
                        return "*";
                    case -5:
                        return "X";
                    case -6:
                        return "*";
                    default:
                        return _conflict.GarantieWaarde.ToString();
                }
            }
            else if (_conflict.GarantieWaarde != -1)
            {
                switch (_conflict.GarantieWaarde)
                {
                    case -1:
                        return "";
                    case -5:
                        return "X";
                    case -6:
                        return "*";
                    default:
                        return _conflict.GarantieWaarde.ToString();
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
                    _conflict.GarantieWaarde = -6;
                    break;
                default:
                    if (Int32.TryParse(value, out confval))
                    {
                        _conflict.GarantieWaarde = confval;
                    }
                    // Ignore false data
                    else
                    {
                        _conflict.GarantieWaarde = -1;
                        HasGarantieConflict = false;
                    }
                    break;
            }

            if(sendmessage)
            {
                Messenger.Default.Send(new InterSignaalGroepChangedMessage(FaseVan, FaseNaar, Conflict));
            }
        }

        #endregion // Private methods

        #region Public methods

        public bool IsOk()
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
            _conflict = new ConflictModel();
            _gelijkstart = new GelijkstartModel();
            _voorstart = new VoorstartModel();
            _naloop = new NaloopModel();
            _meeaanvraag = new MeeaanvraagModel();
            _lateRelease = new LateReleaseModel();

            _interSignaalGroepElements = new Dictionary<IntersignaalGroepTypeEnum, IInterSignaalGroepElement>
            {
                {IntersignaalGroepTypeEnum.Conflict, _conflict},
                {IntersignaalGroepTypeEnum.GarantieConflict, _conflict},
                {IntersignaalGroepTypeEnum.Gelijkstart, _gelijkstart},
                {IntersignaalGroepTypeEnum.Voorstart, _voorstart},
                {IntersignaalGroepTypeEnum.Naloop, _naloop},
                {IntersignaalGroepTypeEnum.Meeaanvraag, _meeaanvraag},
                {IntersignaalGroepTypeEnum.LateRelease, _lateRelease}
            };

            ReferencesSelf = referencetoself;
            if (ReferencesSelf) _conflict.Waarde = -5;
            else _conflict.Waarde = -1;
        }

        #endregion // Constructor
    }
}
