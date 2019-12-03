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

        private ObservableCollection<OVIngreepViewModel> _ingrepen;
        private OVIngreepViewModel _selectedIngreep;

        #endregion // Fields

        #region Properties

        public ObservableCollection<OVIngreepViewModel> Ingrepen
        {
            get
            {
                if (_ingrepen == null)
                {
                    _ingrepen = new ObservableCollection<OVIngreepViewModel>();
                }
                return _ingrepen;
            }
        }

        public OVIngreepViewModel SelectedIngreep
        {
            get { return _selectedIngreep; }
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

        public override string DisplayName
        {
            get
            {
                return "In- en uitmeldingen";
            }
        }

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
                var ovvm = new OVIngreepViewModel(ov);
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
                msg.FaseCyclus = SelectedIngreep.PrioIngreep.FaseCyclus;
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
