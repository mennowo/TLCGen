using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using TLCGen.DataAccess;
using TLCGen.Dialogs;
using TLCGen.GuiActions;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Plugins;
using TLCGen.Settings;
using System.Windows.Shell;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Net;
using GalaSoft.MvvmLight.Threading;
using TLCGen.Dependencies.Providers;

namespace TLCGen.ViewModels
{
    public class MainWindowViewModel : GalaSoft.MvvmLight.ViewModelBase, IDropTarget
    {
        #region Fields

        private ControllerViewModel _ControllerVM;
        private TLCGenStatusBarViewModel _StatusBarVM;
        private List<MenuItem> _ImportMenuItems;
        private List<MenuItem> _PluginMenuItems;
        private List<IGeneratorViewModel> _generators;
        private IGeneratorViewModel _SelectedGenerator;
        private bool _showAlertMessage;
        
        RelayCommand _newFileCommand;
        RelayCommand _openFileCommand;
        RelayCommand _saveFileCommand;
        RelayCommand _saveAsFileCommand;
        RelayCommand _closeFileCommand;
        RelayCommand _exitApplicationCommand;
        RelayCommand _showSettingsWindowCommand;
        RelayCommand _showAboutCommand;
        RelayCommand _showVersionInfoCommand;
        RelayCommand _generateControllerCommand;
        RelayCommand _importControllerCommand;
        RelayCommand _hideAlertMessageCommand;
        RelayCommand _hideAllAlertMessagesCommand;

        private readonly List<Tuple<Version, string>> VersionFiles = new List<Tuple<Version, string>>();

        private List<ITLCGenImporter> _importers;
        private List<ITLCGenTabItem> _tabItems;

        #endregion // Fields

        #region Properties

        public List<Tuple<TLCGenPluginElems, ITLCGenPlugin>> ApplicationParts { get; }

        /// <summary>
        /// ViewModel for the Controller data object that is being edited via the application.
        /// This is the main ViewModel for the application, which holds all other ViewModels.
        /// (Except for the Settings ViewModel, which belongs to the application rather than 
        /// the Controller.)
        /// </summary>
        public ControllerViewModel ControllerVM
        {
            get => _ControllerVM;
            set
            {
                _ControllerVM = value;
                foreach (var pl in ApplicationParts)
                {
                    pl.Item2.Controller = TLCGenControllerDataProvider.Default.Controller;
                }

                RaisePropertyChanged();
                RaisePropertyChanged(nameof(HasController));
            }
        }

        /// <summary>
        /// ViewModel for status bar
        /// </summary>
        public TLCGenStatusBarViewModel StatusBarVM
        {
            get
            {
                if (_StatusBarVM == null)
                    _StatusBarVM = new TLCGenStatusBarViewModel();
                return _StatusBarVM;
            }
        }

        /// <summary>
        /// Boolean used by the View to determine of a Controller has been loaded.
        /// This is used in the View to enable/disable appropriate UI elementents.
        /// </summary>
        public bool HasController => TLCGenControllerDataProvider.Default.Controller != null;

        /// <summary>
        /// A string to be used in the View as the title of the program.
        /// File operations should call OnPropertyChanged for this property,
        /// so the View updates the program title.
        /// </summary>
        public string ProgramTitle
        {
            get
            {
                if (HasController && !string.IsNullOrEmpty(TLCGenControllerDataProvider.Default.ControllerFileName))
                {
                    return "TLCGen - " + TLCGenControllerDataProvider.Default.ControllerFileName;
                }
                return "TLCGen";
            }
        }

        public List<IGeneratorViewModel> Generators => _generators ??= new List<IGeneratorViewModel>();

        public List<ITLCGenImporter> Importers => _importers ??= new List<ITLCGenImporter>();

        public List<ITLCGenTabItem> TabItems => _tabItems ??= new List<ITLCGenTabItem>();

