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
                    SelectedMeldingenData = new OVIngreepInUitMeldingenDataViewModel(_selectedIngreep.OVIngreep.MeldingenData);
                }
                else
                {
                    SelectedMeldingenData = null;
                }

                RaisePropertyChanged("");
            }
        }

        private OVIngreepInUitMeldingenDataViewModel _selectedMeldingenData;
        public OVIngreepInUitMeldingenDataViewModel SelectedMeldingenData
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
            return _Controller?.OVData?.OVIngreepType != Models.Enumerations.OVIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {
            Ingrepen.Clear();
            SelectedIngreep = null;
            foreach (var ov in _Controller.OVData.OVIngrepen)
            {
                var ovvm = new OVIngreepViewModel(ov);
                Ingrepen.Add(ovvm);
            }
            SelectedIngreep = Ingrepen.FirstOrDefault();
        }

        #endregion // TabItem Overrides

        #region Collection Changed

        #endregion // Collection Changed

        private void OnNeedsFaseCyclus(OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage msg)
        {
            if (msg.RequestingObject is OVIngreepInUitMeldingViewModel ||
                msg.RequestingObject is OVIngreepInUitMeldingenDataViewModel)
            {
                msg.FaseCyclus = SelectedIngreep.OVIngreep.FaseCyclus;
            }
        }

        #region Constructor

        public PrioriteitInUitmeldenTabViewModel() : base()
        {
            MessengerInstance.Register<OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage>(this, OnNeedsFaseCyclus);
        }

        #endregion // Constructor
    }
}
