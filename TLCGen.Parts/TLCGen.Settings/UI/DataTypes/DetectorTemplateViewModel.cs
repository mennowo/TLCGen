using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Settings
{
    public class DetectorTemplateViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private TLCGenTemplateModel<DetectorModel> _template;

        #endregion // Fields

        #region Properties

        public bool UseFolderForTemplates => SettingsProvider.Default.Settings.UseFolderForTemplates;

        public bool Editable
        {
            get
            {
                foreach (var t in TemplatesProvider.Default.LoadedTemplates)
                {
                    foreach (var td in t.Templates.DetectorenTemplates)
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

        private List<string> _locations;
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
                    foreach (var td in t.Templates.DetectorenTemplates)
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
                    if (t.Location != value && t.Templates.DetectorenTemplates.Contains(_template))
                    {
                        rem = t;
                    }
                    if (t.Location == value && !t.Templates.DetectorenTemplates.Contains(_template))
                    {
                        t.Templates.DetectorenTemplates.Add(_template);
                    }
                }
                if (rem != null) rem.Templates.DetectorenTemplates.Remove(_template);
            }
        }

        public string Naam
        {
            get => _template.Naam;
            set
            {
                _template.Naam = value;
                RaisePropertyChanged();
            }
        }

        public string Replace
        {
            get => _template.Replace;
            set
            {
                _template.Replace = value;
                RaisePropertyChanged();
            }
        }

        private DetectorModel _selectedDetector;
        public DetectorModel SelectedDetector
        {
            get => _selectedDetector;
            set
            {
                _selectedDetector = value;
                if (value != null)
                    SelectedDetectorTypeString = value.Type.GetDescription();
                else
                    SelectedDetectorTypeString = null;

                RaisePropertyChanged("");
            }
        }


        private ObservableCollection<DetectorModel> _detectoren;
        public ObservableCollection<DetectorModel> Detectoren
        {
            get
            {
                if (_detectoren == null)
                {
                    _detectoren = new ObservableCollection<DetectorModel>();
                }
                return _detectoren;
            }
        }

        private List<string> _detectorTypeOpties;
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

        private string _selectedDetectorTypeString;
        public string SelectedDetectorTypeString
        {
            get => _selectedDetectorTypeString;
            set
            {
                _selectedDetectorTypeString = value;
                if (SelectedDetector != null)
                {
                    if (value == DetectorTypeEnum.Kop.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.Kop;
                    }
                    else if (value == DetectorTypeEnum.Lang.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.Lang;
                    }
                    else if (value == DetectorTypeEnum.Verweg.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.Verweg;
                    }
                    else if (value == DetectorTypeEnum.File.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.File;
                    }
                    else if (value == DetectorTypeEnum.Knop.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.Knop;
                    }
                    else if (value == DetectorTypeEnum.KnopBinnen.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.KnopBinnen;
                    }
                    else if (value == DetectorTypeEnum.KnopBuiten.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.KnopBuiten;
                    }
                    else if (value == DetectorTypeEnum.Radar.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.Radar;
                    }
                    else if (value == DetectorTypeEnum.OpticomIngang.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.OpticomIngang;
                    }
                    else if (value == DetectorTypeEnum.VecomDetector.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.VecomDetector;
                    }
                    else if (value == DetectorTypeEnum.WisselDetector.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.WisselDetector;
                    }
                    else if (value == DetectorTypeEnum.Overig.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.Overig;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Unknown detector type: " + value);
                    }
                }
                RaisePropertyChanged();
            }
        }

        #endregion // Properties

        #region Commands


        RelayCommand _addDetectorCommand;
        public ICommand AddDetectorCommand
        {
            get
            {
                if (_addDetectorCommand == null)
                {
                    _addDetectorCommand = new RelayCommand(AddDetectorCommand_Executed, AddDetectorCommand_CanExecute);
                }
                return _addDetectorCommand;
            }
        }


        RelayCommand _removeDetectorCommand;
        public ICommand RemoveDetectorCommand
        {
            get
            {
                if (_removeDetectorCommand == null)
                {
                    _removeDetectorCommand = new RelayCommand(RemoveDetectorCommand_Executed, RemoveDetectorCommand_CanExecute);
                }
                return _removeDetectorCommand;
            }
        }

        RelayCommand _applyDefaultsCommand;
        public ICommand ApplyDefaultsCommand
        {
            get
            {
                if (_applyDefaultsCommand == null)
                {
                    _applyDefaultsCommand = new RelayCommand(ApplyDefaultsCommand_Executed, ApplyDefaultsCommand_CanExecute);
                }
                return _applyDefaultsCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private void AddDetectorCommand_Executed(object prm)
        {
            var d = new DetectorModel
            {
                FaseCyclus = Replace, Naam = Replace + "_" + (Detectoren.Count + 1), Rijstrook = 1
            };
            Detectoren.Add(d);
        }

        bool AddDetectorCommand_CanExecute(object prm)
        {
            return true;
        }

        void RemoveDetectorCommand_Executed(object prm)
        {
            Detectoren.Remove(SelectedDetector);
            SelectedDetector = null;
        }

        bool RemoveDetectorCommand_CanExecute(object prm)
        {
            return SelectedDetector != null && Detectoren.Count > 0;
        }

        void ApplyDefaultsCommand_Executed(object prm)
        {
            var d = SelectedDetector;
            SelectedDetector = null;
            DefaultsProvider.Default.SetDefaultsOnModel(d, d.Type.ToString());
            SelectedDetector = d;
        }

        bool ApplyDefaultsCommand_CanExecute(object prm)
        {
            return SelectedDetector != null && Detectoren.Count > 0;
        }


        #endregion // Command Functionality

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
                foreach (DetectorModel d in e.NewItems)
                {
                    _template.Items.Add(d);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (DetectorModel d in e.OldItems)
                {
                    _template.Items.Remove(d);
                }
            }
        }

        #endregion // Collection Changed

        #region Constructor

        public DetectorTemplateViewModel(TLCGenTemplateModel<DetectorModel> template)
        {
            _template = template;
            foreach (var fc in template.Items)
            {
                Detectoren.Add(fc);
            }
            Detectoren.CollectionChanged += Detectoren_CollectionChanged;
            if (Detectoren.Any()) SelectedDetector = Detectoren.First();

            DetectorTypeOpties.Clear();
            var descs = Enum.GetValues(typeof(DetectorTypeEnum));
            foreach(DetectorTypeEnum d in descs)
            {
                DetectorTypeOpties.Add(d.GetDescription());
            }
        }

        #endregion // Constructor
    }
}
