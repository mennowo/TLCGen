using System.Collections.ObjectModel;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 4, type: TabItemTypeEnum.OVTab)]
    public class OVInUitmeldenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<FaseCyclusViewModel> _Fasen;
        private FaseCyclusViewModel _SelectedFaseCyclus;

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusViewModel> Fasen
        {
            get
            {
                if (_Fasen == null)
                {
                    _Fasen = new ObservableCollection<FaseCyclusViewModel>();
                }
                return _Fasen;
            }
        }

        public FaseCyclusViewModel SelectedFaseCyclus
        {
            get { return _SelectedFaseCyclus; }
            set
            {
                _SelectedFaseCyclus = value;
                SelectedMeldingenData = null;
                if (_SelectedFaseCyclus != null)
                {
                    foreach (OVIngreepModel ovm in _Controller.OVData.OVIngrepen)
                    {
                        if (ovm.FaseCyclus == SelectedFaseCyclus.Naam)
                        {
                            SelectedMeldingenData = new OVIngreepInUitMeldingenDataViewModel(ovm.MeldingenData);
                            break;
                        }
                    }
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
            var temp = SelectedFaseCyclus;
            Fasen.Clear();
            SelectedFaseCyclus = null;
            foreach (var fcm in _Controller.Fasen)
            {
                var fcvm = new FaseCyclusViewModel(fcm);
                Fasen.Add(fcvm);
                if (temp == null || fcvm.Naam != temp.Naam) continue;
                SelectedFaseCyclus = fcvm;
                temp = null;
            }
            if (SelectedFaseCyclus == null && Fasen.Count > 0)
            {
                SelectedFaseCyclus = Fasen[0];
            }
        }

        #endregion // TabItem Overrides

        #region Collection Changed

        #endregion // Collection Changed

        private void OnNeedsFaseCyclus(OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage msg)
        {
            if (msg.RequestingObject is OVIngreepInUitMeldingViewModel ||
                msg.RequestingObject is OVIngreepInUitMeldingenDataViewModel)
            {
                msg.FaseCyclus = SelectedFaseCyclus.Naam;
            }
        }

        #region Constructor

        public OVInUitmeldenTabViewModel() : base()
        {
            MessengerInstance.Register<OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage>(this, OnNeedsFaseCyclus);
        }

        #endregion // Constructor
    }
}
