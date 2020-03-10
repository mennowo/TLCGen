using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TLCGen.Plugins.DynamischHiaat.Models;
using TLCGen.Extensions;
using TLCGen.Helpers;

namespace TLCGen.Plugins.DynamischHiaat.ViewModels
{
    internal class DynamischHiaatSignalGroupViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        #region Fields

        private ObservableCollectionAroundList<DynamischHiaatDetectorViewModel, DynamischHiaatDetectorModel> _DynamischHiaatDetectoren;

        #endregion // Fields

        #region Properties

        [Browsable(false)]
        public DynamischHiaatSignalGroupModel SignalGroup { get; }

        [Browsable(false)]
        public DynamischHiaatDefaultModel SelectedDefault
        {
            get => _selectedDefault;
            set
            {
                _selectedDefault = value;
                RaisePropertyChanged();
                if(value != null)
                {
                    var v = Snelheid;
                    SelectedDefaultSnelheden = value.Snelheden;
                    RaisePropertyChanged(nameof(SelectedDefaultSnelheden));
                    Snelheid = v;
                }
            }
        }

        public List<DynamischHiaatSpeedDefaultModel> SelectedDefaultSnelheden { get; private set; }

        [Browsable(false)]
        public ObservableCollectionAroundList<DynamischHiaatDetectorViewModel, DynamischHiaatDetectorModel> DynamischHiaatDetectoren => _DynamischHiaatDetectoren ?? (_DynamischHiaatDetectoren = new ObservableCollectionAroundList<DynamischHiaatDetectorViewModel, DynamischHiaatDetectorModel>(SignalGroup.DynamischHiaatDetectoren));

        private AddRemoveItemsManager<DynamischHiaatDetectorViewModel, DynamischHiaatDetectorModel, string> _DynamischHiaatDetectorenManager;
        private DynamischHiaatDefaultModel _selectedDefault;

        [Browsable(false)]
        public AddRemoveItemsManager<DynamischHiaatDetectorViewModel, DynamischHiaatDetectorModel, string> DynamischHiaatDetectorenManager =>
            _DynamischHiaatDetectorenManager ?? (_DynamischHiaatDetectorenManager = new AddRemoveItemsManager<DynamischHiaatDetectorViewModel, DynamischHiaatDetectorModel, string>(
            DynamischHiaatDetectoren,
            x => new DynamischHiaatDetectorViewModel(new DynamischHiaatDetectorModel { DetectorName = x, SignalGroupName = SignalGroup.SignalGroupName }),
            (x, y) => x.Detector.DetectorName == y
            ));

        [Browsable(false)]
        public string SignalGroupName
        {
            get => SignalGroup.SignalGroupName;
            set
            {
                SignalGroup.SignalGroupName = value;
                RaisePropertyChanged();
            }
        }

