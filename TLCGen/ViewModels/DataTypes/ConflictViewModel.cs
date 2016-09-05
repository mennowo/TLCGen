using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class ConflictViewModel : ViewModelBase
    {
        public enum DisplayType { Conflict, GarantieConflict, Gelijkstart, Voorstart, Naloop };

        #region Fields

        private ConflictModel _Conflict;
        private ControllerViewModel _ControllerVM;

        #endregion // Fields

        #region Properties

        DisplayType _WhatToDisplay;
        public DisplayType WhatToDisplay
        {
            get { return _WhatToDisplay; }
            set
            {
                _WhatToDisplay = value;
                OnPropertyChanged("DisplayWaarde");
            }
        }

        public ConflictModel Conflict
        {
            get { return _Conflict; }
        }

        public string FaseVan
        {
            get { return _Conflict.FaseVan; }
            set
            {
                _Conflict.FaseVan = value;
                OnPropertyChanged("FaseVan");
            }
        }
        
        public string FaseNaar
        {
            get { return _Conflict.FaseNaar; }
            set
            {
                _Conflict.FaseNaar = value;
                OnPropertyChanged("FaseNaar");
            }
        }
        
        public int Waarde
        {
            get { return _Conflict.Waarde; }
        }

        public string DisplayWaarde
        {
            get
            {
                switch (WhatToDisplay)
                {
                    case DisplayType.Conflict:
                        return GetConflictValue();
                    case DisplayType.GarantieConflict:
                        return GetGaratieConflictValue();
                    default:
                        return "";
                }
            }
            set
            {
                switch (WhatToDisplay)
                {
                    case DisplayType.Conflict:
                        SetConflictValue(value);
                        break;
                    case DisplayType.GarantieConflict:
                        SetGarantieConflictValue(value);
                        break;
                    default:
                        return;
                }
                OnMonitoredPropertyChanged("DisplayWaarde", _ControllerVM);
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
                    return "X";
                case -6:
                    return "*";
                default:
                    return _Conflict.Waarde.ToString();
            }
        }

        private void SetConflictValue(string value)
        {
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
                        _Conflict.Waarde = -1;
                    break;
            }
            // Set the value of the 'opposite' conflict 
            // (= the conflict whose from/to phasecycles are the inverse of this one)
            _ControllerVM.ConflictMatrixVM.SetOppositeConflict(this);
            // If this conflict is black, set its value from it's opposite
            // We set property 'Waarde', to avoid a loop caused by instances of 
            // ConflictViewModel setting each other's DisplayWaarde property.
            if (_Conflict.Waarde == -1)
                _Conflict.Waarde = _ControllerVM.ConflictMatrixVM.SetBlankConflictFromOppositeConflict(this);
            _ControllerVM.ConflictMatrixVM.MatrixChanged = true;
        }

        public string GetGaratieConflictValue()
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

        public void SetGarantieConflictValue(string value)
        {
            int confval;
            if (Int32.TryParse(value, out confval))
            {
                _Conflict.GarantieWaarde = confval;
            }
            // Ignore false data
            else
                _Conflict.GarantieWaarde = -1;

            // Set the value of the 'opposite' conflict 
            // (= the conflict whose from/to phasecycles are the inverse of this one)
            _ControllerVM.ConflictMatrixVM.SetOppositeConflict(this);
            // If this conflict is black, set its value from it's opposite
            // We set property 'Waarde', to avoid a loop caused by instances of 
            // ConflictViewModel setting each other's DisplayWaarde property.
            if (_Conflict.GarantieWaarde == -1)
                _Conflict.GarantieWaarde = _ControllerVM.ConflictMatrixVM.SetBlankConflictFromOppositeConflict(this);
            _ControllerVM.ConflictMatrixVM.MatrixChanged = true;
        }

        #endregion // Private methods

        #region Public methods

        public override string ToString()
        {
            return DisplayWaarde;
        }

        #endregion // Public methods

        #region Constructor

        public ConflictViewModel(ControllerViewModel controllervm, ConflictModel conflict)
        {
            _ControllerVM = controllervm;
            _Conflict = conflict;
        }

        #endregion // Constructor
    }
}
