using System.Windows.Input;
using GalaSoft.MvvmLight;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace TLCGen.GebruikersOpties
{
    public class GebruikersOptiesImportExportTabViewModel : ViewModelBase
    {
        #region Fields

        private GebruikersOptiesModel _dataModel;
        private GebruikersOptiesTabViewModel _plugin;
        private RelayCommand _exportCommand;
        private RelayCommand _openExternalDataCommand;
        private RelayCommand _importCommand;
        private RelayCommand _replaceInImportCommand;
        private RelayCommand _selectAllCommand;
        private string _replaceInImportFind;
        private string _replaceInImportReplace;
        private bool _replaceInImportRegex;

        #endregion // Fields

        #region Properties

        public ObservableCollection<GebruikersOptieGenericViewModel> ItemsAllPresent { get; } = new ObservableCollection<GebruikersOptieGenericViewModel>();
        public ObservableCollection<GebruikersOptieGenericViewModel> ItemsToImport { get; } = new ObservableCollection<GebruikersOptieGenericViewModel>();

        #endregion // Properties

        #region Commands

        public GebruikersOptiesModel DataModel
        {
            get => _dataModel;
            set
            {
                _dataModel = value;
                RaisePropertyChanged();
            }
        }

        public string ReplaceInImportFind
        {
            get => _replaceInImportFind;
            set
            {
                _replaceInImportFind = value;
                RaisePropertyChanged();
            }
        }

        public string ReplaceInImportReplace
        {
            get => _replaceInImportReplace;
            set
            {
                _replaceInImportReplace = value;
                RaisePropertyChanged();
            }
        }

        public bool ReplaceInImportRegex
        {
            get => _replaceInImportRegex;
            set
            {
                _replaceInImportRegex = value;
                RaisePropertyChanged();
            }
        }

        public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new RelayCommand(
            () =>
            {
                foreach (var i in ItemsAllPresent)
                {
                    i.Selected = true;
                }
            },
            () => ItemsAllPresent.Any()));

        public ICommand ExportCommand
        {
            get
            {
                if (_exportCommand == null)
                {
                    _exportCommand = new RelayCommand(
                        () =>
                        {
                            var dlg = new SaveFileDialog
                            {
                                FileName = "gebruikersopties.xml",
                                Filter = "XML data|*.xml",
                                Title = "Kies XML bestand voor opslag gebruikersopties",
                                DefaultExt = ".xml",
                                CheckFileExists = false
                            };
                            var r = dlg.ShowDialog();
                            if (r == true)
                            {
                                var exp = new GebruikersOptiesExportModel();
                                foreach (var o in ItemsAllPresent.Where(x => x.Selected))
                                {
                                    exp.Items.Add(o);
                                }
                                try
                                {
                                    Helpers.TLCGenSerialization.Serialize(dlg.FileName, exp);
                                }
                                catch (Exception e)
                                {
                                    Dependencies.Providers.TLCGenDialogProvider.Default.ShowMessageBox(
                                        $"Kon gebruikersopties niet exporteren:\n\n{e}",
                                        "Fout bij exporteren gebruikersopties", System.Windows.MessageBoxButton.OK);
                                }
                            }
                        },
                        () => ItemsAllPresent.Any(x => x.Selected));
                }
                return _exportCommand;
            }
        }

        public ICommand OpenExternalDataCommand
        {
            get
            {
                if (_openExternalDataCommand == null)
                {
                    _openExternalDataCommand = new RelayCommand(
                        () =>
                        {
                            var dlg = new OpenFileDialog
                            {
                                FileName = "gebruikersopties.xml",
                                Filter = "XML data|*.xml",
                                Title = "Kies XML bestand met opgeslagen gebruikersopties",
                                DefaultExt = ".xml",
                                CheckFileExists = true
                            };
                            var r = dlg.ShowDialog();
                            if (r == true)
                            {
                                try
                                {
                                    var imp = Helpers.TLCGenSerialization.DeSerialize<GebruikersOptiesExportModel>(dlg.FileName);
                                    ItemsToImport.Clear();
                                    foreach (var o in imp.Items)
                                    {
                                        o.Selected = true;
                                        ItemsToImport.Add(o);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Dependencies.Providers.TLCGenDialogProvider.Default.ShowMessageBox(
                                        $"Kon gebruikersopties niet importeren:\n\n{e.ToString()}",
                                        "Fout bij importeren gebruikersopties", System.Windows.MessageBoxButton.OK);
                                }
                            }
                        },
                        () => true);
                }
                return _openExternalDataCommand;
            }
        }

        public ICommand ImportCommand
        {
            get
            {
                if (_importCommand == null)
                {
                    _importCommand = new RelayCommand(
                        () =>
                        {
                            foreach (var o in ItemsToImport.Where(x => x.Selected))
                            {
                                if (Integrity.TLCGenIntegrityChecker.IsElementNaamUnique(_plugin.Controller, o.Naam, o.ObjectType))
                                {
                                    switch (o.ObjectType)
                                    {
                                        case Models.Enumerations.TLCGenObjectTypeEnum.Output:
                                            _plugin.Uitgangen.Add((GebruikersOptieWithIOViewModel)o.GetRelatedObject());
                                            break;
                                        case Models.Enumerations.TLCGenObjectTypeEnum.Input:
                                            _plugin.Ingangen.Add((GebruikersOptieWithIOViewModel)o.GetRelatedObject());
                                            break;
                                        case Models.Enumerations.TLCGenObjectTypeEnum.CCOLHelpElement:
                                            _plugin.HulpElementen.Add((GebruikersOptieViewModel)o.GetRelatedObject());
                                            break;
                                        case Models.Enumerations.TLCGenObjectTypeEnum.CCOLTimer:
                                            _plugin.Timers.Add((GebruikersOptieViewModel)o.GetRelatedObject());
                                            break;
                                        case Models.Enumerations.TLCGenObjectTypeEnum.CCOLCounter:
                                            _plugin.Counters.Add((GebruikersOptieViewModel)o.GetRelatedObject());
                                            break;
                                        case Models.Enumerations.TLCGenObjectTypeEnum.CCOLSchakelaar:
                                            _plugin.Schakelaars.Add((GebruikersOptieViewModel)o.GetRelatedObject());
                                            break;
                                        case Models.Enumerations.TLCGenObjectTypeEnum.CCOLMemoryElement:
                                            _plugin.GeheugenElementen.Add((GebruikersOptieViewModel)o.GetRelatedObject());
                                            break;
                                        case Models.Enumerations.TLCGenObjectTypeEnum.CCOLParameter:
                                            _plugin.Parameters.Add((GebruikersOptieViewModel)o.GetRelatedObject());
                                            break;
                                    }
                                }
                            }
                        },
                        () => ItemsToImport.Any(x => x.Selected));
                }
                return _importCommand;
            }
        }

        public ICommand ReplaceInImportCommand
        {
            get
            {
                if (_replaceInImportCommand == null)
                {
                    _replaceInImportCommand = new RelayCommand(
                        () =>
                        {
                            foreach (var i in ItemsToImport)
                            {
                                if (ReplaceInImportRegex)
                                {
                                    if (!string.IsNullOrWhiteSpace(i.Naam)) i.Naam = Regex.Replace(i.Naam, ReplaceInImportFind, ReplaceInImportReplace);
                                    if (!string.IsNullOrWhiteSpace(i.Commentaar)) i.Commentaar = Regex.Replace(i.Commentaar, ReplaceInImportFind, ReplaceInImportReplace);
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(i.Naam)) i.Naam = i.Naam.Replace(ReplaceInImportFind, ReplaceInImportReplace);
                                    if (!string.IsNullOrWhiteSpace(i.Commentaar)) i.Commentaar = i.Commentaar.Replace(ReplaceInImportFind, ReplaceInImportReplace);
                                }
                            }
                        },
                        () => { return ItemsToImport.Any(x => x.Selected) && !string.IsNullOrWhiteSpace(ReplaceInImportFind) && !string.IsNullOrWhiteSpace(ReplaceInImportReplace); }
                    );
                }
                return _replaceInImportCommand;
            }
        }

        #endregion // Commands

        #region Public Methods

        public void RebuildAllItems(GebruikersOptiesTabViewModel vm)
        {
            ItemsAllPresent.Clear();
            foreach (var o in vm.Uitgangen) ItemsAllPresent.Add(new GebruikersOptieGenericViewModel(o));
            foreach (var o in vm.Ingangen) ItemsAllPresent.Add(new GebruikersOptieGenericViewModel(o));
            foreach (var o in vm.HulpElementen) ItemsAllPresent.Add(new GebruikersOptieGenericViewModel(o));
            foreach (var o in vm.Timers) ItemsAllPresent.Add(new GebruikersOptieGenericViewModel(o));
            foreach (var o in vm.Counters) ItemsAllPresent.Add(new GebruikersOptieGenericViewModel(o));
            foreach (var o in vm.Schakelaars) ItemsAllPresent.Add(new GebruikersOptieGenericViewModel(o));
            foreach (var o in vm.GeheugenElementen) ItemsAllPresent.Add(new GebruikersOptieGenericViewModel(o));
            foreach (var o in vm.Parameters) ItemsAllPresent.Add(new GebruikersOptieGenericViewModel(o));
        }

        #endregion // Public Methods

        #region Constructor

        public GebruikersOptiesImportExportTabViewModel(GebruikersOptiesTabViewModel plugin)
        {
            _plugin = plugin;
        }

        #endregion // Constructor
    }
}
