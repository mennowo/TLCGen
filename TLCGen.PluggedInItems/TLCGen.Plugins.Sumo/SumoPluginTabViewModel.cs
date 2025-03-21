﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models.Enumerations;
using WindowsInput;

namespace TLCGen.Plugins.Sumo
{
    public class SumoPluginTabViewModel : ViewModelBase
    {
        #region Fields

        SumoDataModel _data;
        SumoPlugin _plugin;
        HotKey _namingHotkey = null;

        #endregion // Fields

        #region Properties

        public SumoDataModel Data
        {
            get => _data;
            set
            {
                _data = value;
                if (_data != null)
                {
                    FaseCycli = new ObservableCollectionAroundList<FaseCyclusSumoDataViewModel, FaseCyclusSumoDataModel>(_data.FaseCycli);
                    Detectoren = new ObservableCollectionAroundList<DetectorSumoDataViewModel, DetectorSumoDataModel>(_data.Detectoren);
                }
                RaisePropertyChanged("");
            }
        }

        public bool GenererenSumoCode
        {
            get => _data != null && _data.GenererenSumoCode;
            set
            {
                if (_data != null)
                {
                    _data.GenererenSumoCode = value;
                    RaisePropertyChanged<object>(nameof(GenererenSumoCode), broadcast: true);
                }
            }
        }

        public int SumoPort
        {
            get => _data != null ? _data.SumoPort : 0;
            set
            {
                if (_data != null)
                {
                    _data.SumoPort = value;
                    RaisePropertyChanged<object>(nameof(SumoPort), broadcast: true);
                }
            }
        }

        public int SumoOrder
        {
            get => _data != null ? _data.SumoOrder : 0;
            set
            {
                if (_data != null)
                {
                    _data.SumoOrder = value;
                    RaisePropertyChanged<object>(nameof(SumoOrder), broadcast: true);
                }
            }
        }

        public int StartTijdUur
        {
            get => _data != null ? _data.StartTijdUur : 0;
            set
            {
                if (_data != null)
                {
                    _data.StartTijdUur = value;
                    RaisePropertyChanged<object>(nameof(StartTijdUur), broadcast: true);
                }
            }
        }

        public int StartTijdMinuut
        {
            get => _data != null ? _data.StartTijdMinuut : 0;
            set
            {
                if (_data != null)
                {
                    _data.StartTijdMinuut = value;
                    RaisePropertyChanged<object>(nameof(StartTijdMinuut), broadcast: true);
                }
            }
        }

        public string SumoKruispuntNaam
        {
            get => _data != null ? _data.SumoKruispuntNaam : "";
            set
            {
                if (_data != null)
                {
                    _data.SumoKruispuntNaam = value;
                    RaisePropertyChanged<object>(nameof(SumoKruispuntNaam), broadcast: true);
                }
            }
        }

        public int SumoKruispuntLinkMax
        {
            get => _data != null ? _data.SumoKruispuntLinkMax : 0;
            set
            {
                if (_data != null)
                {
                    _data.SumoKruispuntLinkMax = value;
                    RaisePropertyChanged<object>(nameof(SumoKruispuntLinkMax), broadcast: true);
                }
            }
        }

        public bool PrependIdToDetectors
        {
            get => _data?.PrependIdToDetectors ?? false;
            set
            {
                if (_data != null)
                {
                    _data.PrependIdToDetectors = value;
                    RaisePropertyChanged<object>(nameof(PrependIdToDetectors), broadcast: true);
                }
            }
        }

        public bool AutoStartSumo
        {
            get => _data?.AutoStartSumo ?? false;
            set
            {
                if (_data != null)
                {
                    _data.AutoStartSumo = value;
                    RaisePropertyChanged<object>(nameof(AutoStartSumo), broadcast: true);
                }
            }
        }

        public string SumoHomePath
        {
            get => _data?.SumoHomePath ?? "";
            set
            {
                if (_data != null)
                {
                    _data.SumoHomePath = value;
                    RaisePropertyChanged<object>(nameof(SumoHomePath), broadcast: true);
                }
            }
        }

        public string SumoConfigPath
        {
            get => _data?.SumoConfigPath ?? "";
            set
            {
                if (_data != null)
                {
                    _data.SumoConfigPath = value;
                    RaisePropertyChanged<object>(nameof(SumoConfigPath), broadcast: true);
                }
            }
        }

