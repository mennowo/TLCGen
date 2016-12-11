using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.ViewModels.Enums;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 3)]
    public class SynchronisatiesTabViewModel : TLCGenTabItemViewModel
    {

        #region Fields
        
        private List<string> _AllDetectoren;
        private ObservableCollection<string> _FasenNames;
        private bool _MatrixChanged;
        private SynchronisatieTypeEnum _DisplayType;

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
                return (DrawingImage)dict["InterSignaalGroepTabDrawingImage"];
            }
        }

        public SynchronisatieTypeEnum DisplayType
        {
            get { return _DisplayType; }
            set
            {
#warning TODO: check integrity before proceding
                _DisplayType = value;
                if (ConflictMatrix != null)
                {
                    foreach (SynchronisatieViewModel cvm in ConflictMatrix)
                    {
                        cvm.DisplayType = value;
                    }
                }
                OnPropertyChanged("DisplayType");
            }
        }

        /// <summary>
        /// Collection of strings used to display matrix column and row headers
        /// </summary>
        public ObservableCollection<string> FasenNames
        {
            get
            {
                if (_FasenNames == null)
                    _FasenNames = new ObservableCollection<string>();
                return _FasenNames;
            }
        }

        /// <summary>
        /// Returns the collection of FaseCyclusViewModel from the main ControllerViewModel
        /// </summary>
        public IReadOnlyList<FaseCyclusModel> Fasen
        {
            get
            {
                return _Controller.Fasen;
            }
        }

        private ObservableCollection<string> _Detectoren;
        public ObservableCollection<string> Detectoren
        {
            get
            {
                if (_Detectoren == null)
                    _Detectoren = new ObservableCollection<string>();
                return _Detectoren;
            }
        }

        private SynchronisatieViewModel _SelectedSynchronisatie;
        public SynchronisatieViewModel SelectedSynchronisatie
        {
            get
            {
                return _SelectedSynchronisatie;
            }
            set
            {
                _SelectedSynchronisatie = value;
                if (_AllDetectoren != null && !string.IsNullOrEmpty(value.FaseVan))
                {
                    Detectoren.Clear();
                    string s = value.FaseVan.Replace(Settings.SettingsProvider.Default.GetFaseCyclusDefinePrefix(), "");
                    var __Detectoren = _AllDetectoren.Where(x => x.StartsWith(s));
                    foreach(var d in __Detectoren)
                    {
                        Detectoren.Add(d);
                    }
                    value.NaloopVM.DetectieAfhankelijkPossible = Detectoren.Count > 0;
                    value.MeeaanvraagVM.DetectieAfhankelijkPossible = Detectoren.Count > 0;
                }
                OnPropertyChanged("SelectedSynchronisatie");
            }
        }

        /// <summary>
        /// Symmetrical, two dimensional matrix used to display phasecycle conflicts.
        /// </summary>
        public SynchronisatieViewModel[,] ConflictMatrix { get; set; }

        /// <summary>
        /// Boolean set by instances of ConflictViewModel when their DisplayWaarde property is 
        /// set by the user. We use this to monitor changes to the model, and to check if we need
        /// to check the matrix for symmetry if the user changes tabs, or tries to save the model.
        /// </summary>
        public bool MatrixChanged
        {
            get { return _MatrixChanged; }
            set
            {
                _MatrixChanged = value;
                if (_MatrixChanged)
                OnMonitoredPropertyChanged("MatrixChanged");
            }
        }

        public bool UseGarantieOntruimingsTijden
        {
            get { return _Controller.Data.GarantieOntruimingsTijden; }
            set
            {
                _Controller.Data.GarantieOntruimingsTijden = value;
                OnMonitoredPropertyChanged("UseGarantieOntruimingsTijden");
                MatrixChanged = true;
            }
        }

        #endregion // Properties

        #region InterSignaalGroep Object Properties

        public int GelijkstartOntruimingstijdFaseVan
        {
            get
            {
                if (SelectedSynchronisatie == null)
                    return 0;

                return SelectedSynchronisatie.Gelijkstart.GelijkstartOntruimingstijdFaseVan;
            }
            set
            {
                SelectedSynchronisatie.Gelijkstart.GelijkstartOntruimingstijdFaseVan = value;
                foreach(SynchronisatieViewModel svm in ConflictMatrix)
                {
                    if(svm.FaseVan == SelectedSynchronisatie.FaseNaar && svm.FaseNaar == SelectedSynchronisatie.FaseVan)
                    {
                        svm.Gelijkstart.GelijkstartOntruimingstijdFaseNaar = value;
                    }
                }
                OnPropertyChanged("GelijkstartOntruimingstijdFaseVan");
            }
        }
        public int GelijkstartOntruimingstijdFaseNaar
        {
            get
            {
                if (SelectedSynchronisatie == null)
                    return 0;

                return SelectedSynchronisatie.Gelijkstart.GelijkstartOntruimingstijdFaseNaar;
            }
            set
            {
                SelectedSynchronisatie.Gelijkstart.GelijkstartOntruimingstijdFaseNaar = value;
                foreach (SynchronisatieViewModel svm in ConflictMatrix)
                {
                    if (svm.FaseVan == SelectedSynchronisatie.FaseNaar && svm.FaseNaar == SelectedSynchronisatie.FaseVan)
                    {
                        svm.Gelijkstart.GelijkstartOntruimingstijdFaseVan = value;
                    }
                }
                OnPropertyChanged("GelijkstartOntruimingstijdFaseNaar");
            }
        }

        public int VoorstartTijd
        {
            get
            {
                if (SelectedSynchronisatie == null)
                    return 0;

                return SelectedSynchronisatie.Voorstart.VoorstartTijd;
            }
            set
            {
                SelectedSynchronisatie.Voorstart.VoorstartTijd = value;
                OnPropertyChanged("VoorstartTijd");
            }
        }

        public int VoorstartOntruimingstijd
        {
            get
            {
                if (SelectedSynchronisatie == null)
                    return 0;

                return SelectedSynchronisatie.Voorstart.VoorstartOntruimingstijd;
            }
            set
            {
                SelectedSynchronisatie.Voorstart.VoorstartOntruimingstijd = value;
                OnPropertyChanged("VoorstartOntruimingstijd");
            }
        }

        public string Comment1
        {
            get
            {
                switch (DisplayType)
                {
                    case SynchronisatieTypeEnum.Gelijkstart:
                        return $"Ontruimingstijd van {SelectedSynchronisatie.FaseVan} naar {SelectedSynchronisatie.FaseNaar}";
                    case SynchronisatieTypeEnum.Voorstart:
                        return $"Voorstarttijd van {SelectedSynchronisatie.FaseVan} naar {SelectedSynchronisatie.FaseNaar}";
                    case SynchronisatieTypeEnum.Meeaanvraag:
                        return $"Type meeaanvraag van {SelectedSynchronisatie.FaseVan} naar {SelectedSynchronisatie.FaseNaar}";
                    default:
                        return "";
                }
            }
        }
        public string Comment2
        {
            get
            {
                switch (DisplayType)
                {
                    case SynchronisatieTypeEnum.Gelijkstart:
                        return $"Ontruimingstijd van {SelectedSynchronisatie.FaseNaar} naar {SelectedSynchronisatie.FaseVan}";
                    case SynchronisatieTypeEnum.Voorstart:
                        return $"Voorstart ontruimingstijd van {SelectedSynchronisatie.FaseVan} naar {SelectedSynchronisatie.FaseNaar}";
                    default:
                        return "";
                }
            }
        }

        public NaloopViewModel NaloopVM
        {
            get
            {
                if (SelectedSynchronisatie == null || !SelectedSynchronisatie.HasNaloop)
                    return null;
                return new NaloopViewModel(SelectedSynchronisatie.Naloop);
            }
        }

        #endregion // SynchInterSignaalGroep BOjecObject Properties

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "InterSignaalGroep";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override bool OnDeselectedPreview()
        {
            string s = Integrity.IntegrityChecker.IsConflictMatrixOK(_Controller);
            if (s == null)
                return true;
            else
            {
                MessageBox.Show(s, "Fout in conflictmatrix");
                return false;
            }
        }

        public override void OnSelected()
        {
            _AllDetectoren = new List<string>();
            foreach (FaseCyclusModel fcm in Controller.Fasen)
            {
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    _AllDetectoren.Add(dm.Naam);
                }
            }
            foreach (DetectorModel dm in Controller.Detectoren)
            {
                _AllDetectoren.Add(dm.Naam);
            }
        }

        #endregion // TabItem Overrides

        #region Commands

        RelayCommand _DeleteValueCommand;
        public ICommand DeleteValueCommand
        {
            get
            {
                if (_DeleteValueCommand == null)
                {
                    _DeleteValueCommand = new RelayCommand(DeleteValueCommand_Executed, DeleteValueCommand_CanExecute);
                }
                return _DeleteValueCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void DeleteValueCommand_Executed(object prm)
        {
            if(SelectedSynchronisatie != null)
                SelectedSynchronisatie.ConflictValue = "";
        }

        bool DeleteValueCommand_CanExecute(object prm)
        {
            return SelectedSynchronisatie != null;
        }

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        /// <summary>
        /// Builds a new string[,] to be exposed to the View. The 2D array is filled with data
        /// from the collection of FaseCyclusViewModel, and then from the collection of ConflictViewModel
        /// they contain.
        /// </summary>
        public void BuildConflictMatrix()
        {
            if (_Controller == null ||
                _Controller.Fasen == null ||
                _Controller.Fasen.Count <= 0)
                return;

            int fccount = Fasen.Count;

            _FasenNames = new ObservableCollection<string>();
            foreach (FaseCyclusModel fcvm in Fasen)
            {
                FasenNames.Add(fcvm.Naam);
            }
            OnPropertyChanged("FasenNames");


            if (fccount == 0)
            {
                ConflictMatrix = null;
                return;
            }

            ConflictMatrix = new SynchronisatieViewModel[fccount, fccount];
            for (int fcm_from = 0; fcm_from < fccount; ++fcm_from)
            {
                for (int fcm_to = 0; fcm_to < fccount; ++fcm_to)
                {
                    if (ConflictMatrix[fcm_from, fcm_to] == null)
                    {
                        if (fcm_from == fcm_to)
                        {
                            ConflictMatrix[fcm_from, fcm_to] = new SynchronisatieViewModel(true);
                        }

                        else
                        {
                            ConflictMatrix[fcm_from, fcm_to] = new SynchronisatieViewModel();
                            ConflictMatrix[fcm_from, fcm_to].FaseVan = Fasen[fcm_from].Define;
                            ConflictMatrix[fcm_from, fcm_to].FaseNaar = Fasen[fcm_to].Define;
                        }
                    }
                    ConflictMatrix[fcm_from, fcm_to].DisplayType = this.DisplayType;
                }
            }

            foreach (ConflictModel cm in _Controller.InterSignaalGroep.Conflicten)
            {
                foreach(SynchronisatieViewModel svm in ConflictMatrix)
                {
                    if (svm.FaseVan == cm.FaseVan && svm.FaseNaar == cm.FaseNaar)
                    {
                        svm.Conflict = cm;
                        break;
                    }
                }
            }

            foreach (NaloopModel nm in _Controller.InterSignaalGroep.Nalopen)
            {
                foreach (SynchronisatieViewModel svm in ConflictMatrix)
                {
                    if (svm.FaseVan == nm.FaseVan && svm.FaseNaar == nm.FaseNaar)
                    {
                        svm.Naloop = nm;
                        foreach (SynchronisatieViewModel svm_opp in ConflictMatrix)
                        {
                            if (svm_opp.FaseVan == nm.FaseNaar && svm_opp.FaseNaar == nm.FaseVan)
                            {
                                svm_opp.HasOppositeNaloop = true;
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            foreach (GelijkstartModel gm in _Controller.InterSignaalGroep.Gelijkstarten)
            {
                foreach (SynchronisatieViewModel svm in ConflictMatrix)
                {
                    if (svm.FaseVan == gm.FaseVan && svm.FaseNaar == gm.FaseNaar)
                    {
                        svm.Gelijkstart = gm;
                        foreach (SynchronisatieViewModel svm_opp in ConflictMatrix)
                        {
                            if (svm_opp.FaseVan == gm.FaseNaar && svm_opp.FaseNaar == gm.FaseVan)
                            {
                                svm_opp.HasGelijkstart = true;
                                svm_opp.Gelijkstart.GelijkstartOntruimingstijdFaseNaar = svm.Gelijkstart.GelijkstartOntruimingstijdFaseVan;
                                svm_opp.Gelijkstart.GelijkstartOntruimingstijdFaseVan = svm.Gelijkstart.GelijkstartOntruimingstijdFaseNaar;
                                break;
                            }
                        }
                        break;
                    }
                }
            }


            foreach (VoorstartModel vm in _Controller.InterSignaalGroep.Voorstarten)
            {
                foreach (SynchronisatieViewModel svm in ConflictMatrix)
                {
                    if (svm.FaseVan == vm.FaseVan && svm.FaseNaar == vm.FaseNaar)
                    {
                        svm.Voorstart = vm;
                        foreach (SynchronisatieViewModel svm_opp in ConflictMatrix)
                        {
                            if (svm_opp.FaseVan == vm.FaseNaar && svm_opp.FaseNaar == vm.FaseVan)
                            {
                                svm_opp.HasOppositeVoorstart = true;
                                break;
                            }
                        }
                    }
                }
            }

            foreach (MeeaanvraagModel mm in _Controller.InterSignaalGroep.Meeaanvragen)
            {
                foreach (SynchronisatieViewModel svm in ConflictMatrix)
                {
                    if (svm.FaseVan == mm.FaseVan && svm.FaseNaar == mm.FaseNaar)
                    {
                        svm.Meeaanvraag = mm;
                        foreach (SynchronisatieViewModel svm_opp in ConflictMatrix)
                        {
                            if (svm_opp.FaseVan == mm.FaseNaar && svm_opp.FaseNaar == mm.FaseVan)
                            {
                                svm_opp.HasOppositeMeeaanvraag = true;
                                break;
                            }
                        }
                    }
                }
            }
            
            OnPropertyChanged("ConflictMatrix");

            if(ConflictMatrix != null && ConflictMatrix.Length > 0)
                SelectedSynchronisatie = ConflictMatrix[0, 0];
        }

        /// <summary>
        /// Reads the property string[,] ConflictMatrix and saves all relevant entries as 
        /// instances of ConflictViewmodel in the collection of the relevant FaseCyclusViewModel.
        /// This also updates the Model data.
        /// </summary>
        public void SaveConflictMatrix()
        {
            if(ConflictMatrix == null || Fasen == null)
            {
                return;
            }

            int fccount = Fasen.Count;

            // Call extension method RemoveAll instead of built-in Clear(), see:
            // http://stackoverflow.com/questions/224155/when-clearing-an-observablecollection-there-are-no-items-in-e-olditems
            _Controller.InterSignaalGroep.Conflicten.RemoveAll();
            _Controller.InterSignaalGroep.Voorstarten.RemoveAll();
            _Controller.InterSignaalGroep.Gelijkstarten.RemoveAll();
            _Controller.InterSignaalGroep.Nalopen.RemoveAll();
            bool[,] gelijkstartsaved = new bool[fccount, fccount];
            for (int fcvm_from = 0; fcvm_from < fccount; ++fcvm_from)
            {
                for (int fcvm_to = 0; fcvm_to < fccount; ++fcvm_to)
                {
                    if (ConflictMatrix[fcvm_from, fcvm_to].HasConflict || ConflictMatrix[fcvm_from, fcvm_to].HasGarantieConflict)
                    {
                        _Controller.InterSignaalGroep.Conflicten.Add(ConflictMatrix[fcvm_from, fcvm_to].Conflict);
                    }

                    if(ConflictMatrix[fcvm_from, fcvm_to].HasGelijkstart)
                    {
                        if(!gelijkstartsaved[fcvm_from, fcvm_to])
                            _Controller.InterSignaalGroep.Gelijkstarten.Add(ConflictMatrix[fcvm_from, fcvm_to].Gelijkstart);
                        gelijkstartsaved[fcvm_from, fcvm_to] = true;
                        gelijkstartsaved[fcvm_to, fcvm_from] = true;
                    }

                    if (ConflictMatrix[fcvm_from, fcvm_to].HasVoorstart)
                    {
                        _Controller.InterSignaalGroep.Voorstarten.Add(ConflictMatrix[fcvm_from, fcvm_to].Voorstart);
                    }

                    if (ConflictMatrix[fcvm_from, fcvm_to].HasNaloop)
                    {
                        _Controller.InterSignaalGroep.Nalopen.Add(ConflictMatrix[fcvm_from, fcvm_to].Naloop);
                    }

                    if (ConflictMatrix[fcvm_from, fcvm_to].HasMeeaanvraag)
                    {
                        _Controller.InterSignaalGroep.Meeaanvragen.Add(ConflictMatrix[fcvm_from, fcvm_to].Meeaanvraag);
                    }
                }
            }
        }

        #endregion // Public methods

        #region Commands

        RelayCommand _SetGarantieValuesCommand;
        public ICommand SetGarantieValuesCommand
        {
            get
            {
                if (_SetGarantieValuesCommand == null)
                {
                    _SetGarantieValuesCommand = new RelayCommand(SetGarantieValuesCommand_Executed, SetGarantieValuesCommand_CanExecute);
                }
                return _SetGarantieValuesCommand;
            }
        }

        private bool SetGarantieValuesCommand_CanExecute(object obj)
        {
            return ConflictMatrix != null && Fasen != null && DisplayType == SynchronisatieTypeEnum.GarantieConflict;
        }

        private void SetGarantieValuesCommand_Executed(object obj)
        {
            int fccount = Fasen.Count;

            for (int fcvm_from = 0; fcvm_from < fccount; ++fcvm_from)
            {
                for (int fcvm_to = 0; fcvm_to < fccount; ++fcvm_to)
                {
                    int i;
                    if (!string.IsNullOrWhiteSpace(ConflictMatrix[fcvm_from, fcvm_to].GetConflictValue()) && Int32.TryParse(ConflictMatrix[fcvm_from, fcvm_to].GetConflictValue(), out i))
                    {
                        ConflictMatrix[fcvm_from, fcvm_to].ConflictValue = "0";
                    }
                }
            }
        }

        #endregion // Commands

        #region Collection Changed

        #endregion // Collection Changed

        #region TLCGen Event handling

        private void OnFasenChanged(FasenChangedMessage message)
        {
            BuildConflictMatrix();
        }

        private void OnFasenSorted(FasenSortedMessage message)
        {
            BuildConflictMatrix();
        }

        private void OnInterSignaalGroepChanged(InterSignaalGroepChangedMessage message)
        {
            // Conflict
            if (message.Conflict != null)
            {
                int fccount = Fasen.Count;
                for (int fcm_from = 0; fcm_from < fccount; ++fcm_from)
                {
                    for (int fcm_to = 0; fcm_to < fccount; ++fcm_to)
                    {
                        if (Fasen[fcm_from].Define == message.Conflict.FaseVan &&
                           Fasen[fcm_to].Define == message.Conflict.FaseNaar)
                        {
                            var cvm2 = ConflictMatrix[fcm_to, fcm_from];
                            switch (ConflictMatrix[fcm_from, fcm_to].ConflictValue)
                            {
                                case "GKL":
                                    if (cvm2.ConflictValue != "GK")
                                        cvm2.ConflictValueNoMessaging = "GK";
                                    break;
                                case "GK":
                                    if (!(cvm2.ConflictValue == "GK" || cvm2.ConflictValue == "GKL"))
                                        cvm2.ConflictValueNoMessaging = "GK";
                                    break;
                                case "FK":
                                    if (cvm2.ConflictValue != "FK")
                                        cvm2.ConflictValueNoMessaging = "FK";
                                    break;
                                case "":
                                    if (cvm2.ConflictValue == "*" || cvm2.ConflictValue == "GK" || cvm2.ConflictValue == "GKL" || cvm2.ConflictValue == "FK")
                                        cvm2.ConflictValueNoMessaging = "";
                                    break;
                                default:
                                    int i;
                                    if (Int32.TryParse(cvm2.ConflictValue, out i))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        cvm2.ConflictValueNoMessaging = "*";
                                    }
                                    break;
                            }
                            return;
                        }
                    }
                }
                // conflict not found: faulty state!
                throw new NotImplementedException();
            }

            // Naloop
            if(message.Naloop != null)
            {
                int fccount = Fasen.Count;
                for (int fcm_from = 0; fcm_from < fccount; ++fcm_from)
                {
                    for (int fcm_to = 0; fcm_to < fccount; ++fcm_to)
                    {
                        if (Fasen[fcm_from].Define == message.Naloop.FaseVan &&
                            Fasen[fcm_to].Define == message.Naloop.FaseNaar)
                        {
                            var cvm2 = ConflictMatrix[fcm_to, fcm_from];
                            cvm2.HasOppositeNaloop = message.IsCoupled;
                        }
                    }
                }
            }

            // Voorstart
            if (message.Voorstart != null)
            {
                int fccount = Fasen.Count;
                for (int fcm_from = 0; fcm_from < fccount; ++fcm_from)
                {
                    for (int fcm_to = 0; fcm_to < fccount; ++fcm_to)
                    {
                        if (Fasen[fcm_from].Define == message.Voorstart.FaseVan &&
                            Fasen[fcm_to].Define == message.Voorstart.FaseNaar)
                        {
                            var cvm2 = ConflictMatrix[fcm_to, fcm_from];
                            cvm2.HasOppositeVoorstart = message.IsCoupled;
                        }
                    }
                }
            }

            // Gelijkstart
            if (message.Gelijkstart != null)
            {
                int fccount = Fasen.Count;
                for (int fcm_from = 0; fcm_from < fccount; ++fcm_from)
                {
                    for (int fcm_to = 0; fcm_to < fccount; ++fcm_to)
                    {
                        if (Fasen[fcm_from].Define == message.Gelijkstart.FaseVan &&
                            Fasen[fcm_to].Define == message.Gelijkstart.FaseNaar)
                        {
                            var cvm2 = ConflictMatrix[fcm_to, fcm_from];
                            cvm2.IsCoupledNoMessaging = message.IsCoupled;
                        }
                    }
                }
            }

            // Meeaanvraag
            if (message.Meeaanvraag != null)
            {
                int fccount = Fasen.Count;
                for (int fcm_from = 0; fcm_from < fccount; ++fcm_from)
                {
                    for (int fcm_to = 0; fcm_to < fccount; ++fcm_to)
                    {
                        if (Fasen[fcm_from].Define == message.Meeaanvraag.FaseVan &&
                            Fasen[fcm_to].Define == message.Meeaanvraag.FaseNaar)
                        {
                            var cvm2 = ConflictMatrix[fcm_to, fcm_from];
                            cvm2.HasOppositeMeeaanvraag = message.IsCoupled;
                        }
                    }
                }
            }
        }

        private void OnProcesSynchornisationsRequested(ProcessSynchronisationsRequest request)
        {
            SaveConflictMatrix();
        }

        #endregion // TLCGen Event handling

        #region Constructor

        public SynchronisatiesTabViewModel(ControllerModel controller) : base(controller)
        {
            DisplayType = SynchronisatieTypeEnum.Conflict;

            BuildConflictMatrix();

            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<FasenSortedMessage>(OnFasenSorted));
            Messenger.Default.Register(this, new Action<InterSignaalGroepChangedMessage>(OnInterSignaalGroepChanged));
            Messenger.Default.Register(this, new Action<ProcessSynchronisationsRequest>(OnProcesSynchornisationsRequested));
        }

        #endregion // Constructor
    }
}
