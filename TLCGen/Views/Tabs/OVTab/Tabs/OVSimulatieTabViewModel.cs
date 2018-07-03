using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 3, type: TabItemTypeEnum.OVTab)]
    public class OVSimulatieTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        private ObservableCollection<DetectorViewModel> _DummyDetectoren;
        public ObservableCollection<DetectorViewModel> DummyDetectoren
        {
            get
            {
                if(_DummyDetectoren == null)
                {
                    _DummyDetectoren = new ObservableCollection<DetectorViewModel>();
                }
                return _DummyDetectoren;
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Simulatie";
            }
        }

        public override bool CanBeEnabled()
        {
            return _Controller?.OVData?.OVIngreepType != Models.Enumerations.OVIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {

        }

        public override ControllerModel Controller
        {
            get { return _Controller; }
            set
            {
                _Controller = value;
                if(_Controller != null)
                {
                    RebuildDetectorenList();
                }
                else
                {
                    _DummyDetectoren = null;
                }
                RaisePropertyChanged("");
            }
        }

        #endregion // TabItem Overrides

        #region Commands

        private RelayCommand _GenerateSimulationValuesCommand;
        public ICommand GenerateSimulationValuesCommand
        {
            get
            {
                if (_GenerateSimulationValuesCommand == null)
                {
                    _GenerateSimulationValuesCommand = new RelayCommand(GenerateSimulationValuesCommand_Executed, GenerateSimulationValuesCommand_CanExecute);
                }
                return _GenerateSimulationValuesCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        private bool GenerateSimulationValuesCommand_CanExecute()
        {
            return DummyDetectoren != null && DummyDetectoren.Count > 0;
        }

        private void GenerateSimulationValuesCommand_Executed()
        {
            foreach (var ov in _Controller.OVData.OVIngrepen)
            {
                if(ov.HasOVIngreepKAR())
                {
                    if(ov.DummyKARInmelding != null)
                    {
                        ov.DummyKARInmelding.Simulatie.Q1 = 3;
                        ov.DummyKARInmelding.Simulatie.Q2 = 5;
                        ov.DummyKARInmelding.Simulatie.Q3 = 10;
                        ov.DummyKARInmelding.Simulatie.Q4 = 15;
                        ov.DummyKARInmelding.Simulatie.Stopline = 1800;
                    }
                    if (ov.DummyKARUitmelding != null)
                    {
                        ov.DummyKARUitmelding.Simulatie.Q1 = 30;
                        ov.DummyKARUitmelding.Simulatie.Q2 = 50;
                        ov.DummyKARUitmelding.Simulatie.Q3 = 100;
                        ov.DummyKARUitmelding.Simulatie.Q4 = 150;
                        ov.DummyKARUitmelding.Simulatie.Stopline = 1800;
                    }
                }
            }

            foreach (var hd in _Controller.OVData.HDIngrepen)
            {
                if (hd.KAR)
                {
                    hd.DummyKARInmelding.Simulatie.Q1 = 3;
                    hd.DummyKARInmelding.Simulatie.Q2 = 5;
                    hd.DummyKARInmelding.Simulatie.Q3 = 10;
                    hd.DummyKARInmelding.Simulatie.Q4 = 15;
                    hd.DummyKARInmelding.Simulatie.Stopline = 1800;
                    hd.DummyKARUitmelding.Simulatie.Q1 = 30;
                    hd.DummyKARUitmelding.Simulatie.Q2 = 50;
                    hd.DummyKARUitmelding.Simulatie.Q3 = 100;
                    hd.DummyKARUitmelding.Simulatie.Q4 = 150;
                    hd.DummyKARUitmelding.Simulatie.Stopline = 1800;
                }
            }

            RaisePropertyChanged("");
            Messenger.Default.Send(new ControllerDataChangedMessage());
            foreach (var d in DummyDetectoren)
            {
                d.RaisePropertyChanged("");
            }
        }

        #endregion // Command functionality

        #region Private methods

        private void RebuildDetectorenList()
        {
            DummyDetectoren.Clear();

            foreach (var ov in Controller.OVData.OVIngrepen)
            {
                if (ov.HasOVIngreepKAR())
                {
                    var m = ov.MeldingenData.Inmeldingen.FirstOrDefault(x => x.Type == Models.Enumerations.OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
                    if (m != null)
                    {
                        DummyDetectoren.Add(new DetectorViewModel(ov.DummyKARInmelding) { FaseCyclus = ov.FaseCyclus });
                    }
                    m = ov.MeldingenData.Uitmeldingen.FirstOrDefault(x => x.Type == Models.Enumerations.OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
                    if (m != null)
                    {
                        DummyDetectoren.Add(new DetectorViewModel(ov.DummyKARUitmelding) { FaseCyclus = ov.FaseCyclus });
                    }
                }
            }
            foreach (var hd in Controller.OVData.HDIngrepen)
            {
                if (hd.KAR)
                {
                    DummyDetectoren.Add(new DetectorViewModel(hd.DummyKARInmelding) { FaseCyclus = hd.FaseCyclus });
                    DummyDetectoren.Add(new DetectorViewModel(hd.DummyKARUitmelding) { FaseCyclus = hd.FaseCyclus });
                }
            }
        }

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen events

        public void OnOVIngrepenChangedMessage(OVIngrepenChangedMessage message)
        {
            RebuildDetectorenList();
        }

        public void OnHDIngrepenChangedMessage(HDIngrepenChangedMessage message)
        {
            RebuildDetectorenList();
        }

		private void OnOVIngreepMeldingChangedMessage(OVIngreepMeldingChangedMessage message)
		{
			RebuildDetectorenList();
		}

        #endregion TLCGen events

        #region Constructor

        public OVSimulatieTabViewModel()
        {
            MessengerInstance.Register(this, new Action<OVIngrepenChangedMessage>(OnOVIngrepenChangedMessage));
            MessengerInstance.Register(this, new Action<HDIngrepenChangedMessage>(OnHDIngrepenChangedMessage));
			MessengerInstance.Register(this, new Action<OVIngreepMeldingChangedMessage>(OnOVIngreepMeldingChangedMessage));
        }

		#endregion // Constructor
	}
}
