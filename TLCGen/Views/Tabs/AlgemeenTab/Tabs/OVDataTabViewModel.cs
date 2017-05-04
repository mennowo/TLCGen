using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1, type: TabItemTypeEnum.AlgemeenTab)]
    public class OVDataTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        private OVDataViewModel _OVData;
        public OVDataViewModel OVData
        {
            get
            {
                if (_OVData == null)
                {
                    _OVData = new OVDataViewModel();
                }
                return _OVData;
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "OV opties";
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
                OVData.Controller = value;
            }
        }

        #endregion // TabItem Overrides

        #region Constructor

        public OVDataTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
