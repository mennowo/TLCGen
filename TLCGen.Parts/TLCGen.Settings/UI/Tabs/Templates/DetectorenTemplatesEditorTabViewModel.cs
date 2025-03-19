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
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddDetectorTemplateCommand;
        public ICommand AddDetectorTemplateCommand
        {
            get
            {
                if (_AddDetectorTemplateCommand == null)
                {
                    _AddDetectorTemplateCommand = new RelayCommand(AddDetectorTemplateCommand_Executed, AddDetectorTemplateCommand_CanExecute);
                }
                return _AddDetectorTemplateCommand;
            }
        }


        RelayCommand _RemoveDetectorTemplateCommand;
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
            return SelectedDetectorTemplate != null && SelectedDetectorTemplate.Editable;
        }

        #endregion // Command Functionality

        #region Private Methods

        #endregion // Private Methods

        #region Constructor

        public DetectorenTemplatesEditorTabViewModel()
        {
            DetectorenTemplates = new ObservableCollectionAroundList<DetectorTemplateViewModel, TLCGenTemplateModel<DetectorModel>>(TemplatesProvider.Default.Templates.DetectorenTemplates);
        }

        #endregion // Constructor
    }
}
