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

        private DefaultsTabViewModel _DefaultsTabVM;
        public DefaultsTabViewModel DefaultsTabVM
        {
            get
            {
                if (_DefaultsTabVM == null)
                {
                    _DefaultsTabVM = new DefaultsTabViewModel();
                }
                return _DefaultsTabVM;
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

        private DetectorenTemplatesEditorTabViewModel _DetectorenTemplatesEditorTabVM;
        public DetectorenTemplatesEditorTabViewModel DetectorenTemplatesEditorTabVM
        {
            get
            {
                if (_DetectorenTemplatesEditorTabVM == null)
                {
                    _DetectorenTemplatesEditorTabVM = new DetectorenTemplatesEditorTabViewModel();
                }
                return _DetectorenTemplatesEditorTabVM;
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
