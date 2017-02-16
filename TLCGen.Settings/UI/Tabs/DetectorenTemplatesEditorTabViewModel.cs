using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.Settings
{
    public class DetectorenTemplatesEditorTabViewModel : ViewModelBase
    {
        #region Fields

        #endregion // Fields

        #region Properties

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
                    _AddDetectorTemplateCommand = new RelayCommand(new Action<object>(AddDetectorTemplateCommand_Executed));
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
                    _RemoveDetectorTemplateCommand = new RelayCommand(new Action<object>(RemoveDetectorTemplateCommand_Executed), new Predicate<object>(RemoveDetectorTemplateCommand_CanExecute));
                }
                return _RemoveDetectorTemplateCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private void AddDetectorTemplateCommand_Executed(object prm)
        {
            var dtm = new TLCGenTemplateModel<DetectorModel>();
            dtm.Naam = "Nieuw template";
            dtm.Replace = "fase";
            var dm = new DetectorModel();
            dm.Naam = "fase_1";
            DefaultsProvider.Default.SetDefaultsOnModel(dm);
            dtm.Items.Add(dm);
            DetectorenTemplates.Add(new DetectorTemplateViewModel(dtm));
        }

        void RemoveDetectorTemplateCommand_Executed(object prm)
        {
            DetectorenTemplates.Remove(SelectedDetectorTemplate);
            SelectedDetectorTemplate = null;
        }

        bool RemoveDetectorTemplateCommand_CanExecute(object prm)
        {
            return SelectedDetectorTemplate != null;
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
