using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.Settings
{
    public class DetectorenTemplatesEditorTabViewModel : ViewModelBase
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
            get { return _SelectedDetectorTemplate; }
            set
            {
                _SelectedDetectorTemplate = value;
                RaisePropertyChanged("SelectedDetectorTemplate");
                RaisePropertyChanged(nameof(HasDC));
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

        private void AddDetectorTemplateCommand_Executed(object prm)
        {
            var dtm = new TLCGenTemplateModel<DetectorModel>
            {
                Naam = "Nieuw template",
                Replace = "fase"
            };
            var dm = new DetectorModel
            {
                Naam = "fase_1"
            };
            DefaultsProvider.Default.SetDefaultsOnModel(dm, "Auto");
            dtm.Items.Add(dm);
            var d = new DetectorTemplateViewModel(dtm);
            DetectorenTemplates.Add(d);
            TemplatesProvider.Default.LoadedTemplates.First(x => x.Editable).Templates.DetectorenTemplates.Add(dtm);
            MessengerInstance.Send(new TemplatesChangedMessage());
            SelectedDetectorTemplate = d;
        }

        bool AddDetectorTemplateCommand_CanExecute(object prm)
        {
            return TemplatesProvider.Default.LoadedTemplates.Any(x => x.Editable);
        }

        void RemoveDetectorTemplateCommand_Executed(object prm)
        {
            DetectorenTemplates.Remove(SelectedDetectorTemplate);
            SelectedDetectorTemplate = null;
        }

        bool RemoveDetectorTemplateCommand_CanExecute(object prm)
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
