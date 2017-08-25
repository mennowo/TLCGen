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
            get { return _Meeaanvraag.Type; }
            set
            {
                if (value != _Meeaanvraag.Type)
                {
                    _Meeaanvraag.Type = value;
                    RaisePropertyChanged<object>("Type", broadcast: true);
                    RaisePropertyChanged("TypeStartGroen");
                }
            }
        }

        public bool TypeStartGroen => Type == MeeaanvraagTypeEnum.Startgroen;

        public bool TypeInstelbaarOpStraat
        {
            get { return _Meeaanvraag.TypeInstelbaarOpStraat; }
            set
            {
                _Meeaanvraag.TypeInstelbaarOpStraat = value;
                RaisePropertyChanged<object>("TypeInstelbaarOpStraat", broadcast: true);
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
            get { return _Meeaanvraag.DetectieAfhankelijk; }
            set
            {
                _Meeaanvraag.DetectieAfhankelijk = value;
                RaisePropertyChanged<object>("DetectieAfhankelijk", broadcast: true);
            }
        }

        public bool Uitgesteld
        {
            get { return _Meeaanvraag.Uitgesteld; }
            set
            {
                _Meeaanvraag.Uitgesteld = value;
                RaisePropertyChanged<object>("Uitgesteld", broadcast: true);
            }
        }

        public int UitgesteldTijdsduur
        {
            get { return _Meeaanvraag.UitgesteldTijdsduur; }
            set
            {
                _Meeaanvraag.UitgesteldTijdsduur = value;
                RaisePropertyChanged<object>("UitgesteldTijdsduur", broadcast: true);
            }
        }

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

        private DetectorManagerViewModel<MeeaanvraagDetectorModel, string> _DetectorManager;
        public DetectorManagerViewModel<MeeaanvraagDetectorModel, string> DetectorManager
        {
            get
            {
                if (_DetectorManager == null && _Meeaanvraag != null && _Meeaanvraag.FaseVan != null)
                {
                    List<string> dets = 
                        DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.
                            Where(x => x.Naam == _Meeaanvraag.FaseVan).
                            First().
                            Detectoren.
                            Select(x => x.Naam).
                            ToList();
                    _DetectorManager = new DetectorManagerViewModel<MeeaanvraagDetectorModel, string>(
                        Detectoren,
                        dets,
                        (x) => { var md = new MeeaanvraagDetectorModel() { MeeaanvraagDetector = x }; return md; },
                        (x) => { return !Detectoren.Where(y => y.MeeaanvraagDetector == x).Any(); },
                        null,
                        () => { RaisePropertyChanged("SelectedDetector"); },
                        () => { RaisePropertyChanged("SelectedDetector"); }
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

            Detectoren.CollectionChanged += Detectoren_CollectionChanged;
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
