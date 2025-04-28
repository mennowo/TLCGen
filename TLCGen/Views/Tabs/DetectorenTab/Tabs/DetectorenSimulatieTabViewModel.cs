
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;


namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 40, type: TabItemTypeEnum.DetectieTab)]
    public class DetectorenSimulatieTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        RelayCommand _GenerateSimulationValuesCommand;
        private DetectorViewModel _SelectedDetector;
        public ObservableCollection<DetectorViewModel> _Detectoren;
        private volatile bool _SettingMultiple;

        #endregion // Fields

        #region Properties

        public ObservableCollection<DetectorViewModel> Detectoren { get; } = [];

        public DetectorViewModel SelectedDetector
        {
            get => _SelectedDetector;
            set
            {
                _SelectedDetector = value;
                OnPropertyChanged();
            }
        }

        public IList SelectedDetectoren { get; set; } = new ArrayList();

        #endregion // Properties

        #region Commands

        public ICommand GenerateSimulationValuesCommand => _GenerateSimulationValuesCommand ??= new RelayCommand(() =>
            {
                var rd = new Random();
            
                foreach (var fcm in _Controller.Fasen)
                {
                    var max = 3;
                    var numbers = new List<int> { 200, 100, 50 };
                    var n = rd.Next(numbers.Count);
                    var numbersLow = new List<int> { 2, 4 };
                    var q1 = numbers[n];
                    var r = numbers[n];
                    numbers.Remove(r);
                    --max;
                    n = rd.Next(numbers.Count);
                    var q2 = numbers[n];
                    r = numbers[n];
                    numbers.Remove(r);
                    var q3 = numbers[0];
                    n = rd.Next(2);
                    var q4 = numbersLow[n];

                    foreach (var dm in fcm.Detectoren)
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
                        if (dm.Simulatie.FCNr?.ToUpper() != "NG")
                            dm.Simulatie.FCNr = fcm.Naam;
                    }
                }
                foreach (var dm in _Controller.Detectoren)
                {
                    var max = 3;
                    var numbers = new List<int> { 200, 100, 50 };
                    var n = rd.Next(numbers.Count);
                    var numbersLow = new List<int> { 2, 4 };
                    var q1 = numbers[n];
                    var r = numbers[n];
                    numbers.Remove(r);
                    --max;
                    n = rd.Next(numbers.Count);
                    var q2 = numbers[n];
                    r = numbers[n];
                    numbers.Remove(r);
                    var q3 = numbers[0];
                    n = rd.Next(2);
                    var q4 = numbersLow[n];

                    dm.Simulatie.Q1 = q1;
                    dm.Simulatie.Q2 = q2;
                    dm.Simulatie.Q3 = q3;
                    dm.Simulatie.Q4 = q4;

                    dm.Simulatie.Stopline = 1800;
                    if (dm.Simulatie.FCNr != "NG" && Controller.Fasen.All(x => x.Naam != dm.Simulatie.FCNr))
                        dm.Simulatie.FCNr = "NG";
                }
                foreach (var dm in _Controller.SelectieveDetectoren)
                {
                    var max = 3;
                    var numbers = new List<int> { 200, 100, 50 };
                    var n = rd.Next(numbers.Count);
                    var numbersLow = new List<int> { 2, 4 };
                    var q1 = numbers[n];
                    var r = numbers[n];
                    numbers.Remove(r);
                    --max;
                    n = rd.Next(numbers.Count);
                    var q2 = numbers[n];
                    r = numbers[n];
                    numbers.Remove(r);
                    var q3 = numbers[0];
                    n = rd.Next(2);
                    var q4 = numbersLow[n];

                    dm.Simulatie.Q1 = q1;
                    dm.Simulatie.Q2 = q2;
                    dm.Simulatie.Q3 = q3;
                    dm.Simulatie.Q4 = q4;

                    dm.Simulatie.Stopline = 1800;
                    if (dm.Simulatie.FCNr != "NG" && Controller.Fasen.All(x => x.Naam != dm.Simulatie.FCNr))
                        dm.Simulatie.FCNr = "NG";
                }

                OnPropertyChanged("");
                WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
                foreach(var d in Detectoren)
                {
                    d.OnPropertyChanged("");
                }
            },
            () => Detectoren.Count > 0);

        #endregion // Commands

        #region Command functionality

        private bool GenerateSimulationValuesCommand_CanExecute()
        {
            return Detectoren != null && Detectoren.Count > 0;
        }

        private void GenerateSimulationValuesCommand_Executed()
        {
            var rd = new Random();
            
            foreach (var fcm in _Controller.Fasen)
            {
                var max = 3;
                var numbers = new List<int> { 200, 100, 50 };
                var n = rd.Next(numbers.Count);
                var numbersLow = new List<int> { 2, 4 };
                var q1 = numbers[n];
                var r = numbers[n];
                numbers.Remove(r);
                --max;
                n = rd.Next(numbers.Count);
                var q2 = numbers[n];
                r = numbers[n];
                numbers.Remove(r);
                var q3 = numbers[0];
                n = rd.Next(2);
                var q4 = numbersLow[n];

                foreach (var dm in fcm.Detectoren)
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
                    if (dm.Simulatie.FCNr?.ToUpper() != "NG")
                        dm.Simulatie.FCNr = fcm.Naam;
                }
            }
            foreach (var dm in _Controller.Detectoren)
            {
                var max = 3;
                var numbers = new List<int> { 200, 100, 50 };
                var n = rd.Next(numbers.Count);
                var numbersLow = new List<int> { 2, 4 };
                var q1 = numbers[n];
                var r = numbers[n];
                numbers.Remove(r);
                --max;
                n = rd.Next(numbers.Count);
                var q2 = numbers[n];
                r = numbers[n];
                numbers.Remove(r);
                var q3 = numbers[0];
                n = rd.Next(2);
                var q4 = numbersLow[n];

                dm.Simulatie.Q1 = q1;
                dm.Simulatie.Q2 = q2;
                dm.Simulatie.Q3 = q3;
                dm.Simulatie.Q4 = q4;

                dm.Simulatie.Stopline = 1800;
                if (dm.Simulatie.FCNr != "NG" && Controller.Fasen.All(x => x.Naam != dm.Simulatie.FCNr))
                    dm.Simulatie.FCNr = "NG";
            }
            foreach (var dm in _Controller.SelectieveDetectoren)
            {
                var max = 3;
                var numbers = new List<int> { 200, 100, 50 };
                var n = rd.Next(numbers.Count);
                var numbersLow = new List<int> { 2, 4 };
                var q1 = numbers[n];
                var r = numbers[n];
                numbers.Remove(r);
                --max;
                n = rd.Next(numbers.Count);
                var q2 = numbers[n];
                r = numbers[n];
                numbers.Remove(r);
                var q3 = numbers[0];
                n = rd.Next(2);
                var q4 = numbersLow[n];

                dm.Simulatie.Q1 = q1;
                dm.Simulatie.Q2 = q2;
                dm.Simulatie.Q3 = q3;
                dm.Simulatie.Q4 = q4;

                dm.Simulatie.Stopline = 1800;
                if (dm.Simulatie.FCNr != "NG" && Controller.Fasen.All(x => x.Naam != dm.Simulatie.FCNr))
                    dm.Simulatie.FCNr = "NG";
            }

            OnPropertyChanged("");
WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
            foreach(var d in Detectoren)
            {
                d.OnPropertyChanged("");
            }
        }

        #endregion // Command functionality

        #region Private Methods

        private void UpdateDetectoren()
        {
            foreach (var d in Detectoren) d.PropertyChanged -= Detector_PropertyChanged;
            Detectoren.Clear();
            foreach (var fcm in _Controller.Fasen)
            {
                foreach (var dm in fcm.Detectoren)
                {
                    var dvm = new DetectorViewModel(dm);
                    dvm.PropertyChanged += Detector_PropertyChanged;
                    Detectoren.Add(dvm);
                }
            }
            foreach (var dm in _Controller.Detectoren)
            {
                var dvm = new DetectorViewModel(dm);
                dvm.PropertyChanged += Detector_PropertyChanged;
                Detectoren.Add(dvm);
            }
            foreach (DetectorModel dm in _Controller.SelectieveDetectoren)
            {
                var dvm = new DetectorViewModel(dm);
                dvm.PropertyChanged += Detector_PropertyChanged;
                Detectoren.Add(dvm);
            }
        }

        #endregion // Private Methods

        #region Public methods

        #endregion // Public methods

        #region TabItem Overrides

        public override string DisplayName => "Simulatie";

        public override bool IsEnabled
        {
            get => true;
            set { }
        }

        public override void OnSelected()
        {
            UpdateDetectoren();
        }

        #endregion // TabItem Overrides

        #region TLCGen Events
        
        private void OnDetectorenChanged(object sender, DetectorenChangedMessage message)
        {
            UpdateDetectoren();
            _GenerateSimulationValuesCommand?.NotifyCanExecuteChanged();
        }

        #endregion // TLCGen Events

        #region Event handling

        private void Detector_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_SettingMultiple || string.IsNullOrEmpty(e.PropertyName))
                return;

            if (SelectedDetectoren != null && SelectedDetectoren.Count > 1)
            {
                _SettingMultiple = true;
                MultiPropertySetter.SetPropertyForAllItems<DetectorViewModel>(sender, e.PropertyName, SelectedDetectoren);
            }
            _SettingMultiple = false;
        }

        #endregion // Event Handling

        #region Constructor

        public DetectorenSimulatieTabViewModel() : base()
        {
            WeakReferenceMessengerEx.Default.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
        }

        #endregion // Constructor
    }
}
