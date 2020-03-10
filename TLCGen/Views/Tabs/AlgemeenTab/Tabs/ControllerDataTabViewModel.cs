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

        public override string DisplayName => "Info & opties";

        public override bool IsEnabled
        {
            get => true;
            set { }
        }

        public override void OnSelected()
        {
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

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
