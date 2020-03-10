using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Settings;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 0, type: TabItemTypeEnum.PrioriteitTab)]
    public class PrioIngrepenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<FaseCyclusWithPrioViewModel> _fasen;
        private RelayCommand _removeItemCommand;
        private RelayCommand _removeIngreepCommand;
        private object _selectedObject;
        private FaseCyclusWithPrioViewModel _selectedFaseCyclus;
        private PrioIngreepMeldingenListViewModel _selectedMeldingenList;
        private PrioIngreepViewModel _selectedIngreep;
        private PrioIngreepInUitMeldingViewModel _selectedMelding;

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusWithPrioViewModel> Fasen => _fasen ?? (_fasen = new ObservableCollection<FaseCyclusWithPrioViewModel>());

        public object SelectedObject
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value;
                switch (SelectedObject)
                {
                    case FaseCyclusWithPrioViewModel fc:
                        SelectedMeldingenList = null;
                        SelectedIngreep = null;
                        SelectedMelding = null;
                        SelectedFaseCyclus = fc;
                        break;
                    case PrioIngreepMeldingenListViewModel list:
                        SelectedFaseCyclus = null;
                        SelectedIngreep = null;
                        SelectedMelding = null;
                        SelectedMeldingenList = list;
                        break;
                    case PrioIngreepViewModel ing:
                        SelectedFaseCyclus = null;
                        SelectedMeldingenList = null;
                        SelectedMelding = null;
                        SelectedIngreep = ing;
                        break;
                    case PrioIngreepInUitMeldingViewModel mel:
                        SelectedFaseCyclus = null;
                        SelectedIngreep = null;
                        SelectedMeldingenList = null;
                        SelectedMelding = mel;
                        break;
                    default:
                        SelectedFaseCyclus = null;
                        SelectedIngreep = null;
                        SelectedMeldingenList = null;
                        SelectedMelding = null;
                        break;
                }
                RaisePropertyChanged();
            }
        }

        public FaseCyclusWithPrioViewModel SelectedFaseCyclus
        {
            get => _selectedFaseCyclus;
            set
            {
                _selectedFaseCyclus = value; 
                RaisePropertyChanged();
            }
        }
        
        public PrioIngreepViewModel SelectedIngreep
        {
            get => _selectedIngreep;
            set
            {
                _selectedIngreep = value; 
                RaisePropertyChanged();
            }
        }
        
        public PrioIngreepMeldingenListViewModel SelectedMeldingenList
        {
            get => _selectedMeldingenList;
            set
            {
                _selectedMeldingenList = value; 
                RaisePropertyChanged();
            }
        }

        public PrioIngreepInUitMeldingViewModel SelectedMelding
        {
            get => _selectedMelding;
            set
            {
                _selectedMelding = value; 
                RaisePropertyChanged();
            }
        }

        #endregion // Properties

        
        #region Commands

        #endregion // Commands

        #region TabItem Overrides

        public override string DisplayName => "Ingrepen";

        public override bool CanBeEnabled()
        {
            return _Controller?.PrioData?.PrioIngreepType != PrioIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {
            foreach (var fcm in _Controller.Fasen)
            {
                var fcvm = 
                    new FaseCyclusWithPrioViewModel(
                        fcm.Naam, 
                        _Controller.PrioData.PrioIngrepen.Where(x => x.FaseCyclus == fcm.Naam).ToList());
                Fasen.Add(fcvm);
            }
        }

        #endregion // TabItem Overrides

        #region Private Methods
        #endregion // Private Methods

        #region Constructor

        public PrioIngrepenTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
