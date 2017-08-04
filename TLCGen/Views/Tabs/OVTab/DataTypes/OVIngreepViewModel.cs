using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using TLCGen.Controls;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
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

        [Category("Opties")]
        public bool KAR
        {
            get { return _OVIngreep.KAR; }
            set
            {
                _OVIngreep.KAR = value;
                if(value)
                {
                    _OVIngreep.DummyKARInmelding = new DetectorModel() { Dummy = true };
                    _OVIngreep.DummyKARUitmelding = new DetectorModel() { Dummy = true };
                    _OVIngreep.DummyKARInmelding.Naam = "dummykarin" + _OVIngreep.FaseCyclus;
                    _OVIngreep.DummyKARUitmelding.Naam = "dummykaruit" + _OVIngreep.FaseCyclus;
                }
                else
                {
                    _OVIngreep.DummyKARInmelding = null;
                    _OVIngreep.DummyKARUitmelding = null;
                }
                RaisePropertyChanged<object>("KAR", broadcast: true);
                Messenger.Default.Send(new OVIngrepenChangedMessage());
            }
        }

        public bool Vecom
        {
            get { return _OVIngreep.Vecom; }
            set
            {
                _OVIngreep.Vecom = value;
                if (value)
                {
                    _OVIngreep.DummyVecomInmelding = new DetectorModel() { Dummy = true };
                    _OVIngreep.DummyVecomUitmelding = new DetectorModel() { Dummy = true };
                    _OVIngreep.DummyVecomInmelding.Naam = "dummyvecomin" + _OVIngreep.FaseCyclus;
                    _OVIngreep.DummyVecomUitmelding.Naam = "dummyvecomuit" + _OVIngreep.FaseCyclus;
                }
                else
                {
                    _OVIngreep.DummyVecomInmelding = null;
                    _OVIngreep.DummyVecomUitmelding = null;
                }
                RaisePropertyChanged<object>("Vecom", broadcast: true);
                Messenger.Default.Send(new OVIngrepenChangedMessage());
            }
        }

        //public bool MassaDetectie
        //{
        //    get { return _OVIngreep.MassaDetectie; }
        //    set
        //    {
        //        _OVIngreep.MassaDetectie = value;
        //        RaisePropertyChanged<object>("MassaDetectie", broadcast: true);
        //    }
        //}

        [Description("Type voertuig")]
        public OVIngreepVoertuigTypeEnum Type
        {
            get { return _OVIngreep.Type; }
            set
            {
                _OVIngreep.Type = value;
                RaisePropertyChanged<object>("Type", broadcast: true);
            }
        }

        [Description("Versnelde inmelding koplus")]
        [EnabledCondition("HasKoplus")]
        public bool VersneldeInmeldingKoplus
        {
            get => _OVIngreep.VersneldeInmeldingKoplus;
            set
            {
                _OVIngreep.VersneldeInmeldingKoplus = value;
                RaisePropertyChanged<object>("VersneldeInmeldingKoplus", broadcast: true);
            }
        }

        //[Description("Min. rijtijd versn. inm.")]
        //[EnabledCondition("VersneldeInmeldingKoplus")]
        //public int MinimaleRijtijdVoorVersneldeInmelding
        //{
        //    get => _OVIngreep.MinimaleRijtijdVoorVersneldeInmelding;
        //    set
        //    {
        //        _OVIngreep.MinimaleRijtijdVoorVersneldeInmelding = value;
        //        RaisePropertyChanged<object>("MinimaleRijtijdVoorVersneldeInmelding", broadcast: true);
        //    }
        //}

        [Browsable(false)]
        public bool HasKoplus
        {
            get
            {
                var fc = TLCGenModelManager.Default.Controller.Fasen.FirstOrDefault(x => x.Naam == _OVIngreep.FaseCyclus);
                return fc != null && fc.Detectoren.Any(x => x.Type == DetectorTypeEnum.Kop);
            }
        }

        [Category("Tijden")]
        [Description("Rijtijd ongehinderd")]
        public int RijTijdOngehinderd
        {
            get { return _OVIngreep.RijTijdOngehinderd; }
            set
            {
                _OVIngreep.RijTijdOngehinderd = value;
                RaisePropertyChanged<object>("RijTijdOngehinderd", broadcast: true);
            }
        }

        [Description("Rijtijd beperkt gehinderd")]
        public int RijTijdBeperktgehinderd
        {
            get { return _OVIngreep.RijTijdBeperktgehinderd; }
            set
            {
                _OVIngreep.RijTijdBeperktgehinderd = value;
                RaisePropertyChanged<object>("RijTijdBeperktgehinderd", broadcast: true);
            }
        }

        [Description("Rijtijd gehinderd")]
        public int RijTijdGehinderd
        {
            get { return _OVIngreep.RijTijdGehinderd; }
            set
            {
                _OVIngreep.RijTijdGehinderd = value;
                RaisePropertyChanged<object>("RijTijdGehinderd", broadcast: true);
            }
        }

        [Description("Ondermaximum")]
        public int OnderMaximum
        {
            get { return _OVIngreep.OnderMaximum; }
            set
            {
                _OVIngreep.OnderMaximum = value;
                RaisePropertyChanged<object>("OnderMaximum", broadcast: true);
            }
        }

        [Description("Groenbewaking")]
        public int GroenBewaking
        {
            get { return _OVIngreep.GroenBewaking; }
            set
            {
                _OVIngreep.GroenBewaking = value;
                RaisePropertyChanged<object>("GroenBewaking", broadcast: true);
            }
        }

        [Category("Prioriteitsopties")]
        [Description("Afkappen conflicten")]
        public bool AfkappenConflicten
        {
            get { return _OVIngreep.AfkappenConflicten; }
            set
            {
                _OVIngreep.AfkappenConflicten = value;
                RaisePropertyChanged<object>("AfkappenConflicten", broadcast: true);
            }
        }

        [Description("Afkappen conflicterend OV")]
        public bool AfkappenConflictenOV
        {
            get { return _OVIngreep.AfkappenConflictenOV; }
            set
            {
                _OVIngreep.AfkappenConflictenOV = value;
                RaisePropertyChanged<object>("AfkappenConflictenOV", broadcast: true);
            }
        }

        [Description("Vasthouden groen")]
        public bool VasthoudenGroen
        {
            get { return _OVIngreep.VasthoudenGroen; }
            set
            {
                _OVIngreep.VasthoudenGroen = value;
                RaisePropertyChanged<object>("VasthoudenGroen", broadcast: true);
            }
        }

        [Description("Tussendoor realiseren")]
        public bool TussendoorRealiseren
        {
            get { return _OVIngreep.TussendoorRealiseren; }
            set
            {
                _OVIngreep.TussendoorRealiseren = value;
                RaisePropertyChanged<object>("TussendoorRealiseren", broadcast: true);
            }
        }

        [Description("Prioriteit voor alle lijnen")]
        public bool AlleLijnen
        {
            get { return _OVIngreep.AlleLijnen; }
            set
            {
                _OVIngreep.AlleLijnen = value;
                RaisePropertyChanged<object>("AlleLijnen", broadcast: true);
            }
        }

        [Browsable(false)]
        public OVIngreepLijnNummerViewModel SelectedLijnNummer
        {
            get { return _SelectedLijnNummer; }
            set
            {
                _SelectedLijnNummer = value;
                RaisePropertyChanged("SelectedLijnNummer");
            }
        }

        [Browsable(false)]
        public string NewLijnNummer
        {
            get { return _NewLijnNummer; }
            set
            {
                _NewLijnNummer = value;
                RaisePropertyChanged("NewLijnNummer");
            }
        }

        [Browsable(false)]
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
        
        #endregion // Properties

        #region Commands

        RelayCommand _AddLijnNummerCommand;
        public ICommand AddLijnNummerCommand
        {
            get
            {
                if (_AddLijnNummerCommand == null)
                {
                    _AddLijnNummerCommand = new RelayCommand(AddLijnNummerCommand_Executed, AddLijnNummerCommand_CanExecute);
                }
                return _AddLijnNummerCommand;
            }
        }

        RelayCommand _Add10LijnNummersCommand;
        public ICommand Add10LijnNummersCommand
        {
            get
            {
                if (_Add10LijnNummersCommand == null)
                {
                    _Add10LijnNummersCommand = new RelayCommand(Add10LijnNummersCommand_Executed, Add10LijnNummersCommand_CanExecute);
                }
                return _Add10LijnNummersCommand;
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

        void AddLijnNummerCommand_Executed(object prm)
        {
            if (!string.IsNullOrWhiteSpace(NewLijnNummer))
            {
                OVIngreepLijnNummerModel nummer = new OVIngreepLijnNummerModel()
                {
                    Nummer = NewLijnNummer
                };
                LijnNummers.Add(new OVIngreepLijnNummerViewModel(nummer));
            }
            else
            {
                OVIngreepLijnNummerModel nummer = new OVIngreepLijnNummerModel()
                {
                    Nummer = "0"
                };
                LijnNummers.Add(new OVIngreepLijnNummerViewModel(nummer));
            }
            NewLijnNummer = "";
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        bool AddLijnNummerCommand_CanExecute(object prm)
        {
            return LijnNummers != null;
        }

        void Add10LijnNummersCommand_Executed(object prm)
        {
            for(int i = 0; i < 10; ++i)
            {
                AddLijnNummerCommand.Execute(prm);
            }
        }

        bool Add10LijnNummersCommand_CanExecute(object prm)
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
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ControllerDataChangedMessage());
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
