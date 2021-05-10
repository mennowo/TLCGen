using GalaSoft.MvvmLight;
using System.Linq;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

namespace TLCGen.Settings
{
    public class PrioIngrepenTemplatesEditorTabViewModel : ViewModelBase
    {
        #region Fields

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

        private PrioIngreepTemplateViewModel _selectedPrioIngreepTemplate;
        public PrioIngreepTemplateViewModel SelectedPrioIngreepTemplate
        {
            get => _selectedPrioIngreepTemplate;
            set
            {
                _selectedPrioIngreepTemplate = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(HasDc));
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _addPrioIngreepTemplateCommand;
        public ICommand AddPrioIngreepTemplateCommand
        {
            get
            {
                if (_addPrioIngreepTemplateCommand == null)
                {
                    _addPrioIngreepTemplateCommand = new RelayCommand(() =>
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
                        TemplatesProvider.Default.LoadedTemplates.First(x => x.Editable).Templates.PrioIngreepTemplates.Add(pmt);
                        MessengerInstance.Send(new TemplatesChangedMessage());
                        SelectedPrioIngreepTemplate = pvm;
                    }, () => TemplatesProvider.Default.LoadedTemplates.Any(x => x.Editable));
                }
                return _addPrioIngreepTemplateCommand;
            }
        }


        RelayCommand _removePrioIngreepTemplateCommand;
        public ICommand RemovePrioIngreepTemplateCommand
        {
            get
            {
                if (_removePrioIngreepTemplateCommand == null)
                {
                    _removePrioIngreepTemplateCommand = new RelayCommand(() =>
                    {
                        PrioIngrepenTemplates.Remove(SelectedPrioIngreepTemplate);
                        SelectedPrioIngreepTemplate = null;
                    }, () => SelectedPrioIngreepTemplate != null && SelectedPrioIngreepTemplate.Editable);
                }
                return _removePrioIngreepTemplateCommand;
            }
        }

        #endregion // Commands
        
        #region Private Methods

        #endregion // Private Methods

        #region Constructor

        public PrioIngrepenTemplatesEditorTabViewModel()
        {
            PrioIngrepenTemplates = new ObservableCollectionAroundList<PrioIngreepTemplateViewModel, TLCGenTemplateModel<PrioIngreepModel>>(TemplatesProvider.Default.Templates.PrioIngreepTemplates);
        }

        #endregion // Constructor
    }
}
