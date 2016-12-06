using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 2, type: TabItemTypeEnum.DetectieTab)]
    public class DetectorenRichtingGevoeligTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<RichtingGevoeligeAanvraagViewModel> _RichtingGevoeligeAanvragen;
        private ObservableCollection<RichtingGevoeligVerlengViewModel> _RichtingGevoeligVerlengen;

        private RichtingGevoeligeAanvraagViewModel _SelectedRichtingGevoeligeAanvraag;
        private RichtingGevoeligVerlengViewModel _SelectedRichtingGevoeligVerleng;

        private ObservableCollection<string> _AlleDetectoren;
        private ObservableCollection<string> _DetectorenAanvraag1;
        private ObservableCollection<string> _DetectorenAanvraag2;
        private ObservableCollection<string> _DetectorenVerleng1;
        private ObservableCollection<string> _DetectorenVerleng2;
        private ObservableCollection<string> _Fasen;

        private string _SelectedFaseAanvraag;
        private string _SelectedDetectorAanvraag1;
        private string _SelectedDetectorAanvraag2;
        private string _SelectedFaseVerleng;
        private string _SelectedDetectorVerleng1;
        private string _SelectedDetectorVerleng2;

        #endregion // Fields

        #region Properties

        public ObservableCollection<string> Fasen
        {
            get
            {
                if (_Fasen == null)
                {
                    _Fasen = new ObservableCollection<string>();
                }
                return _Fasen;
            }
        }

        public ObservableCollection<string> DetectorenAanvraag1
        {
            get
            {
                if (_DetectorenAanvraag1 == null)
                {
                    _DetectorenAanvraag1 = new ObservableCollection<string>();
                }
                return _DetectorenAanvraag1;
            }
        }

        public ObservableCollection<string> DetectorenAanvraag2
        {
            get
            {
                if (_DetectorenAanvraag2 == null)
                {
                    _DetectorenAanvraag2 = new ObservableCollection<string>();
                }
                return _DetectorenAanvraag2;
            }
        }

        public ObservableCollection<string> DetectorenVerleng1
        {
            get
            {
                if (_DetectorenVerleng1 == null)
                {
                    _DetectorenVerleng1 = new ObservableCollection<string>();
                }
                return _DetectorenVerleng1;
            }
        }

        public ObservableCollection<string> DetectorenVerleng2
        {
            get
            {
                if (_DetectorenVerleng2 == null)
                {
                    _DetectorenVerleng2 = new ObservableCollection<string>();
                }
                return _DetectorenVerleng2;
            }
        }

        public ObservableCollection<string> AlleDetectoren
        {
            get
            {
                if (_AlleDetectoren == null)
                {
                    _AlleDetectoren = new ObservableCollection<string>();
                }
                return _AlleDetectoren;
            }
        }

        public string SelectedFaseAanvraag
        {
            get { return _SelectedFaseAanvraag; }
            set
            {
                _SelectedFaseAanvraag = value;
                _SelectedDetectorAanvraag1 = null;
                _SelectedDetectorAanvraag2 = null;
                DetectorenAanvraag1.Clear();
                DetectorenAanvraag1.AddRange(_AlleDetectoren.Where(x =>
                {
                    return 
                        x.StartsWith(value) && 
                        x != SelectedDetectorAanvraag2 && 
                        !(RichtingGevoeligeAanvragen.Where(y => y.FaseCyclus == SelectedFaseAanvraag && (y.VanDetector == x || y.NaarDetector == x)).Count() > 0);
                }));
                DetectorenAanvraag2.Clear();
                DetectorenAanvraag2.AddRange(_AlleDetectoren.Where(x =>
                {
                    return 
                        x.StartsWith(value) && 
                        x != SelectedDetectorAanvraag1 &&
                        !(RichtingGevoeligeAanvragen.Where(y => y.FaseCyclus == SelectedFaseAanvraag && (y.VanDetector == x || y.NaarDetector == x)).Count() > 0);
                }));
                OnPropertyChanged("SelectedFaseAanvraag");
                OnPropertyChanged("SelectedDetectorAanvraag1");
                OnPropertyChanged("SelectedDetectorAanvraag2");
            }
        }
        public string SelectedDetectorAanvraag1
        {
            get { return _SelectedDetectorAanvraag1; }
            set
            {
                _SelectedDetectorAanvraag1 = value;
                DetectorenAanvraag2.Clear();
                DetectorenAanvraag2.AddRange(_AlleDetectoren.Where(x =>
                {
                    return
                        x.StartsWith(SelectedFaseAanvraag) &&
                        x != SelectedDetectorAanvraag1 &&
                        !(RichtingGevoeligeAanvragen.Where(y => y.FaseCyclus == SelectedFaseAanvraag && (y.VanDetector == x || y.NaarDetector == x)).Count() > 0);
                }));
                OnPropertyChanged("SelectedDetectorAanvraag1");
            }
        }
        public string SelectedDetectorAanvraag2
        {
            get { return _SelectedDetectorAanvraag2; }
            set
            {
                _SelectedDetectorAanvraag2 = value;
                OnPropertyChanged("SelectedDetectorAanvraag2");
            }
        }

        public string SelectedFaseVerleng
        {
            get { return _SelectedFaseVerleng; }
            set
            {
                _SelectedFaseVerleng = value;
                _SelectedDetectorVerleng1 = null;
                _SelectedDetectorVerleng2 = null;
                DetectorenVerleng1.Clear();
                DetectorenVerleng1.AddRange(_AlleDetectoren.Where(x =>
                {
                    return
                        x.StartsWith(value) &&
                        x != SelectedDetectorVerleng2 &&
                        !(RichtingGevoeligVerlengen.Where(y => y.FaseCyclus == SelectedFaseVerleng && (y.VanDetector == x || y.NaarDetector == x)).Count() > 0);
                }));
                DetectorenVerleng2.Clear();
                DetectorenVerleng2.AddRange(_AlleDetectoren.Where(x =>
                {
                    return
                        x.StartsWith(value) &&
                        x != SelectedDetectorVerleng1 &&
                        !(RichtingGevoeligVerlengen.Where(y => y.FaseCyclus == SelectedFaseVerleng && (y.VanDetector == x || y.NaarDetector == x)).Count() > 0);
                }));
                OnPropertyChanged("SelectedFaseVerleng");
                OnPropertyChanged("SelectedDetectorVerleng1");
                OnPropertyChanged("SelectedDetectorVerleng2");
            }
        }
        public string SelectedDetectorVerleng1
        {
            get { return _SelectedDetectorVerleng1; }
            set
            {
                _SelectedDetectorVerleng1 = value;
                DetectorenVerleng2.Clear();
                DetectorenVerleng2.AddRange(_AlleDetectoren.Where(x =>
                {
                    return
                        x.StartsWith(SelectedFaseVerleng) &&
                        x != SelectedDetectorVerleng1 &&
                        !(RichtingGevoeligVerlengen.Where(y => y.FaseCyclus == SelectedFaseVerleng && (y.VanDetector == x || y.NaarDetector == x)).Count() > 0);
                }));
                OnPropertyChanged("SelectedDetectorVerleng1");
            }
        }
        public string SelectedDetectorVerleng2
        {
            get { return _SelectedDetectorVerleng2; }
            set
            {
                _SelectedDetectorVerleng2 = value;
                OnPropertyChanged("SelectedDetectorVerleng2");
            }
        }


        public ObservableCollection<RichtingGevoeligeAanvraagViewModel> RichtingGevoeligeAanvragen
        {
            get
            {
                if (_RichtingGevoeligeAanvragen == null)
                {
                    _RichtingGevoeligeAanvragen = new ObservableCollection<RichtingGevoeligeAanvraagViewModel>();
                }
                return _RichtingGevoeligeAanvragen;
            }
        }

        public ObservableCollection<RichtingGevoeligVerlengViewModel> RichtingGevoeligVerlengen
        {
            get
            {
                if (_RichtingGevoeligVerlengen == null)
                {
                    _RichtingGevoeligVerlengen = new ObservableCollection<RichtingGevoeligVerlengViewModel>();
                }
                return _RichtingGevoeligVerlengen;
            }
        }

        public RichtingGevoeligeAanvraagViewModel SelectedRichtingGevoeligeAanvraag
        {
            get { return _SelectedRichtingGevoeligeAanvraag; }
            set
            {
                _SelectedRichtingGevoeligeAanvraag = value;
                OnPropertyChanged("SelectedRichtingGevoeligeAanvraag");
            }
        }


        public RichtingGevoeligVerlengViewModel SelectedRichtingGevoeligVeleng
        {
            get { return _SelectedRichtingGevoeligVerleng; }
            set
            {
                _SelectedRichtingGevoeligVerleng = value;
                OnPropertyChanged("SelectedRichtingGevoeligVerleng");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddRichtingGevoeligeAanvraag;
        public ICommand AddRichtingGevoeligeAanvraag
        {
            get
            {
                if (_AddRichtingGevoeligeAanvraag == null)
                {
                    _AddRichtingGevoeligeAanvraag = new RelayCommand(AddRichtingGevoeligeAanvraag_Executed, AddRichtingGevoeligeAanvraag_CanExecute);
                }
                return _AddRichtingGevoeligeAanvraag;
            }
        }

        RelayCommand _AddRichtingGevoeligVerleng;
        public ICommand AddRichtingGevoeligVerleng
        {
            get
            {
                if (_AddRichtingGevoeligVerleng == null)
                {
                    _AddRichtingGevoeligVerleng = new RelayCommand(AddRichtingGevoeligVerleng_Executed, AddRichtingGevoeligVerleng_CanExecute);
                }
                return _AddRichtingGevoeligVerleng;
            }
        }

        RelayCommand _RemoveRichtingGevoeligeAanvraag;
        public ICommand RemoveRichtingGevoeligeAanvraag
        {
            get
            {
                if (_RemoveRichtingGevoeligeAanvraag == null)
                {
                    _RemoveRichtingGevoeligeAanvraag = new RelayCommand(RemoveRichtingGevoeligeAanvraag_Executed, RemoveRichtingGevoeligeAanvraag_CanExecute);
                }
                return _RemoveRichtingGevoeligeAanvraag;
            }
        }

        RelayCommand _RemoveRichtingGevoeligVerleng;
        public ICommand RemoveRichtingGevoeligVerleng
        {
            get
            {
                if (_RemoveRichtingGevoeligVerleng == null)
                {
                    _RemoveRichtingGevoeligVerleng = new RelayCommand(RemoveRichtingGevoeligVerleng_Executed, RemoveRichtingGevoeligVerleng_CanExecute);
                }
                return _RemoveRichtingGevoeligVerleng;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private bool AddRichtingGevoeligeAanvraag_CanExecute(object prm)
        {
            return SelectedFaseAanvraag != null && SelectedDetectorAanvraag1 != null && SelectedDetectorAanvraag2 != null;
        }

        private void AddRichtingGevoeligeAanvraag_Executed(object prm)
        {
            RichtingGevoeligeAanvraagModel rga = new RichtingGevoeligeAanvraagModel();
            rga.FaseCyclus = SelectedFaseAanvraag;
            rga.VanDetector = SelectedDetectorAanvraag1;
            rga.NaarDetector = SelectedDetectorAanvraag2;
            RichtingGevoeligeAanvragen.Add(new RichtingGevoeligeAanvraagViewModel(rga));

            _SelectedFaseAanvraag = null;
            _SelectedDetectorAanvraag1 = null;
            _SelectedDetectorAanvraag2 = null;
            OnPropertyChanged("SelectedFaseAanvraag");
            OnPropertyChanged("SelectedDetectorAanvraag1");
            OnPropertyChanged("SelectedDetectorAanvraag2");
        }

        private bool AddRichtingGevoeligVerleng_CanExecute(object prm)
        {
            return SelectedFaseVerleng != null && SelectedDetectorVerleng1 != null && SelectedDetectorVerleng2 != null;
        }

        private void AddRichtingGevoeligVerleng_Executed(object prm)
        {
            RichtingGevoeligVerlengModel rgv = new RichtingGevoeligVerlengModel();
            rgv.FaseCyclus = SelectedFaseVerleng;
            rgv.VanDetector = SelectedDetectorVerleng1;
            rgv.NaarDetector = SelectedDetectorVerleng2;
            RichtingGevoeligVerlengen.Add(new RichtingGevoeligVerlengViewModel(rgv));

            _SelectedFaseVerleng = null;
            _SelectedDetectorVerleng1 = null;
            _SelectedDetectorVerleng2 = null;
            OnPropertyChanged("SelectedFaseVerleng");
            OnPropertyChanged("SelectedDetectorVerleng1");
            OnPropertyChanged("SelectedDetectorVerleng2");
        }

        private bool RemoveRichtingGevoeligeAanvraag_CanExecute(object prm)
        {
            return SelectedRichtingGevoeligeAanvraag != null;
        }

        private void RemoveRichtingGevoeligeAanvraag_Executed(object prm)
        {
            RichtingGevoeligeAanvragen.Remove(SelectedRichtingGevoeligeAanvraag);
            SelectedRichtingGevoeligeAanvraag = null;
        }

        private bool RemoveRichtingGevoeligVerleng_CanExecute(object prm)
        {
            return SelectedRichtingGevoeligVeleng != null;
        }

        private void RemoveRichtingGevoeligVerleng_Executed(object prm)
        {
            RichtingGevoeligVerlengen.Remove(SelectedRichtingGevoeligVeleng);
            SelectedRichtingGevoeligVeleng = null;
        }

        #endregion // Command Functionality

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Richting\ngevoelig";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
            Fasen.Clear();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                Fasen.Add(fcm.Naam);
                foreach(DetectorModel dm in fcm.Detectoren)
                {
                    AlleDetectoren.Add(dm.Naam);
                }
            }
        }

        #endregion // TabItem Overrides

        #region Collection Changed

        private void RichtingGevoeligVerlengen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (RichtingGevoeligVerlengViewModel rgv in e.NewItems)
                {
                    _Controller.RichtingGevoeligVerlengen.Add(rgv.RichtingGevoeligVerleng);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (RichtingGevoeligVerlengViewModel rgv in e.OldItems)
                {
                    _Controller.RichtingGevoeligVerlengen.Remove(rgv.RichtingGevoeligVerleng);
                }
            };
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        private void RichtingGevoeligeAanvragen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (RichtingGevoeligeAanvraagViewModel rga in e.NewItems)
                {
                    _Controller.RichtingGevoeligeAanvragen.Add(rga.RichtingGevoeligeAanvraag);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (RichtingGevoeligeAanvraagViewModel rga in e.OldItems)
                {
                    _Controller.RichtingGevoeligeAanvragen.Remove(rga.RichtingGevoeligeAanvraag);
                }
            };
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region Constructor

        public DetectorenRichtingGevoeligTabViewModel(ControllerModel controller) : base(controller)
        {
            foreach(RichtingGevoeligeAanvraagModel rga in controller.RichtingGevoeligeAanvragen)
            {
                RichtingGevoeligeAanvragen.Add(new RichtingGevoeligeAanvraagViewModel(rga));
            }
            foreach (RichtingGevoeligVerlengModel rgv in controller.RichtingGevoeligVerlengen)
            {
                RichtingGevoeligVerlengen.Add(new RichtingGevoeligVerlengViewModel(rgv));
            }

            RichtingGevoeligeAanvragen.CollectionChanged += RichtingGevoeligeAanvragen_CollectionChanged;
            RichtingGevoeligVerlengen.CollectionChanged += RichtingGevoeligVerlengen_CollectionChanged;
        }


        #endregion // Constructor
    }
}