        /// <summary>
        /// Holds the selected code generator, on which the appropriate function calls are invoked
        /// when commands relating to generators are executed.
        /// </summary>
        public IGeneratorViewModel SelectedGenerator
        {
            get => _SelectedGenerator;
            set
            {
                _SelectedGenerator = value;
                TLCGenControllerDataProvider.Default.CurrentGenerator = value.Generator;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Holds a list of available menu items that are bound to the View. This allows the user
        /// to click a menu item to instruct an importer to import data.
        /// </summary>
        public List<MenuItem> ImportMenuItems
        {
            get
            {
                if (_ImportMenuItems == null)
                {
                    _ImportMenuItems = new List<MenuItem>();
                }
                return _ImportMenuItems;
            }
        }

        public List<MenuItem> PluginMenuItems
        {
            get
            {
                if (_PluginMenuItems == null)
                {
                    _PluginMenuItems = new List<MenuItem>();
                }
                return _PluginMenuItems;
            }
        }

        public bool IsPluginMenuVisible => _PluginMenuItems?.Count > 0;

        public bool ShowAlertMessage
        {
            get => _showAlertMessage; set
            {
                _showAlertMessage = value;
                RaisePropertyChanged();
            }
        }

        public string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string ControllerVersion => ControllerVM?.Controller?.Data?.TLCGenVersie;

        public ObservableCollection<ControllerAlertMessage> AlertMessages => TLCGenModelManager.Default.ControllerAlerts;

        public bool ShowAlertMessages => AlertMessages.Any(x => x.Shown);

        #endregion // Properties

        #region Events

        public EventHandler<string> FileOpened;
        public EventHandler<string> FileOpenFailed;
        public EventHandler<string> FileSaved;

        #endregion // Events

        #region Commands

        public ICommand NewFileCommand => _newFileCommand ??= new RelayCommand(NewFileCommand_Executed, NewFileCommand_CanExecute);

        public ICommand OpenFileCommand => _openFileCommand ??= new RelayCommand(OpenFileCommand_Executed, OpenFileCommand_CanExecute);

        public ICommand SaveFileCommand => _saveFileCommand ??= new RelayCommand(SaveFileCommand_Executed, SaveFileCommand_CanExecute);

        public ICommand SaveAsFileCommand => _saveAsFileCommand ??= new RelayCommand(SaveAsFileCommand_Executed, SaveAsFileCommand_CanExecute);

        public ICommand CloseFileCommand => _closeFileCommand ??= new RelayCommand(CloseFileCommand_Executed, CloseFileCommand_CanExecute);

        public ICommand ExitApplicationCommand => _exitApplicationCommand ??= new RelayCommand(ExitApplicationCommand_Executed, ExitApplicationCommand_CanExecute);

        public ICommand GenerateControllerCommand => _generateControllerCommand ??= new RelayCommand(GenerateControllerCommand_Executed, GenerateControllerCommand_CanExecute);

        public ICommand ImportControllerCommand => _importControllerCommand ??= new RelayCommand(ImportControllerCommand_Executed, ImportControllerCommand_CanExecute);

        public ICommand ShowSettingsWindowCommand => _showSettingsWindowCommand ??= new RelayCommand(ShowSettingsWindowCommand_Executed, null);

        public ICommand ShowAboutCommand => _showAboutCommand ??= new RelayCommand(ShowAboutCommand_Executed, null);

        public ICommand ShowVersionInfoCommand => _showVersionInfoCommand ??= new RelayCommand(ShowVersionInfoCommand_Executed, null);

        public ICommand HideAlertMessageCommand => _hideAlertMessageCommand ??= new RelayCommand(HideAlertMessageCommand_Executed, null);

        public ICommand HideAllAlertMessagesCommand => _hideAllAlertMessagesCommand ??= new RelayCommand(HideAllAlertMessagesCommand_Executed, HideAllAlertMessagesCommand_CanExecute);

        #endregion // Commands

        #region Command functionality

        private void NewFileCommand_Executed(object prm)
        {
            if (TLCGenControllerDataProvider.Default.NewController())
            {
                var lastfilename = TLCGenControllerDataProvider.Default.ControllerFileName;

                // This allows plugins to reset their content
                ControllerVM.Controller = null;

                TLCGenControllerDataProvider.Default.NewController();
                SetControllerForStatics(TLCGenControllerDataProvider.Default.Controller);
                ControllerVM.Controller = TLCGenControllerDataProvider.Default.Controller;
                Messenger.Default.Send(new ControllerFileNameChangedMessage(TLCGenControllerDataProvider.Default.ControllerFileName, lastfilename));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                RaisePropertyChanged(nameof(HasController));
                RaisePropertyChanged(nameof(ProgramTitle));
                ShowAlertMessage = false;
            }
        }

        private bool NewFileCommand_CanExecute(object prm)
        {
            return true;
        }

        private void OpenFileCommand_Executed(object prm)
        {
            LoadController();
        }

        private bool OpenFileCommand_CanExecute(object prm)
        {
            return true;
        }

        private void SaveFileCommand_Executed(object prm)
        {
            if (TLCGenControllerDataProvider.Default.SaveController())
            {
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                GuiActionsManager.SetStatusBarMessage(
                    DateTime.Now.ToLongTimeString() +
                    " - Regeling " + TLCGenControllerDataProvider.Default.Controller.Data.Naam ?? "" + " opgeslagen");
                FileSaved?.Invoke(this, TLCGenControllerDataProvider.Default.ControllerFileName);
            }
        }

        private bool SaveFileCommand_CanExecute(object prm)
        {
            return TLCGenControllerDataProvider.Default.Controller != null &&
                   TLCGenControllerDataProvider.Default.ControllerHasChanged;
        }

        private void SaveAsFileCommand_Executed(object prm)
        {
            var lastfilename = TLCGenControllerDataProvider.Default.ControllerFileName;
            if (TLCGenControllerDataProvider.Default.SaveControllerAs())
            {
                Messenger.Default.Send(new ControllerFileNameChangedMessage(TLCGenControllerDataProvider.Default.ControllerFileName, lastfilename));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                RaisePropertyChanged(nameof(ProgramTitle));
                GuiActionsManager.SetStatusBarMessage(
                    DateTime.Now.ToLongTimeString() +
                    " - Regeling " + TLCGenControllerDataProvider.Default.Controller.Data.Naam ?? "" + " opgeslagen");
                FileSaved?.Invoke(this, TLCGenControllerDataProvider.Default.ControllerFileName);
            }
        }

        private bool SaveAsFileCommand_CanExecute(object prm)
        {
            return TLCGenControllerDataProvider.Default.Controller != null
                && ControllerVM.Controller.Data.Naam != null;
        }

        private void CloseFileCommand_Executed(object prm)
        {
            var lastfilename = TLCGenControllerDataProvider.Default.ControllerFileName;
            if (TLCGenControllerDataProvider.Default.CloseController())
            {
                DefaultsProvider.Default.Controller = null;
                Messenger.Default.Send(new ControllerFileNameChangedMessage(TLCGenControllerDataProvider.Default.ControllerFileName, lastfilename));
                RaisePropertyChanged(nameof(HasController));
                RaisePropertyChanged(nameof(ProgramTitle));
                GuiActionsManager.SetStatusBarMessage("");
                ShowAlertMessage = false;
                SetControllerForStatics(null);
            }
        }

        private bool CloseFileCommand_CanExecute(object prm)
        {
            return TLCGenControllerDataProvider.Default.Controller != null;
        }

        private void ExitApplicationCommand_Executed(object prm)
        {
            Application.Current.Shutdown();
        }

        private bool ExitApplicationCommand_CanExecute(object prm)
        {
            return true;
        }

        private void ImportControllerCommand_Executed(object obj)
        {
            if (obj == null)
                throw new NullReferenceException();
            if (!(obj is ITLCGenImporter imp))
                throw new InvalidCastException();

            // Import into existing controller
            if (TLCGenControllerDataProvider.Default.CheckChanged()) return;
            if (imp.ImportsIntoExisting)
            {
                // Request to process all synchronisation data from matrix to model
                Messenger.Default.Send(new ProcessSynchronisationsRequest());

                // Check data integrity
                var s1 = TLCGenIntegrityChecker.IsConflictMatrixOK(ControllerVM.Controller);
                if (s1 != null)
                {
                    TLCGenDialogProvider.Default.ShowMessageBox("Kan niet importeren:\n\n" + s1, "Error bij importeren: fout in regeling", MessageBoxButton.OK);
                    return;
                }
                // Import to clone of original (so we can discard if wrong)
                var c1 = DeepCloner.DeepClone(ControllerVM.Controller);
                var c2 = imp.ImportController(c1);

                // Do nothing if the importer returned nothing
                if (c2 == null)
                {
                    TLCGenDialogProvider.Default.ShowMessageBox("Importeren is afgebroken door de gebruiker", "Importeren afgebroken", MessageBoxButton.OK);
                    return;
                }

                // Check data integrity
                s1 = TLCGenIntegrityChecker.IsConflictMatrixOK(c2);
                if (s1 != null)
                {
                    TLCGenDialogProvider.Default.ShowMessageBox("Fout bij importeren:\n\n" + s1, "Error bij importeren: fout in data", MessageBoxButton.OK);
                    return;
                }
                SetController(c2);
                ControllerVM.ReloadController();
                MessengerInstance.Send(new ControllerDataChangedMessage());
                GuiActionsManager.SetStatusBarMessage(
                    DateTime.Now.ToLongTimeString() +
                    " - Data in regeling " + TLCGenControllerDataProvider.Default.Controller.Data.Naam + " geïmporteerd");
            }
            // Import as new controller
            else
            {
                var c1 = imp.ImportController();

                // Do nothing if the importer returned nothing
                if (c1 == null)
                    return;

                // Check data integrity
                var s1 = TLCGenIntegrityChecker.IsConflictMatrixOK(c1);
                if (s1 != null)
                {
                    TLCGenDialogProvider.Default.ShowMessageBox("Fout bij importeren:\n\n" + s1, "Error bij importeren: fout in data", MessageBoxButton.OK);
                    return;
                }
                TLCGenControllerDataProvider.Default.CloseController();
                DefaultsProvider.Default.SetDefaultsOnModel(c1.Data);
                DefaultsProvider.Default.SetDefaultsOnModel(c1.PrioData);
                SetController(c1);
                ControllerVM.ReloadController();
                GuiActionsManager.SetStatusBarMessage(
                    DateTime.Now.ToLongTimeString() +
                    " - Regeling geïmporteerd");
                MessengerInstance.Send(new ControllerDataChangedMessage());
            }
            Messenger.Default.Send(new UpdateTabsEnabledMessage());
            RaisePropertyChanged(nameof(HasController));
        }

        private bool GenerateControllerCommand_CanExecute(object obj)
        {
            return SelectedGenerator != null && SelectedGenerator.Generator != null && SelectedGenerator.Generator.CanGenerateController();
        }

        private void GenerateControllerCommand_Executed(object obj)
        {
            // Request to process all synchronisation data from matrix to model
            Messenger.Default.Send(new ProcessSynchronisationsRequest());

            var s = TLCGenIntegrityChecker.IsControllerDataOK(TLCGenControllerDataProvider.Default.Controller);
            if (s == null)
            {
                TLCGenControllerDataProvider.Default.Controller.Data.TLCGenVersie = Assembly.GetEntryAssembly().GetName().Version.ToString();
                SelectedGenerator.Generator.GenerateController();
                MessengerInstance.Send(new ControllerCodeGeneratedMessage());
            }
            else
            {
                TLCGenDialogProvider.Default.ShowMessageBox(s + "\n\nKan regeling niet genereren.", "Error bij genereren: fout in regeling", MessageBoxButton.OK);
            }
        }

        private bool ImportControllerCommand_CanExecute(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is ITLCGenImporter imp))
                throw new InvalidCastException();

            if (imp.ImportsIntoExisting)
                return TLCGenControllerDataProvider.Default.Controller != null;

            return true;
        }