        [Browsable(false)]
        public bool HasDynamischHiaat
        {
            get => SignalGroup.HasDynamischHiaat;
            set
            {
                SignalGroup.HasDynamischHiaat = value;
                if (value)
                {
                    var fc = DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.FirstOrDefault(x => x.Naam == SignalGroupName);
                    if (fc != null)
                    {
                        fc.ToepassenMK2 = true;
                        foreach (var d in fc.Detectoren)
                        {
                            if (!DynamischHiaatDetectoren.Any(x => x.DetectorName == d.Naam) &&
                                (d.Type == TLCGen.Models.Enumerations.DetectorTypeEnum.Kop ||
                                 d.Type == TLCGen.Models.Enumerations.DetectorTypeEnum.Lang ||
                                 d.Type == TLCGen.Models.Enumerations.DetectorTypeEnum.Verweg))
                            {
                                DynamischHiaatDetectoren.Add(new DynamischHiaatDetectorViewModel(new DynamischHiaatDetectorModel
                                {
                                    DetectorName = d.Naam,
                                    SignalGroupName = SignalGroupName
                                }));
                            }
                        }
                        DynamischHiaatDetectoren.BubbleSort();
                        ApplySnelheidsDefaultsToDetectoren(Snelheid);
                    }
                }
                else
                {
                    DynamischHiaatDetectoren.RemoveAll();
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool Opdrempelen
        {
            get => SignalGroup.Opdrempelen;
            set
            {
                SignalGroup.Opdrempelen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string Snelheid
        {
            get => SignalGroup.Snelheid;
            set
            {
                if (SignalGroup.Snelheid != value)
                {
                    ApplySnelheidsDefaultsToDetectoren(value);
                    RaisePropertyChanged<object>(broadcast: true);
                }
                SignalGroup.Snelheid = value;
            }
        }

        [Description("Hiaatmeting vanaf ED koplus")]
        public bool KijkenNaarKoplus
        {
            get => SignalGroup.KijkenNaarKoplus;
            set
            {
                SignalGroup.KijkenNaarKoplus = value;
                RaisePropertyChanged();
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region IViewModelWithItem

        public object GetItem()
        {
            return SignalGroup;
        }

        #endregion // IViewModelWithItem

        #region IComparable

        public int CompareTo(object obj)
        {
            return SignalGroup.SignalGroupName.CompareTo(((DynamischHiaatSignalGroupViewModel)obj).SignalGroup.SignalGroupName);
        }

        #endregion // IComparable

        #region Public Methods

        public void UpdateSelectableDetectoren(IEnumerable<string> detectoren)
        {
            DynamischHiaatDetectorenManager.UpdateSelectables(detectoren);
        }

        #endregion // Public Methods

        #region Private Methods

        private void ApplySnelheidsDefaultsToDetectoren(string snelheid)
        {
            if (snelheid != null)
            {
                var dr = new int[10];
                var sd = SelectedDefault.Snelheden.FirstOrDefault(x => x.Name == snelheid);
                if (sd != null)
                {
                    for (var d = 0; d < DynamischHiaatDetectoren.Count; d++)
                    {
                        var od = DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.SelectMany(x => x.Detectoren).FirstOrDefault(x => x.Naam == DynamischHiaatDetectoren[d].DetectorName);
                        if(od != null && od.Rijstrook.HasValue && od.Rijstrook > 0 && od.Rijstrook <= 10)
                        {
                            dr[od.Rijstrook.Value - 1]++;
                        }
                        else
                        {
                            continue;
                        }
                        if (dr[od.Rijstrook.Value - 1] > 0 && (dr[od.Rijstrook.Value - 1] - 1) < sd.Detectoren.Count)
                        {
                            DynamischHiaatDetectoren[d].Moment1 = sd.Detectoren[dr[od.Rijstrook.Value - 1] - 1].Moment1;
                            DynamischHiaatDetectoren[d].Moment2 = sd.Detectoren[dr[od.Rijstrook.Value - 1] - 1].Moment2;
                            DynamischHiaatDetectoren[d].TDH1 = sd.Detectoren[dr[od.Rijstrook.Value - 1] - 1].TDH1;
                            DynamischHiaatDetectoren[d].TDH2 = sd.Detectoren[dr[od.Rijstrook.Value - 1] - 1].TDH2;
                            DynamischHiaatDetectoren[d].Maxtijd = sd.Detectoren[dr[od.Rijstrook.Value - 1] - 1].Maxtijd;
                            DynamischHiaatDetectoren[d].SpringStart = sd.Detectoren[dr[od.Rijstrook.Value - 1] - 1].SpringStart;
                            DynamischHiaatDetectoren[d].VerlengNiet = sd.Detectoren[dr[od.Rijstrook.Value - 1] - 1].VerlengNiet;
                            DynamischHiaatDetectoren[d].VerlengExtra = sd.Detectoren[dr[od.Rijstrook.Value - 1] - 1].VerlengExtra;
                            DynamischHiaatDetectoren[d].SpringGroen = sd.Detectoren[dr[od.Rijstrook.Value - 1] - 1].SpringGroen;
                            DynamischHiaatDetectoren[d].DirectAftellen = sd.Detectoren[dr[od.Rijstrook.Value - 1] - 1].DirectAftellen;
                        }
                    }
                }
            }
            UpdateSelectableDetectoren(null);
        }

        #endregion

        #region Constructor

        public DynamischHiaatSignalGroupViewModel(DynamischHiaatSignalGroupModel signalGroup)
        {
            SignalGroup = signalGroup;
        }

        #endregion // Constructor
    }
}
