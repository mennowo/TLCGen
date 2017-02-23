using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Settings
{
    public class FaseCyclusTemplateViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private TLCGenTemplateModel<FaseCyclusModel> _Template;

        #endregion // Fields

        #region Properties

        public string Naam
        {
            get { return _Template.Naam; }
            set
            {
                _Template.Naam = value;
                RaisePropertyChanged("Naam");
            }
        }

        public string Replace
        {
            get { return _Template.Replace; }
            set
            {
                _Template.Replace = value;
                RaisePropertyChanged("Replace");
            }
        }

        public bool IsReplaceAvailable
        {
            get
            {
                return Fasen.Count == 1;
            }
        }

        private FaseCyclusModel _SelectedFaseCyclus;
        public FaseCyclusModel SelectedFaseCyclus
        {
            get { return _SelectedFaseCyclus; }
            set
            {
                _SelectedFaseCyclus = value;
                FaseDetectoren.CollectionChanged -= FaseDetectoren_CollectionChanged;
                FaseDetectoren.Clear();
                if (value != null)
                {
                    foreach (DetectorModel d in value.Detectoren)
                    {
                        FaseDetectoren.Add(d);
                    }
                }
                FaseDetectoren.CollectionChanged += FaseDetectoren_CollectionChanged;

                if (value != null)
                    SelectedFaseCyclusTypeString = value.Type.GetDescription();
                else
                    SelectedFaseCyclusTypeString = null;

                RaisePropertyChanged(null);
            }
        }
        
        public string SelectedFaseCyclusDetectorNaam
        {
            get { return _SelectedFaseCyclusDetector?.Naam; }
            set
            {
                _SelectedFaseCyclusDetector.Naam = value;

                // Hack to update name in listview without writing new VMs
                var sd = SelectedFaseCyclusDetector;
                FaseDetectoren.CollectionChanged -= FaseDetectoren_CollectionChanged;
                FaseDetectoren.Clear();
                if (_SelectedFaseCyclus != null)
                {
                    foreach (DetectorModel d in _SelectedFaseCyclus.Detectoren)
                    {
                        FaseDetectoren.Add(d);
                    }
                }
                FaseDetectoren.CollectionChanged += FaseDetectoren_CollectionChanged;
                SelectedFaseCyclusDetector = sd;
                // End of trick

                RaisePropertyChanged("SelectedFaseCyclusDetectorNaam");
            }
        }

        public string SelectedFaseCyclusDetectorVissimNaam
        {
            get { return _SelectedFaseCyclusDetector?.VissimNaam; }
            set
            {
                _SelectedFaseCyclusDetector.VissimNaam = value;
                RaisePropertyChanged("SelectedFaseCyclusDetectorVissimNaam");
            }
        }

        private DetectorModel _SelectedFaseCyclusDetector;
        public DetectorModel SelectedFaseCyclusDetector
        {
            get { return _SelectedFaseCyclusDetector; }
            set
            {
                _SelectedFaseCyclusDetector = value;
                if (value != null)
                    SelectedDetectorTypeString = value.Type.GetDescription();
                else
                    SelectedDetectorTypeString = null;

                RaisePropertyChanged("SelectedFaseCyclusDetectorNaam");
                RaisePropertyChanged("SelectedFaseCyclusDetectorVissimNaam");
                RaisePropertyChanged("SelectedFaseCyclusDetector");
                RaisePropertyChanged("SelectedDetectorTypeString");
            }
        }

        private ObservableCollection<FaseCyclusModel> _Fasen;
        public ObservableCollection<FaseCyclusModel> Fasen
        {
            get
            {
                if (_Fasen == null)
                {
                    _Fasen = new ObservableCollection<FaseCyclusModel>();
                }
                return _Fasen;
            }
        }

        private ObservableCollection<DetectorModel> _FaseDetectoren;
        public ObservableCollection<DetectorModel> FaseDetectoren
        {
            get
            {
                if (_FaseDetectoren == null)
                {
                    _FaseDetectoren = new ObservableCollection<DetectorModel>();
                }
                return _FaseDetectoren;
            }
        }

        private List<string> _FaseCyclusTypeOpties;
        public List<string> FaseCyclusTypeOpties
        {
            get
            {
                if (_FaseCyclusTypeOpties == null)
                {
                    _FaseCyclusTypeOpties = new List<string>();
                }
                return _FaseCyclusTypeOpties;
            }
        }

        private List<string> _DetectorTypeOpties;
        public List<string> DetectorTypeOpties
        {
            get
            {
                if (_DetectorTypeOpties == null)
                {
                    _DetectorTypeOpties = new List<string>();
                }
                return _DetectorTypeOpties;
            }
        }

        private string _SelectedFaseCyclusTypeString;
        public string SelectedFaseCyclusTypeString
        {
            get { return _SelectedFaseCyclusTypeString; }
            set
            {
                _SelectedFaseCyclusTypeString = value;
                if (SelectedFaseCyclus != null)
                {
                    if (value == FaseTypeEnum.Auto.GetDescription())
                    {
                        SelectedFaseCyclus.Type = FaseTypeEnum.Auto;

                    }
                    else if (value == FaseTypeEnum.Fiets.GetDescription())
                    {
                        SelectedFaseCyclus.Type = FaseTypeEnum.Fiets;

                    }
                    else if (value == FaseTypeEnum.Voetganger.GetDescription())
                    {
                        SelectedFaseCyclus.Type = FaseTypeEnum.Voetganger;

                    }
                    else if (value == FaseTypeEnum.OV.GetDescription())
                    {
                        SelectedFaseCyclus.Type = FaseTypeEnum.OV;
                    }
                }
                RaisePropertyChanged("SelectedFaseCyclusTypeString");
            }
        }

        private string _SelectedDetectorTypeString;
        public string SelectedDetectorTypeString
        {
            get { return _SelectedDetectorTypeString; }
            set
            {
                _SelectedDetectorTypeString = value;
                if (SelectedFaseCyclusDetector != null)
                {
                    if (value == DetectorTypeEnum.Kop.GetDescription())
                    {
                        SelectedFaseCyclusDetector.Type = DetectorTypeEnum.Kop;

                    }
                    else if (value == DetectorTypeEnum.Lang.GetDescription())
                    {
                        SelectedFaseCyclusDetector.Type = DetectorTypeEnum.Lang;

                    }
                    else if (value == DetectorTypeEnum.Verweg.GetDescription())
                    {
                        SelectedFaseCyclusDetector.Type = DetectorTypeEnum.Verweg;

                    }
                    else if (value == DetectorTypeEnum.File.GetDescription())
                    {
                        SelectedFaseCyclusDetector.Type = DetectorTypeEnum.File;
                    }
                    else if (value == DetectorTypeEnum.KnopBinnen.GetDescription())
                    {
                        SelectedFaseCyclusDetector.Type = DetectorTypeEnum.KnopBinnen;
                    }
                    else if (value == DetectorTypeEnum.KnopBuiten.GetDescription())
                    {
                        SelectedFaseCyclusDetector.Type = DetectorTypeEnum.KnopBuiten;
                    }
                    else if (value == DetectorTypeEnum.Radar.GetDescription())
                    {
                        SelectedFaseCyclusDetector.Type = DetectorTypeEnum.Radar;
                    }
                }
                RaisePropertyChanged("SelectedDetectorTypeString");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddFaseCommand;
        public ICommand AddFaseCommand
        {
            get
            {
                if (_AddFaseCommand == null)
                {
                    _AddFaseCommand = new RelayCommand(new Action<object>(AddFaseCommand_Executed), new Predicate<object>(AddFaseCommand_CanExecute));
                }
                return _AddFaseCommand;
            }
        }


        RelayCommand _RemoveFaseCommand;
        public ICommand RemoveFaseCommand
        {
            get
            {
                if (_RemoveFaseCommand == null)
                {
                    _RemoveFaseCommand = new RelayCommand(new Action<object>(RemoveFaseCommand_Executed), new Predicate<object>(RemoveFaseCommand_CanExecute));
                }
                return _RemoveFaseCommand;
            }
        }

        RelayCommand _AddFaseDetectorCommand;
        public ICommand AddFaseDetectorCommand
        {
            get
            {
                if (_AddFaseDetectorCommand == null)
                {
                    _AddFaseDetectorCommand = new RelayCommand(new Action<object>(AddFaseDetectorCommand_Executed), new Predicate<object>(AddFaseDetectorCommand_CanExecute));
                }
                return _AddFaseDetectorCommand;
            }
        }


        RelayCommand _RemoveFaseDetectorCommand;
        public ICommand RemoveFaseDetectorCommand
        {
            get
            {
                if (_RemoveFaseDetectorCommand == null)
                {
                    _RemoveFaseDetectorCommand = new RelayCommand(new Action<object>(RemoveFaseDetectorCommand_Executed), new Predicate<object>(RemoveFaseDetectorCommand_CanExecute));
                }
                return _RemoveFaseDetectorCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private void AddFaseCommand_Executed(object prm)
        {
            FaseCyclusModel fc = new FaseCyclusModel();
            fc.Naam = "fase" + (Fasen.Count + 1);
            Fasen.Add(fc);
        }

        private bool AddFaseCommand_CanExecute(object prm)
        {
            return string.IsNullOrEmpty(Replace);
        }

        void RemoveFaseCommand_Executed(object prm)
        {
            Fasen.Remove(SelectedFaseCyclus);
            SelectedFaseCyclus = null;
        }

        bool RemoveFaseCommand_CanExecute(object prm)
        {
            return SelectedFaseCyclus != null && Fasen.Count > 1;
        }

        private void AddFaseDetectorCommand_Executed(object prm)
        {
            DetectorModel d = new DetectorModel();
            d.Naam = "fase_" + (FaseDetectoren.Count + 1);
            FaseDetectoren.Add(d);
        }

        bool AddFaseDetectorCommand_CanExecute(object prm)
        {
            return SelectedFaseCyclus != null;
        }

        void RemoveFaseDetectorCommand_Executed(object prm)
        {
            FaseDetectoren.Remove(SelectedFaseCyclusDetector);
            SelectedFaseCyclusDetector = null;
        }

        bool RemoveFaseDetectorCommand_CanExecute(object prm)
        {
            return SelectedFaseCyclus != null && SelectedFaseCyclusDetector != null;
        }

        #endregion // Command Functionality

        #region IViewModelWithItem

        public object GetItem()
        {
            return _Template;
        }

        #endregion // IViewModelWithItem

        #region Collection Changed

        private void Fasen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach(FaseCyclusModel fc in e.NewItems)
                {
                    _Template.Items.Add(fc);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (FaseCyclusModel fc in e.OldItems)
                {
                    _Template.Items.Remove(fc);
                }
            }
            RaisePropertyChanged("IsReplaceAvailable");
        }

        private void FaseDetectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (DetectorModel d in e.NewItems)
                {
                    SelectedFaseCyclus.Detectoren.Add(d);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (DetectorModel d in e.OldItems)
                {
                    SelectedFaseCyclus.Detectoren.Remove(d);
                }
            }
        }

        #endregion // Collection Changed

        #region Constructor

        public FaseCyclusTemplateViewModel(TLCGenTemplateModel<FaseCyclusModel> template)
        {
            _Template = template;
            foreach(var fc in template.Items)
            {
                Fasen.Add(fc);
            }
            Fasen.CollectionChanged += Fasen_CollectionChanged;

            FaseCyclusTypeOpties.Clear();
            FaseCyclusTypeOpties.Add(FaseTypeEnum.Auto.GetDescription());
            FaseCyclusTypeOpties.Add(FaseTypeEnum.Fiets.GetDescription());
            FaseCyclusTypeOpties.Add(FaseTypeEnum.Voetganger.GetDescription());
            FaseCyclusTypeOpties.Add(FaseTypeEnum.OV.GetDescription());

            DetectorTypeOpties.Clear();
            DetectorTypeOpties.Add(DetectorTypeEnum.Kop.GetDescription());
            DetectorTypeOpties.Add(DetectorTypeEnum.Lang.GetDescription());
            DetectorTypeOpties.Add(DetectorTypeEnum.Verweg.GetDescription());
            DetectorTypeOpties.Add(DetectorTypeEnum.File.GetDescription());
            DetectorTypeOpties.Add(DetectorTypeEnum.KnopBinnen.GetDescription());
            DetectorTypeOpties.Add(DetectorTypeEnum.KnopBuiten.GetDescription());
            DetectorTypeOpties.Add(DetectorTypeEnum.Radar.GetDescription());
        }

        #endregion // Constructor
    }
}