        private void ShowSettingsWindowCommand_Executed(object obj)
        {
            var settingswin = new Settings.Views.TLCGenSettingsWindow
            {
                DataContext = new TLCGenSettingsViewModel(),
                Owner = Application.Current.MainWindow
            };
            settingswin.ShowDialog();
        }

        private void ShowAboutCommand_Executed(object obj)
        {
            var about = new AboutWindow
            {
                Owner = Application.Current.MainWindow
            };
            about.ShowDialog();
        }

        private void ShowVersionInfoCommand_Executed(object obj)
        {
            var infoW = new VersionInfoWindow(ControllerVersion, VersionFiles)
            {
                Owner = Application.Current.MainWindow
            };
            infoW.ShowDialog();
        }

        private void HideAlertMessageCommand_Executed(object obj)
        {
            ShowAlertMessage = false;
        }

        private void HideAllAlertMessagesCommand_Executed(object obj)
        {
            foreach (var msg in AlertMessages) msg.Shown = false;
        }

        private bool HideAllAlertMessagesCommand_CanExecute(object obj)
        {
            return AlertMessages.Any(x => x.Shown);
        }

        #endregion // Command functionality

        #region Private methods

        private void SetControllerForStatics(ControllerModel c)
        {
            DefaultsProvider.Default.Controller = c;
            TLCGenControllerModifier.Default.Controller = c;
            TLCGenModelManager.Default.Controller = c;
        }

