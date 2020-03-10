using System.Collections.ObjectModel;
using System.Linq;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1, type: TabItemTypeEnum.PrioriteitTab)]
    public class PrioriteitInUitmeldenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<PrioIngreepViewModel> _ingrepen;
        private PrioIngreepViewModel _selectedIngreep;

        #endregion // Fields

        #region Properties

        public ObservableCollection<PrioIngreepViewModel> Ingrepen
        {
            get
            {
                if (_ingrepen == null)
                {
                    _ingrepen = new ObservableCollection<PrioIngreepViewModel>();
                }
                return _ingrepen;
            }
        }

        public PrioIngreepViewModel SelectedIngreep
        {
            get => _selectedIngreep;
            set
            {
                _selectedIngreep = value;
                if (_selectedIngreep != null)
                {
                    SelectedMeldingenData = new PrioIngreepInUitMeldingenDataViewModel(_selectedIngreep.PrioIngreep.MeldingenData);
                }
                else
                {
                    SelectedMeldingenData = null;
                }

                RaisePropertyChanged("");
            }
        }

        private PrioIngreepInUitMeldingenDataViewModel _selectedMeldingenData;
        public PrioIngreepInUitMeldingenDataViewModel SelectedMeldingenData
        {
            get => _selectedMeldingenData;
            set
            {
                _selectedMeldingenData = value;
                RaisePropertyChanged();
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName => "In- en uitmeldingen";

        public override bool CanBeEnabled()
        {
            return _Controller?.PrioData?.PrioIngreepType != Models.Enumerations.PrioIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {
            Ingrepen.Clear();
            SelectedIngreep = null;
            foreach (var ov in _Controller.PrioData.PrioIngrepen)
            {
                var ovvm = new PrioIngreepViewModel(ov);
                Ingrepen.Add(ovvm);
            }
            SelectedIngreep = Ingrepen.FirstOrDefault();
        }

        #endregion // TabItem Overrides

        #region Collection Changed

        #endregion // Collection Changed

        private void OnNeedsFaseCyclus(PrioIngreepMassaDetectieObjectNeedsFaseCyclusMessage msg)
        {
            if (msg.RequestingObject is PrioIngreepInUitMeldingViewModel ||
                msg.RequestingObject is PrioIngreepInUitMeldingenDataViewModel)
            {
                msg.FaseCyclus = SelectedIngreep?.PrioIngreep.FaseCyclus;
            }
        }

        #region Constructor

        public PrioriteitInUitmeldenTabViewModel() : base()
        {
            MessengerInstance.Register<PrioIngreepMassaDetectieObjectNeedsFaseCyclusMessage>(this, OnNeedsFaseCyclus);
        }

        #endregion // Constructor
    }
}
