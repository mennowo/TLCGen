using System.Collections;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Plugins.RangeerElementen.Models;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

namespace TLCGen.Plugins.RangeerElementen.ViewModels
{
    public class RangeerElementenTabViewModel : ViewModelBase
    {
        #region Fields

        private readonly RangeerElementenPlugin _plugin;
        private RangeerElementenDataModel _rangeerElementenModel;
        private RelayCommand _moveDetectorUpCommand;
        private RelayCommand _moveDetectorDownCommand;
        private RelayCommand _moveSignalGroupUpCommand;
        private RelayCommand _moveSignalGroupDownCommand;
        private RangeerElementViewModel _selectedRangeerDetector;
        private RangeerSignalGroupViewModel _selectedRangeerSignalGroup; 
        private IList _selectedRangeerDetectors = new ArrayList();
        private IList _selectedRangeerSignalGroups = new ArrayList();

        #endregion // Fields
        
        #region Properties

        public RangeerElementViewModel SelectedRangeerDetector
        {
            get => _selectedRangeerDetector;
            set
            {
                _selectedRangeerDetector = value;
                RaisePropertyChanged();
            }
        }

        public IList SelectedRangeerDetectors
        {
            get => _selectedRangeerDetectors;
            set
            {
                _selectedRangeerDetectors = value;
                RaisePropertyChanged();
            }
        }

        public RangeerSignalGroupViewModel SelectedRangeerSignalGroup
        {
            get => _selectedRangeerSignalGroup;
            set
            {
                _selectedRangeerSignalGroup = value;
                RaisePropertyChanged();
            }
        }

        public IList SelectedRangeerSignalGroups
        {
            get => _selectedRangeerSignalGroups;
            set
            {
                _selectedRangeerSignalGroups = value;
                RaisePropertyChanged();
            }
        }

        public RangeerElementenDataModel RangeerElementenModel
        {
            get => _rangeerElementenModel;
            set
            {
                _rangeerElementenModel = value;
                if (RangeerDetectors != null) RangeerDetectors.CollectionChanged -= RangeerElementen_CollectionChanged;
                RangeerDetectors = new ObservableCollectionAroundList<RangeerElementViewModel, RangeerElementModel>(_rangeerElementenModel.RangeerElementen);
                RangeerDetectors.CollectionChanged += RangeerElementen_CollectionChanged;

                if (RangeerSignalGroups != null) RangeerSignalGroups.CollectionChanged -= RangeerSignalGroups_CollectionChanged;
                RangeerSignalGroups = new ObservableCollectionAroundList<RangeerSignalGroupViewModel, RangeerSignaalGroepModel>(_rangeerElementenModel.RangeerSignaalGroepen);
                RangeerSignalGroups.CollectionChanged += RangeerSignalGroups_CollectionChanged;
            }
        }

        private void RangeerElementen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RangeerDetectors.RebuildList();
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        private void RangeerSignalGroups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RangeerSignalGroups.RebuildList();
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        public ObservableCollectionAroundList<RangeerElementViewModel, RangeerElementModel> RangeerDetectors { get; private set; }
        public ObservableCollectionAroundList<RangeerSignalGroupViewModel, RangeerSignaalGroepModel> RangeerSignalGroups { get; private set; }

