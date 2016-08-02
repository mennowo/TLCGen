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

        #region Fields

        private ConflictModel _Conflict;
        private ControllerViewModel _ControllerVM;

        #endregion // Fields

        #region Properties

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
            set
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
                            _Conflict.Waarde = confval;
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
                OnMonitoredPropertyChanged("DisplayWaarde", _ControllerVM);
                _ControllerVM.ConflictMatrixVM.MatrixChanged = true;
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

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
