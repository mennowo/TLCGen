using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.Settings
{
    public class PeriodeTemplateViewModel : ObservableObject, IViewModelWithItem
    {
        #region Fields

        private TLCGenTemplateModel<PeriodeModel> _Template;

        private RelayCommand _AddPeriodeCommand;
        private RelayCommand _RemovePeriodeCommand;

        #endregion // Fields

        #region Properties

        public bool UseFolderForTemplates => SettingsProvider.Default.Settings.UseFolderForTemplates;

        public bool Editable
        {
            get
            {
                foreach (var t in TemplatesProvider.Default.LoadedTemplates)
                {
                    foreach (var tp in t.Templates.PeriodenTemplates)
                    {
                        if (ReferenceEquals(tp, _Template))
                        {
                            return t.Editable;
                        }
                    }
                }
                return false;
            }
        }

        private List<string> _Locations;
        public List<string> Locations
        {
            get
            {
                if (_Locations == null)
                {
                    _Locations = new List<string>();
                    foreach (var t in TemplatesProvider.Default.LoadedTemplates)
                    {
                        _Locations.Add(t.Location);
                    }
                }
                return _Locations;
            }
        }

        public string Location
        {
            get
            {
                foreach (var t in TemplatesProvider.Default.LoadedTemplates)
                {
                    foreach (var tp in t.Templates.PeriodenTemplates)
                    {
                        if (ReferenceEquals(tp, _Template))
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

                var rem = new TLCGenTemplatesModelWithLocation();
                foreach (var t in TemplatesProvider.Default.LoadedTemplates)
                {
                    if (t.Location != value && t.Templates.PeriodenTemplates.Contains(_Template))
                    {
                        rem = t;
                    }
                    if (t.Location == value && !t.Templates.PeriodenTemplates.Contains(_Template))
                    {
                        t.Templates.PeriodenTemplates.Add(_Template);
                    }
                }
                if (rem != null) rem.Templates.PeriodenTemplates.Remove(_Template);
            }
        }

        public string Naam
        {
            get => _Template.Naam;
            set
            {
                _Template.Naam = value;
                OnPropertyChanged("Naam");
            }
        }
        
        private PeriodeModel _SelectedPeriode;
        public PeriodeModel SelectedPeriode
        {
            get => _SelectedPeriode;
            set
            {
                _SelectedPeriode = value;
                OnPropertyChanged("");
            }
        }


        private ObservableCollection<PeriodeModel> _Perioden;
        public ObservableCollection<PeriodeModel> Perioden
        {
            get
            {
                if (_Perioden == null)
                {
                    _Perioden = new ObservableCollection<PeriodeModel>();
                }
                return _Perioden;
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

        #endregion // Properties

        #region Commands

        public ICommand AddPeriodeCommand => _AddPeriodeCommand ??= new RelayCommand(AddPeriodeCommand_Executed, AddPeriodeCommand_CanExecute);

        public ICommand RemovePeriodeCommand => _RemovePeriodeCommand ??= new RelayCommand(RemovePeriodeCommand_Executed, RemovePeriodeCommand_CanExecute);

        #endregion // Commands

        #region Command Functionality
        
        private void AddPeriodeCommand_Executed()
        {
            var d = new PeriodeModel();
            d.Naam = "per" + (Perioden.Count + 1);
            Perioden.Add(d);
        }

        bool AddPeriodeCommand_CanExecute()
        {
            return true;
        }

        void RemovePeriodeCommand_Executed()
        {
            Perioden.Remove(SelectedPeriode);
            SelectedPeriode = null;
        }

        bool RemovePeriodeCommand_CanExecute()
        {
            return SelectedPeriode != null && Perioden.Count > 0;
        }

        #endregion // Command Functionality

        #region IViewModelWithItem

        public object GetItem()
        {
            return _Template;
        }

        #endregion // IViewModelWithItem

        #region Collection Changed
        
        private void Perioden_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (PeriodeModel per in e.NewItems)
                {
                    _Template.Items.Add(per);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (PeriodeModel per in e.OldItems)
                {
                    _Template.Items.Remove(per);
                }
            }
        }

        #endregion // Collection Changed

        #region Constructor

        public PeriodeTemplateViewModel(TLCGenTemplateModel<PeriodeModel> template)
        {
            _Template = template;
            foreach (var fc in template.Items)
            {
                Perioden.Add(fc);
            }
            Perioden.CollectionChanged += Perioden_CollectionChanged;
        }

        #endregion // Constructor
    }
}
