using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Settings
{
    public class FaseCyclusTemplateViewModel : ObservableObject, IViewModelWithItem
    {
        #region Fields

        private readonly TLCGenTemplateModel<FaseCyclusModel> _template;
        private FaseCyclusModel _selectedFaseCyclus;
        private List<string> _locations;
        private DetectorModel _selectedFaseCyclusDetector;
        private ObservableCollection<FaseCyclusModel> _fasen;
        private ObservableCollection<DetectorModel> _faseDetectoren;
        private List<string> _faseCyclusTypeOpties;
        private List<string> _detectorTypeOpties;
        private string _selectedFaseCyclusTypeString;
        private string _selectedDetectorTypeString;
        private RelayCommand _addFaseCommand;
        private RelayCommand _removeFaseCommand;
        private RelayCommand _addFaseDetectorCommand;
        private RelayCommand _applyDefaultsToFaseCommand;
        private RelayCommand _applyDefaultsToFaseDetectorCommand;
        private RelayCommand _removeFaseDetectorCommand;
        
        #endregion // Fields

        #region Properties

        public bool UseFolderForTemplates => SettingsProvider.Default.Settings.UseFolderForTemplates;

        public bool Editable
        {
            get
            {
                foreach (var t in TemplatesProvider.Default.LoadedTemplates)
                {
                    foreach (var tfc in t.Templates.FasenTemplates)
                    {
                        if (ReferenceEquals(tfc, _template))
                        {
                            return t.Editable;
                        }
                    }
                }
                return false;
            }
        }

        public List<string> Locations
        {
            get
            {
                if (_locations == null)
                {
                    _locations = new List<string>();
                    foreach(var t in TemplatesProvider.Default.LoadedTemplates)
                    {
                        _locations.Add(t.Location);
                    }
                }
                return _locations;
            }
        }

        public string Location
        {
            get
            {
                foreach(var t in TemplatesProvider.Default.LoadedTemplates)
                {
                    foreach(var tfc in t.Templates.FasenTemplates)
                    {
                        if(ReferenceEquals(tfc, _template))
                        {
                            return t.Location;
                        }
                    }
                }
                return null;
            }
            set
            {
                if (value == null) return;

                TLCGenTemplatesModelWithLocation rem = null;
                foreach (var t in TemplatesProvider.Default.LoadedTemplates)
                {
                    if (t.Location != value && t.Templates.FasenTemplates.Contains(_template))
                    {
                        rem = t;
                    }
                    if (t.Location == value && !t.Templates.FasenTemplates.Contains(_template))
                    {
                        t.Templates.FasenTemplates.Add(_template);
                    }
                }
                if (rem != null) rem.Templates.FasenTemplates.Remove(_template);
            }
        }

        public string Naam
        {
            get => _template.Naam;
            set
            {
                _template.Naam = value;
                OnPropertyChanged();
            }
        }

        public string Replace
        {
            get => _template.Replace;
            set
            {
                _template.Replace = value;
                OnPropertyChanged();
                _addFaseCommand?.NotifyCanExecuteChanged();
            }
        }

        public bool IsReplaceAvailable => Fasen.Count == 1;

        public FaseCyclusModel SelectedFaseCyclus
        {
            get => _selectedFaseCyclus;
            set
            {
                _selectedFaseCyclus = value;
                FaseDetectoren.CollectionChanged -= FaseDetectoren_CollectionChanged;
                FaseDetectoren.Clear();
                if (value != null)
                {
                    foreach (var d in value.Detectoren)
                    {
                        FaseDetectoren.Add(d);
                    }
                }
                FaseDetectoren.CollectionChanged += FaseDetectoren_CollectionChanged;

                if (value != null)
                    SelectedFaseCyclusTypeString = value.Type.GetDescription();
                else
                    SelectedFaseCyclusTypeString = null;

                OnPropertyChanged("");
                _removeFaseCommand?.NotifyCanExecuteChanged();
                _addFaseDetectorCommand?.NotifyCanExecuteChanged();
                _removeFaseDetectorCommand?.NotifyCanExecuteChanged();
                _applyDefaultsToFaseCommand?.NotifyCanExecuteChanged();
                _applyDefaultsToFaseDetectorCommand?.NotifyCanExecuteChanged();
            }
        }
        
        public string SelectedFaseCyclusDetectorNaam
        {
            get => _selectedFaseCyclusDetector?.Naam;
            set
            {
                _selectedFaseCyclusDetector.Naam = value;

                // Hack to update name in listview without writing new VMs
                var sd = SelectedFaseCyclusDetector;
                FaseDetectoren.CollectionChanged -= FaseDetectoren_CollectionChanged;
                FaseDetectoren.Clear();
                if (_selectedFaseCyclus != null)
                {
                    foreach (var d in _selectedFaseCyclus.Detectoren)
                    {
                        FaseDetectoren.Add(d);
                    }
                }
                FaseDetectoren.CollectionChanged += FaseDetectoren_CollectionChanged;
                SelectedFaseCyclusDetector = sd;
                // End of trick

                OnPropertyChanged();
            }
        }

        public string SelectedFaseCyclusDetectorVissimNaam
        {
            get => _selectedFaseCyclusDetector?.VissimNaam;
            set
            {
                _selectedFaseCyclusDetector.VissimNaam = value;
                OnPropertyChanged();
            }
        }

        public DetectorModel SelectedFaseCyclusDetector
        {
            get => _selectedFaseCyclusDetector;
            set
            {
                _selectedFaseCyclusDetector = value;
                if (value != null)
                    SelectedDetectorTypeString = value.Type.GetDescription();
                else
                    SelectedDetectorTypeString = null;

                OnPropertyChanged(nameof(SelectedFaseCyclusDetectorNaam));
                OnPropertyChanged(nameof(SelectedFaseCyclusDetectorVissimNaam));
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedDetectorTypeString));
                _removeFaseDetectorCommand?.NotifyCanExecuteChanged();
                _applyDefaultsToFaseDetectorCommand?.NotifyCanExecuteChanged();
            }
        }

        public ObservableCollection<FaseCyclusModel> Fasen
        {
            get
            {
                if (_fasen == null)
                {
                    _fasen = new ObservableCollection<FaseCyclusModel>();
                }
                return _fasen;
            }
        }

        public ObservableCollection<DetectorModel> FaseDetectoren
        {
            get
            {
                if (_faseDetectoren == null)
                {
                    _faseDetectoren = new ObservableCollection<DetectorModel>();
                }
                return _faseDetectoren;
            }
        }

        public List<string> FaseCyclusTypeOpties
        {
            get
            {
                if (_faseCyclusTypeOpties == null)
                {
                    _faseCyclusTypeOpties = new List<string>();
                }
                return _faseCyclusTypeOpties;
            }
        }

        public List<string> DetectorTypeOpties
        {
            get
            {
                if (_detectorTypeOpties == null)
                {
                    _detectorTypeOpties = new List<string>();
                }
                return _detectorTypeOpties;
            }
        }

        public string SelectedFaseCyclusTypeString
        {
            get => _selectedFaseCyclusTypeString;
            set
            {
                _selectedFaseCyclusTypeString = value;
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
                    else
                    {
                        throw new ArgumentOutOfRangeException("Unknown fase type: " + value);
                    }
                }
                OnPropertyChanged();
            }
        }

        public string SelectedDetectorTypeString
        {
            get => _selectedDetectorTypeString;
            set
            {
                _selectedDetectorTypeString = value;
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
                    else if (value == DetectorTypeEnum.Knop.GetDescription())
                    {
                        SelectedFaseCyclusDetector.Type = DetectorTypeEnum.Knop;
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
                    else if (value == DetectorTypeEnum.OpticomIngang.GetDescription())
                    {
                        SelectedFaseCyclusDetector.Type = DetectorTypeEnum.OpticomIngang;
                    }
                    else if (value == DetectorTypeEnum.VecomDetector.GetDescription())
                    {
                        SelectedFaseCyclusDetector.Type = DetectorTypeEnum.VecomDetector;
                    }
                    else if (value == DetectorTypeEnum.WisselDetector.GetDescription())
                    {
                        SelectedFaseCyclusDetector.Type = DetectorTypeEnum.WisselDetector;
                    }
                    else if (value == DetectorTypeEnum.Overig.GetDescription())
                    {
                        SelectedFaseCyclusDetector.Type = DetectorTypeEnum.Overig;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Unknown detector type: " + value);
                    }
                }
                OnPropertyChanged();
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddFaseCommand => _addFaseCommand ??= new RelayCommand(() =>
            {
                var fc = new FaseCyclusModel();
                fc.Naam = "fase" + (Fasen.Count + 1);
                Fasen.Add(fc);
            }, 
            () => string.IsNullOrEmpty(Replace));

        public ICommand RemoveFaseCommand => _removeFaseCommand ??= new RelayCommand(() =>
            {
                Fasen.Remove(SelectedFaseCyclus);
                SelectedFaseCyclus = null;
            }, 
            () => SelectedFaseCyclus != null && Fasen.Count > 1);

        public ICommand AddFaseDetectorCommand => _addFaseDetectorCommand ??= new RelayCommand(() =>
            {
                var d = new DetectorModel
                {
                    FaseCyclus = SelectedFaseCyclus.Naam, Naam = "fase_" + (FaseDetectoren.Count + 1), Rijstrook = 1
                };
                FaseDetectoren.Add(d);
            }, 
            () => SelectedFaseCyclus != null);

        public ICommand RemoveFaseDetectorCommand => _removeFaseDetectorCommand ??= new RelayCommand(() =>
            {
                FaseDetectoren.Remove(SelectedFaseCyclusDetector);
                SelectedFaseCyclusDetector = null;
            }, 
            () => SelectedFaseCyclus != null && SelectedFaseCyclusDetector != null);

        public ICommand ApplyDefaultsToFaseCommand => _applyDefaultsToFaseCommand ??= new RelayCommand(() =>
            {
                var fc = SelectedFaseCyclus;
                SelectedFaseCyclus = null;
                DefaultsProvider.Default.SetDefaultsOnModel(fc, fc.Type.ToString());
                SelectedFaseCyclus = fc;
            }, 
            () => SelectedFaseCyclus != null);

        public ICommand ApplyDefaultsToFaseDetectorCommand => _applyDefaultsToFaseDetectorCommand ??= new RelayCommand(() =>
            {
                var d = SelectedFaseCyclusDetector;
                SelectedFaseCyclusDetector = null;
                DefaultsProvider.Default.SetDefaultsOnModel(d, d.Type.ToString(), SelectedFaseCyclus.Type.ToString());
                SelectedFaseCyclusDetector = d;
            },
            () => SelectedFaseCyclus != null && SelectedFaseCyclusDetector != null);

        #endregion // Commands

        #region IViewModelWithItem

        public object GetItem()
        {
            return _template;
        }

        #endregion // IViewModelWithItem

        #region Collection Changed

        private void Fasen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach(FaseCyclusModel fc in e.NewItems)
                {
                    _template.Items.Add(fc);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (FaseCyclusModel fc in e.OldItems)
                {
                    _template.Items.Remove(fc);
                }
            }
            OnPropertyChanged(nameof(IsReplaceAvailable));
            _removeFaseCommand?.NotifyCanExecuteChanged();
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
            _template = template;
            foreach(var fc in template.Items)
            {
                Fasen.Add(fc);
            }
            Fasen.CollectionChanged += Fasen_CollectionChanged;
            if(Fasen.Any()) SelectedFaseCyclus = Fasen.First();

            FaseCyclusTypeOpties.Clear();
            var fdescs = Enum.GetValues(typeof(FaseTypeEnum));
            foreach (FaseTypeEnum d in fdescs)
            {
                FaseCyclusTypeOpties.Add(d.GetDescription());
            }

            DetectorTypeOpties.Clear();
            var descs = Enum.GetValues(typeof(DetectorTypeEnum));
            foreach (DetectorTypeEnum d in descs)
            {
                DetectorTypeOpties.Add(d.GetDescription());
            }
        }

        #endregion // Constructor
    }
}