        public bool RangeerElementenToepassen
        {
            get => _rangeerElementenModel.RangeerElementenToepassen;
            set
            {
                _rangeerElementenModel.RangeerElementenToepassen = value;
                if (value)
                {
                    _plugin.UpdateModel();
                    RangeerDetectors.RebuildList();
                    RangeerDetectors.BubbleSort();
                }
                else
                {
                    RangeerDetectors.RemoveAll();
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool RangeerSignaalGroepenToepassen
        {
            get => _rangeerElementenModel.RangeerSignaalGroepenToepassen;
            set
            {
                _rangeerElementenModel.RangeerSignaalGroepenToepassen = value;
                if (value)
                {
                    _plugin.UpdateModel();
                    RangeerSignalGroups.RebuildList();
                    RangeerSignalGroups.BubbleSort();
                }
                else
                {
                    RangeerSignalGroups.RemoveAll();
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand MoveDetectorUpCommand => _moveDetectorUpCommand ?? (_moveDetectorUpCommand = new RelayCommand(MoveDetectorUpCommand_executed, MoveDetectorUpCommand_canExecute));

        private bool MoveDetectorUpCommand_canExecute()
        {
            return SelectedRangeerDetector != null && RangeerDetectors.IndexOf(SelectedRangeerDetector) > 0 
                   || SelectedRangeerDetectors != null && SelectedRangeerDetectors.Count > 0;
        }

        private void MoveDetectorUpCommand_executed()
        {
            if (SelectedRangeerDetectors != null && SelectedRangeerDetectors.Count > 0)
            {
                foreach (RangeerElementViewModel elem in SelectedRangeerDetectors)
                {
                    var i = RangeerDetectors.IndexOf(elem);
                    if (i > 0)
                        RangeerDetectors.Move(i, i - 1);
                    else
                        break;
                }
            }
            else
            {
                var i = RangeerDetectors.IndexOf(SelectedRangeerDetector);
                if (i > 0)
                    RangeerDetectors.Move(i, i - 1);
            }
            RangeerDetectors.RebuildList();
        }

        public ICommand MoveDetectorDownCommand => _moveDetectorDownCommand ?? (_moveDetectorDownCommand = new RelayCommand(MoveDetectorDownCommand_executed, MoveDetectorDownCommand_canExecute));

        private bool MoveDetectorDownCommand_canExecute()
        {
            return SelectedRangeerDetector != null && RangeerDetectors.IndexOf(SelectedRangeerDetector) < RangeerDetectors.Count - 1
                   || SelectedRangeerDetectors != null && SelectedRangeerDetectors.Count > 0;
        }

        private void MoveDetectorDownCommand_executed()
        {
            if (SelectedRangeerDetectors != null && SelectedRangeerDetectors.Count > 0)
            {
                var ok = true;
                var list = new List<RangeerElementViewModel>();
                foreach (RangeerElementViewModel elem in SelectedRangeerDetectors)
                {
                    if (RangeerDetectors.IndexOf(elem) + 1 >= RangeerDetectors.Count)
                    {
                        ok = false;
                        break;
                    }
                    list.Add(elem);
                }
                if(!ok) return;

                list.Reverse();
                foreach (var elem in list)
                {
                    var i = RangeerDetectors.IndexOf(elem);
                    if (i < RangeerDetectors.Count - 1)
                        RangeerDetectors.Move(i, i + 1);
                    else
                        break;
                }
            }
            else
            {
                var i = RangeerDetectors.IndexOf(SelectedRangeerDetector);
                if (i < RangeerDetectors.Count - 1)
                    RangeerDetectors.Move(i, i + 1);
            }

            RangeerDetectors.RebuildList();
        }

        
        public ICommand MoveSignalGroupUpCommand => _moveSignalGroupUpCommand ?? (_moveSignalGroupUpCommand = new RelayCommand(MoveSignalGroupUpCommand_executed, MoveSignalGroupUpCommand_canExecute));

        private bool MoveSignalGroupUpCommand_canExecute()
        {
            return SelectedRangeerSignalGroup != null && RangeerSignalGroups.IndexOf(SelectedRangeerSignalGroup) > 0 
                   || SelectedRangeerSignalGroups != null && SelectedRangeerSignalGroups.Count > 0;
        }

        private void MoveSignalGroupUpCommand_executed()
        {
            if (SelectedRangeerSignalGroups != null && SelectedRangeerSignalGroups.Count > 0)
            {
                foreach (RangeerSignalGroupViewModel elem in SelectedRangeerSignalGroups)
                {
                    var i = RangeerSignalGroups.IndexOf(elem);
                    if (i > 0)
                        RangeerSignalGroups.Move(i, i - 1);
                    else
                        break;
                }
            }
            else
            {
                var i = RangeerSignalGroups.IndexOf(SelectedRangeerSignalGroup);
                if (i > 0)
                    RangeerSignalGroups.Move(i, i - 1);
            }
            RangeerSignalGroups.RebuildList();
        }

        public ICommand MoveSignalGroupDownCommand => _moveSignalGroupDownCommand ?? (_moveSignalGroupDownCommand = new RelayCommand(MoveSignalGroupDownCommand_executed, MoveSignalGroupDownCommand_canExecute));

        private bool MoveSignalGroupDownCommand_canExecute()
        {
            return SelectedRangeerSignalGroup != null && RangeerSignalGroups.IndexOf(SelectedRangeerSignalGroup) < RangeerSignalGroups.Count - 1
                   || SelectedRangeerSignalGroups != null && SelectedRangeerSignalGroups.Count > 0;
        }

        private void MoveSignalGroupDownCommand_executed()
        {
            if (SelectedRangeerSignalGroups != null && SelectedRangeerSignalGroups.Count > 0)
            {
                var ok = true;
                var list = new List<RangeerSignalGroupViewModel>();
                foreach (RangeerSignalGroupViewModel elem in SelectedRangeerSignalGroups)
                {
                    if (RangeerSignalGroups.IndexOf(elem) + 1 >= RangeerSignalGroups.Count)
                    {
                        ok = false;
                        break;
                    }
                    list.Add(elem);
                }
                if(!ok) return;

                list.Reverse();
                foreach (var elem in list)
                {
                    var i = RangeerSignalGroups.IndexOf(elem);
                    if (i < RangeerSignalGroups.Count - 1)
                        RangeerSignalGroups.Move(i, i + 1);
                    else
                        break;
                }
            }
            else
            {
                var i = RangeerSignalGroups.IndexOf(SelectedRangeerSignalGroup);
                if (i < RangeerSignalGroups.Count - 1)
                    RangeerSignalGroups.Move(i, i + 1);
            }

            RangeerSignalGroups.RebuildList();
        }

        #endregion // Commands

        #region TLCGen messaging

        private void OnDetectorenChanged(DetectorenChangedMessage msg)
        {
            if (!RangeerElementenToepassen) return;
            
            if (msg.RemovedDetectoren != null && msg.RemovedDetectoren.Any())
            {
                foreach (var d in msg.RemovedDetectoren)
                {
                    var RangeerElementenFc = RangeerDetectors.FirstOrDefault(x => x.Element == d.Naam);
                    if (RangeerElementenFc != null)
                    {
                        RangeerDetectors.Remove(RangeerElementenFc);
                    }
                }
            }
            if (msg.AddedDetectoren != null && msg.AddedDetectoren.Any())
            {
                foreach (var d in msg.AddedDetectoren)
                {
                    var rangeerElementenD = new RangeerElementViewModel(
                                new RangeerElementModel { Element = d.Naam });
                    RangeerDetectors.Add(rangeerElementenD);
                }
            }
        }

        
        private void OnFasenChanged(FasenChangedMessage msg)
        {
            if (!RangeerSignaalGroepenToepassen) return;
            
            if (msg.RemovedFasen != null && msg.RemovedFasen.Any())
            {
                foreach (var rFc in msg.RemovedFasen)
                {
                    var rangeerElementenFc = RangeerSignalGroups.FirstOrDefault(x => x.SignalGroup == rFc.Naam);
                    if (rangeerElementenFc != null)
                    {
                        RangeerSignalGroups.Remove(rangeerElementenFc);
                    }
                }
            }
            if (msg.AddedFasen != null && msg.AddedFasen.Any())
            {
                foreach (var d in msg.AddedFasen)
                {
                    var RangeerElementenFc = new RangeerSignalGroupViewModel(
                        new RangeerSignaalGroepModel { SignaalGroep = d.Naam });
                    RangeerSignalGroups.Add(RangeerElementenFc);
                }
            }
        }

        private void OnNameChanged(NameChangedMessage msg)
        {
            if (msg.ObjectType == TLCGen.Models.Enumerations.TLCGenObjectTypeEnum.Detector)
            {
                TLCGenModelManager.Default.ChangeNameOnObject(RangeerElementenModel, msg.OldName, msg.NewName, TLCGen.Models.Enumerations.TLCGenObjectTypeEnum.Detector);
                RangeerDetectors.Rebuild();
            }
            if (msg.ObjectType == TLCGen.Models.Enumerations.TLCGenObjectTypeEnum.Fase)
            {
                TLCGenModelManager.Default.ChangeNameOnObject(RangeerElementenModel, msg.OldName, msg.NewName, TLCGen.Models.Enumerations.TLCGenObjectTypeEnum.Fase);
                RangeerSignalGroups.Rebuild();
            }
        }

        #endregion // TLCGen messaging

        #region Private Methods 

        

        #endregion // Private Methods 

        #region Public Methods

        public void UpdateMessaging()
        {
            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            MessengerInstance.Register<FasenChangedMessage>(this, OnFasenChanged);
            MessengerInstance.Register<NameChangedMessage>(this, OnNameChanged);
            MessengerInstance.Register<PrepareForGenerationRequest>(this, OnPrepareForGenerationRequestReceived);
        }

        private void OnPrepareForGenerationRequestReceived(PrepareForGenerationRequest obj)
        {
            var c = obj.Controller;



            if (_rangeerElementenModel.RangeerSignaalGroepenToepassen)
            {
                foreach (var sg in c.Fasen)
                {
                    var rangeSg = _rangeerElementenModel.RangeerSignaalGroepen.FirstOrDefault(x => x.SignaalGroep == sg.Naam);
                    if (rangeSg != null)
                    {
                        sg.RangeerIndex = rangeSg.RangeerIndex;
                    }
                }
            }

            if (_rangeerElementenModel.RangeerElementenToepassen)
            {
                for (int i = 0; i < _rangeerElementenModel.RangeerElementen.Count; i++)
                {
                    _rangeerElementenModel.RangeerElementen[i].RangeerIndex = i;
                }
                foreach (var d in c.GetAllDetectors())
                {
                    var rangeD = _rangeerElementenModel.RangeerElementen.FirstOrDefault(x => x.Element == d.Naam);
                    if (rangeD != null)
                    {
                        d.RangeerIndex = rangeD.RangeerIndex;
                    }
                }
            }
        }

        #endregion // Public Methods

        #region Constructor

        public RangeerElementenTabViewModel(RangeerElementenPlugin plugin)
        {
            _plugin = plugin;
        }

        #endregion // Constructor
    }
}
