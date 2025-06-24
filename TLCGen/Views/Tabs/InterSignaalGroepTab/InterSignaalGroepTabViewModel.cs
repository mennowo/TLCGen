﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.ViewModels.Enums;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 6)]
    public class SynchronisatiesTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private List<string> _AllDetectoren;
        private ObservableCollection<string> _FasenNames;
        private ObservableCollection<string> _Detectoren;
        private SynchronisatieViewModel _selectedSynchronisatie;
        private GarantieTijdConvertHelper _SelectedGarantieTijdenConvertValue;
        private bool _MatrixChanged;
        private IntersignaalGroepTypeEnum _DisplayType;

        private RelayCommand _DeleteValueCommand;
        private RelayCommand _CheckItCommand;
        private RelayCommand _AddGarantieConvertValue;
        private RelayCommand _RemoveGarantieConvertValue;
        private RelayCommand _SetGarantieValuesCommand;

        #endregion // Fields

        #region Properties

        public IntersignaalGroepTypeEnum DisplayType
        {
            get => _DisplayType;
            set
            {
                _DisplayType = value;
                if (ConflictMatrix != null)
                {
                    foreach (var cvm in ConflictMatrix)
                    {
                        cvm.DisplayType = value;
                    }
                }
                if (SelectedSynchronisatie == null && ConflictMatrix != null && ConflictMatrix.GetLength(0) > 1 && ConflictMatrix.GetLength(1) > 1)
                {
                    SelectedSynchronisatie = ConflictMatrix[0, 1];
                }
                OnPropertyChanged("");
                _CheckItCommand?.NotifyCanExecuteChanged();
                _SetGarantieValuesCommand?.NotifyCanExecuteChanged();
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
        public IReadOnlyList<FaseCyclusModel> Fasen => _Controller.Fasen;

        public string TijdenLabel => _Controller?.Data?.Intergroen == true ? "Intergroen tijden" : "Ontruimingstijden";

        public string GarantieTijdenLabel => _Controller?.Data?.Intergroen == true ? "Gar. interg. tijden" : "Gar. ontr. tijden";

        public ObservableCollection<string> Detectoren => _Detectoren ??= new ObservableCollection<string>();

        public ObservableCollection<GarantieTijdConvertHelper> GarantieTijdenConvertValues { get; } = [];

        public GarantieTijdConvertHelper SelectedGarantieTijdenConvertValue
        {
            get => _SelectedGarantieTijdenConvertValue;
            set
            {
                _SelectedGarantieTijdenConvertValue = value;
                OnPropertyChanged();
                _RemoveGarantieConvertValue?.NotifyCanExecuteChanged();
            }
        }

        public SynchronisatieViewModel SelectedSynchronisatie
        {
            get => _selectedSynchronisatie;
            set
            {
                _selectedSynchronisatie = value;
                if (_Controller != null && value != null && _AllDetectoren != null && !string.IsNullOrEmpty(value?.FaseVan))
                {
                    Detectoren.Clear();
                    foreach (var fc in _Controller.Fasen)
                    {
                        if (fc.Naam == _selectedSynchronisatie.FaseVan)
                        {
                            foreach (var dm in fc.Detectoren)
                            {
                                Detectoren.Add(dm.Naam);
                            }
                            break;
                        }
                    }
                    var br = false;
                    for (var i1 = 0; i1 < ConflictMatrix.GetLength(0); ++i1)
                    {
                        for (var i2 = 0; i2 < ConflictMatrix.GetLength(1); ++i2)
                        {
                            if (ConflictMatrix[i1, i2] == value)
                            {
                                value.GelijkstartVm.MirroredViewModel = ConflictMatrix[i2, i1].GelijkstartVm;
                                br = true;
                                break;
                            }
                        }
                        if (br) break;
                    }
                    value.NaloopVm.DetectieAfhankelijkPossible = Detectoren.Count > 0;
                    value.MeeaanvraagVm.DetectieAfhankelijkPossible = Detectoren.Count > 0;
                }
                if (_selectedSynchronisatie?.DisplayType == IntersignaalGroepTypeEnum.Gelijkstart)
                {
                    OnPropertyChanged(nameof(GelijkstartDeelConflict));
                    OnPropertyChanged(nameof(GelijkstartOntruimingstijdFaseVan));
                    OnPropertyChanged(nameof(GelijkstartOntruimingstijdFaseNaar));
                }
                OnPropertyChanged("");
                _DeleteValueCommand?.NotifyCanExecuteChanged();
                _CheckItCommand?.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Symmetrical, two dimensional matrix used to display phasecycle conflicts.
        /// </summary>
        public SynchronisatieViewModel[,] ConflictMatrix
        {
            get => _conflictMatrix;
            set
            {
                _conflictMatrix = value;
                OnPropertyChanged();
                _SetGarantieValuesCommand?.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Boolean set by instances of ConflictViewModel when their DisplayWaarde property is 
        /// set by the user. We use this to monitor changes to the model, and to check if we need
        /// to check the matrix for symmetry if the user changes tabs, or tries to save the model.
        /// </summary>
        public bool MatrixChanged
        {
            get => _MatrixChanged;
            set
            {
                // TODO ckeck OK? if (value) OnPropertyChanged(nameof(MatrixChanged), _MatrixChanged, true, true);
                if (value) OnPropertyChanged(broadcast: true);
                _MatrixChanged = value;
            }
        }

        public bool UseGarantieOntruimingsTijden
        {
            get => _Controller?.Data?.GarantieOntruimingsTijden ?? false;
            set
            {
                _Controller.Data.GarantieOntruimingsTijden = value;
                MatrixChanged = true;
                OnPropertyChanged(broadcast: true);
                // TODO check OK? OnPropertyChanged(nameof(UseGarantieOntruimingsTijden), _Controller.Data.GarantieOntruimingsTijden, value, true);
            }
        }

        public bool RealFuncBepaalRealisatieTijdenAltijd
        {
            get => _Controller?.Data?.RealFuncBepaalRealisatieTijdenAltijd ?? false;
            set
            {
                _Controller.Data.RealFuncBepaalRealisatieTijdenAltijd = value;
                MatrixChanged = true;
                // TODO check OK? OnPropertyChanged(nameof(RealFuncBepaalRealisatieTijdenAltijd), _Controller.Data.RealFuncBepaalRealisatieTijdenAltijd, value, true);
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool IsSynchRealType => _Controller?.Data?.SynchronisatiesType == SynchronisatiesTypeEnum.RealFunc;

        public SynchronisatiesTypeEnum SynchronisatiesType
        {
            get => _Controller?.Data?.SynchronisatiesType ?? SynchronisatiesTypeEnum.RealFunc;
            set
            {
                _Controller.Data.SynchronisatiesType = value;
                SelectedSynchronisatie?.UpdateView();
                TLCGenModelManager.Default.UpdateControllerAlerts();
                OnPropertyChanged(nameof(SynchronisatiesType), broadcast: true);
                OnPropertyChanged(nameof(IsSynchRealType));
                WeakReferenceMessengerEx.Default.Send(new SynchronisatiesTypeChangedMessage());
            }
        }

        public bool ShowGarantieTijdenConverter => DisplayType == IntersignaalGroepTypeEnum.GarantieConflict;

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
                foreach (var svm in ConflictMatrix)
                {
                    if (svm.FaseVan == SelectedSynchronisatie.FaseNaar && svm.FaseNaar == SelectedSynchronisatie.FaseVan)
                    {
                        svm.Gelijkstart.GelijkstartOntruimingstijdFaseNaar = value;
                    }
                }
                OnPropertyChanged();
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
                foreach (var svm in ConflictMatrix)
                {
                    if (svm.FaseVan == SelectedSynchronisatie.FaseNaar && svm.FaseNaar == SelectedSynchronisatie.FaseVan)
                    {
                        svm.Gelijkstart.GelijkstartOntruimingstijdFaseVan = value;
                    }
                }
                OnPropertyChanged();
            }
        }
        public bool GelijkstartDeelConflict
        {
            get
            {
                if (SelectedSynchronisatie == null)
                    return false;

                return SelectedSynchronisatie.Gelijkstart.DeelConflict;
            }
            set
            {
                SelectedSynchronisatie.Gelijkstart.DeelConflict = value;
                foreach (var svm in ConflictMatrix)
                {
                    if (svm.FaseVan == SelectedSynchronisatie.FaseNaar && svm.FaseNaar == SelectedSynchronisatie.FaseVan)
                    {
                        svm.Gelijkstart.DeelConflict = value;
                    }
                }
                OnPropertyChanged();
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
                OnPropertyChanged(nameof(VoorstartTijd), broadcast: true);
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
                OnPropertyChanged(nameof(VoorstartOntruimingstijd), broadcast: true);
            }
        }

        public string Comment1
        {
            get
            {
                switch (DisplayType)
                {
                    case IntersignaalGroepTypeEnum.Gelijkstart:
                        return $"Fictive ontruimingstijd van {SelectedSynchronisatie.FaseVan} naar {SelectedSynchronisatie.FaseNaar}";
                    case IntersignaalGroepTypeEnum.Voorstart:
                        return $"Voorstarttijd van {SelectedSynchronisatie.FaseVan} naar {SelectedSynchronisatie.FaseNaar}";
                    case IntersignaalGroepTypeEnum.Meeaanvraag:
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
                var tijd = _Controller.Data.Intergroen ? "intergroentijd" : "ontruimingstijd";
                switch (DisplayType)
                {
                    case IntersignaalGroepTypeEnum.Gelijkstart:
                        return $"Fictieve {tijd} van {SelectedSynchronisatie.FaseNaar} naar {SelectedSynchronisatie.FaseVan}";
                    case IntersignaalGroepTypeEnum.Voorstart:
                        return $"Voorstart {tijd} van {SelectedSynchronisatie.FaseVan} naar {SelectedSynchronisatie.FaseNaar}";
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

        public override ImageSource Icon
        {
            get
            {
                var dict = new ResourceDictionary();
                var u = new Uri("pack://application:,,,/" +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                    ";component/" + "Resources/TabIcons.xaml");
                dict.Source = u;
                return (DrawingImage)dict["InterSignaalGroepTabDrawingImage"];
            }
        }

        public override string DisplayName => "InterSignaalGroep";

        public override bool IsEnabled
        {
            get => true;
            set { }
        }

        public override bool OnDeselectedPreview()
        {
            if (_Controller != null)
            {
                return IsMatrixOK();
            }
            else
            {
                return true;
            }
        }

        public override void OnSelected()
        {
            _AllDetectoren = new List<string>();
            if (Controller != null)
            {
                foreach (var fcm in Controller.Fasen)
                {
                    foreach (var dm in fcm.Detectoren)
                    {
                        _AllDetectoren.Add(dm.Naam);
                    }
                }
                foreach (var dm in Controller.Detectoren)
                {
                    _AllDetectoren.Add(dm.Naam);
                }
            }
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                DisplayType = IntersignaalGroepTypeEnum.Conflict;
                BuildConflictMatrix();
            }
        }

        #endregion // TabItem Overrides

        #region Commands

        public ICommand DeleteValueCommand => _DeleteValueCommand ??= new RelayCommand(() =>
            {
                if (SelectedSynchronisatie != null)
                    SelectedSynchronisatie.ConflictValue = "";
            }, 
            () => SelectedSynchronisatie != null);

        public ICommand CheckItCommand => _CheckItCommand ??= new RelayCommand(() =>
            {
                var b = SelectedSynchronisatie.IsCoupled;
                SelectedSynchronisatie.IsCoupled = !b;
            }, () => SelectedSynchronisatie != null &&
                 DisplayType != IntersignaalGroepTypeEnum.Conflict &&
                 DisplayType != IntersignaalGroepTypeEnum.GarantieConflict);

        public ICommand SetGarantieValuesCommand => _SetGarantieValuesCommand ??= new RelayCommand(() =>
            {
                var fccount = Fasen.Count;

                for (var fcvm_from = 0; fcvm_from < fccount; ++fcvm_from)
                {
                    for (var fcvm_to = 0; fcvm_to < fccount; ++fcvm_to)
                    {
                        int i;
                        if (!string.IsNullOrWhiteSpace(ConflictMatrix[fcvm_from, fcvm_to].GetConflictValue()) && Int32.TryParse(ConflictMatrix[fcvm_from, fcvm_to].GetConflictValue(), out i))
                        {
                            foreach (var conv in GarantieTijdenConvertValues)
                            {
                                if (i >= conv.Van && i < conv.Tot)
                                {
                                    i -= conv.Verschil;
                                    if (i < 0) i = 0;
                                    break;
                                }
                            }
                            ConflictMatrix[fcvm_from, fcvm_to].ConflictValue = i.ToString();
                        }
                    }
                }
            }, 
            () => ConflictMatrix != null &&
                     DisplayType == IntersignaalGroepTypeEnum.GarantieConflict &&
                     GarantieTijdenConvertValues.Count > 0);

        public ICommand AddGarantieConvertValue => _AddGarantieConvertValue ??= new RelayCommand(() =>
            {
                var h = new GarantieTijdConvertHelper(this);

                _SettingGarConvs = true;

                if (GarantieTijdenConvertValues.Count > 0)
                {
                    h.Tot = GarantieTijdenConvertValues[GarantieTijdenConvertValues.Count - 1].Tot;
                    h.MinVan = h.Van = h.Tot;
                }
                else
                {
                    h.MinVan = h.Van = h.Tot = 0;
                }
                h.Verschil = 0;

                _SettingGarConvs = false;

                GarantieTijdenConvertValues.Add(h);
                _SetGarantieValuesCommand?.NotifyCanExecuteChanged();
            });

        public ICommand RemoveGarantieConvertValue => _RemoveGarantieConvertValue ??= new RelayCommand(() =>
            {
                GarantieTijdenConvertValues.Remove(SelectedGarantieTijdenConvertValue);
                SelectedGarantieTijdenConvertValue = null;

                SetGarantieConvertValuesVan();
                SetGarantieConvertValuesTot();
                _SetGarantieValuesCommand?.NotifyCanExecuteChanged();
            }, () => SelectedGarantieTijdenConvertValue != null);

        #endregion // Commands

        #region Private methods

        private bool IsMatrixOK()
        {
            SaveConflictMatrix();

            var s = Integrity.TLCGenIntegrityChecker.IsConflictMatrixOK(_Controller);
            if (s == null)
            {
                if (_MatrixChanged == true)
                {
                    Integrity.TLCGenControllerModifier.Default.CorrectModel_AlteredConflicts();
WeakReferenceMessengerEx.Default.Send(new ConflictsChangedMessage());
                }
                _MatrixChanged = false;
                return true;
            }
            else
            {
                Task.Factory.StartNew(() => MessageBox.Show(s, "Fout in conflictmatrix"));
                return false;
            }
        }

        #endregion // Private methods

        #region Public methods

        private bool _SettingGarConvs = false;

        public void SetGarantieConvertValuesVan()
        {
            if (_SettingGarConvs || GarantieTijdenConvertValues.Count < 2)
                return;

            _SettingGarConvs = true;

            for (var i = 1; i < GarantieTijdenConvertValues.Count; ++i)
            {
                GarantieTijdenConvertValues[i].MinVan = GarantieTijdenConvertValues[i - 1].Tot;
                GarantieTijdenConvertValues[i].Van = GarantieTijdenConvertValues[i].MinVan;
            }

            _SettingGarConvs = false;
        }

        public void SetGarantieConvertValuesTot()
        {
            if (_SettingGarConvs || GarantieTijdenConvertValues.Count < 2)
                return;

            _SettingGarConvs = true;

            for (var i = 0; i < (GarantieTijdenConvertValues.Count - 1); ++i)
            {
                GarantieTijdenConvertValues[i].Tot = GarantieTijdenConvertValues[i + 1].Van;
            }

            _SettingGarConvs = false;
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
            {
                ConflictMatrix = null;
                OnPropertyChanged("ConflictMatrix");
                return;
            }

            var fccount = Fasen.Count;

            _FasenNames = new ObservableCollection<string>();
            foreach (var fcvm in Fasen)
            {
                FasenNames.Add(fcvm.Naam);
            }
            OnPropertyChanged("FasenNames");

            ConflictMatrix = new SynchronisatieViewModel[fccount, fccount];
            for (var fcm_from = 0; fcm_from < fccount; ++fcm_from)
            {
                for (var fcm_to = 0; fcm_to < fccount; ++fcm_to)
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
                            ConflictMatrix[fcm_from, fcm_to].FaseVan = Fasen[fcm_from].Naam;
                            ConflictMatrix[fcm_from, fcm_to].FaseNaar = Fasen[fcm_to].Naam;
                        }
                    }
                    ConflictMatrix[fcm_from, fcm_to].DisplayType = this.DisplayType;
                }
            }

            foreach (var cm in _Controller.InterSignaalGroep.Conflicten)
            {
                foreach (var svm in ConflictMatrix)
                {
                    if (svm.FaseVan == cm.FaseVan && svm.FaseNaar == cm.FaseNaar)
                    {
                        svm.Conflict = cm;
                        break;
                    }
                }
            }

            foreach (var nm in _Controller.InterSignaalGroep.Nalopen)
            {
                foreach (var svm in ConflictMatrix)
                {
                    if (svm.FaseVan == nm.FaseVan && svm.FaseNaar == nm.FaseNaar)
                    {
                        svm.Naloop = nm;
                        foreach (var svm_opp in ConflictMatrix)
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

            foreach (var gm in _Controller.InterSignaalGroep.Gelijkstarten)
            {
                foreach (var svm in ConflictMatrix)
                {
                    if (svm.FaseVan == gm.FaseVan && svm.FaseNaar == gm.FaseNaar)
                    {
                        svm.Gelijkstart = gm;
                        foreach (var svm_opp in ConflictMatrix)
                        {
                            if (svm_opp.FaseVan == gm.FaseNaar && svm_opp.FaseNaar == gm.FaseVan)
                            {
                                svm_opp.HasGelijkstart = true;
                                svm_opp.Gelijkstart.GelijkstartOntruimingstijdFaseNaar = svm.Gelijkstart.GelijkstartOntruimingstijdFaseVan;
                                svm_opp.Gelijkstart.GelijkstartOntruimingstijdFaseVan = svm.Gelijkstart.GelijkstartOntruimingstijdFaseNaar;
                                svm_opp.Gelijkstart.Schakelbaar = svm.Gelijkstart.Schakelbaar;
                                svm_opp.Gelijkstart.DeelConflict = svm.Gelijkstart.DeelConflict;
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            foreach (var vm in _Controller.InterSignaalGroep.Voorstarten)
            {
                foreach (var svm in ConflictMatrix)
                {
                    if (svm.FaseVan == vm.FaseVan && svm.FaseNaar == vm.FaseNaar)
                    {
                        svm.Voorstart = vm;
                        foreach (var svm_opp in ConflictMatrix)
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

            foreach (var lrm in _Controller.InterSignaalGroep.LateReleases)
            {
                foreach (var svm in ConflictMatrix)
                {
                    if (svm.FaseVan == lrm.FaseVan && svm.FaseNaar == lrm.FaseNaar)
                    {
                        svm.LateRelease = lrm;
                        foreach (var svm_opp in ConflictMatrix)
                        {
                            if (svm_opp.FaseVan == lrm.FaseNaar && svm_opp.FaseNaar == lrm.FaseVan)
                            {
                                svm_opp.HasOppositeLateRelease = true;
                                break;
                            }
                        }
                    }
                }
            }

            foreach (var mm in _Controller.InterSignaalGroep.Meeaanvragen)
            {
                foreach (var svm in ConflictMatrix)
                {
                    if (svm.FaseVan == mm.FaseVan && svm.FaseNaar == mm.FaseNaar)
                    {
                        svm.Meeaanvraag = mm;
                        foreach (var svm_opp in ConflictMatrix)
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

            _MatrixChanged = false;

            if (SelectedSynchronisatie == null && ConflictMatrix != null && ConflictMatrix.GetLength(0) > 1 && ConflictMatrix.GetLength(1) > 1)
            {
                SelectedSynchronisatie = ConflictMatrix[0, 1];
            }
        }

        /// <summary>
        /// Reads the property string[,] ConflictMatrix and saves all relevant entries as 
        /// instances of ConflictViewmodel in the collection of the relevant FaseCyclusViewModel.
        /// This also updates the Model data.
        /// </summary>
        public void SaveConflictMatrix()
        {
            if (ConflictMatrix == null || Fasen == null || !_MatrixChanged)
            {
                return;
            }

            var fccount = Fasen.Count;

            // Call extension method RemoveAll instead of built-in Clear(), see:
            // http://stackoverflow.com/questions/224155/when-clearing-an-observablecollection-there-are-no-items-in-e-olditems
            _Controller.InterSignaalGroep.Conflicten.RemoveAll();
            _Controller.InterSignaalGroep.Voorstarten.RemoveAll();
            _Controller.InterSignaalGroep.Gelijkstarten.RemoveAll();
            _Controller.InterSignaalGroep.Nalopen.RemoveAll();
            _Controller.InterSignaalGroep.Meeaanvragen.RemoveAll();
            _Controller.InterSignaalGroep.LateReleases.RemoveAll();
            var gelijkstartsaved = new bool[fccount, fccount];
            for (var fcvm_from = 0; fcvm_from < fccount; ++fcvm_from)
            {
                for (var fcvm_to = 0; fcvm_to < fccount; ++fcvm_to)
                {
                    if (ConflictMatrix[fcvm_from, fcvm_to].ReferencesSelf) continue;

                    if (ConflictMatrix[fcvm_from, fcvm_to].HasConflict || ConflictMatrix[fcvm_from, fcvm_to].HasGarantieConflict)
                    {
                        _Controller.InterSignaalGroep.Conflicten.Add(ConflictMatrix[fcvm_from, fcvm_to].Conflict);
                    }

                    if (ConflictMatrix[fcvm_from, fcvm_to].HasGelijkstart)
                    {
                        if (!gelijkstartsaved[fcvm_from, fcvm_to])
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

                    if (ConflictMatrix[fcvm_from, fcvm_to].HasLateRelease)
                    {
                        _Controller.InterSignaalGroep.LateReleases.Add(ConflictMatrix[fcvm_from, fcvm_to].LateRelease);
                    }
                }
            }
        }

        #endregion // Public methods

        #region TLCGen Event handling

        private void OnFasenChanged(object sender, FasenChangedMessage message)
        {
            BuildConflictMatrix();
        }

        private void OnFasenSorted(object sender, FasenSortedMessage message)
        {
            BuildConflictMatrix();
        }

        private void OnNameChanged(object sender, NameChangedMessage message)
        {
            if (Fasen.Any(x => x.Naam == message.NewName))
            {
                BuildConflictMatrix();
            }
        }

        private void OnInterSignaalGroepChanged(object sender, InterSignaalGroepChangedMessage message)
        {
            _MatrixChanged = true;

            if (message.IsNew)
            {
                BuildConflictMatrix();
                return;
            }

            // Conflict
            if (message.Conflict != null)
            {
                var fccount = Fasen.Count;
                for (var fcm_from = 0; fcm_from < fccount; ++fcm_from)
                {
                    for (var fcm_to = 0; fcm_to < fccount; ++fcm_to)
                    {
                        if (Fasen[fcm_from].Naam == message.Conflict.FaseVan &&
                           Fasen[fcm_to].Naam == message.Conflict.FaseNaar)
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
                                    else
                                    {
                                        if (int.TryParse(cvm2.ConflictValue, out _))
                                        {
                                            ConflictMatrix[fcm_from, fcm_to].ConflictValueNoMessaging = "*";
                                        }
                                    }
                                    break;
                                default:
                                    if (int.TryParse(cvm2.ConflictValue, out _))
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
                throw new InvalidOperationException();
            }

            // Naloop
            if (message.Naloop != null)
            {
                var fccount = Fasen.Count;
                for (var fcm_from = 0; fcm_from < fccount; ++fcm_from)
                {
                    for (var fcm_to = 0; fcm_to < fccount; ++fcm_to)
                    {
                        if (Fasen[fcm_from].Naam == message.Naloop.FaseVan &&
                            Fasen[fcm_to].Naam == message.Naloop.FaseNaar)
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
                var fccount = Fasen.Count;
                for (var fcm_from = 0; fcm_from < fccount; ++fcm_from)
                {
                    for (var fcm_to = 0; fcm_to < fccount; ++fcm_to)
                    {
                        if (Fasen[fcm_from].Naam == message.Voorstart.FaseVan &&
                            Fasen[fcm_to].Naam == message.Voorstart.FaseNaar)
                        {
                            var cvm2 = ConflictMatrix[fcm_to, fcm_from];
                            cvm2.HasOppositeVoorstart = message.IsCoupled;
                        }
                    }
                }
            }

            // LateRelease
            if (message.LateRelease != null)
            {
                var fccount = Fasen.Count;
                for (var fcm_from = 0; fcm_from < fccount; ++fcm_from)
                {
                    for (var fcm_to = 0; fcm_to < fccount; ++fcm_to)
                    {
                        if (Fasen[fcm_from].Naam == message.LateRelease.FaseVan &&
                            Fasen[fcm_to].Naam == message.LateRelease.FaseNaar)
                        {
                            var cvm2 = ConflictMatrix[fcm_to, fcm_from];
                            cvm2.HasOppositeLateRelease = message.IsCoupled;
                        }
                    }
                }
            }

            // Gelijkstart
            if (message.Gelijkstart != null)
            {
                var fccount = Fasen.Count;
                for (var fcm_from = 0; fcm_from < fccount; ++fcm_from)
                {
                    for (var fcm_to = 0; fcm_to < fccount; ++fcm_to)
                    {
                        if (Fasen[fcm_from].Naam == message.Gelijkstart.FaseVan &&
                            Fasen[fcm_to].Naam == message.Gelijkstart.FaseNaar)
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
                var fccount = Fasen.Count;
                for (var fcm_from = 0; fcm_from < fccount; ++fcm_from)
                {
                    for (var fcm_to = 0; fcm_to < fccount; ++fcm_to)
                    {
                        if (Fasen[fcm_from].Naam == message.Meeaanvraag.FaseVan &&
                            Fasen[fcm_to].Naam == message.Meeaanvraag.FaseNaar)
                        {
                            var cvm2 = ConflictMatrix[fcm_to, fcm_from];
                            cvm2.HasOppositeMeeaanvraag = message.IsCoupled;
                        }
                    }
                }
            }
        }

        private bool _IsProcessing = false;
        private SynchronisatieViewModel[,] _conflictMatrix;

        private void OnProcesSynchornisationsRequested(object sender, ProcessSynchronisationsRequest request)
        {
            if (_IsProcessing)
                return;

            _IsProcessing = true;
            SaveConflictMatrix();
            _IsProcessing = false;
        }

        private void OnIntergreenTimesTypeChanged(object sender, ControllerIntergreenTimesTypeChangedMessage msg)
        {
            OnPropertyChanged(nameof(TijdenLabel));
            OnPropertyChanged(nameof(GarantieTijdenLabel));
        }

        #endregion // TLCGen Event handling

        #region Constructor

        public SynchronisatiesTabViewModel() : base()
        {
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessengerEx.Default.Register<FasenSortedMessage>(this, OnFasenSorted);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
            WeakReferenceMessengerEx.Default.Register<InterSignaalGroepChangedMessage>(this, OnInterSignaalGroepChanged);
            WeakReferenceMessengerEx.Default.Register<ProcessSynchronisationsRequest>(this, OnProcesSynchornisationsRequested);
        }

        #endregion // Constructor
    }
}