        public ObservableCollectionAroundList<FaseCyclusSumoDataViewModel, FaseCyclusSumoDataModel> FaseCycli
        {
            get;
            private set;
        }

        public ObservableCollectionAroundList<DetectorSumoDataViewModel, DetectorSumoDataModel> Detectoren
        {
            get;
            private set;
        }

        public DetectorSumoDataViewModel SelectedDetector
        {
            get => _selectedDetector;
            set
            {
                _selectedDetector = value;
                RaisePropertyChanged();
            }
        }
        #endregion // Properties

        #region Commands

        private GalaSoft.MvvmLight.CommandWpf.RelayCommand _startSUMODetectorNamingCommand;
        public ICommand StartSUMODetectorNamingCommand => _startSUMODetectorNamingCommand ?? (_startSUMODetectorNamingCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(() =>
            {
                _namingHotkey = new HotKey(Key.F6, KeyModifier.None, OnHotKeyHandler);
            },
            () => _namingHotkey == null && SelectedDetector != null));

        private GalaSoft.MvvmLight.CommandWpf.RelayCommand _stopSUMODetectorNamingCommand;
        private DetectorSumoDataViewModel _selectedDetector;
        private GalaSoft.MvvmLight.CommandWpf.RelayCommand _getLinkIdsFromNetworkCommand;

        public ICommand StopSUMODetectorNamingCommand => _stopSUMODetectorNamingCommand ?? (_stopSUMODetectorNamingCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(() =>
            {
            _namingHotkey.Unregister();
            _namingHotkey.Dispose();
            _namingHotkey = null;
            },
            () => _namingHotkey != null));

