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
    public class PrioIngrepenTemplatesEditorTabViewModel : ObservableObject
    {
        #region Fields

        private RelayCommand _addPrioIngreepTemplateCommand;
        private RelayCommand _removePrioIngreepTemplateCommand;
        private PrioIngreepTemplateViewModel _selectedPrioIngreepTemplate;

        #endregion // Fields

        #region Properties

        public System.Windows.Visibility HasDc =>
            SelectedPrioIngreepTemplate == null ?
            System.Windows.Visibility.Collapsed :
            System.Windows.Visibility.Visible;

        public ObservableCollectionAroundList<PrioIngreepTemplateViewModel, TLCGenTemplateModel<PrioIngreepModel>> PrioIngrepenTemplates
        {
            get;
            private set;
        }

        public PrioIngreepTemplateViewModel SelectedPrioIngreepTemplate
        {
            get => _selectedPrioIngreepTemplate;
            set
            {
                _selectedPrioIngreepTemplate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasDc));
                _removePrioIngreepTemplateCommand?.NotifyCanExecuteChanged();
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddPrioIngreepTemplateCommand => _addPrioIngreepTemplateCommand ?? (_addPrioIngreepTemplateCommand = new RelayCommand(() =>
            {
                var pmt = new TLCGenTemplateModel<PrioIngreepModel>
                {
                    Naam = "Nieuw template"
                };
                var pm = new PrioIngreepModel();
                DefaultsProvider.Default.SetDefaultsOnModel(pm, pm.Type.ToString());
                pmt.Items.Add(pm);
                var pvm = new PrioIngreepTemplateViewModel(pmt);
                PrioIngrepenTemplates.Add(pvm);
                TemplatesProvider.Default.LoadedTemplates.First(x => x.Editable).Templates.PrioIngreepTemplates
                    .Add(pmt);
                WeakReferenceMessenger.Default.Send(new TemplatesChangedMessage());
                SelectedPrioIngreepTemplate = pvm;
            }, 
            () => TemplatesProvider.Default.LoadedTemplates.Any(x => x.Editable)));


        public ICommand RemovePrioIngreepTemplateCommand => _removePrioIngreepTemplateCommand ??= new RelayCommand(() =>
            {
                PrioIngrepenTemplates.Remove(SelectedPrioIngreepTemplate);
                SelectedPrioIngreepTemplate = null;
            }, 
            () => SelectedPrioIngreepTemplate is { Editable: true });

        #endregion // Commands

        #region Constructor

        public PrioIngrepenTemplatesEditorTabViewModel()
        {
            TemplatesProvider.Default.LoadedTemplatesChanged += DefaultOnLoadedTemplatesChanged;
            PrioIngrepenTemplates = new ObservableCollectionAroundList<PrioIngreepTemplateViewModel, TLCGenTemplateModel<PrioIngreepModel>>(TemplatesProvider.Default.Templates.PrioIngreepTemplates);
        }

        private void DefaultOnLoadedTemplatesChanged(object sender, EventArgs e)
        {
            _addPrioIngreepTemplateCommand?.NotifyCanExecuteChanged();
        }

        ~PrioIngrepenTemplatesEditorTabViewModel()
        {
            TemplatesProvider.Default.LoadedTemplatesChanged -= DefaultOnLoadedTemplatesChanged;
        }

        #endregion // Constructor
    }
}
