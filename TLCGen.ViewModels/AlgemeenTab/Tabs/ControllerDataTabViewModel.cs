using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Messaging.Messages;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 0, type: TabItemTypeEnum.AlgemeenTab)]
    public class ControllerDataTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        private ControllerDataViewModel _ControllerData;
        public ControllerDataViewModel ControllerData
        {
            get
            {
                if(_ControllerData == null)
                {
                    _ControllerData = new ControllerDataViewModel();
                }
                return _ControllerData;
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Info & opties";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
        }

        public override ControllerModel Controller
        {
            get
            {
                return base.Controller;
            }

            set
            {
                base.Controller = value;
                ControllerData.Controller = value;
            }
        }

        #endregion // TabItem Overrides

        #region Constructor

        public ControllerDataTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