        /// <summary>
        /// Called before the application is closed. This allows to save data and settings
        /// </summary>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (TLCGenControllerDataProvider.Default.CheckChanged())
            {
                e.Cancel = true;
            }
            else
            {
                foreach (var pl in ApplicationParts)
                {
                    if (pl.Item2 is ITLCGenHasSettings setpl)
                    {
                        setpl.SaveSettings();
                    }
                }

                SaveGeneratorControllerSettingsToModel();
                SettingsProvider.Default.SaveApplicationSettings();
                DefaultsProvider.Default.SaveSettings();
                //TemplatesProvider.Default.SaveSettings();
            }
        }

        // TODO: Make this generic, just like how settings are loaded
        private void SaveGeneratorControllerSettingsToModel()
        {
            //SettingsVM.Settings.CustomData.AddinSettings.Clear();
            foreach (var genvm in Generators)
            {
                var gen = genvm.Generator;
                var gendata = new AddinSettingsModel
                {
                    Naam = gen.GetGeneratorName()
                };
                var t = gen.GetType();
                // From the Generator, real all properties attributed with [TLCGenGeneratorSetting]
                var dllprops = t.GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(TLCGenCustomSettingAttribute)));
                foreach (var propertyInfo in dllprops)
                {
                    if (propertyInfo.CanRead)
                    {
                        var propattr = (TLCGenCustomSettingAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(TLCGenCustomSettingAttribute));
                        if (propattr.SettingType == TLCGenCustomSettingAttribute.SettingTypeEnum.Application)
                        {
                            try
                            {

                                var name = propertyInfo.Name;
                                var v = propertyInfo.GetValue(gen);
                                if (v != null)
                                {
                                    var value = v.ToString();
                                    var prop = new AddinSettingsPropertyModel
                                    {
                                        Naam = name,
                                        Setting = value
                                    };
                                    gendata.Properties.Add(prop);
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                //SettingsVM.Settings.CustomData.AddinSettings.Add(gendata);
            }
        }

        #endregion // Private methods

        #region Public methods

        /// <summary>
        /// Updates the ViewModel structure, causing the View to reload all bound properties.
        /// </summary>
        public void UpdateController()
        {
            ControllerVM.ReloadController();
            RaisePropertyChanged("");
        }

        /// <summary>
        /// Used to set the loaded Controller to a the parsed instance of ControllerModel. The method also 
        /// checks for changes, sets program title, and updates bound properties.
        /// </summary>
        /// <param name="cm">The instance of ControllerModel to be loaded.</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetController(ControllerModel cm)
        {
            if (TLCGenControllerDataProvider.Default.SetController(cm))
            {
                SetControllerForStatics(cm);
                ControllerVM.Controller = cm;
                return true;
            }
            return false;
        }

        public bool LoadController(string filename = null)
        {
            if (TLCGenControllerDataProvider.Default.OpenController(filename))
            {
                var lastfilename = TLCGenControllerDataProvider.Default.ControllerFileName;
                SetControllerForStatics(TLCGenControllerDataProvider.Default.Controller);
                ControllerVM.Controller = TLCGenControllerDataProvider.Default.Controller;
                Messenger.Default.Send(new ControllerFileNameChangedMessage(TLCGenControllerDataProvider.Default.ControllerFileName, lastfilename));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                RaisePropertyChanged(nameof(ProgramTitle));
                RaisePropertyChanged(nameof(HasController));
                RaisePropertyChanged(nameof(ControllerVersion));
                FileOpened?.Invoke(this, TLCGenControllerDataProvider.Default.ControllerFileName);
                var jumpTask = new JumpTask
                {
                    Title = Path.GetFileName(TLCGenControllerDataProvider.Default.ControllerFileName),
                    Arguments = TLCGenControllerDataProvider.Default.ControllerFileName
                };
                JumpList.AddToRecentCategory(jumpTask);
                ShowAlertMessage = Version.Parse(ControllerVersion) < VersionFiles.Max(x => x.Item1);
                return true;
            }
            if (filename != null) FileOpenFailed?.Invoke(this, filename);
            return false;
        }

        public void CheckCommandLineArgs()
        {
            var args = Environment.GetCommandLineArgs();

            if (args.Length > 1 && args[1].ToLower().EndsWith(".tlc") && File.Exists(args[1]))
            {
                LoadController(args[1]);
            }
        }

        #endregion // Public methods

        #region TLCGen Messaging

        private void OnPrepareForGenerationRequest(PrepareForGenerationRequest request)
        {
            var procreq = new ProcessSynchronisationsRequest();
            MessengerInstance.Send(procreq);
        }

        private void OnControllerCodeGenerated(ControllerCodeGeneratedMessage message)
        {
            GuiActionsManager.SetStatusBarMessage(
                DateTime.Now.ToLongTimeString() +
                " - Regeling " + TLCGenControllerDataProvider.Default.Controller.Data.Naam + " gegenereerd");
        }

        private void OnControllerProjectGenerated(ControllerProjectGeneratedMessage message)
        {
            GuiActionsManager.SetStatusBarMessage(
                DateTime.Now.ToLongTimeString() +
                " - Project voor regeling " + TLCGenControllerDataProvider.Default.Controller.Data.Naam + " gegenereerd");
        }

        private void OnControllerFileNameChanged(ControllerFileNameChangedMessage message)
        {
            RaisePropertyChanged(nameof(ProgramTitle));
        }

        #endregion // TLCGen Messaging

        #region Constructor

        public MainWindowViewModel()
        {
            var tmpCurDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            GuiActionsManager.SetStatusBarMessage = text => { StatusBarVM.StatusText = text; };

            MessengerInstance.Register(this,
                new Action<PrepareForGenerationRequest>(OnPrepareForGenerationRequest));
            MessengerInstance.Register(this, new Action<ControllerCodeGeneratedMessage>(OnControllerCodeGenerated));
            MessengerInstance.Register(this,
                new Action<ControllerProjectGeneratedMessage>(OnControllerProjectGenerated));
            MessengerInstance.Register(this, new Action<ControllerFileNameChangedMessage>(OnControllerFileNameChanged));

            // Load application settings and defaults
            ControllerAccessProvider.Default.Setup();
            TLCGenSplashScreenHelper.ShowText("Laden instellingen en defaults...");
            SettingsProvider.Default.LoadApplicationSettings();
            DefaultsProvider.Default.LoadSettings();
            TemplatesProvider.Default.LoadSettings();

            TLCGenModelManager.Default.InjectDefaultAction((x, s) => DefaultsProvider.Default.SetDefaultsOnModel(x, s));
            TLCGenControllerDataProvider.Default.InjectDefaultAction(
                x => DefaultsProvider.Default.SetDefaultsOnModel(x));

            var executingAssembly = Assembly.GetExecutingAssembly();

            // Load available applicationparts and plugins
            var types = from t in executingAssembly.GetTypes()
                where t.IsClass && t.Namespace == "TLCGen.ViewModels"
                select t;
            TLCGenSplashScreenHelper.ShowText("Laden applicatie onderdelen...");
            TLCGenPluginManager.Default.LoadApplicationParts(types.ToList());
            TLCGenSplashScreenHelper.ShowText("Laden plugins...");
            TLCGenPluginManager.Default.LoadPlugins(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins\\"));

            // Load version info
            var folderName = $"{executingAssembly.GetName().Name}.Resources.VersionInfo";
            var files = executingAssembly.GetManifestResourceNames()
                .Where(r => r.StartsWith(folderName) && r.EndsWith(".rtf")).ToList();
            files.Sort();
            foreach (var f in files)
            {
                var reader = new StreamReader(executingAssembly.GetManifestResourceStream(f));
                var text = reader.ReadToEnd();
                VersionFiles.Add(
                    new Tuple<Version, string>(Version.Parse(f.Replace($"{folderName}.", "").Replace(".rtf", "")),
                        text));
            }

            // Instantiate all parts
            ApplicationParts = new List<Tuple<TLCGenPluginElems, ITLCGenPlugin>>();
            var parts = TLCGenPluginManager.Default.ApplicationParts.Concat(TLCGenPluginManager.Default
                .ApplicationPlugins);
            foreach (var part in parts)
            {
                var instpl = part.Item2;
                TLCGenSplashScreenHelper.ShowText($"Laden plugin {instpl.GetPluginName()}...");
                var flags = Enum.GetValues(typeof(TLCGenPluginElems));
                foreach (TLCGenPluginElems elem in flags)
                {
                    if ((part.Item1 & elem) == elem)
                    {
                        switch (elem)
                        {
                            case TLCGenPluginElems.Generator:
                                Generators.Add(new IGeneratorViewModel(instpl as ITLCGenGenerator));
                                break;
                            case TLCGenPluginElems.HasSettings:
                                ((ITLCGenHasSettings) instpl).LoadSettings();
                                break;
                            case TLCGenPluginElems.Importer:
                                var mi = new MenuItem
                                {
                                    Header = instpl.GetPluginName(),
                                    Command = ImportControllerCommand,
                                    CommandParameter = instpl
                                };
                                ImportMenuItems.Add(mi);
                                break;
                            case TLCGenPluginElems.IOElementProvider:
                                break;
                            case TLCGenPluginElems.MenuControl:
                                PluginMenuItems.Add(((ITLCGenMenuItem) instpl).Menu);
                                break;
                            case TLCGenPluginElems.TabControl:
                                break;
                            case TLCGenPluginElems.ToolBarControl:
                                break;
                            case TLCGenPluginElems.XMLNodeWriter:
                                break;
                            case TLCGenPluginElems.PlugMessaging:
                                (instpl as ITLCGenPlugMessaging).UpdateTLCGenMessaging();
                                break;
                            case TLCGenPluginElems.Switcher:
                                (instpl as ITLCGenSwitcher).ControllerSet += (sender, model) =>
                                {
                                    SetController(model);
                                };
                                (instpl as ITLCGenSwitcher).FileNameSet += (sender, model) =>
                                {
                                    TLCGenControllerDataProvider.Default.ControllerFileName = model;
                                };
                                break;
                        }
                    }

                    TLCGenPluginManager.LoadAddinSettings(instpl, part.Item2.GetType(),
                        SettingsProvider.Default.Settings.CustomData);
                }

                ApplicationParts.Add(new Tuple<TLCGenPluginElems, ITLCGenPlugin>(part.Item1, instpl as ITLCGenPlugin));
            }

            if (Generators.Count > 0) SelectedGenerator = Generators[0];

            // Construct the ViewModel
            ControllerVM = new ControllerViewModel();

            if (!DesignMode.IsInDesignMode)
            {
                if (Application.Current != null && Application.Current.MainWindow != null)
                    Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
            }

            Directory.SetCurrentDirectory(tmpCurDir);

            TLCGenModelManager.Default.ControllerAlertsUpdated += (sender, args) => RaisePropertyChanged(nameof(ShowAlertMessages));
            AlertMessages.CollectionChanged += (sender, args) => RaisePropertyChanged(nameof(ShowAlertMessages));

#if !DEBUG
            // Find out if there is a newer version available via online check
            Task.Run(() =>
            {
				// clean potential old data
	            var key = Registry.CurrentUser.OpenSubKey("Software", true);
	            var sk1 = key?.OpenSubKey("CodingConnected e.U.", true);
	            var sk2 = sk1?.OpenSubKey("TLCGen", true);
	            var tempFile = (string) sk2?.GetValue("TempInstallFile", null);
	            if (tempFile != null)
	            {
		            if (File.Exists(tempFile))
		            {
			            File.Delete(tempFile);
		            }
					sk2?.DeleteValue("TempInstallFile");
	            }

                try
                {
                    var webRequest = WebRequest.Create(@"https://www.codingconnected.eu/tlcgen/deploy/tlcgenversioning");
                    webRequest.UseDefaultCredentials = true;
                    using var response = webRequest.GetResponse();
                    using var content = response.GetResponseStream();
                    if (content == null) return;
                    using var reader = new StreamReader(content);
                    var data = reader.ReadToEnd();
                    var all = data.Split('\r');
                    var tlcgenVer = all.FirstOrDefault(v => v.StartsWith("TLCGen="));
                    if (tlcgenVer == null) return;
                    var oldvers = Assembly.GetEntryAssembly()?.GetName().Version.ToString().Split('.');
                    var newvers = tlcgenVer.Replace("TLCGen=", "").Split('.');
                    var newer = false;
                    if (oldvers.Length > 0 && oldvers.Length == newvers.Length)
                    {
                        for (var i = 0; i < newvers.Length; i++)
                        {
                            var o = int.Parse(oldvers[i]);
                            var n = int.Parse(newvers[i]);
                            if (o > n)
                            {
                                break;
                            }

                            if (n > o)
                            {
                                newer = true;
                                break;
                            }
                        }
                    }

                    if (newer)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            var w = new NewVersionAvailableWindow(tlcgenVer.Replace("TLCGen=", ""))
                            {
                                Owner = Application.Current.MainWindow
                            };
                            w.ShowDialog();
                        });
                    }
                }
                catch
                {
                    // ignored
                }
            });
#endif
        }

        #endregion // Constructor

        #region IDropTarget

        public void DragOver(IDropInfo dropInfo)
        {
            if (!(dropInfo.Data is DataObject d) || !d.ContainsFileDropList())
            {
                dropInfo.NotHandled = true;
                return;
            }
            var files = d.GetFileDropList();
            if (files.Count == 1 && files[0].ToLower().EndsWith(".tlc") || files[0].ToLower().EndsWith(".tlcgz"))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (!(dropInfo.Data is DataObject d) || !d.ContainsFileDropList()) return;
            {
                var files = d.GetFileDropList();
                if (files.Count == 1 && files[0].ToLower().EndsWith(".tlc") || files[0].ToLower().EndsWith(".tlcgz"))
                {
                    LoadController(files[0]);
                }
            }
        }

        public void DragEnter(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        public void DragLeave(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        #endregion // IDropTarget
    }

}
