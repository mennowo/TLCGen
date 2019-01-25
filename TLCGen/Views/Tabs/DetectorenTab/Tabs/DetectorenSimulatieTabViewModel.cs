using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 4, type: TabItemTypeEnum.DetectieTab)]
    public class DetectorenSimulatieTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        RelayCommand _GenerateSimulationValuesCommand;
        private DetectorViewModel _SelectedDetector;
        public ObservableCollection<DetectorViewModel> _Detectoren;

        #endregion // Fields

        #region Properties

        public ObservableCollection<DetectorViewModel> Detectoren
        {
            get
            {
                if (_Detectoren == null)
                    _Detectoren = new ObservableCollection<DetectorViewModel>();
                return _Detectoren;
            }
        }

        public DetectorViewModel SelectedDetector
        {
            get { return _SelectedDetector; }
            set
            {
                _SelectedDetector = value;
                RaisePropertyChanged("SelectedDetector");
            }
        }

        #endregion // Properties

        #region Commands

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

        private bool GenerateSimulationValuesCommand_CanExecute(object obj)
        {
            return Detectoren != null && Detectoren.Count > 0;
        }

        private void GenerateSimulationValuesCommand_Executed(object obj)
        {
            Random rd = new Random();
            
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                var max = 3;
                var n = rd.Next(max);
                var numbers = new List<int> { 200, 100, 50 };
                var numbersLow = new List<int> { 25, 10 };
                var q1 = numbers[n];
                numbers.Remove(n);
                --max;
                n = rd.Next(max);
                var q2 = numbers[n];
                numbers.Remove(n);
                var q3 = numbers[0];
                n = rd.Next(2);
                var q4 = numbersLow[n];

                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    dm.Simulatie.Q1 = q1;
                    dm.Simulatie.Q2 = q2;
                    dm.Simulatie.Q3 = q3;
                    dm.Simulatie.Q4 = q4;

                    switch (fcm.Type)
                    {
                        case Models.Enumerations.FaseTypeEnum.Auto:
                            dm.Simulatie.Stopline = 1800;
                            break;
                        case Models.Enumerations.FaseTypeEnum.Fiets:
                            dm.Simulatie.Stopline = 5000;
                            break;
                        case Models.Enumerations.FaseTypeEnum.Voetganger:
                            dm.Simulatie.Stopline = 10000;
                            break;
                    }
                    dm.Simulatie.FCNr = fcm.Naam;
                }
            }
            foreach (DetectorModel dm in _Controller.Detectoren)
            {
                var max = 3;
                var n = rd.Next(max);
                var numbers = new List<int> { 200, 100, 50 };
                var numbersLow = new List<int> { 25, 10 };
                dm.Simulatie.Q1 = numbers[n];
                numbers.Remove(n);
                n = rd.Next(max);
                dm.Simulatie.Q2 = numbers[n];
                numbers.Remove(n);
                n = rd.Next(max);
                dm.Simulatie.Q3 = numbers[n];
                n = rd.Next(2);
                dm.Simulatie.Q4 = numbersLow[n];
                dm.Simulatie.Stopline = 1800;
            }
            foreach (DetectorModel dm in _Controller.SelectieveDetectoren)
            {
                var max = 3;
                var n = rd.Next(max);
                var numbers = new List<int> { 200, 100, 50 };
                var numbersLow = new List<int> { 25, 10 };
                dm.Simulatie.Q1 = numbers[n];
                numbers.Remove(n);
                n = rd.Next(max);
                dm.Simulatie.Q2 = numbers[n];
                numbers.Remove(n);
                n = rd.Next(max);
                dm.Simulatie.Q3 = numbers[n];
                n = rd.Next(2);
                dm.Simulatie.Q4 = numbersLow[n];
                dm.Simulatie.Stopline = 1800;
            }

            RaisePropertyChanged("");
            Messenger.Default.Send(new ControllerDataChangedMessage());
            foreach(var d in Detectoren)
            {
                d.RaisePropertyChanged("");
            }
        }

        #endregion // Command functionality

        #region Private Methods

        private void UpdateDetectoren()
        {
            Detectoren.Clear();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    Detectoren.Add(new DetectorViewModel(dm));
                }
            }
            foreach (DetectorModel dm in _Controller.Detectoren)
            {
                Detectoren.Add(new DetectorViewModel(dm));
            }
            foreach (DetectorModel dm in _Controller.SelectieveDetectoren)
            {
                Detectoren.Add(new DetectorViewModel(dm));
            }
        }

        #endregion // Private Methods

        #region Public methods

        #endregion // Public methods

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Simulatie";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
            UpdateDetectoren();
        }

        #endregion // TabItem Overrides

        #region TLCGen Events
        
        private void OnDetectorenChanged(DetectorenChangedMessage message)
        {
            UpdateDetectoren();
        }

        #endregion // TLCGen Events

        #region Constructor

        public DetectorenSimulatieTabViewModel() : base()
        {
            Messenger.Default.Register(this, new Action<DetectorenChangedMessage>(OnDetectorenChanged));
        }

        #endregion // Constructor
    }
}
