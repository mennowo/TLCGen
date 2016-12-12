using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 2, type: TabItemTypeEnum.SpecialsTab)]
    public class VAOntruimenTabViewModel : TLCGenTabItemViewModel
    {

        #region Fields

        #endregion // Fields

        #region Properties

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen TabItem overrides

        public override string DisplayName
        {
            get { return "VA ontruimen"; }
        }

        #endregion // TLCGen TabItem overrides

        #region Constructor

        public VAOntruimenTabViewModel(ControllerModel controller) : base(controller)
        {

        }

        #endregion // Constructor
    }
}
