using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1, type: TabItemTypeEnum.AlgemeenTab)]
    public class OVDataTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private PrioDataViewModel _prioData;

        #endregion // Fields

        #region Properties

        public PrioDataViewModel PrioData
        {
            get
            {
                if (_prioData == null)
                {
                    _prioData = new PrioDataViewModel();
                }
                return _prioData;
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
                PrioData.Controller = value;
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
