using TLCGen.Models;
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
                return "Prioriteit opties";
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
