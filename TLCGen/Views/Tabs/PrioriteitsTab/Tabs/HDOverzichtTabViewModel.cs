using System.Collections;
using System.Collections.ObjectModel;
using TLCGen.Helpers;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 4, type: TabItemTypeEnum.PrioriteitTab)]
	public class HDOverzichtTabViewModel : TLCGenTabItemViewModel
	{		
        #region Fields

		private ObservableCollection<OVHDFaseDataOverviewViewModel> _Fasen;
		private OVHDFaseDataOverviewViewModel _SelectedFaseCyclus;
		private IList _SelectedFaseCycli = new ArrayList();
		
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

		
		public IList SelectedFaseCycli
		{
			get => _SelectedFaseCycli;
			set
			{
				_SelectedFaseCycli = value;
				_settingMultiple = false;
				RaisePropertyChanged();
			}
		}

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName => "HD overzicht";

		public override bool CanBeEnabled()
        {
            return _Controller?.PrioData?.PrioIngreepType != Models.Enumerations.PrioIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {
	        var temp = SelectedFaseCyclus;

	        foreach (var fcvm in Fasen)
	        {
		        fcvm.PropertyChanged -= OVHDFaseData_PropertyChanged;
		        if(fcvm.HDIngreep != null) fcvm.HDIngreep.PropertyChanged -= HDIngreep_PropertyChanged;
	        }

	        Fasen.Clear();
	        SelectedFaseCyclus = null;
	        foreach (var fcm in _Controller.Fasen)
	        {
		        var fcvm = new OVHDFaseDataOverviewViewModel(fcm, this, _Controller);
		        Fasen.Add(fcvm);
		        fcvm.PropertyChanged += OVHDFaseData_PropertyChanged;
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

		#region Event Handling

		private bool _settingMultiple;
		private void OVHDFaseData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (_settingMultiple || string.IsNullOrEmpty(e.PropertyName))
				return;

			if (SelectedFaseCycli != null && SelectedFaseCycli.Count > 1)
			{
				_settingMultiple = true;
				MultiPropertySetter.SetPropertyForAllItems<OVHDFaseDataOverviewViewModel>(sender, e.PropertyName, SelectedFaseCycli);
			}
			_settingMultiple = false;
		}

		public void HDIngreep_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (_settingMultiple || string.IsNullOrEmpty(e.PropertyName))
				return;

			if (SelectedFaseCycli != null && SelectedFaseCycli.Count > 1)
			{
				_settingMultiple = true;
				var l = new ArrayList();
				foreach (OVHDFaseDataOverviewViewModel fc in SelectedFaseCycli)
				{
                    if(fc.FaseCyclusNaam != null)
                    {
					    l.Add(fc.HDIngreep);
                    }
				}
				MultiPropertySetter.SetPropertyForAllItems<HDIngreepViewModel>(sender, e.PropertyName, l);
			}
			_settingMultiple = false;
		}

		#endregion // Event Handling

		#region Constructor

		public HDOverzichtTabViewModel()
        {
        }

        #endregion // Constructor
    }
}