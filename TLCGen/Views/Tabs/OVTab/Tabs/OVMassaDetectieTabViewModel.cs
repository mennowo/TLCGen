using System.Collections.ObjectModel;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 4, type: TabItemTypeEnum.OVTab)]
    public class OVMassaDetectieTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<FaseCyclusViewModel> _Fasen;
        private FaseCyclusViewModel _SelectedFaseCyclus;
        private OVIngreepMassaDetectieDataViewModel _SelectedMassaDetectieData;
        private bool _isMassaDetectieDisabled;

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
                SelectedMassaDetectieData = null;
                if (_SelectedFaseCyclus != null)
                {
                    foreach (OVIngreepModel ovm in _Controller.OVData.OVIngrepen)
                    {
                        if (ovm.FaseCyclus == SelectedFaseCyclus.Naam && ovm.MassaDetectie)
                        {
                            SelectedMassaDetectieData = new OVIngreepMassaDetectieDataViewModel(ovm.MassaDetectieData);
                            break;
                        }
                    }
                }

                RaisePropertyChanged("");

                IsMassaDetectieDisabled = SelectedMassaDetectieData != null;
            }
        }

        public OVIngreepMassaDetectieDataViewModel SelectedMassaDetectieData
        {
            get => _SelectedMassaDetectieData;
            set
            {
                _SelectedMassaDetectieData = value;
                RaisePropertyChanged();
            }
        }

        public bool IsMassaDetectieDisabled
        {
            get => _isMassaDetectieDisabled;
            set
            {
                _isMassaDetectieDisabled = value;
                RaisePropertyChanged();
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Massa detectie";
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
            if (msg.RequestingObject is OVIngreepMassaDetectieMeldingVoorwaardeViewModel vw)
            {
                msg.FaseCyclus = SelectedFaseCyclus.Naam;
            }
        }

        #region Constructor

        public OVMassaDetectieTabViewModel() : base()
        {
            MessengerInstance.Register<OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage>(this, OnNeedsFaseCyclus);
        }

        #endregion // Constructor
    }
}
