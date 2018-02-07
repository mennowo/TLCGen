using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using TLCGen.DataAccess;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class NaloopViewModel : ViewModelBase
    {
        #region Fields

        private readonly NaloopModel _naloop;
        private ObservableCollection<NaloopTijdModel> _tijden;
        private ObservableCollection<NaloopDetectorModel> _detectoren;
        private bool _detectieAfhankelijkPossible;
        
        #endregion // Fields

        #region Properties

        public NaloopTypeEnum Type
        {
            get => _naloop.Type;
            set
            {
                if (value != _naloop.Type)
                {
                    _naloop.Type = value;
                    SetNaloopTijden();
                    RaisePropertyChanged<object>(nameof(Type), broadcast: true);
                }
            }
        }

        public bool VasteNaloop
        {
            get => _naloop.VasteNaloop;
            set
            {
                if (!value && !DetectieAfhankelijk)
                    return;

                _naloop.VasteNaloop = value;
                SetNaloopTijden();
                RaisePropertyChanged<object>(nameof(VasteNaloop), broadcast: true);
            }
        }
        public bool InrijdenTijdensGroen
        {
            get => _naloop.InrijdenTijdensGroen;
            set
            {
                _naloop.InrijdenTijdensGroen = value;
                RaisePropertyChanged<object>(nameof(InrijdenTijdensGroen), broadcast: true);
            }
        }

        public bool DetectieAfhankelijkPossible
        {
            get => _detectieAfhankelijkPossible;
            set
            {
                _detectieAfhankelijkPossible = value;
                RaisePropertyChanged();
            }
        }


        public bool DetectieAfhankelijk
        {
            get => _naloop.DetectieAfhankelijk;
            set
            {
                if (!value && !VasteNaloop)
                    return;

                _naloop.DetectieAfhankelijk = value;
                SetNaloopTijden();
                RaisePropertyChanged<object>(nameof(DetectieAfhankelijk), broadcast: true);
            }
        }

        public int? MaximaleVoorstart
        {
            get => _naloop.MaximaleVoorstart;
            set
            {
                _naloop.MaximaleVoorstart = value;
                RaisePropertyChanged<object>(nameof(MaximaleVoorstart), broadcast: true);
            }
        }

        public ObservableCollection<NaloopTijdModel> Tijden => _tijden ?? (_tijden = new ObservableCollection<NaloopTijdModel>());

        public ObservableCollection<NaloopDetectorModel> Detectoren => _detectoren ?? (_detectoren = new ObservableCollection<NaloopDetectorModel>());

        public NaloopDetectorModel SelectedDetector
        {
            get => DetectorManager?.SelectedDetector;
            set
            {
                if (DetectorManager == null) return;
                DetectorManager.SelectedDetector = value;
                RaisePropertyChanged();
            }
        }

        private DetectorManagerViewModel<NaloopDetectorModel, string> _detectorManager;
        public DetectorManagerViewModel<NaloopDetectorModel, string> DetectorManager
        {
            get
            {
                if (_detectorManager == null && _naloop?.FaseVan != null)
                {
                    var dets =
                        TLCGenControllerDataProvider.Default.Controller.Fasen.
                            First(x => x.Naam == _naloop.FaseVan).
                                Detectoren.
                                Select(x => x.Naam).
                                ToList();
                    _detectorManager = new DetectorManagerViewModel<NaloopDetectorModel, string>(
                        Detectoren,
                        dets,
                        x => { var md = new NaloopDetectorModel { Detector = x }; return md; },
                        x => { return Detectoren.All(y => y.Detector != x); },
                        null,
                        () => { RaisePropertyChanged(nameof(SelectedDetector)); },
                        () => { RaisePropertyChanged(nameof(SelectedDetector)); }
                        );
                }
                return _detectorManager;
            }
        }

        #endregion // Properties

        #region Private methods

        private void SetNaloopTijden()
        {
            var oldtijden = _naloop.Tijden;
            _naloop.Tijden = new List<NaloopTijdModel>();
            switch (_naloop.Type)
            {
                case NaloopTypeEnum.StartGroen:
                    if(_naloop.VasteNaloop)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.StartGroen });
                    }
                    if (_naloop.DetectieAfhankelijk)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.StartGroenDetectie });
                    }
                    break;
                case NaloopTypeEnum.EindeGroen:
                    if (_naloop.VasteNaloop)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroen });
                    }
                    if (_naloop.DetectieAfhankelijk)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroenDetectie });
                    }
                    if (_naloop.VasteNaloop)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeGroen });
                    }
                    if (_naloop.DetectieAfhankelijk)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeGroenDetectie });
                    }
                    break;
                case NaloopTypeEnum.CyclischVerlengGroen:
                    if (_naloop.VasteNaloop)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroen });
                    }
                    if (_naloop.DetectieAfhankelijk)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroenDetectie });
                    }
                    if (_naloop.VasteNaloop)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeVerlengGroen });
                    }
                    if (_naloop.DetectieAfhankelijk)
                    {
                        _naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeVerlengGroenDetectie });
                    }
                    break;
            }
            foreach(var tmo in oldtijden)
            {
                foreach(var tmn in _naloop.Tijden)
                {
                    if(tmo.Type == tmn.Type)
                    {
                        tmn.Waarde = tmo.Waarde;
                    }
                }
            }
            _tijden = new ObservableCollection<NaloopTijdModel>(_naloop.Tijden);
            RaisePropertyChanged(nameof(Tijden));
        }

        #endregion // Private methods

        #region Collection changed

        private void Detectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (NaloopDetectorModel d in e.NewItems)
                {
                    _naloop.Detectoren.Add(d);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (NaloopDetectorModel d in e.OldItems)
                {
                    _naloop.Detectoren.Remove(d);
                }
            }
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        private void Tijden_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (NaloopTijdModel t in e.NewItems)
                {
                    _naloop.Tijden.Add(t);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (NaloopTijdModel t in e.OldItems)
                {
                    _naloop.Tijden.Remove(t);
                }
            }
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection changed

        #region TLCGen Events

        private void OnDetectorenChanged(DetectorenChangedMessage message)
        {
            if (Detectoren != null)
            {
                Detectoren.CollectionChanged -= Detectoren_CollectionChanged;
                Detectoren.Clear();
                foreach (var ndm in _naloop.Detectoren)
                {
                    Detectoren.Add(ndm);
                }
                Detectoren.CollectionChanged += Detectoren_CollectionChanged;
            }

            _detectorManager = null;
            RaisePropertyChanged(nameof(DetectorManager));
        }

        private void OnNameChanged(NameChangedMessage msg)
        {
            _detectorManager = null;
            RaisePropertyChanged(nameof(DetectorManager));
        }
        
        #endregion // TLCGen Events

        #region Constructor

        public NaloopViewModel(NaloopModel nm)
        {
            _naloop = nm;

            foreach (var ndm in nm.Detectoren)
            {
                Detectoren.Add(ndm);
            }
            foreach(var ntm in nm.Tijden)
            {
                Tijden.Add(ntm);
            }

            SetNaloopTijden();
            Detectoren.CollectionChanged += Detectoren_CollectionChanged;
            Tijden.CollectionChanged += Tijden_CollectionChanged;

            Messenger.Default.Register(this, new Action<DetectorenChangedMessage>(OnDetectorenChanged));
            Messenger.Default.Register(this, new Action<NameChangedMessage>(OnNameChanged));
        }

        #endregion // Constructor
    }
}
