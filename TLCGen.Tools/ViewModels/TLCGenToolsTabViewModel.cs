using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Models;

namespace TLCGen.Plugins.Tools
{
    public class TLCGenToolsTabViewModel : ViewModelBase
    {
        #region Fields

        private RelayCommand _applyTemplateCommand;
        private CombinatieTemplateViewModel _selectedTemplate;
        private ObservableCollection<string> _fasen;
        private ControllerModel _controller;
        private CombinatieTemplateViewModel _selectedCombinatieTemplate;

        #endregion // Fields

        #region Properties

        public ControllerModel Controller
        {
            get => _controller;
            set
            {
                _controller = value;
                SelectedTemplate = null;
                UpdateFasen();
            }
        }

        public List<CombinatieTemplateViewModel> CombinatieTemplates { get; }

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
                        TLCGen.GuiActions.GuiActionsManager.SetStatusBarMessage(DateTime.Now.ToLongTimeString() + $": template \"{SelectedTemplate.Name}\" toegepast.");
                    }
                    else
                    {
                        TLCGen.GuiActions.GuiActionsManager.SetStatusBarMessage(DateTime.Now.ToLongTimeString() + $": fout bij toepassen template \"{SelectedTemplate.Name}\" toegepast.");
                    }
                }
                else
                {
                    TLCGen.GuiActions.GuiActionsManager.SetStatusBarMessage(DateTime.Now.ToLongTimeString() + $": template \"{SelectedTemplate.Name}\" toegepast.");
                }
            },
            () => SelectedTemplate != null));

        #endregion // Commands

        #region Constructor

        public TLCGenToolsTabViewModel(List<CombinatieTemplateViewModel> combinatieTemplates)
        {
            CombinatieTemplates = combinatieTemplates;
            MessengerInstance.Register<Messaging.Messages.FasenChangedMessage>(this, OnFasenChanged);
        }

        #endregion // Constructor
    }
}
