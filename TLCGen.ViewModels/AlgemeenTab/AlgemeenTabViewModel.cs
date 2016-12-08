using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TLCGen.Helpers;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index:0)]
    public class AlgemeenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ControllerDataModel _ControllerData;
        private VersieViewModel _SelectedVersie;

        #endregion // Fields

        #region Properties

        public DrawingImage Icon
        {
            get
            {
                ResourceDictionary dict = new ResourceDictionary();
                Uri u = new Uri("pack://application:,,,/" +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                    ";component/" + "TabIcons.xaml");
                dict.Source = u;
                return (DrawingImage)dict["AlgemeenTabDrawingImage"];
            }
        }

        public string Naam
        {
            get { return _ControllerData.Naam; }
            set
            {
                _ControllerData.Naam = value;
                OnMonitoredPropertyChanged("Naam");
            }
        }

        public string Stad
        {
            get { return _ControllerData.Stad; }
            set
            {
                _ControllerData.Stad = value;
                OnMonitoredPropertyChanged("Stad");
            }
        }

        public string Straat1
        {
            get { return _ControllerData.Straat1; }
            set
            {
                _ControllerData.Straat1 = value;
                OnMonitoredPropertyChanged("Straat1");
            }
        }

        public string Straat2
        {
            get { return _ControllerData.Straat2; }
            set
            {
                _ControllerData.Straat2 = value;
                OnMonitoredPropertyChanged("Straat2");
            }
        }

        public string BitmapNaam
        {
            get { return _ControllerData.BitmapNaam; }
            set
            {
                _ControllerData.BitmapNaam = value;
                OnMonitoredPropertyChanged("BitmapNaam");
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
            }
        }

        public GroentijdenTypeEnum TypeGroentijden
        {
            get { return _ControllerData.TypeGroentijden; }
            set
            {
                _ControllerData.TypeGroentijden = value;
                OnMonitoredPropertyChanged("TypeGroentijden");
                Messenger.Default.Send(new GroentijdenTypeChangedMessage(value));
            }
        }

        public CCOLVersieEnum CCOLVersie
        {
            get { return _ControllerData.CCOLVersie; }
            set
            {
                _ControllerData.CCOLVersie = value;
                OnMonitoredPropertyChanged("CCOLVersie");
            }
        }

        public KWCTypeEnum KWCType
        {
            get { return _ControllerData.KWCType; }
            set
            {
                _ControllerData.KWCType = value;
                OnMonitoredPropertyChanged("KWCType");
            }
        }

        public VLOGTypeEnum VLOGType
        {
            get { return _ControllerData.VLOGType; }
            set
            {
                _ControllerData.VLOGType = value;
                OnMonitoredPropertyChanged("VLOGType");
            }
        }

        public bool ExtraMeeverlengenInWG
        {
            get { return _ControllerData.ExtraMeeverlengenInWG; }
            set
            {
                _ControllerData.ExtraMeeverlengenInWG = value;
                OnMonitoredPropertyChanged("ExtraMeeverlengenInWG");
            }
        }

        public AansturingWaitsignalenEnum AansturingWaitsignalen
        {
            get { return _ControllerData.AansturingWaitsignalen; }
            set
            {
                _ControllerData.AansturingWaitsignalen = value;
                OnMonitoredPropertyChanged("AansturingWaitsignalen");
            }
        }

        public OVIngreepTypeEnum OVIngreep
        {
            get { return _ControllerData.OVIngreep; }
            set
            {
                _ControllerData.OVIngreep = value;
                OnMonitoredPropertyChanged("OVIngreep");
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
            }
        }

        public VersieViewModel SelectedVersie
        {
            get { return _SelectedVersie; }
            set
            {
                _SelectedVersie = value;
                OnPropertyChanged("SelectedVersie");
            }
        }
        
        private ObservableCollection<VersieViewModel> _Versies;
        public ObservableCollection<VersieViewModel> Versies
        {
            get
            {
                if (_Versies == null)
                {
                    _Versies = new ObservableCollection<VersieViewModel>();
                }
                return _Versies;
            }
        }

        #endregion // Properties

        #region Public methods

        #endregion // Public methods

        #region Commands

        RelayCommand _AddVersieCommand;
        public ICommand AddVersieCommand
        {
            get
            {
                if (_AddVersieCommand == null)
                {
                    _AddVersieCommand = new RelayCommand(AddVersieCommand_Executed, AddVersieCommand_CanExecute);
                }
                return _AddVersieCommand;
            }
        }

        RelayCommand _RemoveVersieCommand;
        public ICommand RemoveVersieCommand
        {
            get
            {
                if (_RemoveVersieCommand == null)
                {
                    _RemoveVersieCommand = new RelayCommand(RemoveVersieCommand_Executed, RemoveVersieCommand_CanExecute);
                }
                return _RemoveVersieCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        void AddVersieCommand_Executed(object prm)
        {
            VersieModel vm = new VersieModel();
            vm.Datum = DateTime.Now;
            string nextver = null;
            if(Versies != null && Versies.Count > 0)
            {
                Match m = Regex.Match(Versies[Versies.Count - 1].Versie, @"([0-9]+)\.([0-9]+)\.([0-9]+)");
                if(m.Groups.Count == 4)
                {
                    string midver = m.Groups[2].Value;
                    int nextmidver;
                    if(Int32.TryParse(midver, out nextmidver))
                    {
                        nextver = m.Groups[1].Value + "." + (nextmidver + 1).ToString() + ".0";
                    }
                }
            }
            vm.Versie = nextver == null ? "1.0.0" : nextver;
            vm.Ontwerper = Environment.UserName;
            VersieViewModel vvm = new VersieViewModel(vm);
            Versies.Add(vvm);
        }

        bool AddVersieCommand_CanExecute(object prm)
        {
            return Versies != null;
        }

        void RemoveVersieCommand_Executed(object prm)
        {
            Versies.Remove(SelectedVersie);
            SelectedVersie = null;
        }

        bool RemoveVersieCommand_CanExecute(object prm)
        {
            return Versies != null && Versies.Count > 0 && SelectedVersie != null;
        }

        #endregion // Command Functionality

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Algemeen";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        #endregion // TabItem Overrides

        #region Collection Changed

        private void Versies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (VersieViewModel vvm in e.NewItems)
                {
                    _ControllerData.Versies.Add(vvm.VersieEntry);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (VersieViewModel vvm in e.OldItems)
                {
                    _ControllerData.Versies.Remove(vvm.VersieEntry);
                }
            }
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region Constructor

        public AlgemeenTabViewModel(ControllerModel controller) : base(controller)
        {
            _ControllerData = controller.Data;

            foreach(VersieModel vm in _ControllerData.Versies)
            {
                VersieViewModel vvm = new VersieViewModel(vm);
                Versies.Add(vvm);
            }
            Versies.CollectionChanged += Versies_CollectionChanged;

        }

        #endregion // Constructor
    }
}
