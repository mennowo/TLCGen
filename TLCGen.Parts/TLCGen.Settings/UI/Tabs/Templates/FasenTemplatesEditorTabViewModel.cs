using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using System;

namespace TLCGen.Settings
{
    public class FasenTemplatesEditorTabViewModel : ObservableObject
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public System.Windows.Visibility HasDC =>
            SelectedFaseCyclusTemplate == null ?
            System.Windows.Visibility.Collapsed :
            System.Windows.Visibility.Visible;

        public ObservableCollectionAroundList<FaseCyclusTemplateViewModel, TLCGenTemplateModel<FaseCyclusModel>> FasenTemplates
        {
            get;
            private set;
        }

        private FaseCyclusTemplateViewModel _SelectedFaseCyclusTemplate;
        public FaseCyclusTemplateViewModel SelectedFaseCyclusTemplate
        {
            get => _SelectedFaseCyclusTemplate;
            set
            {
                _SelectedFaseCyclusTemplate = value;
                OnPropertyChanged(nameof(SelectedFaseCyclusTemplate));
                OnPropertyChanged(nameof(HasDC));
                _RemoveFaseTemplateCommand?.NotifyCanExecuteChanged();
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
                    _AddFaseTemplateCommand = new RelayCommand(AddFaseTemplateCommand_Executed, AddFaseTemplateCommand_CanExecute);
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
                    _RemoveFaseTemplateCommand = new RelayCommand(RemoveFaseTemplateCommand_Executed, RemoveFaseTemplateCommand_CanExecute);
                }
                return _RemoveFaseTemplateCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private void AddFaseTemplateCommand_Executed()
        {
            var fct = new TLCGenTemplateModel<FaseCyclusModel>
            {
                Naam = "Nieuw template",
                Replace = "fase"
            };
            var fc = new FaseCyclusModel
            {
                Naam = "fase"
            };
            DefaultsProvider.Default.SetDefaultsOnModel(fc);
            fct.Items.Add(fc);
            var f = new FaseCyclusTemplateViewModel(fct);
            FasenTemplates.Add(f);
            TemplatesProvider.Default.LoadedTemplates.First(x => x.Editable).Templates.FasenTemplates.Add(fct);
            WeakReferenceMessengerEx.Default.Send(new TemplatesChangedMessage());
            SelectedFaseCyclusTemplate = f;
        }

        bool AddFaseTemplateCommand_CanExecute()
        {
            return TemplatesProvider.Default.LoadedTemplates.Any(x => x.Editable);
        }

        void RemoveFaseTemplateCommand_Executed()
        {
            FasenTemplates.Remove(SelectedFaseCyclusTemplate);
            SelectedFaseCyclusTemplate = null;
        }

        bool RemoveFaseTemplateCommand_CanExecute()
        {
            return SelectedFaseCyclusTemplate is { Editable: true };
        }

        #endregion // Command Functionality

        #region Constructor

        public FasenTemplatesEditorTabViewModel()
        {
            TemplatesProvider.Default.LoadedTemplatesChanged += DefaultOnLoadedTemplatesChanged;
            FasenTemplates = new ObservableCollectionAroundList<FaseCyclusTemplateViewModel, TLCGenTemplateModel<FaseCyclusModel>>(TemplatesProvider.Default.Templates.FasenTemplates);
        }

        private void DefaultOnLoadedTemplatesChanged(object sender, EventArgs e)
        {
            _AddFaseTemplateCommand?.NotifyCanExecuteChanged();
        }

        ~FasenTemplatesEditorTabViewModel()
        {
            TemplatesProvider.Default.LoadedTemplatesChanged -= DefaultOnLoadedTemplatesChanged;
        }

        #endregion // Constructor
    }
}
