using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Settings
{
    public class PrioIngreepTemplateViewModel : ObservableObject, IViewModelWithItem
    {
        #region Fields

        private readonly TLCGenTemplateModel<PrioIngreepModel> _template;
        private ObservableCollection<PrioIngreepModel> _prioIngrepen;
        private List<string> _locations;
        private PrioIngreepModel _selectedPrioIngreep;
        private RelayCommand _addPrioIngreepCommand;
        private RelayCommand _removePrioIngreepCommand;
        private RelayCommand _applyDefaultsCommand;
        private RelayCommand _addPrioIngreepInmeldingCommand;
        private RelayCommand _removePrioIngreepInmeldingCommand;
        private ObservableCollection<PrioIngreepInUitMeldingModel> _prioIngreepInmeldingen;
        private PrioIngreepInUitMeldingModel _prioIngreepSelectedInmelding;
        private ObservableCollection<PrioIngreepInUitMeldingModel> _prioIngreepUitmeldingen;
        private PrioIngreepInUitMeldingModel _prioIngreepSelectedUitmelding;
        private RelayCommand _addPrioIngreepUitmeldingCommand;
        private RelayCommand _removePrioIngreepUitmeldingCommand;

        #endregion // Fields

        #region Properties

        public bool UseFolderForTemplates => SettingsProvider.Default.Settings.UseFolderForTemplates;

        public bool Editable
        {
            get
            {
                foreach (var t in TemplatesProvider.Default.LoadedTemplates)
                {
                    foreach (var td in t.Templates.PrioIngreepTemplates)
                    {
                        if (ReferenceEquals(td, _template))
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
                    foreach (var t in TemplatesProvider.Default.LoadedTemplates)
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
                foreach (var t in TemplatesProvider.Default.LoadedTemplates)
                {
                    foreach (var td in t.Templates.PrioIngreepTemplates)
                    {
                        if (ReferenceEquals(td, _template))
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
                    if (t.Location != value && t.Templates.PrioIngreepTemplates.Contains(_template))
                    {
                        rem = t;
                    }
                    if (t.Location == value && !t.Templates.PrioIngreepTemplates.Contains(_template))
                    {
                        t.Templates.PrioIngreepTemplates.Add(_template);
                    }
                }
                if (rem != null) rem.Templates.PrioIngreepTemplates.Remove(_template);
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
            }
        }

        public ObservableCollection<PrioIngreepModel> PrioIngrepen
        {
            get
            {
                if (_prioIngrepen == null)
                {
                    _prioIngrepen = new ObservableCollection<PrioIngreepModel>();
                }
                return _prioIngrepen;
            }
        }

        public PrioIngreepModel SelectedPrioIngreep
        {
            get => _selectedPrioIngreep;
            set
            {
                _selectedPrioIngreep = value;
                PrioIngreepInmeldingen.CollectionChanged -= PrioIngreepInmeldingenOnCollectionChanged;
                PrioIngreepUitmeldingen.CollectionChanged -= PrioIngreepUitmeldingenOnCollectionChanged;
                PrioIngreepInmeldingen.Clear();
                PrioIngreepUitmeldingen.Clear();
                foreach (var m in _selectedPrioIngreep.MeldingenData.Inmeldingen) PrioIngreepInmeldingen.Add(m);
                foreach (var m in _selectedPrioIngreep.MeldingenData.Uitmeldingen) PrioIngreepUitmeldingen.Add(m);
                PrioIngreepInmeldingen.CollectionChanged += PrioIngreepInmeldingenOnCollectionChanged;
                PrioIngreepUitmeldingen.CollectionChanged += PrioIngreepUitmeldingenOnCollectionChanged;
                PrioIngreepSelectedInmelding = PrioIngreepInmeldingen.FirstOrDefault();
                PrioIngreepSelectedUitmelding = PrioIngreepUitmeldingen.FirstOrDefault();
                OnPropertyChanged();
            }
        }

        private void PrioIngreepInmeldingenOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (PrioIngreepInUitMeldingModel m in e.NewItems)
                {
                    _selectedPrioIngreep.MeldingenData.Inmeldingen.Add(m);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (PrioIngreepInUitMeldingModel m in e.OldItems)
                {
                    _selectedPrioIngreep.MeldingenData.Inmeldingen.Remove(m);
                }
            }
        }

        private void PrioIngreepUitmeldingenOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (PrioIngreepInUitMeldingModel m in e.NewItems)
                {
                    _selectedPrioIngreep.MeldingenData.Uitmeldingen.Add(m);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (PrioIngreepInUitMeldingModel m in e.OldItems)
                {
                    _selectedPrioIngreep.MeldingenData.Uitmeldingen.Remove(m);
                }
            }
        }

        public ObservableCollection<PrioIngreepInUitMeldingModel> PrioIngreepInmeldingen
        {
            get
            {
                if (_prioIngreepInmeldingen == null)
                {
                    _prioIngreepInmeldingen = new ObservableCollection<PrioIngreepInUitMeldingModel>();
                }
                return _prioIngreepInmeldingen;
            }
        }

        public PrioIngreepInUitMeldingModel PrioIngreepSelectedInmelding
        {
            get => _prioIngreepSelectedInmelding;
            set
            {
                _prioIngreepSelectedInmelding = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PrioIngreepInUitMeldingModel> PrioIngreepUitmeldingen
        {
            get
            {
                if (_prioIngreepUitmeldingen == null)
                {
                    _prioIngreepUitmeldingen = new ObservableCollection<PrioIngreepInUitMeldingModel>();
                }
                return _prioIngreepUitmeldingen;
            }
        }

        public PrioIngreepInUitMeldingModel PrioIngreepSelectedUitmelding
        {
            get => _prioIngreepSelectedUitmelding;
            set
            {
                _prioIngreepSelectedUitmelding = value;
                OnPropertyChanged();
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddPrioIngreepCommand =>
            _addPrioIngreepCommand ?? (_addPrioIngreepCommand = new RelayCommand(() =>
            {
                var d = new PrioIngreepModel()
                {
                    FaseCyclus = Replace, Naam = Replace + "_" + (PrioIngrepen.Count + 1)
                };
                PrioIngrepen.Add(d);
            }));

        public ICommand RemovePrioIngreepCommand =>
            _removePrioIngreepCommand ?? (_removePrioIngreepCommand = new RelayCommand(() =>
            {
                PrioIngrepen.Remove(SelectedPrioIngreep);
                SelectedPrioIngreep = null;
            }, () => SelectedPrioIngreep != null && PrioIngrepen.Count > 0));
        
        public ICommand ApplyDefaultsCommand =>
            _applyDefaultsCommand ?? (_applyDefaultsCommand = new RelayCommand(() =>
            {
                var d = SelectedPrioIngreep;
                SelectedPrioIngreep = null;
                DefaultsProvider.Default.SetDefaultsOnModel(d, d.Type.ToString());
                SelectedPrioIngreep = d;
            }, () => SelectedPrioIngreep != null && PrioIngrepen.Count > 0));
        
        public ICommand AddPrioIngreepInmeldingCommand =>
            _addPrioIngreepInmeldingCommand ?? (_addPrioIngreepInmeldingCommand = new RelayCommand(() =>
            {
                var d = new PrioIngreepInUitMeldingModel{Naam = "inmelding", InUit = PrioIngreepInUitMeldingTypeEnum.Inmelding};
                _prioIngreepInmeldingen.Add(d);
            }, () => SelectedPrioIngreep != null && PrioIngreepInmeldingen != null));

        public ICommand RemovePrioIngreepInmeldingCommand =>
            _removePrioIngreepInmeldingCommand ?? (_removePrioIngreepInmeldingCommand = new RelayCommand(() =>
            {
                
            }, () => SelectedPrioIngreep != null && PrioIngreepSelectedInmelding != null && PrioIngreepInmeldingen.Count > 0));
        
        public ICommand AddPrioIngreepUitmeldingCommand =>
            _addPrioIngreepUitmeldingCommand ?? (_addPrioIngreepUitmeldingCommand = new RelayCommand(() =>
            {
                var d = new PrioIngreepInUitMeldingModel{Naam = "uitmelding", InUit = PrioIngreepInUitMeldingTypeEnum.Uitmelding};
                _prioIngreepUitmeldingen.Add(d);
            }, () => SelectedPrioIngreep != null && PrioIngreepUitmeldingen != null));

        public ICommand RemovePrioIngreepUitmeldingCommand =>
            _removePrioIngreepUitmeldingCommand ?? (_removePrioIngreepUitmeldingCommand = new RelayCommand(() =>
            {
                
            }, () => SelectedPrioIngreep != null && PrioIngreepSelectedUitmelding != null && PrioIngreepUitmeldingen.Count > 0));
        #endregion // Commands

        #region IViewModelWithItem

        public object GetItem()
        {
            return _template;
        }

        #endregion // IViewModelWithItem

        #region Collection Changed
        
        private void Detectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (PrioIngreepModel d in e.NewItems)
                {
                    _template.Items.Add(d);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (PrioIngreepModel d in e.OldItems)
                {
                    _template.Items.Remove(d);
                }
            }
        }

        #endregion // Collection Changed

        #region Constructor

        public PrioIngreepTemplateViewModel(TLCGenTemplateModel<PrioIngreepModel> template)
        {
            _template = template;
            foreach (var fc in template.Items)
            {
                PrioIngrepen.Add(fc);
            }
            PrioIngrepen.CollectionChanged += Detectoren_CollectionChanged;
            if (PrioIngrepen.Any()) SelectedPrioIngreep = PrioIngrepen.First();
        }

        #endregion // Constructor
    }
}
