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
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.ViewModels.Enums;

namespace TLCGen.ViewModels
{
    public class SynchronisatiesTabViewModel : TLCGenTabItemViewModel
    {

        #region Fields
        
        private ObservableCollection<string> _FasenNames;
        private bool _MatrixChanged;
        private SynchronisatieTypeEnum _DisplayType;

        #endregion // Fields

        #region Properties

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
                OnPropertyChanged(null);
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
            get { return _Controller.Data.Instellingen.GarantieOntruimingsTijden; }
            set
            {
                _Controller.Data.Instellingen.GarantieOntruimingsTijden = value;
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
                        svm.Gelijkstart.GelijkstartOntruimingstijdFaseVan = value;
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
                        svm.Gelijkstart.GelijkstartOntruimingstijdFaseNaar = value;
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

        public string IsMatrixOkWithGarantueed()
        {
            for (int i = 0; i < Fasen.Count; ++i)
            {
                for (int j = 0; j < Fasen.Count; ++j)
                {
                    // Skip from>to self
                    if (i == j || string.IsNullOrWhiteSpace(ConflictMatrix[i, j].ConflictValue))
                        continue;

                    string conf = ConflictMatrix[i, j].GetConflictValue();
                    string gconf = ConflictMatrix[i, j].GetGaratieConflictValue();
                    int outc;
                    int outgc;

                    if (Int32.TryParse(conf, out outc))
                    {
                        if (Int32.TryParse(gconf, out outgc))
                        {
                            if (outc < outgc)
                            {
                                return "Ontruimingstijd van " + Fasen[i].Naam + " naar " + Fasen[j].Naam + " lager dan garantie ontruimmingstijd.";
                            }
                        }
                        else
                        {
                            return "Ontbrekende garantie ontruimingstijd van " + Fasen[i].Naam + " naar " + Fasen[j].Naam + ".";
                        }
                    }
                    else if (Int32.TryParse(gconf, out outgc))
                    {
                        return "Ontbrekende ontruimingstijd van " + Fasen[i].Naam + " naar " + Fasen[j].Naam + ".";
                    }
                }
            }
            return null;
        }

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

                    foreach (ConflictModel cm in _Controller.InterSignaalGroep.Conflicten)
                    {
                        if (Fasen[fcm_from].Define == cm.FaseVan && Fasen[fcm_to].Define == cm.FaseNaar)
                        {
                            ConflictMatrix[fcm_from, fcm_to].Conflict = cm;
                            break;
                        }
                    }

                    foreach (NaloopModel nm in _Controller.InterSignaalGroep.Nalopen)
                    {
                        if (Fasen[fcm_from].Define == nm.FaseVan && Fasen[fcm_to].Define == nm.FaseNaar)
                        {
                            ConflictMatrix[fcm_from, fcm_to].Naloop = nm;
                            if (ConflictMatrix[fcm_to, fcm_from] == null)
                            {
                                ConflictMatrix[fcm_to, fcm_from] = new SynchronisatieViewModel();
                                ConflictMatrix[fcm_to, fcm_from].FaseVan = Fasen[fcm_from].Define;
                                ConflictMatrix[fcm_to, fcm_from].FaseNaar = Fasen[fcm_to].Define;
                            }
                            ConflictMatrix[fcm_to, fcm_from].HasOppositeNaloop = true;
                            break;
                        }
                    }

                    foreach (GelijkstartModel gm in _Controller.InterSignaalGroep.Gelijkstarten)
                    {
                        if (Fasen[fcm_from].Define == gm.FaseVan && Fasen[fcm_to].Define == gm.FaseNaar)
                        {
                            ConflictMatrix[fcm_from, fcm_to].Gelijkstart = gm;
                            break;
                        }
                    }

                    foreach (VoorstartModel vm in _Controller.InterSignaalGroep.Voorstarten)
                    {
                        if (Fasen[fcm_from].Define == vm.FaseVan && Fasen[fcm_to].Define == vm.FaseNaar)
                        {
                            ConflictMatrix[fcm_from, fcm_to].Voorstart = vm;
                            if (ConflictMatrix[fcm_to, fcm_from] == null)
                            {
                                ConflictMatrix[fcm_to, fcm_from] = new SynchronisatieViewModel();
                                ConflictMatrix[fcm_to, fcm_from].FaseVan = Fasen[fcm_from].Define;
                                ConflictMatrix[fcm_to, fcm_from].FaseNaar = Fasen[fcm_to].Define;
                            }
                            ConflictMatrix[fcm_to, fcm_from].HasOppositeVoorstart = true;
                            break;
                        }
                    }

                    if(!ConflictMatrix[fcm_from, fcm_to].IsOK())
                    {
                        throw new NotImplementedException();
                    }

                    ConflictMatrix[fcm_from, fcm_to].DisplayType = this.DisplayType;
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
            for (int fcvm_from = 0; fcvm_from < fccount; ++fcvm_from)
            {
                for (int fcvm_to = 0; fcvm_to < fccount; ++fcvm_to)
                {
                    var conflict = ConflictMatrix[fcvm_from, fcvm_to].GetConflictValue();
                    if (!string.IsNullOrWhiteSpace(conflict) && conflict != "X" && conflict != "*")
                    {
                        _Controller.InterSignaalGroep.Conflicten.Add(ConflictMatrix[fcvm_from, fcvm_to].Conflict);
                    }

                    if(ConflictMatrix[fcvm_from, fcvm_to].HasGelijkstart)
                    {
                        _Controller.InterSignaalGroep.Gelijkstarten.Add(ConflictMatrix[fcvm_from, fcvm_to].Gelijkstart);
                    }

                    if (ConflictMatrix[fcvm_from, fcvm_to].HasVoorstart)
                    {
                        _Controller.InterSignaalGroep.Voorstarten.Add(ConflictMatrix[fcvm_from, fcvm_to].Voorstart);
                    }

                    if (ConflictMatrix[fcvm_from, fcvm_to].HasNaloop)
                    {
                        _Controller.InterSignaalGroep.Nalopen.Add(ConflictMatrix[fcvm_from, fcvm_to].Naloop);
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
                if (message.Conflict.Waarde == -4)
                    return;

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

            MessageManager.Instance.Subscribe(this, new Action<FasenChangedMessage>(OnFasenChanged));
            MessageManager.Instance.Subscribe(this, new Action<FasenSortedMessage>(OnFasenSorted));
            MessageManager.Instance.Subscribe(this, new Action<InterSignaalGroepChangedMessage>(OnInterSignaalGroepChanged));
            MessageManager.Instance.Subscribe(this, new Action<ProcessSynchronisationsRequest>(OnProcesSynchornisationsRequested));
        }

        #endregion // Constructor
    }
}
