using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class DetectorenTabViewModel : ViewModelBase
    {
        #region Fields

        private ControllerViewModel _ControllerVM;
        private DetectorenFasenLijstViewModel _DetectorenFasenLijstVM;
        private DetectorenExtraLijstViewModel _DetectorenExtraLijstVM;
        private DetectorenAllesLijstViewModel _DetectorenAllesLijstVM;

        #endregion // Fields

        #region Properties

        public DetectorenExtraLijstViewModel DetectorenExtraLijstVM
        {
            get { return _DetectorenExtraLijstVM; }
        }

        public DetectorenFasenLijstViewModel DetectorenFasenLijstVM
        {
            get { return _DetectorenFasenLijstVM; }
        }

        public DetectorenAllesLijstViewModel DetectorenAllesLijstVM
        {
            get { return _DetectorenAllesLijstVM; }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Constructor

        public DetectorenTabViewModel(ControllerViewModel controllervm)
        {
            _ControllerVM = controllervm;
            _DetectorenExtraLijstVM = new DetectorenExtraLijstViewModel(_ControllerVM);
            _DetectorenFasenLijstVM = new DetectorenFasenLijstViewModel(_ControllerVM);
            _DetectorenAllesLijstVM = new DetectorenAllesLijstViewModel(_ControllerVM);
        }

        #endregion // Constructor
    }
}