        private void OnHotKeyHandler(HotKey hotKey)
        {
            var name = (PrependIdToDetectors ? SumoKruispuntNaam : "") + SelectedDetector.SumoNaam1;
            var index = Detectoren.IndexOf(SelectedDetector);
            if (index < Detectoren.Count - 1)
            {
                SelectedDetector = Detectoren[index + 1];
            }
            var sim = new InputSimulator();
            Clipboard.SetText(name);
            sim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL);
            sim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_A);
            sim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL);
            sim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL);
            sim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_V);
            sim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL);
        }

        public ICommand GetLinkIdsFromNetworkCommand => _getLinkIdsFromNetworkCommand ?? (_getLinkIdsFromNetworkCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(() =>
        {
            foreach (var f in FaseCycli)
            {
                f.SumoIds = "";
            }

            var linkIds = new Dictionary<string, string>();
            XmlDocument sumoConfig = new XmlDocument();
            sumoConfig.Load(SumoConfigPath);
            XmlNode node = sumoConfig.DocumentElement.SelectSingleNode("/configuration/input/net-file");
            if (node == null) node = sumoConfig.DocumentElement.SelectSingleNode("/sumoConfiguration/input/net-file");
            if (node.Attributes.Count > 0)
            {
                var netFile = node.Attributes["value"];
                var sumoConfigPath = Path.GetDirectoryName(SumoConfigPath);
                if (!string.IsNullOrEmpty(netFile.InnerText) && (File.Exists(netFile.InnerText) || File.Exists(Path.Combine(sumoConfigPath, netFile.InnerText))))
                { 
                    XmlDocument sumoNet = new XmlDocument();
                    if (File.Exists(netFile.InnerText))
                    {
                        sumoNet.Load(netFile.InnerText);
                    }
                    else if (File.Exists(Path.Combine(sumoConfigPath, netFile.InnerText)))
                    {
                        sumoNet.Load(Path.Combine(sumoConfigPath, netFile.InnerText));
                    }
                    XmlNode netNode = sumoNet.DocumentElement.SelectSingleNode("/net");
                    foreach (XmlNode n in netNode.ChildNodes)
                    {
                        if (n.Name == "connection")
                        {
                            if (n.ChildNodes.Count > 0)
                            {
                                foreach (XmlNode p in n.ChildNodes)
                                {
                                    if (p.Name == "param")
                                    {
                                        var key = p.Attributes["key"];
                                        var value = p.Attributes["value"];
                                        if (key.InnerText == "fc" 
                                            && n.Attributes["linkIndex"] != null
                                            && n.Attributes["tl"] != null
                                            && n.Attributes["tl"].InnerText == SumoKruispuntNaam)
                                        {
                                            if (linkIds.ContainsKey(value.InnerText))
                                            {
                                                linkIds[value.InnerText] = linkIds[value.InnerText] + "," + n.Attributes["linkIndex"].InnerText;
                                            }
                                            else
                                            {
                                                linkIds.Add(value.InnerText, n.Attributes["linkIndex"].InnerText);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (var link in linkIds)
            {
                var fc = FaseCycli.FirstOrDefault(x => x.Naam == link.Key);
                if (fc != null)
                {
                    fc.SumoIds = link.Value;
                }
            }
        }, () => !string.IsNullOrEmpty(SumoConfigPath) && File.Exists(SumoConfigPath)));

        #endregion // Commands

        #region Command Functionality

        #endregion // Command Functionality

        #region Public Methods

        public void UpdateTLCGenMessaging()
        {
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<DetectorenChangedMessage>(OnDetectorenChanged));
            Messenger.Default.Register(this, new Action<NameChangedMessage>(OnNameChanged));
            Messenger.Default.Register(this, new Action<FasenSortedMessage>(OnFasenSorted));
        }

        #endregion // Public Methods

        #region Private Methods

        #endregion // Private Methods

        #region TLCGen Events

        private void OnModelManagerMessage(PrioIngreepMeldingChangedMessage message)
        {
            _plugin.UpdateModel();
        }

        private void OnFasenChanged(FasenChangedMessage message)
        {
            if (message.AddedFasen?.Count > 0)
            {
                foreach (var fc in message.AddedFasen)
                {
                    FaseCycli.Add(
                        new FaseCyclusSumoDataViewModel(
                            new FaseCyclusSumoDataModel { Naam = fc.Naam, SumoIds = "" }));
                }
            }
            if (message.RemovedFasen?.Count > 0)
            {
                foreach (var fc in message.RemovedFasen)
                {
                    var rfc = FaseCycli.FirstOrDefault(x => x.Naam == fc.Naam);
                    if (rfc != null)
                    {
                        FaseCycli.Remove(rfc);
                    }
                }
            }
            FaseCycli.Rebuild();
        }

        private void OnDetectorenChanged(DetectorenChangedMessage message)
        {
            if (message.AddedDetectoren?.Count > 0)
            {
                foreach (var d in message.AddedDetectoren)
                {
                    Detectoren.Add(
                        new DetectorSumoDataViewModel(
                            new DetectorSumoDataModel
                            {
                                Naam = d.Naam,
                                SumoNaam1 = d.Naam
                            }));
                }
            }
            if (message.RemovedDetectoren?.Count > 0)
            {
                foreach (var fc in message.RemovedDetectoren)
                {
                    var rd = Detectoren.FirstOrDefault(x => x.Naam == fc.Naam);
                    if (rd != null)
                    {
                        Detectoren.Remove(rd);
                    }
                }
            }
            if (message.AddedDetectoren == null && message.RemovedDetectoren == null)
            {
                _plugin.UpdateModel();
            }
            Detectoren.Rebuild();
        }

        private void OnNameChanged(NameChangedMessage message)
        {
            switch (message.ObjectType)
            {
                case Models.Enumerations.TLCGenObjectTypeEnum.Fase:
                    var fc = FaseCycli.FirstOrDefault(x => x.Naam == message.OldName);
                    if (fc != null)
                    {
                        fc.Naam = message.NewName;
                    }
                    break;
                case Models.Enumerations.TLCGenObjectTypeEnum.Detector:
                    var d = Detectoren.FirstOrDefault(x => x.Naam == message.OldName);
                    if (d != null)
                    {
                        d.Naam = message.NewName;
                    }
                    break;
                case Models.Enumerations.TLCGenObjectTypeEnum.Output:
                    break;
                case Models.Enumerations.TLCGenObjectTypeEnum.Input:
                    break;
            }
        }

        private void OnFasenSorted(FasenSortedMessage message)
        {
            FaseCycli.BubbleSort();
        }

        #endregion // TLCGen Events

        #region Constructor

        public SumoPluginTabViewModel(SumoPlugin plugin, IMessenger messenger = null) : base(messenger)
        {
            _plugin = plugin;
        }

        #endregion // Constructor
    }
}
