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
using TLCGen.ModelManagement;
using TLCGen.Plugins.RangeerElementen.Models;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

namespace TLCGen.Plugins.RangeerElementen.ViewModels
{
    public class RangeerElementenTabViewModel : ViewModelBase
    {
        #region Fields

        private RangeerElementenPlugin _plugin;
        private RelayCommand _moveUpCommand;
        private RelayCommand _moveDownCommand;
        private RangeerElementViewModel _selectedRangeerElement;
        private IList _selectedRangeerElements = new ArrayList();

        #endregion // Fields
        
        #region Properties

        public RangeerElementViewModel SelectedRangeerElement
        {
            get => _selectedRangeerElement;
            set
            {
                _selectedRangeerElement = value;
                RaisePropertyChanged();
            }
        }

        public IList SelectedRangeerElements
        {
            get => _selectedRangeerElements;
            set
            {
                _selectedRangeerElements = value;
                RaisePropertyChanged();
            }
        }

        private RangeerElementenDataModel _rangeerElementenModel;
        public RangeerElementenDataModel RangeerElementenModel
        {
            get => _rangeerElementenModel;
            set
            {
                _rangeerElementenModel = value;
                if (RangeerElementen != null) RangeerElementen.CollectionChanged -= RangeerElementen_CollectionChanged;
                RangeerElementen = new ObservableCollectionAroundList<RangeerElementViewModel, RangeerElementModel>(_rangeerElementenModel.RangeerElementen);
                RangeerElementen.CollectionChanged += RangeerElementen_CollectionChanged;
            }
        }

        private void RangeerElementen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        public ObservableCollectionAroundList<RangeerElementViewModel, RangeerElementModel> RangeerElementen { get; private set; }

