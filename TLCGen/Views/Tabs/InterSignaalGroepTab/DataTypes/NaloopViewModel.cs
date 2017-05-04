using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class NaloopViewModel : ViewModelBase
    {
        #region Fields

        private NaloopModel _Naloop;
        private ObservableCollection<NaloopTijdModel> _Tijden;
        private ObservableCollection<NaloopDetectorModel> _Detectoren;
        private bool _DetectieAfhankelijkPossible;
        
        #endregion // Fields

        #region Properties

        public NaloopTypeEnum Type
        {
            get { return _Naloop.Type; }
            set
            {
                if (value != _Naloop.Type)
                {
                    _Naloop.Type = value;
                    SetNaloopTijden();
                    RaisePropertyChanged<NaloopViewModel>("Type", broadcast: true);
                }
            }
        }

        public bool VasteNaloop
        {
            get { return _Naloop.VasteNaloop; }
            set
            {
                if (!value && !DetectieAfhankelijk)
                    return;

                _Naloop.VasteNaloop = value;
                SetNaloopTijden();
                RaisePropertyChanged<NaloopViewModel>("VasteNaloop", broadcast: true);
            }
        }

        public bool DetectieAfhankelijkPossible
        {
            get { return _DetectieAfhankelijkPossible; }
            set
            {
                _DetectieAfhankelijkPossible = value;
                RaisePropertyChanged("DetectieAfhankelijkPossible");
            }
        }


        public bool DetectieAfhankelijk
        {
            get { return _Naloop.DetectieAfhankelijk; }
            set
            {
                if (!value && !VasteNaloop)
                    return;

                _Naloop.DetectieAfhankelijk = value;
                SetNaloopTijden();
                RaisePropertyChanged<NaloopViewModel>("DetectieAfhankelijk", broadcast: true);
            }
        }

        public int? MaximaleVoorstart
        {
            get { return _Naloop.MaximaleVoorstart; }
            set
            {
                _Naloop.MaximaleVoorstart = value;
                RaisePropertyChanged<NaloopViewModel>("MaximaleVoorstart", broadcast: true);
            }
        }

        public ObservableCollection<NaloopTijdModel> Tijden
        {
            get
            {
                if (_Tijden == null)
                    _Tijden = new ObservableCollection<NaloopTijdModel>();
                return _Tijden;
            }
        }

        public ObservableCollection<NaloopDetectorModel> Detectoren
        {
            get
            {
                if (_Detectoren == null)
                    _Detectoren = new ObservableCollection<NaloopDetectorModel>();
                return _Detectoren;
            }
        }

        public NaloopDetectorModel SelectedDetector
        {
            get { return DetectorManager?.SelectedDetector; }
            set
            {
                if (DetectorManager != null)
                {
                    DetectorManager.SelectedDetector = value;
                    RaisePropertyChanged("SelectedDetector");
                }
            }
        }

        private DetectorManagerViewModel<NaloopDetectorModel, string> _DetectorManager;
        public DetectorManagerViewModel<NaloopDetectorModel, string> DetectorManager
        {
            get
            {
                if (_DetectorManager == null && _Naloop != null && _Naloop.FaseVan != null)
                {
                    List<string> dets =
                        DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.
                            Where(x => x.Naam == _Naloop.FaseVan).
                            First().
                            Detectoren.
                            Select(x => x.Naam).
                            ToList();
                    _DetectorManager = new DetectorManagerViewModel<NaloopDetectorModel, string>(
                        Detectoren,
                        dets,
                        (x) => { var md = new NaloopDetectorModel() { Detector = x }; return md; },
                        (x) => { return !Detectoren.Where(y => y.Detector == x).Any(); },
                        null,
                        () => { RaisePropertyChanged("SelectedDetector"); },
                        () => { RaisePropertyChanged("SelectedDetector"); }
                        );
                }
                return _DetectorManager;
            }
        }

        #endregion // Properties

        #region Private methods

        private void SetNaloopTijden()
        {
            List<NaloopTijdModel> _oldtijden = _Naloop.Tijden;
            _Naloop.Tijden = new List<NaloopTijdModel>();
            switch (_Naloop.Type)
            {
                case NaloopTypeEnum.StartGroen:
                    if(_Naloop.VasteNaloop)
                    {
                        _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.StartGroen });
                    }
                    if (_Naloop.DetectieAfhankelijk)
                    {
                        _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.StartGroenDetectie });
                    }
                    break;
                case NaloopTypeEnum.EindeGroen:
                    if (_Naloop.VasteNaloop)
                    {
                        _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroen });
                    }
                    if (_Naloop.DetectieAfhankelijk)
                    {
                        _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroenDetectie });
                    }
                    if (_Naloop.VasteNaloop)
                    {
                        _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeGroen });
                    }
                    if (_Naloop.DetectieAfhankelijk)
                    {
                        _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeGroenDetectie });
                    }
                    break;
                case NaloopTypeEnum.CyclischVerlengGroen:
                    if (_Naloop.VasteNaloop)
                    {
                        _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroen });
                    }
                    if (_Naloop.DetectieAfhankelijk)
                    {
                        _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroenDetectie });
                    }
                    if (_Naloop.VasteNaloop)
                    {
                        _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeVerlengGroen });
                    }
                    if (_Naloop.DetectieAfhankelijk)
                    {
                        _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeVerlengGroenDetectie });
                    }
                    break;
            }
            foreach(NaloopTijdModel tmo in _oldtijden)
            {
                foreach(NaloopTijdModel tmn in _Naloop.Tijden)
                {
                    if(tmo.Type == tmn.Type)
                    {
                        tmn.Waarde = tmo.Waarde;
                    }
                }
            }
            _Tijden = new ObservableCollection<NaloopTijdModel>(_Naloop.Tijden);
            RaisePropertyChanged("Tijden");
        }

        #endregion // Private methods

        #region Collection changed

        private void Detectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (NaloopDetectorModel d in e.NewItems)
                {
                    _Naloop.Detectoren.Add(d);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (NaloopDetectorModel d in e.OldItems)
                {
                    _Naloop.Detectoren.Remove(d);
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
                    _Naloop.Tijden.Add(t);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (NaloopTijdModel t in e.OldItems)
                {
                    _Naloop.Tijden.Remove(t);
                }
            }
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection changed

        #region TLCGen Events

        private void OnDetectorenChanged(DetectorenChangedMessage message)
        {
            _DetectorManager = null;
            RaisePropertyChanged("DetectorManager");

            if (Detectoren?.Count == 0)
                return;

            Detectoren.CollectionChanged += Detectoren_CollectionChanged;
            foreach (NaloopDetectorModel ndm in _Naloop.Detectoren)
            {
                Detectoren.Add(ndm);
            }
            Detectoren.CollectionChanged += Detectoren_CollectionChanged;
        }
        
        #endregion // TLCGen Events

        #region Constructor

        public NaloopViewModel(NaloopModel nm)
        {
            _Naloop = nm;

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
        }

        #endregion // Constructor
    }
}
