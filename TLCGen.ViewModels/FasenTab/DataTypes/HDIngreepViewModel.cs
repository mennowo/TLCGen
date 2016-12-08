using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class HDIngreepViewModel : ViewModelBase
    {
        #region Fields

        private HDIngreepModel _HDIngreep;
        private HDIngreepMeerealiserendeFaseCyclusViewModel _SelectedMeerealiserendeFase;
        private ObservableCollection<HDIngreepMeerealiserendeFaseCyclusViewModel> _MeeRealiserendeFasen;
        private ObservableCollection<string> _Fasen;
        private string _SelectedFase;
        private ControllerModel _Controller;

        #endregion // Fields

        #region Properties

        public HDIngreepModel HDIngreep
        {
            get { return _HDIngreep; }
            set
            {
                _HDIngreep = value;
            }
        }

        public ObservableCollection<string> Fasen
        {
            get
            {
                if(_Fasen == null)
                {
                    _Fasen = new ObservableCollection<string>();
                }
                return _Fasen;
            }
        }

        public string SelectedFase
        {
            get { return _SelectedFase; }
            set
            {
                _SelectedFase = value;
                OnMonitoredPropertyChanged("SelectedFase");
            }
        }

        public HDIngreepMeerealiserendeFaseCyclusViewModel SelectedMeerealiserendeFase
        {
            get { return _SelectedMeerealiserendeFase; }
            set
            {
                _SelectedMeerealiserendeFase = value;
                OnPropertyChanged("SelectedMeerealiserendeFase");
            }
        }

        public ObservableCollection<HDIngreepMeerealiserendeFaseCyclusViewModel> MeerealiserendeFasen
        {
            get
            {
                if(_MeeRealiserendeFasen == null)
                {
                    _MeeRealiserendeFasen = new ObservableCollection<HDIngreepMeerealiserendeFaseCyclusViewModel>();
                }
                return _MeeRealiserendeFasen;
            }
        }

        public bool KAR
        {
            get { return _HDIngreep.KAR; }
            set
            {
                _HDIngreep.KAR = value;
                OnMonitoredPropertyChanged("KAR");
            }
        }

        public bool Vecom
        {
            get { return _HDIngreep.Vecom; }
            set
            {
                _HDIngreep.Vecom = value;
                OnMonitoredPropertyChanged("Vecom");
            }
        }

        public bool Opticom
        {
            get { return _HDIngreep.Opticom; }
            set
            {
                _HDIngreep.Opticom = value;
                OnMonitoredPropertyChanged("Opticom");
            }
        }

        public bool Sirene
        {
            get { return _HDIngreep.Sirene; }
            set
            {
                _HDIngreep.Sirene = value;
                OnMonitoredPropertyChanged("Sirene");
            }
        }

        public int GroenBewaking
        {
            get { return _HDIngreep.GroenBewaking; }
            set
            {
                _HDIngreep.GroenBewaking = value;
                OnMonitoredPropertyChanged("GroenBewaking");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddMeerealiserendeFaseCommand;
        public ICommand AddMeerealiserendeFaseCommand
        {
            get
            {
                if (_AddMeerealiserendeFaseCommand == null)
                {
                    _AddMeerealiserendeFaseCommand = new RelayCommand(AddNewMeerealiserendeFaseCommand_Executed, AddNewMeerealiserendeFaseCommand_CanExecute);
                }
                return _AddMeerealiserendeFaseCommand;
            }
        }


        RelayCommand _RemoveMeerealiserendeFaseCommand;
        public ICommand RemoveMeerealiserendeFaseCommand
        {
            get
            {
                if (_RemoveMeerealiserendeFaseCommand == null)
                {
                    _RemoveMeerealiserendeFaseCommand = new RelayCommand(RemoveMeerealiserendeFaseCommand_Executed, RemoveMeerealiserendeFaseCommand_CanExecute);
                }
                return _RemoveMeerealiserendeFaseCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewMeerealiserendeFaseCommand_Executed(object prm)
        {
            if (!(MeerealiserendeFasen.Where(x => x.FaseCyclus.FaseCyclus == SelectedFase).Count() > 0))
            {
                MeerealiserendeFasen.Add(
                    new HDIngreepMeerealiserendeFaseCyclusViewModel(
                        new HDIngreepMeerealiserendeFaseCyclusModel() { FaseCyclus = SelectedFase }));


            }

            Fasen.Clear();
            foreach (FaseCyclusModel m in _Controller.Fasen)
            {
                if (m.Define != _HDIngreep.FaseCyclus && !(_HDIngreep.MeerealiserendeFaseCycli.Where(x => x.FaseCyclus == m.Naam).Count() > 0))
                    Fasen.Add(m.Naam);
            }
            if(Fasen.Count > 0)
            {
                SelectedFase = Fasen[0];
            }
        }

        bool AddNewMeerealiserendeFaseCommand_CanExecute(object prm)
        {
            return MeerealiserendeFasen != null && SelectedFase != null;
        }

        void RemoveMeerealiserendeFaseCommand_Executed(object prm)
        {
            MeerealiserendeFasen.Remove(SelectedMeerealiserendeFase);
            if (MeerealiserendeFasen.Count > 0)
                SelectedMeerealiserendeFase = MeerealiserendeFasen[MeerealiserendeFasen.Count - 1];
            else
                SelectedMeerealiserendeFase = null;
        }

        bool RemoveMeerealiserendeFaseCommand_CanExecute(object prm)
        {
            return MeerealiserendeFasen != null && MeerealiserendeFasen.Count > 0;
        }

        #endregion // Command functionality

        #region Collection changed

        private void MeerealiserendeFasen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (HDIngreepMeerealiserendeFaseCyclusViewModel mr in e.NewItems)
                {
                    _HDIngreep.MeerealiserendeFaseCycli.Add(mr.FaseCyclus);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (HDIngreepMeerealiserendeFaseCyclusViewModel mr in e.OldItems)
                {
                    _HDIngreep.MeerealiserendeFaseCycli.Remove(mr.FaseCyclus);
                }
            }
        }

        #endregion // Collection changed

        #region Constructor

        public HDIngreepViewModel(ControllerModel controller, HDIngreepModel hdingreep)
        {
            _HDIngreep = hdingreep;
            _Controller = controller;

            Fasen.Clear();
            foreach (FaseCyclusModel m in _Controller.Fasen)
            {
                if (m.Define != _HDIngreep.FaseCyclus && !(_HDIngreep.MeerealiserendeFaseCycli.Where(x => x.FaseCyclus == m.Naam).Count() > 0))
                    Fasen.Add(m.Naam);
            }
            if (Fasen.Count > 0)
            {
                SelectedFase = Fasen[0];
            }

            foreach (HDIngreepMeerealiserendeFaseCyclusModel mr in _HDIngreep.MeerealiserendeFaseCycli)
            {
                MeerealiserendeFasen.Add(new HDIngreepMeerealiserendeFaseCyclusViewModel(mr));
            }

            MeerealiserendeFasen.CollectionChanged += MeerealiserendeFasen_CollectionChanged;
        }

        #endregion // Constructor
    }
}
