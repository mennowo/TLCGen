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
    public class PeriodenTemplatesEditorTabViewModel : ViewModelBase
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public ObservableCollectionAroundList<PeriodeTemplateViewModel, TLCGenTemplateModel<PeriodeModel>> PeriodenTemplates
        {
            get;
            private set;
        }

        private PeriodeTemplateViewModel _SelectedPeriodeTemplate;
        public PeriodeTemplateViewModel SelectedPeriodeTemplate
        {
            get { return _SelectedPeriodeTemplate; }
            set
            {
                _SelectedPeriodeTemplate = value;
                RaisePropertyChanged("SelectedPeriodeTemplate");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddPeriodeTemplateCommand;
        public ICommand AddPeriodeTemplateCommand
        {
            get
            {
                if (_AddPeriodeTemplateCommand == null)
                {
                    _AddPeriodeTemplateCommand = new RelayCommand(new Action<object>(AddPeriodeTemplateCommand_Executed));
                }
                return _AddPeriodeTemplateCommand;
            }
        }


        RelayCommand _RemovePeriodeTemplateCommand;
        public ICommand RemovePeriodeTemplateCommand
        {
            get
            {
                if (_RemovePeriodeTemplateCommand == null)
                {
                    _RemovePeriodeTemplateCommand = new RelayCommand(new Action<object>(RemovePeriodeTemplateCommand_Executed), new Predicate<object>(RemovePeriodeTemplateCommand_CanExecute));
                }
                return _RemovePeriodeTemplateCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private void AddPeriodeTemplateCommand_Executed(object prm)
        {
            var dtm = new TLCGenTemplateModel<PeriodeModel>();
            dtm.Naam = "Nieuw template";
            dtm.Replace = "fase";
            var dm = new PeriodeModel();
            dm.Naam = "fase_1";
            DefaultsProvider.Default.SetDefaultsOnModel(dm, "Auto");
            dtm.Items.Add(dm);
            PeriodenTemplates.Add(new PeriodeTemplateViewModel(dtm));
            MessengerInstance.Send(new TemplatesChangedMessage());
        }

        void RemovePeriodeTemplateCommand_Executed(object prm)
        {
            PeriodenTemplates.Remove(SelectedPeriodeTemplate);
            SelectedPeriodeTemplate = null;
        }

        bool RemovePeriodeTemplateCommand_CanExecute(object prm)
        {
            return SelectedPeriodeTemplate != null;
        }

        #endregion // Command Functionality

        #region Private Methods

        #endregion // Private Methods

        #region Constructor

        public PeriodenTemplatesEditorTabViewModel()
        {
            PeriodenTemplates = new ObservableCollectionAroundList<PeriodeTemplateViewModel, TLCGenTemplateModel<PeriodeModel>>(TemplatesProvider.Default.Templates.PeriodenTemplates);
        }

        #endregion // Constructor
    }
}
