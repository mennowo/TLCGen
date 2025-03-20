using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using TLCGen.Dialogs;
using TLCGen.Helpers;

namespace TLCGen.Settings
{
    public class TLCGenSettingsViewModel : ObservableObject
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public bool UseFileForTemplates => !UseFolderForTemplates;

        public bool UseFolderForTemplates
        {
            get => SettingsProvider.Default.Settings.UseFolderForTemplates;
            set
            {
                SettingsProvider.Default.Settings.UseFolderForTemplates = value;
                SettingsProvider.Default.Settings.TemplatesLocation = null;
                TemplatesProvider.Default.LoadSettings();
                _FasenTemplatesEditorTabVM = null;
                _DetectorenTemplatesEditorTabVM = null;
                _PeriodenTemplatesEditorTabVM = null;
                OnPropertyChanged("");
            }
        }

        public string TemplatesLocation
        {
            get => SettingsProvider.Default.Settings.TemplatesLocation;
            set
            {
                if(!UseFolderForTemplates)
                {
                    SettingsProvider.Default.Settings.TemplatesLocation = (!string.IsNullOrWhiteSpace(value) ? (value.ToLower().EndsWith(".xml") ? value : value + ".xml") : "");
                }
                else
                {
                    SettingsProvider.Default.Settings.TemplatesLocation = (!string.IsNullOrWhiteSpace(value) ? value : "");
                }
                if (!string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.TemplatesLocation) &&
                    !UseFolderForTemplates && !File.Exists(SettingsProvider.Default.Settings.TemplatesLocation))
                {
                    TLCGenSerialization.Serialize(SettingsProvider.Default.Settings.TemplatesLocation, new TLCGenTemplatesModel());
                }
                TemplatesProvider.Default.LoadSettings();
                _FasenTemplatesEditorTabVM = null;
                _DetectorenTemplatesEditorTabVM = null;
                _PeriodenTemplatesEditorTabVM = null;
                _CreateTemplateFileCommand?.NotifyCanExecuteChanged();
                OnPropertyChanged("");
            }
        }

        public string DefaultsFileLocation
        {
            get => SettingsProvider.Default.Settings.DefaultsFileLocation;
            set
            {
                SettingsProvider.Default.Settings.DefaultsFileLocation = (!string.IsNullOrWhiteSpace(value) ? (value.ToLower().EndsWith(".xml") ? value : value + ".xml") : "");
                if (!string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.DefaultsFileLocation) && !File.Exists(SettingsProvider.Default.Settings.DefaultsFileLocation))
                {
                    TLCGenSerialization.Serialize(SettingsProvider.Default.Settings.DefaultsFileLocation, new TLCGenDefaultsModel());
                }
                DefaultsProvider.Default.LoadSettings();
				_DefaultsTabVM = null;
                OnPropertyChanged("");
            }
        }

        private DefaultsTabViewModel _DefaultsTabVM;
        public DefaultsTabViewModel DefaultsTabVM
        {
            get
            {
                if (_DefaultsTabVM == null)
                {
                    _DefaultsTabVM = new DefaultsTabViewModel();
                }
                return _DefaultsTabVM;
            }
        }

        private FasenTemplatesEditorTabViewModel _FasenTemplatesEditorTabVM;
        public FasenTemplatesEditorTabViewModel FasenTemplatesEditorTabVM
        {
            get
            {
                if (_FasenTemplatesEditorTabVM == null)
                {
                    _FasenTemplatesEditorTabVM = new FasenTemplatesEditorTabViewModel();
                }
                return _FasenTemplatesEditorTabVM;
            }
        }

        private DetectorenTemplatesEditorTabViewModel _DetectorenTemplatesEditorTabVM;
        public DetectorenTemplatesEditorTabViewModel DetectorenTemplatesEditorTabVM
        {
            get
            {
                if (_DetectorenTemplatesEditorTabVM == null)
                {
                    _DetectorenTemplatesEditorTabVM = new DetectorenTemplatesEditorTabViewModel();
                }
                return _DetectorenTemplatesEditorTabVM;
            }
        }

        private PeriodenTemplatesEditorTabViewModel _PeriodenTemplatesEditorTabVM;
        public PeriodenTemplatesEditorTabViewModel PeriodenTemplatesEditorTabVM
        {
            get
            {
                if (_PeriodenTemplatesEditorTabVM == null)
                {
                    _PeriodenTemplatesEditorTabVM = new PeriodenTemplatesEditorTabViewModel();
                }
                return _PeriodenTemplatesEditorTabVM;
            }
        }

        private PrioIngrepenTemplatesEditorTabViewModel _PrioIngrepenTemplatesEditorTabVM;
        public PrioIngrepenTemplatesEditorTabViewModel PrioIngrepenTemplatesEditorTabVM
        {
            get
            {
                if (_PrioIngrepenTemplatesEditorTabVM == null)
                {
                    _PrioIngrepenTemplatesEditorTabVM = new PrioIngrepenTemplatesEditorTabViewModel();
                }
                return _PrioIngrepenTemplatesEditorTabVM;
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _CreateTemplateFileCommand;
        public ICommand CreateTemplateFileCommand
        {
            get
            {
                if (_CreateTemplateFileCommand == null)
                {
                    _CreateTemplateFileCommand = new RelayCommand(CreateTemplateFileCommand_Executed, CreateTemplateFileCommand_CanExecute);
                }
                return _CreateTemplateFileCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private bool CreateTemplateFileCommand_CanExecute()
        {
            return !string.IsNullOrEmpty(TemplatesLocation) && Directory.Exists(TemplatesLocation);
        }

        private void CreateTemplateFileCommand_Executed()
        {
            if (Directory.Exists(TemplatesLocation))
            {
                var dlg = new AddNewTemplateFileWindow();
                if(dlg.ShowDialog() == true)
                {
                    if (TemplatesProvider.Default.LoadedTemplates.Any(x => x.Location == dlg.Name))
                    {
                        MessageBox.Show($"Een template file genaamd \"{dlg.Name}\" bestaat al.", "Naam reeds in gebruik");
                    }
                    else
                    {
                        var t = new TLCGenTemplatesModel();
                        var twl = new TLCGenTemplatesModelWithLocation
                        {
                            Location = dlg.Name,
                            Editable = true,
                            Templates = t
                        };
                        TemplatesProvider.Default.LoadedTemplates.Add(twl);
                    }
                }
            }
        }

        #endregion // Command Functionality

        #region Private Methods

        #endregion // Private Methods

        #region Constructor

        public TLCGenSettingsViewModel()
        {
        }

        #endregion // Constructor
    }
}
