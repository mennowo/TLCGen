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
    public class FasenTemplatesEditorTabViewModel : ViewModelBase
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public ObservableCollectionAroundList<FaseCyclusTemplateViewModel, TLCGenTemplateModel<FaseCyclusModel>> FasenTemplates
        {
            get;
            private set;
        }

        private FaseCyclusTemplateViewModel _SelectedFaseCyclusTemplate;
        public FaseCyclusTemplateViewModel SelectedFaseCyclusTemplate
        {
            get { return _SelectedFaseCyclusTemplate; }
            set
            {
                _SelectedFaseCyclusTemplate = value;
                RaisePropertyChanged("SelectedFaseCyclusTemplate");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddFaseTemplateCommand;
        public ICommand AddFaseTemplateCommand
        {
            get
            {
                if (_AddFaseTemplateCommand == null)
                {
                    _AddFaseTemplateCommand = new RelayCommand(new Action<object>(AddFaseTemplateCommand_Executed));
                }
                return _AddFaseTemplateCommand;
            }
        }


        RelayCommand _RemoveFaseTemplateCommand;
        public ICommand RemoveFaseTemplateCommand
        {
            get
            {
                if (_RemoveFaseTemplateCommand == null)
                {
                    _RemoveFaseTemplateCommand = new RelayCommand(new Action<object>(RemoveFaseTemplateCommand_Executed), new Predicate<object>(RemoveFaseTemplateCommand_CanExecute));
                }
                return _RemoveFaseTemplateCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private void AddFaseTemplateCommand_Executed(object prm)
        {
            var fct = new TLCGenTemplateModel<FaseCyclusModel>();
            fct.Naam = "Nieuw template";
            fct.Replace = "fase";
            var fc = new FaseCyclusModel();
            fc.Naam = "fase";
            DefaultsProvider.Default.SetDefaultsOnModel(fc);
            fct.Items.Add(fc);
            FasenTemplates.Add(new FaseCyclusTemplateViewModel(fct));
        }

        void RemoveFaseTemplateCommand_Executed(object prm)
        {
            FasenTemplates.Remove(SelectedFaseCyclusTemplate);
            SelectedFaseCyclusTemplate = null;
        }

        bool RemoveFaseTemplateCommand_CanExecute(object prm)
        {
            return SelectedFaseCyclusTemplate != null;
        }

        #endregion // Command Functionality

        #region Private Methods

        #endregion // Private Methods

        #region Constructor

        public FasenTemplatesEditorTabViewModel()
        {
            FasenTemplates = new ObservableCollectionAroundList<FaseCyclusTemplateViewModel, TLCGenTemplateModel<FaseCyclusModel>>(TemplatesProvider.Default.Templates.FasenTemplates);
        }

        #endregion // Constructor
    }
}
