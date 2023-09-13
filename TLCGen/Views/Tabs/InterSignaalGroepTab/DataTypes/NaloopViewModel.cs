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
using TLCGen.Helpers;

namespace TLCGen.ViewModels
{
    public class NaloopViewModel : ViewModelBase
    {
        #region Fields

        private readonly NaloopModel _naloop;
        private ObservableCollectionAroundList<NaloopTijdViewModel, NaloopTijdModel> _tijden;
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
                    RaisePropertyChanged(nameof(ShowNotSupportedInCCOLWarning));
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
                if (value)
                {
                    MaximaleVoorstart = null;
                }
                RaisePropertyChanged(nameof(MaximaleVoorstartAllowed));
                RaisePropertyChanged(nameof(ShowNotSupportedInCCOLWarning));
            }
        }

        public bool InrijdenTijdensGroenPossible => 
            TLCGenControllerDataProvider.Default.Controller.Data.SynchronisatiesType != SynchronisatiesTypeEnum.RealFunc ||
            TLCGenControllerDataProvider.Default.Controller.Data.SynchronisatiesType != SynchronisatiesTypeEnum.InterFunc;

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
                if (value && !Detectoren.Any())
                {
                    var fc = TLCGenControllerDataProvider.Default.Controller.Fasen.First(x => x.Naam == _naloop.FaseVan);
                    switch (fc.Type)
                    {
                        case FaseTypeEnum.Auto:
                        case FaseTypeEnum.Fiets:
                        case FaseTypeEnum.OV:
                            foreach (var d in fc.Detectoren.Where(x => x.Type == DetectorTypeEnum.Kop))
                            {
                                DetectorManager.SelectedItemToAdd = d.Naam;
                                DetectorManager.AddItemCommand.Execute(null);
                            }
                            break;
                        case FaseTypeEnum.Voetganger:
                            foreach (var d in fc.Detectoren.Where(x => x.Type == DetectorTypeEnum.KnopBuiten))
                            {
                                DetectorManager.SelectedItemToAdd = d.Naam;
                                DetectorManager.AddItemCommand.Execute(null);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
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

        public int MaxUitverlengenVolgrichting
        {
            get => _naloop.MaxUitverlengenVolgrichting;
            set
            {
                _naloop.MaxUitverlengenVolgrichting = value;
                RaisePropertyChanged<object>(nameof(MaxUitverlengenVolgrichting), broadcast: true);
            }
        }

        public bool CanHaveMaxUitverlengenVolgrichting => 
            TLCGenControllerDataProvider.Default.Controller.Data.SynchronisatiesType == SynchronisatiesTypeEnum.InterFunc;

        public bool MaximaleVoorstartAllowed => 
            !InrijdenTijdensGroen || TLCGenControllerDataProvider.Default.Controller.Data.SynchronisatiesType == SynchronisatiesTypeEnum.RealFunc;

        public ObservableCollectionAroundList<NaloopTijdViewModel, NaloopTijdModel> Tijden => _tijden ??= new ObservableCollectionAroundList<NaloopTijdViewModel, NaloopTijdModel>(_naloop.Tijden);

        public ObservableCollection<NaloopDetectorModel> Detectoren => _detectoren ??= new ObservableCollection<NaloopDetectorModel>();

        public NaloopDetectorModel SelectedDetector
        {
            get => DetectorManager?.SelectedItem;
            set
            {
                if (DetectorManager == null) return;
                DetectorManager.SelectedItem = value;
                RaisePropertyChanged();
            }
        }

        private ItemsManagerViewModel<NaloopDetectorModel, string> _detectorManager;
        public ItemsManagerViewModel<NaloopDetectorModel, string> DetectorManager
        {
            get
            {
                if (TLCGenControllerDataProvider.Default.Controller != null && 
                    _detectorManager == null && _naloop?.FaseVan != null)
                {
                    _detectorManager = new ItemsManagerViewModel<NaloopDetectorModel, string>(
                        Detectoren,
                        ControllerAccessProvider.Default.AllDetectorStrings,
                        x => new NaloopDetectorModel { Detector = x },
                        x => Detectoren.All(y => y.Detector != x),
                        null,
                        () => RaisePropertyChanged(nameof(SelectedDetector)),
                        () => RaisePropertyChanged(nameof(SelectedDetector)));
                }
                return _detectorManager;
            }
        }

        public System.Windows.Visibility ShowNotSupportedInCCOLWarning =>
            (Type == NaloopTypeEnum.EindeGroen) && InrijdenTijdensGroen ? 
            System.Windows.Visibility.Visible : 
            System.Windows.Visibility.Collapsed;

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
            _tijden = new ObservableCollectionAroundList<NaloopTijdViewModel, NaloopTijdModel>(_naloop.Tijden);
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

            _detectorManager?.Refresh();
        }

        private void OnNameChanged(NameChangedMessage msg)
        {
            _detectorManager?.Refresh();
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

            SetNaloopTijden();
            Detectoren.CollectionChanged += Detectoren_CollectionChanged;
            Tijden.CollectionChanged += Tijden_CollectionChanged;

            Messenger.Default.Register(this, new Action<DetectorenChangedMessage>(OnDetectorenChanged));
            Messenger.Default.Register(this, new Action<NameChangedMessage>(OnNameChanged));
        }

        #endregion // Constructor
    }
}
