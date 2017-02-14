using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;

namespace TLCGen.Settings
{
    public class DetectorTemplateViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private DetectorTemplateModel _Template;

        #endregion // Fields

        #region Properties
        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return _Template;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public DetectorTemplateViewModel(DetectorTemplateModel template)
        {
            _Template = template;
        }

        #endregion // Constructor
    }
}
