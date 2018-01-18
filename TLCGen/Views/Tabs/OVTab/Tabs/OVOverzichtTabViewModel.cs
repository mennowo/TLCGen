using System.Collections.ObjectModel;
using TLCGen.Extensions;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1, type: TabItemTypeEnum.OVTab)]
	public class OVOverzichtTabViewModel : TLCGenTabItemViewModel
	{		
        #region Fields

		private ObservableCollection<OVHDFaseDataOverviewViewModel> _Fasen;
		private OVHDFaseDataOverviewViewModel _SelectedFaseCyclus;
		
        #endregion // Fields

        #region Properties

        public ObservableCollection<OVHDFaseDataOverviewViewModel> Fasen
        {
            get
            {
                if (_Fasen == null)
                {
                    _Fasen = new ObservableCollection<OVHDFaseDataOverviewViewModel>();
                }
                return _Fasen;
            }
        }

        public OVHDFaseDataOverviewViewModel SelectedFaseCyclus
        {
            get => _SelectedFaseCyclus;
	        set
            {
                _SelectedFaseCyclus = value;
                RaisePropertyChanged();
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName => "Overzicht";

		public override bool CanBeEnabled()
        {
            return _Controller?.OVData?.OVIngreepType != Models.Enumerations.OVIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {
	        var temp = SelectedFaseCyclus;
	        Fasen.Clear();
	        SelectedFaseCyclus = null;
	        foreach (var fcm in _Controller.Fasen)
	        {
		        var fcvm = new OVHDFaseDataOverviewViewModel(fcm, _Controller);
		        Fasen.Add(fcvm);
		        if (temp == null || fcvm.FaseCyclusNaam != temp.FaseCyclusNaam) continue;
		        SelectedFaseCyclus = fcvm;
		        temp = null;
	        }
	        if(SelectedFaseCyclus == null && Fasen.Count > 0)
	        {
		        SelectedFaseCyclus = Fasen[0];
	        }
        }

        #endregion // TabItem Overrides

        #region Commands
		
        #endregion // Commands

        #region Command functionality
		
		#endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen events
		
        #endregion TLCGen events

        #region Constructor

        public OVOverzichtTabViewModel()
        {
        }

        #endregion // Constructor
    }
}