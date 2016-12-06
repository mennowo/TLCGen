using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.DataAccess;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// ViewModel for the list of extra detectors, not belonging to a PhaseCyclus
    /// </summary>
    [TLCGenTabItem(index: 1, type: TabItemTypeEnum.DetectieTab)]
    public class DetectorenExtraTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields
        
        private ObservableCollection<string> _Templates;
        private DetectorViewModel _SelectedDetector;
        private IList _SelectedDetectoren = new ArrayList();

        #endregion // Fields

        #region Properties

        private ObservableCollection<DetectorViewModel> _Detectoren;
        public ObservableCollection<DetectorViewModel> Detectoren
        {
            get
            {
                if(_Detectoren == null)
                {
                    _Detectoren = new ObservableCollection<DetectorViewModel>();
                }
                return _Detectoren;
            }
        }

        public ObservableCollection<string> Templates
        {
            get
            {
                if (_Templates == null)
                {
                    _Templates = new ObservableCollection<string>();
                    _Templates.Add("Template placeholder");
                }
                return _Templates;
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

        public IList SelectedDetectoren
        {
            get { return _SelectedDetectoren; }
            set
            {
                _SelectedDetectoren = value;
                OnPropertyChanged("SelectedDetectoren");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddDetectorCommand;
        public ICommand AddDetectorCommand
        {
            get
            {
                if (_AddDetectorCommand == null)
                {
                    _AddDetectorCommand = new RelayCommand(AddNewDetectorCommand_Executed, AddNewDetectorCommand_CanExecute);
                }
                return _AddDetectorCommand;
            }
        }


        RelayCommand _RemoveDetectorCommand;
        public ICommand RemoveDetectorCommand
        {
            get
            {
                if (_RemoveDetectorCommand == null)
                {
                    _RemoveDetectorCommand = new RelayCommand(RemoveDetectorCommand_Executed, RemoveDetectorCommand_CanExecute);
                }
                return _RemoveDetectorCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewDetectorCommand_Executed(object prm)
        {
            DetectorModel dm = new DetectorModel();
            string newname = "001";
            int inewname = 1;
            foreach (DetectorViewModel dvm in Detectoren)
            {
                if (Regex.IsMatch(dvm.Naam, @"[0-9]+"))
                {
                    Match m = Regex.Match(dvm.Naam, @"[0-9]+");
                    string next = m.Value;
                    if (Int32.TryParse(next, out inewname))
                    {
                        IsElementIdentifierUniqueRequest message;
                        do
                        {
                            newname = inewname.ToString("000");
                            message = new IsElementIdentifierUniqueRequest(newname, ElementIdentifierType.Naam);
                            Messenger.Default.Send(message);
                            if(!message.IsUnique) inewname++;
                        }
                        while (!message.IsUnique);
                    }
                }
            }
            dm.Naam = newname;
            dm.Define = SettingsProvider.Default.GetDetectorDefinePrefix() + newname;
            DetectorViewModel dvm1 = new DetectorViewModel(dm);
            Detectoren.Add(dvm1);
        }

        bool AddNewDetectorCommand_CanExecute(object prm)
        {
            return Detectoren != null;
        }

        void RemoveDetectorCommand_Executed(object prm)
        {
            if (SelectedDetectoren != null && SelectedDetectoren.Count > 0)
            {
                // Create temporary List cause we cannot directly remove the selection,
                // as it will cause the selection to change while we loop it
                List<DetectorViewModel> ldvm = new List<DetectorViewModel>();
                foreach (DetectorViewModel dvm in SelectedDetectoren)
                {
                    ldvm.Add(dvm);
                }
                foreach (DetectorViewModel dvm in ldvm)
                {
                    Detectoren.Remove(dvm);
                }
            }
            else if (SelectedDetector != null)
            {
                Detectoren.Remove(SelectedDetector);
            }
        }

        bool RemoveDetectorCommand_CanExecute(object prm)
        {
            return Detectoren != null &&
                (SelectedDetector != null ||
                 SelectedDetectoren != null && SelectedDetectoren.Count > 0);
        }

        #endregion // Command functionality

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Extra";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
        }

        public override void OnDeselected()
        {
            this.Detectoren.BubbleSort();
        }

        #endregion // TabItem Overrides

        #region Collection Changed

        private void Detectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (DetectorViewModel dvm in e.NewItems)
                {
                    _Controller.Detectoren.Add(dvm.Detector);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (DetectorViewModel dvm in e.OldItems)
                {
                    _Controller.Detectoren.Remove(dvm.Detector);
                }
            };
            Messenger.Default.Send(new DetectorenExtraListChangedMessage(_Controller.Detectoren));
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region Constructor

        public DetectorenExtraTabViewModel(ControllerModel controller) : base(controller)
        {
            foreach(DetectorModel dm in _Controller.Detectoren)
            {
                Detectoren.Add(new DetectorViewModel(dm));
            }

            Detectoren.CollectionChanged += Detectoren_CollectionChanged;
        }

        #endregion // Constructor
    }
}
