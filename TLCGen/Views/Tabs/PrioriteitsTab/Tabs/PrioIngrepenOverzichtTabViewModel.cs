using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TLCGen.Helpers;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1, type: TabItemTypeEnum.PrioriteitTab)]
    public class PrioIngrepenOverzichtTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<PrioIngreepViewModel> _ingrepen;
        private List<FaseCyclusWithPrioViewModel> _fasen;
        private PrioIngreepViewModel _selectedPrioIngreep;
        private IList _selectedIngrepen = new ArrayList();

        #endregion // Fields

        #region Properties

        public ObservableCollection<PrioIngreepViewModel> Ingrepen => _ingrepen ??= new ObservableCollection<PrioIngreepViewModel>();

        public PrioIngreepViewModel SelectedPrioIngreep
        {
            get => _selectedPrioIngreep;
            set
            {
                _selectedPrioIngreep = value;
                OnPropertyChanged();
            }
        }

        public IList SelectedIngrepen
        {
            get => _selectedIngrepen;
            set
            {
                _selectedIngrepen = value;
                _settingMultiple = false;
                OnPropertyChanged();
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName => "Overzicht";

        public override bool CanBeEnabled()
        {
            return _Controller?.PrioData?.PrioIngreepType != Models.Enumerations.PrioIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {
            var temp = SelectedPrioIngreep;

            foreach (var fcvm in Ingrepen)
            {
                fcvm.PropertyChanged -= IngreepData_PropertyChanged;
            }

            Ingrepen.Clear();
            SelectedPrioIngreep = null;
            _fasen = new List<FaseCyclusWithPrioViewModel>();
            foreach (var fc in _Controller.Fasen)
            {
                var ingrepen = _Controller.PrioData.PrioIngrepen.Where(x => x.FaseCyclus == fc.Naam).ToArray();
                if (ingrepen.Length == 0) continue;
                _fasen.Add(new FaseCyclusWithPrioViewModel(fc.Naam, ingrepen));
            }

            foreach (var fcvm in _fasen.SelectMany(x => x.Ingrepen))
            {
                Ingrepen.Add(fcvm);
                fcvm.PropertyChanged += IngreepData_PropertyChanged;
                if (temp == null || fcvm.FaseCyclus != temp.FaseCyclus) continue;
                SelectedPrioIngreep = fcvm;
                temp = null;
            }

            if (SelectedPrioIngreep == null && Ingrepen.Count > 0)
            {
                SelectedPrioIngreep = Ingrepen[0];
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

        private void IngreepData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_settingMultiple || string.IsNullOrEmpty(e.PropertyName))
                return;

            if (SelectedIngrepen != null && SelectedIngrepen.Count > 1)
            {
                _settingMultiple = true;
                MultiPropertySetter.SetPropertyForAllItems<PrioIngreepViewModel>(sender, e.PropertyName, SelectedIngrepen);
            }

            _settingMultiple = false;
        }

        #endregion // Event Handling

        #region Constructor

        public PrioIngrepenOverzichtTabViewModel()
        {
        }

        #endregion // Constructor
    }
}
