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
    public class MeeaanvraagViewModel : ViewModelBase
    {
        #region Fields

        private readonly MeeaanvraagModel _Meeaanvraag;
        private ObservableCollection<MeeaanvraagDetectorModel> _Detectoren;
        private bool _DetectieAfhankelijkPossible;

        #endregion // Fields

        #region Properties

        public MeeaanvraagTypeEnum Type
        {
            get => _Meeaanvraag.Type;
	        set
            {
                if (value != _Meeaanvraag.Type)
                {
                    _Meeaanvraag.Type = value;
                    RaisePropertyChanged<object>(nameof(Type), broadcast: true);
                    RaisePropertyChanged(nameof(UitgesteldPossible));
                }
            }
        }

        public AltijdAanUitEnum AanUit
        {
            get => _Meeaanvraag.AanUit;
            set
            {
                _Meeaanvraag.AanUit = value;
                RaisePropertyChanged<object>(nameof(AanUit), broadcast: true);
            }
        }

        public bool TypeInstelbaarOpStraat
        {
            get => _Meeaanvraag.TypeInstelbaarOpStraat;
	        set
            {
                _Meeaanvraag.TypeInstelbaarOpStraat = value;
                RaisePropertyChanged<object>(nameof(TypeInstelbaarOpStraat), broadcast: true);
                RaisePropertyChanged(nameof(UitgesteldPossible));
            }
        }

        public bool DetectieAfhankelijkPossible
        {
            get => _DetectieAfhankelijkPossible;
	        set
            {
                _DetectieAfhankelijkPossible = value;
                RaisePropertyChanged();
            }
        }

        public bool DetectieAfhankelijk
        {
            get => _Meeaanvraag.DetectieAfhankelijk;
	        set
            {
                _Meeaanvraag.DetectieAfhankelijk = value;
	            if (value && !Detectoren.Any())
	            {
		            var fc = TLCGenControllerDataProvider.Default.Controller.Fasen.First(x => x.Naam == _Meeaanvraag.FaseVan);
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

        public bool Uitgesteld
        {
            get => _Meeaanvraag.Uitgesteld;
	        set
            {
                _Meeaanvraag.Uitgesteld = value;
                RaisePropertyChanged<object>(nameof(Uitgesteld), broadcast: true);
            }
        }

        public int UitgesteldTijdsduur
        {
            get => _Meeaanvraag.UitgesteldTijdsduur;
	        set
            {
                _Meeaanvraag.UitgesteldTijdsduur = value;
                RaisePropertyChanged<object>(nameof(UitgesteldTijdsduur), broadcast: true);
            }
        }

        public bool UitgesteldPossible => Type == MeeaanvraagTypeEnum.Startgroen || TypeInstelbaarOpStraat;

        public ObservableCollection<MeeaanvraagDetectorModel> Detectoren
        {
            get
            {
                if (_Detectoren == null)
                    _Detectoren = new ObservableCollection<MeeaanvraagDetectorModel>();
                return _Detectoren;
            }
        }
        
        public MeeaanvraagDetectorModel SelectedDetector
        {
            get => DetectorManager?.SelectedItem;
	        set
            {
                if (DetectorManager != null)
                {
                    DetectorManager.SelectedItem = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ItemsManagerViewModel<MeeaanvraagDetectorModel, string> _DetectorManager;
        public ItemsManagerViewModel<MeeaanvraagDetectorModel, string> DetectorManager
        {
            get
            {
                if (_DetectorManager == null && _Meeaanvraag != null && _Meeaanvraag.FaseVan != null)
                {
                    List<string> dets = 
                        TLCGenControllerDataProvider.Default.Controller.Fasen.
                            First(x => x.Naam == _Meeaanvraag.FaseVan).
                            Detectoren.
                            Select(x => x.Naam).
                            ToList();
                    _DetectorManager = new ItemsManagerViewModel<MeeaanvraagDetectorModel, string>(
                        Detectoren,
                        dets,
                        x => { var md = new MeeaanvraagDetectorModel() { MeeaanvraagDetector = x }; return md; },
                        x => { return Detectoren.All(y => y.MeeaanvraagDetector != x); },
                        null,
                        () => { RaisePropertyChanged(nameof(SelectedDetector)); },
                        () => { RaisePropertyChanged(nameof(SelectedDetector)); }
                        );
                }
                return _DetectorManager;
            }
        }

        #endregion // Properties

        #region Collection changed

        private void Detectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (MeeaanvraagDetectorModel d in e.NewItems)
                {
                    _Meeaanvraag.Detectoren.Add(d);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (MeeaanvraagDetectorModel d in e.OldItems)
                {
                    _Meeaanvraag.Detectoren.Remove(d);
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

            Detectoren.CollectionChanged -= Detectoren_CollectionChanged;
            foreach (MeeaanvraagDetectorModel ndm in _Meeaanvraag.Detectoren)
            {
                Detectoren.Add(ndm);
            }
            Detectoren.CollectionChanged += Detectoren_CollectionChanged;
        }

        #endregion // TLCGen Events

        #region Constructor

        public MeeaanvraagViewModel(MeeaanvraagModel mm)
        {
            _Meeaanvraag = mm;
            foreach(var d in _Meeaanvraag.Detectoren)
            {
                Detectoren.Add(d);
            }
            Detectoren.CollectionChanged += Detectoren_CollectionChanged;

            Messenger.Default.Register(this, new Action<DetectorenChangedMessage>(OnDetectorenChanged));
        }

        #endregion // Constructor

    }
}