        public bool RangeerElementenToepassen
        {
            get => _rangeerElementenModel.RangeerElementenToepassen;
            set
            {
                _rangeerElementenModel.RangeerElementenToepassen = value;
                if (value)
                {
                    _plugin.UpdateModel();
                    RangeerElementen.Rebuild();
                    RangeerElementen.BubbleSort();
                }
                else
                {
                    RangeerElementen.RemoveAll();
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand MoveUpCommand => _moveUpCommand ?? (_moveUpCommand = new RelayCommand(MoveUpCommand_executed, MoveUpCommand_canExecute));

        private bool MoveUpCommand_canExecute()
        {
            return SelectedRangeerElement != null && RangeerElementen.IndexOf(SelectedRangeerElement) > 0 
                   || SelectedRangeerElements != null && SelectedRangeerElements.Count > 0;
        }

        private void MoveUpCommand_executed()
        {
            if (SelectedRangeerElements != null && SelectedRangeerElements.Count > 0)
            {
                foreach (RangeerElementViewModel elem in SelectedRangeerElements)
                {
                    var i = RangeerElementen.IndexOf(elem);
                    if (i > 0)
                        RangeerElementen.Move(i, i - 1);
                    else
                        break;
                }
            }
            else
            {
                var i = RangeerElementen.IndexOf(SelectedRangeerElement);
                if (i > 0)
                    RangeerElementen.Move(i, i - 1);
            }
            RangeerElementen.RebuildList();
        }

        public ICommand MoveDownCommand => _moveDownCommand ?? (_moveDownCommand = new RelayCommand(MoveDownCommand_executed, MoveDownCommand_canExecute));

        private bool MoveDownCommand_canExecute()
        {
            return SelectedRangeerElement != null && RangeerElementen.IndexOf(SelectedRangeerElement) < RangeerElementen.Count - 1
                   || SelectedRangeerElements != null && SelectedRangeerElements.Count > 0;
        }

        private void MoveDownCommand_executed()
        {
            if (SelectedRangeerElements != null && SelectedRangeerElements.Count > 0)
            {
                var ok = true;
                var list = new List<RangeerElementViewModel>();
                foreach (RangeerElementViewModel elem in SelectedRangeerElements)
                {
                    if (RangeerElementen.IndexOf(elem) + 1 >= RangeerElementen.Count)
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
                    var i = RangeerElementen.IndexOf(elem);
                    if (i < RangeerElementen.Count - 1)
                        RangeerElementen.Move(i, i + 1);
                    else
                        break;
                }
            }
            else
            {
                var i = RangeerElementen.IndexOf(SelectedRangeerElement);
                if (i < RangeerElementen.Count - 1)
                    RangeerElementen.Move(i, i + 1);
            }

            RangeerElementen.RebuildList();
        }

        #endregion // Commands

        #region TLCGen messaging

        private void OnDetectorenenChanged(DetectorenChangedMessage msg)
        {
            if (!RangeerElementenToepassen) return;
            
            if (msg.RemovedDetectoren != null && msg.RemovedDetectoren.Any())
            {
                foreach (var d in msg.RemovedDetectoren)
                {
                    var RangeerElementenFc = RangeerElementen.FirstOrDefault(x => x.Element == d.Naam);
                    if (RangeerElementenFc != null)
                    {
                        RangeerElementen.Remove(RangeerElementenFc);
                    }
                }
            }
            if (msg.AddedDetectoren != null && msg.AddedDetectoren.Any())
            {
                foreach (var d in msg.AddedDetectoren)
                {
                    var RangeerElementenfc = new RangeerElementViewModel(
                                new RangeerElementModel { Element = d.Naam });
                    RangeerElementen.Add(RangeerElementenfc);
                }
            }
        }

        private void OnNameChanged(NameChangedMessage msg)
        {
            if (msg.ObjectType == TLCGen.Models.Enumerations.TLCGenObjectTypeEnum.Detector)
            {
                TLCGenModelManager.Default.ChangeNameOnObject(RangeerElementenModel, msg.OldName, msg.NewName, TLCGen.Models.Enumerations.TLCGenObjectTypeEnum.Detector);
                RangeerElementen.Rebuild();
            }
        }

        private void OnGenerated(ControllerCodeGeneratedMessage obj)
        {
            if (!_rangeerElementenModel.RangeerElementenToepassen) return;
            var sysFile =
                Path.Combine(Path.GetDirectoryName(DataAccess.TLCGenControllerDataProvider.Default.ControllerFileName), _plugin.Controller.Data.Naam + "sys.h");
            if (File.Exists(sysFile))
            {
                var sb = new StringBuilder();
                var lines = File.ReadAllLines(sysFile);
                var inserted = false;
                var detectieFound = false;
                foreach (var l in lines)
                {
                    if (!inserted && Regex.IsMatch(l, $@"\s*#define\s+{_plugin.Dpf}[0-9a-zA-Z_]+\s+[0-9]+.*"))
                    {
                        var m = Regex.Match(l, $@"\s*#define\s+(?<def>{_plugin.Dpf}[0-9a-zA-Z_]+)\s+[0-9]+.*");
                        if (m.Success)
                        {
                            detectieFound = true;
                            var rd = RangeerElementen.FirstOrDefault(x => _plugin.Dpf + x.Element == m.Groups["def"].Value);
                            if (rd != null)
                            {
                                continue;
                            }
                        }
                    }
                    if (!inserted && detectieFound &&
                        (Regex.IsMatch(l, $@"\s*#ifndef\s+AUTOMAAT.*") || 
                         Regex.IsMatch(l, $@"\s*#if\s+!defined\s+AUTOMAAT.*") || 
                         Regex.IsMatch(l, $@"\s*#if\s+!\(defined\s+AUTOMAAT.*") ||
                         Regex.IsMatch(l, $@"\s*#if\s+\(!defined\s+AUTOMAAT.*")))
                    {
                        sb.Append(GetNewDetectorDefines());
                        inserted = true;
                    }
                    if (!inserted && Regex.IsMatch(l, $@"\s*#define\s+DPMAX.*"))
                    {
                        sb.Append(GetNewDetectorDefines());
                        inserted = true;
                    }
                    sb.AppendLine(l);
                }
                try
                {
                    File.Delete(sysFile);
                    File.WriteAllText(sysFile, sb.ToString(), Encoding.Default);
                }
                catch
                {
                    MessageBox.Show("De rangeer elementen plugin kan de sys.h niet overschrijven; is die elders open?", "Fout bij overschrijven sys.h");
                }
            }
        }

        #endregion // TLCGen messaging

        #region Private Methods 

        private string GetNewDetectorDefines()
        {
            var sb = new StringBuilder();

            int pad1 = "ISMAX".Length;
            if (RangeerElementen.Any())
            {
                pad1 = RangeerElementen.Max(x => (_plugin.Dpf + x.Element).Length);
            }
            if (_plugin.Controller.SelectieveDetectoren.Any())
            {
                int _pad1 = _plugin.Controller.SelectieveDetectoren.Max(x => (_plugin.Dpf + x.Naam).Length);
                pad1 = _pad1 > pad1 ? _pad1 : pad1;
            }
            var ovdummies = _plugin.Controller.PrioData.GetAllDummyDetectors();
            if (ovdummies.Any())
            {
                pad1 = ovdummies.Max(x => (_plugin.Dpf + x.Naam).Length);
            }
            pad1 = pad1 + $"{_plugin.Ts}#define  ".Length;

            int pad2 = _plugin.Controller.Fasen.Count.ToString().Length;

            int index = 0;
            foreach (var dm in RangeerElementen)
            {
                sb.Append($"{_plugin.Ts}#define {_plugin.Dpf}{dm.Element} ".PadRight(pad1));
                sb.AppendLine($"{index.ToString()}".PadLeft(pad2));
                ++index;
            }

            return sb.ToString();
        }

        #endregion // Private Methods 

        #region Public Methods

        public void UpdateMessaging()
        {
            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenenChanged);
            MessengerInstance.Register<NameChangedMessage>(this, OnNameChanged);
            MessengerInstance.Register<ControllerCodeGeneratedMessage>(this, OnGenerated);
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
