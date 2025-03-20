using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.Settings
{
    public class DetectorenTemplatesEditorTabViewModel : ObservableObject
    {
        #region Fields

        private RelayCommand _AddDetectorTemplateCommand;
        private RelayCommand _RemoveDetectorTemplateCommand;

        #endregion // Fields

        #region Properties

        public System.Windows.Visibility HasDC => 
            SelectedDetectorTemplate == null ? 
            System.Windows.Visibility.Collapsed : 
            System.Windows.Visibility.Visible;

        public ObservableCollectionAroundList<DetectorTemplateViewModel, TLCGenTemplateModel<DetectorModel>> DetectorenTemplates
        {
            get;
            private set;
        }

        private DetectorTemplateViewModel _SelectedDetectorTemplate;
        public DetectorTemplateViewModel SelectedDetectorTemplate
        {
            get => _SelectedDetectorTemplate;
            set
            {
                _SelectedDetectorTemplate = value;
                OnPropertyChanged("SelectedDetectorTemplate");
                OnPropertyChanged(nameof(HasDC));
                _RemoveDetectorTemplateCommand?.NotifyCanExecuteChanged();
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddDetectorTemplateCommand => _AddDetectorTemplateCommand ??= new RelayCommand(AddDetectorTemplateCommand_Executed, AddDetectorTemplateCommand_CanExecute);

        public ICommand RemoveDetectorTemplateCommand
        {
            get
            {
                if (_RemoveDetectorTemplateCommand == null)
                {
                    _RemoveDetectorTemplateCommand = new RelayCommand(RemoveDetectorTemplateCommand_Executed, RemoveDetectorTemplateCommand_CanExecute);
                }
                return _RemoveDetectorTemplateCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private void AddDetectorTemplateCommand_Executed()
        {
            var dtm = new TLCGenTemplateModel<DetectorModel>
            {
                Naam = "Nieuw template",
                Replace = "fase"
            };
            var dm = new DetectorModel
            {
                FaseCyclus = "fase",
                Naam = "fase_1"
            };
            DefaultsProvider.Default.SetDefaultsOnModel(dm, "Auto");
            dtm.Items.Add(dm);
            var d = new DetectorTemplateViewModel(dtm);
            DetectorenTemplates.Add(d);
            TemplatesProvider.Default.LoadedTemplates.First(x => x.Editable).Templates.DetectorenTemplates.Add(dtm);
            WeakReferenceMessenger.Default.Send(new TemplatesChangedMessage());
            SelectedDetectorTemplate = d;
        }

        bool AddDetectorTemplateCommand_CanExecute()
        {
            return TemplatesProvider.Default.LoadedTemplates.Any(x => x.Editable);
        }

        void RemoveDetectorTemplateCommand_Executed()
        {
            DetectorenTemplates.Remove(SelectedDetectorTemplate);
            SelectedDetectorTemplate = null;
        }

        bool RemoveDetectorTemplateCommand_CanExecute()
        {
            return SelectedDetectorTemplate is { Editable: true };
        }

        #endregion // Command Functionality

        #region Private Methods

        #endregion // Private Methods

        #region Constructor

        public DetectorenTemplatesEditorTabViewModel()
        {
            TemplatesProvider.Default.LoadedTemplatesChanged += DefaultOnLoadedTemplatesChanged;
            DetectorenTemplates = new ObservableCollectionAroundList<DetectorTemplateViewModel, TLCGenTemplateModel<DetectorModel>>(TemplatesProvider.Default.Templates.DetectorenTemplates);
        }

        private void DefaultOnLoadedTemplatesChanged(object sender, EventArgs e)
        {
            _AddDetectorTemplateCommand?.NotifyCanExecuteChanged();
        }

        ~DetectorenTemplatesEditorTabViewModel()
        {
            TemplatesProvider.Default.LoadedTemplatesChanged -= DefaultOnLoadedTemplatesChanged;
        }

        #endregion // Constructor
    }
}
