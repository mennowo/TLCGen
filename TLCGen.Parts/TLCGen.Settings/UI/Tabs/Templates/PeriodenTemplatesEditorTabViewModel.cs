﻿using GalaSoft.MvvmLight;
using System.Linq;
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

        public System.Windows.Visibility HasDC =>
            SelectedPeriodeTemplate == null ?
            System.Windows.Visibility.Collapsed :
            System.Windows.Visibility.Visible;

        public ObservableCollectionAroundList<PeriodeTemplateViewModel, TLCGenTemplateModel<PeriodeModel>> PeriodenTemplates
        {
            get;
            private set;
        }

        private PeriodeTemplateViewModel _SelectedPeriodeTemplate;
        public PeriodeTemplateViewModel SelectedPeriodeTemplate
        {
            get => _SelectedPeriodeTemplate;
            set
            {
                _SelectedPeriodeTemplate = value;
                RaisePropertyChanged("SelectedPeriodeTemplate");
                RaisePropertyChanged(nameof(HasDC));
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
                    _AddPeriodeTemplateCommand = new RelayCommand(AddPeriodeTemplateCommand_Executed, AddPeriodeTemplateCommand_CanExecute);
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
                    _RemovePeriodeTemplateCommand = new RelayCommand(RemovePeriodeTemplateCommand_Executed, RemovePeriodeTemplateCommand_CanExecute);
                }
                return _RemovePeriodeTemplateCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private void AddPeriodeTemplateCommand_Executed(object prm)
        {
            var pmt = new TLCGenTemplateModel<PeriodeModel>
            {
                Naam = "Nieuw template"
            };
            var pm = new PeriodeModel
            {
                DagCode = Models.Enumerations.PeriodeDagCodeEnum.AlleDagen,
                Type = Models.Enumerations.PeriodeTypeEnum.Groentijden,
                Naam = "per_1"
            };
            DefaultsProvider.Default.SetDefaultsOnModel(pm, pm.Type.ToString());
            pmt.Items.Add(pm);
            var pvm = new PeriodeTemplateViewModel(pmt);
            PeriodenTemplates.Add(pvm);
            TemplatesProvider.Default.LoadedTemplates.First(x => x.Editable).Templates.PeriodenTemplates.Add(pmt);
            MessengerInstance.Send(new TemplatesChangedMessage());
            SelectedPeriodeTemplate = pvm;
        }

        bool AddPeriodeTemplateCommand_CanExecute(object prm)
        {
            return TemplatesProvider.Default.LoadedTemplates.Any(x => x.Editable);
        }

        void RemovePeriodeTemplateCommand_Executed(object prm)
        {
            PeriodenTemplates.Remove(SelectedPeriodeTemplate);
            SelectedPeriodeTemplate = null;
        }

        bool RemovePeriodeTemplateCommand_CanExecute(object prm)
        {
            return SelectedPeriodeTemplate != null && SelectedPeriodeTemplate.Editable;
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
