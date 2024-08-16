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

		private ObservableCollection<HDFaseDataOverviewViewModel> _Fasen;
		private HDFaseDataOverviewViewModel _SelectedFaseCyclus;
		private IList _SelectedFaseCycli = new ArrayList();
		
        #endregion // Fields

        #region Properties

        public ObservableCollection<HDFaseDataOverviewViewModel> Fasen => _Fasen ??= new ObservableCollection<HDFaseDataOverviewViewModel>();

        public HDFaseDataOverviewViewModel SelectedFaseCyclus
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
		        fcvm.PropertyChanged -= HDIngreep_PropertyChanged;
	        }

	        Fasen.Clear();
	        SelectedFaseCyclus = null;
	        foreach (var fcm in _Controller.Fasen)
	        {
		        var fcvm = new HDFaseDataOverviewViewModel(fcm, this, _Controller);
		        Fasen.Add(fcvm);
		        fcvm.PropertyChanged += HDIngreep_PropertyChanged;
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

		public void HDIngreep_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (_settingMultiple || string.IsNullOrEmpty(e.PropertyName))
				return;

			if (SelectedFaseCycli != null && SelectedFaseCycli.Count > 1)
			{
				_settingMultiple = true;
				var l = new ArrayList();
				foreach (HDFaseDataOverviewViewModel fc in SelectedFaseCycli)
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