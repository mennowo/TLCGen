using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.Settings
{
    public class TLCGenSettingsViewModel : ViewModelBase
    {
        #region Fields

        #endregion // Fields

        #region Properties

        private FasenDefaultsTabViewModel _FasenDefaultsTabVM;
        public FasenDefaultsTabViewModel FasenDefaultsTabVM
        {
            get
            {
                if (_FasenDefaultsTabVM == null)
                {
                    _FasenDefaultsTabVM = new FasenDefaultsTabViewModel();
                }
                return _FasenDefaultsTabVM;
            }
        }

        private DetectorenDefaultsTabViewModel _DetectorenDefaultsTabVM;
        public DetectorenDefaultsTabViewModel DetectorenDefaultsTabVM
        {
            get
            {
                if (_DetectorenDefaultsTabVM == null)
                {
                    _DetectorenDefaultsTabVM = new DetectorenDefaultsTabViewModel();
                }
                return _DetectorenDefaultsTabVM;
            }
        }

        private FasenTemplatesEditorTabViewModel _FasenTemplatesEditorTabVM;
        public FasenTemplatesEditorTabViewModel FasenTemplatesEditorTabVM
        {
            get
            {
                if (_FasenTemplatesEditorTabVM == null)
                {
                    _FasenTemplatesEditorTabVM = new FasenTemplatesEditorTabViewModel();
                }
                return _FasenTemplatesEditorTabVM;
            }
        }

        #endregion // Properties

        #region Private Methods

        #endregion // Private Methods

        #region Constructor

        public TLCGenSettingsViewModel()
        {
        }

        #endregion // Constructor
    }
}
