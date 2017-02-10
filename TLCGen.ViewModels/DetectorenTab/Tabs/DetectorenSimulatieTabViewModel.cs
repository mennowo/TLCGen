using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Models;

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
                OnPropertyChanged("SelectedDetector");
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
            int[] qs = { 25, 50, 100, 200,
                         5,  25, 50,  200,
                         5,  25, 100, 200,
                         5,  50, 100, 200 };
            int qsmax = 16;

            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                int qthis = rd.Next(qsmax);
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    dm.Simulatie.Q1 = qs[qthis];
                    int next = qthis + 1;
                    if (next >= qsmax) next -= qsmax;
                    dm.Simulatie.Q2 = qs[next];
                    ++next;
                    if (next >= qsmax) next -= qsmax;
                    dm.Simulatie.Q3 = qs[next];
                    ++next;
                    if (next >= qsmax) next -= qsmax;
                    dm.Simulatie.Q4 = qs[next];

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
                int qthis = rd.Next(qsmax);
                dm.Simulatie.Q1 = qs[qthis];
                int next = qthis + 1;
                if (next >= qsmax) next -= qsmax;
                dm.Simulatie.Q2 = qs[next];
                ++next;
                if (next >= qsmax) next -= qsmax;
                dm.Simulatie.Q3 = qs[next];
                ++next;
                if (next >= qsmax) next -= qsmax;
                dm.Simulatie.Q4 = qs[next];
                dm.Simulatie.Stopline = 1800;
            }

            OnMonitoredPropertyChanged(null);
            foreach(var d in Detectoren)
            {
                d.OnMonitoredPropertyChanged(null);
            }
        }

        #endregion // Command functionality

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
        }

        #endregion // TabItem Overrides

        #region Constructor

        public DetectorenSimulatieTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
