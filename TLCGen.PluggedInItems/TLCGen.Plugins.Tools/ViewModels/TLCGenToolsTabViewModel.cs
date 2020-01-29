using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.Plugins.Tools
{
    public class TLCGenToolsTabViewModel : ViewModelBase
    {
        #region Fields

        private RelayCommand _applyTemplateCommand;
        private RelayCommand _settingsWindowCommand;
        private CombinatieTemplateViewModel _selectedTemplate;
        private ObservableCollection<string> _fasen;
        private ControllerModel _controller;
        private TLCGenToolsPlugin _plugin;

        #endregion // Fields

        #region Properties

        public ControllerModel Controller
        {
            get => _controller;
            set
            {
                _controller = value;
                UpdateFasen();
                SelectedTemplate = null;
            }
        }

        public ObservableCollection<CombinatieTemplateViewModel> CombinatieTemplates { get; }

        public ObservableCollection<string> Fasen
        {
            get => _fasen;
            private set
            {
                _fasen = value;
                RaisePropertyChanged();
            }
        }

        public bool HasNoSelectedItem => SelectedTemplate == null;

        public CombinatieTemplateViewModel SelectedTemplate
        {
            get => _selectedTemplate;
            set
            {
                _selectedTemplate = value;
                if (value != null)
                {
                    _selectedTemplate.SelectedItem?.SetSelectableItems();
                    foreach (var o in _selectedTemplate.Opties)
                    {
                        if (o.Type == CombinatieTemplateOptieTypeEnum.Fase)
                        {
                            if (Fasen.All(x => x != o.Replace))
                            {
                                o.Replace = Fasen.FirstOrDefault();
                            }
                        }
                    }
                }
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(HasNoSelectedItem));
            }
        }

        #endregion // Properties

        #region Private Methods

        private void OnFasenChanged(Messaging.Messages.FasenChangedMessage msg)
        {
            UpdateFasen();
            foreach (var c in CombinatieTemplates)
            {
                foreach (var o in c.Opties)
                {
                    if (o.Type == CombinatieTemplateOptieTypeEnum.Fase && !Fasen.Any(x => o.Replace == x))
                    {
                        o.SetReplaceToNull();
                    }
                }
            }
        }

        private void OnNameChanged(NameChangedMessage obj)
        {
            UpdateFasen();
        }

        private void UpdateFasen()
        {
            if (Controller == null)
            {
                if (Fasen?.Any() == true) Fasen.Clear();
                return;
            }
            var fasen = new ObservableCollection<string>();
            foreach (var f in Controller.Fasen)
            {
                fasen.Add(f.Naam);
            }
            Fasen = fasen;
            RaisePropertyChanged(nameof(Fasen));
        }

        #endregion // Private Methods

        #region Commands

        public ICommand ApplyTemplateCommand => _applyTemplateCommand ?? (_applyTemplateCommand = new RelayCommand(() =>
        {
            var alert = CombinatieTemplateProvider.ApplyCombinatieTemplate(Controller, SelectedTemplate.Template);
            if (!alert.Item1 || !string.IsNullOrEmpty(alert.Item2))
            {
                var cap = alert.Item1 ? "Template toepassen succesvol" : "Fout bij toepassen template";
                Dependencies.Providers.TLCGenDialogProvider.Default.ShowMessageBox(alert.Item2, cap, System.Windows.MessageBoxButton.OK);
                if (alert.Item1)
                {
                    GuiActions.GuiActionsManager.SetStatusBarMessage(DateTime.Now.ToLongTimeString() + $": template \"{SelectedTemplate.Name}\" toegepast.");
                }
                else
                {
                    GuiActions.GuiActionsManager.SetStatusBarMessage(DateTime.Now.ToLongTimeString() + $": fout bij toepassen template \"{SelectedTemplate.Name}\" toegepast.");
                }
            }
            else
            {
                GuiActions.GuiActionsManager.SetStatusBarMessage(DateTime.Now.ToLongTimeString() + $": template \"{SelectedTemplate.Name}\" toegepast.");
            }
        },
        () => SelectedTemplate != null));

        public ICommand SettingsWindowCommand => _settingsWindowCommand ?? (_settingsWindowCommand = new RelayCommand(() => 
        {
            var window = new CombinatieTemplatesSettingsWindow
            {
                DataContext = new CombinatieTemplatesSettingsWindowViewModel(CombinatieTemplates, _plugin)
            };
            window.ShowDialog();
        }));

        #endregion // Commands

        #region Constructor

        public TLCGenToolsTabViewModel(ObservableCollection<CombinatieTemplateViewModel> combinatieTemplates, TLCGenToolsPlugin plugin)
        {
            _plugin = plugin;
            CombinatieTemplates = combinatieTemplates;
            MessengerInstance.Register<Messaging.Messages.FasenChangedMessage>(this, OnFasenChanged);
            MessengerInstance.Register<Messaging.Messages.NameChangedMessage>(this, OnNameChanged);
        }

        #endregion // Constructor
    }
}
