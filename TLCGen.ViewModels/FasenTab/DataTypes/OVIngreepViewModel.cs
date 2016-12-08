using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class OVIngreepViewModel : ViewModelBase
    {
        #region Fields

        private OVIngreepModel _OVIngreep;
        private OVIngreepLijnNummerViewModel _SelectedLijnNummer;
        private ObservableCollection<OVIngreepLijnNummerViewModel> _LijnNummers;
        private string _NewLijnNummer;

        #endregion // Fields

        #region Properties

        public OVIngreepModel OVIngreep
        {
            get { return _OVIngreep; }
            set
            {
                _OVIngreep = value;
            }
        }

        public OVIngreepLijnNummerViewModel SelectedLijnNummer
        {
            get { return _SelectedLijnNummer; }
            set
            {
                _SelectedLijnNummer = value;
                OnPropertyChanged("SelectedLijnNummer");
            }
        }

        public string NewLijnNummer
        {
            get { return _NewLijnNummer; }
            set
            {
                _NewLijnNummer = value;
                OnPropertyChanged("NewLijnNummer");
            }
        }

        public ObservableCollection<OVIngreepLijnNummerViewModel> LijnNummers
        {
            get
            {
                if(_LijnNummers == null)
                {
                    _LijnNummers = new ObservableCollection<OVIngreepLijnNummerViewModel>();
                }
                return _LijnNummers;
            }
        }

        public bool KAR
        {
            get { return _OVIngreep.KAR; }
            set
            {
                _OVIngreep.KAR = value;
                OnMonitoredPropertyChanged("KAR");
            }
        }

        public bool Vecom
        {
            get { return _OVIngreep.Vecom; }
            set
            {
                _OVIngreep.Vecom = value;
                OnMonitoredPropertyChanged("Vecom");
            }
        }

        public bool MassaDetectie
        {
            get { return _OVIngreep.MassaDetectie; }
            set
            {
                _OVIngreep.MassaDetectie = value;
                OnMonitoredPropertyChanged("MassaDetectie");
            }
        }

        public OVIngreepVoertuigTypeEnum Type
        {
            get { return _OVIngreep.Type; }
            set
            {
                _OVIngreep.Type = value;
                OnMonitoredPropertyChanged("Type");
            }
        }

        public bool AlleLijnen
        {
            get { return _OVIngreep.AlleLijnen; }
            set
            {
                _OVIngreep.AlleLijnen = value;
                OnMonitoredPropertyChanged("AlleLijnen");
            }
        }

        public int RijTijdOngehinderd
        {
            get { return _OVIngreep.RijTijdOngehinderd; }
            set
            {
                _OVIngreep.RijTijdOngehinderd = value;
                OnMonitoredPropertyChanged("RijTijdOngehinderd");
            }
        }

        public int RijTijdBeperktgehinderd
        {
            get { return _OVIngreep.RijTijdBeperktgehinderd; }
            set
            {
                _OVIngreep.RijTijdBeperktgehinderd = value;
                OnMonitoredPropertyChanged("RijTijdBeperktgehinderd");
            }
        }

        public int RijTijdGehinderd
        {
            get { return _OVIngreep.RijTijdGehinderd; }
            set
            {
                _OVIngreep.RijTijdGehinderd = value;
                OnMonitoredPropertyChanged("RijTijdGehinderd");
            }
        }

        public int OnderMaximum
        {
            get { return _OVIngreep.OnderMaximum; }
            set
            {
                _OVIngreep.OnderMaximum = value;
                OnMonitoredPropertyChanged("OnderMaximum");
            }
        }

        public int GroenBewaking
        {
            get { return _OVIngreep.GroenBewaking; }
            set
            {
                _OVIngreep.GroenBewaking = value;
                OnMonitoredPropertyChanged("GroenBewaking");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddLijnNummerCommand;
        public ICommand AddLijnNummerCommand
        {
            get
            {
                if (_AddLijnNummerCommand == null)
                {
                    _AddLijnNummerCommand = new RelayCommand(AddNewLijnNummerCommand_Executed, AddNewLijnNummerCommand_CanExecute);
                }
                return _AddLijnNummerCommand;
            }
        }


        RelayCommand _RemoveLijnNummerCommand;
        public ICommand RemoveLijnNummerCommand
        {
            get
            {
                if (_RemoveLijnNummerCommand == null)
                {
                    _RemoveLijnNummerCommand = new RelayCommand(RemoveLijnNummerCommand_Executed, RemoveLijnNummerCommand_CanExecute);
                }
                return _RemoveLijnNummerCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewLijnNummerCommand_Executed(object prm)
        {
            if (!string.IsNullOrWhiteSpace(NewLijnNummer))
            {
                OVIngreepLijnNummerModel nummer = new OVIngreepLijnNummerModel();
                nummer.Nummer = NewLijnNummer;
                LijnNummers.Add(new OVIngreepLijnNummerViewModel(nummer));
            }
            else
            {
                OVIngreepLijnNummerModel nummer = new OVIngreepLijnNummerModel();
                nummer.Nummer = "0";
                LijnNummers.Add(new OVIngreepLijnNummerViewModel(nummer));
            }
            NewLijnNummer = "";
        }

        bool AddNewLijnNummerCommand_CanExecute(object prm)
        {
            return LijnNummers != null;
        }

        void RemoveLijnNummerCommand_Executed(object prm)
        {
            if (SelectedLijnNummer != null)
            {
                LijnNummers.Remove(SelectedLijnNummer);
                SelectedLijnNummer = null;
            }
            else
            {
                LijnNummers.RemoveAt(LijnNummers.Count - 1);
            }
        }

        bool RemoveLijnNummerCommand_CanExecute(object prm)
        {
            return LijnNummers != null && LijnNummers.Count > 0;
        }

        #endregion // Command functionality

        #region Collection changed

        private void LijnNummers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (OVIngreepLijnNummerViewModel num in e.NewItems)
                {
                    _OVIngreep.LijnNummers.Add(num.LijnNummer);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (OVIngreepLijnNummerViewModel num in e.OldItems)
                {
                    _OVIngreep.LijnNummers.Remove(num.LijnNummer);
                }
            }
        }

        #endregion // Collection changed

        #region Constructor

        public OVIngreepViewModel(OVIngreepModel ovingreep)
        {
            _OVIngreep = ovingreep;

            foreach(OVIngreepLijnNummerModel num in _OVIngreep.LijnNummers)
            {
                LijnNummers.Add(new OVIngreepLijnNummerViewModel(num));
            }

            LijnNummers.CollectionChanged += LijnNummers_CollectionChanged;
        }

        #endregion // Constructor
    }
}
