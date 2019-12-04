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
    [TLCGenTabItem(index: 5, type: TabItemTypeEnum.PrioriteitTab)]
    public class PrioriteitSimulatieTabViewModel : TLCGenTabItemViewModel
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
            return _Controller?.PrioData?.PrioIngreepType != Models.Enumerations.PrioIngreepTypeEnum.Geen;
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
            foreach (var prio in _Controller.PrioData.PrioIngrepen)
            {
                if(prio.HasOVIngreepKAR())
                {
                    if(prio.DummyKARInmelding != null)
                    {
                        prio.DummyKARInmelding.Simulatie.Q1 = 3;
                        prio.DummyKARInmelding.Simulatie.Q2 = 5;
                        prio.DummyKARInmelding.Simulatie.Q3 = 10;
                        prio.DummyKARInmelding.Simulatie.Q4 = 15;
                        prio.DummyKARInmelding.Simulatie.Stopline = 1800;
                    }
                    if (prio.DummyKARUitmelding != null)
                    {
                        prio.DummyKARUitmelding.Simulatie.Q1 = 200;
                        prio.DummyKARUitmelding.Simulatie.Q2 = 200;
                        prio.DummyKARUitmelding.Simulatie.Q3 = 200;
                        prio.DummyKARUitmelding.Simulatie.Q4 = 200;
                        prio.DummyKARUitmelding.Simulatie.Stopline = 1800;
                    }
                }
            }

            foreach (var hd in _Controller.PrioData.HDIngrepen)
            {
                if (hd.KAR)
                {
                    hd.DummyKARInmelding.Simulatie.Q1 = 3;
                    hd.DummyKARInmelding.Simulatie.Q2 = 5;
                    hd.DummyKARInmelding.Simulatie.Q3 = 10;
                    hd.DummyKARInmelding.Simulatie.Q4 = 15;
                    hd.DummyKARInmelding.Simulatie.Stopline = 1800;
                    hd.DummyKARUitmelding.Simulatie.Q1 = 200;
                    hd.DummyKARUitmelding.Simulatie.Q2 = 200;
                    hd.DummyKARUitmelding.Simulatie.Q3 = 200;
                    hd.DummyKARUitmelding.Simulatie.Q4 = 200;
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

            foreach (var prio in Controller.PrioData.PrioIngrepen)
            {
                if (prio.HasOVIngreepKAR())
                {
                    var m = prio.MeldingenData.Inmeldingen.FirstOrDefault(x => x.Type == Models.Enumerations.PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
                    if (m != null)
                    {
                        DummyDetectoren.Add(new DetectorViewModel(prio.DummyKARInmelding) { FaseCyclus = prio.FaseCyclus });
                    }
                    m = prio.MeldingenData.Uitmeldingen.FirstOrDefault(x => x.Type == Models.Enumerations.PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
                    if (m != null)
                    {
                        DummyDetectoren.Add(new DetectorViewModel(prio.DummyKARUitmelding) { FaseCyclus = prio.FaseCyclus });
                    }
                }
            }
            foreach (var hd in Controller.PrioData.HDIngrepen)
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

        public void OnPrioIngrepenChangedMessage(PrioIngrepenChangedMessage message)
        {
            RebuildDetectorenList();
        }

        public void OnHDIngrepenChangedMessage(HDIngrepenChangedMessage message)
        {
            RebuildDetectorenList();
        }

		private void OnPrioIngreepMeldingChangedMessage(PrioIngreepMeldingChangedMessage message)
		{
			RebuildDetectorenList();
		}

        #endregion TLCGen events

        #region Constructor

        public PrioriteitSimulatieTabViewModel()
        {
            MessengerInstance.Register(this, new Action<PrioIngrepenChangedMessage>(OnPrioIngrepenChangedMessage));
            MessengerInstance.Register(this, new Action<HDIngrepenChangedMessage>(OnHDIngrepenChangedMessage));
			MessengerInstance.Register(this, new Action<PrioIngreepMeldingChangedMessage>(OnPrioIngreepMeldingChangedMessage));
        }

		#endregion // Constructor
	}
}
