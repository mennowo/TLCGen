﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models.Enumerations;

namespace TLCGen.Plugins.Sumo
{
    public class SumoPluginTabViewModel : ViewModelBase
    {
        #region Fields

        SumoDataModel _data;
        SumoPlugin _plugin;

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
                    RaisePropertyChanged<object>("GenererenSumoCode", broadcast: true);
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
                    RaisePropertyChanged<object>("SumoPort", broadcast: true);
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
                    RaisePropertyChanged<object>("SumoOrder", broadcast: true);
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
                    RaisePropertyChanged<object>("StartTijdUur", broadcast: true);
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
                    RaisePropertyChanged<object>("StartTijdMinuut", broadcast: true);
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
                    RaisePropertyChanged<object>("SumoKruispuntNaam", broadcast: true);
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
                    RaisePropertyChanged<object>("SumoKruispuntLinkMax", broadcast: true);
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
                    RaisePropertyChanged<object>("AutoStartSumo", broadcast: true);
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
                    RaisePropertyChanged<object>("SumoHomePath", broadcast: true);
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
                    RaisePropertyChanged<object>("SumoConfigPath", broadcast: true);
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

        #endregion // Properties

        #region Commands

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

        private void OnModelManagerMessage(OVIngreepMeldingChangedMessage message)
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
                                SumoNaam1 = d.Naam,
                                SumoNaam2 = d.Naam,
                                SumoNaam3 = d.Naam
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
            if(message.AddedDetectoren == null && message.RemovedDetectoren == null)
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
