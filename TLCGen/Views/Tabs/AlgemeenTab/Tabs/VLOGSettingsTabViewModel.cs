using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Models;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 3, type: TabItemTypeEnum.AlgemeenTab)]
    public class VLOGSettingsTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        private VLOGSettingsDataViewModel _VLOGSettingsData;
        public VLOGSettingsDataViewModel VLOGSettingsData
        {
            get
            {
                if(_VLOGSettingsData == null && _Controller?.Data?.VLOGSettings != null)
                {
                    _VLOGSettingsData = new VLOGSettingsDataViewModel(_Controller.Data.VLOGSettings);
                }
                return _VLOGSettingsData;
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName => "VLOG settings";

        public override bool CanBeEnabled()
        {
            return _Controller?.Data?.CCOLVersie > CCOLVersieEnum.CCOL8;
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
                if(value != null)
                {
                    if(value.Data.CCOLVersie > CCOLVersieEnum.CCOL8 && value.Data.VLOGSettings == null)
                    {
                        value.Data.VLOGSettings = new VLOGSettingsDataModel();
                        DefaultsProvider.Default.SetDefaultsOnModel(value.Data.VLOGSettings, value.Data.VLOGSettings.VLOGVersie.ToString());
                        _VLOGSettingsData = new VLOGSettingsDataViewModel(value.Data.VLOGSettings);
                    }
                    else
                    {
                        _VLOGSettingsData = null;
                    }
                }
                OnPropertyChanged("");
            }
        }

        #endregion // TabItem Overrides

        #region Constructor

        public VLOGSettingsTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
