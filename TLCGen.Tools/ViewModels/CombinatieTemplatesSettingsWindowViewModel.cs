using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using TLCGen.Models;

namespace TLCGen.Plugins.Tools
{
    public class CombinatieTemplatesSettingsWindowViewModel : ViewModelBase
    {
        #region Fields

        private RelayCommand _addCombinatieTemplateCommand;
        private RelayCommand _removeCombinatieTemplateCommand;
        private CombinatieTemplateViewModel _selectedTemplate;
        private TLCGenToolsPlugin _plugin;

        #endregion // Fields

        #region Properties

        public ObservableCollection<CombinatieTemplateViewModel> CombinatieTemplates { get; }

        public CombinatieTemplateViewModel SelectedTemplate
        {
            get => _selectedTemplate;
            set
            {
                _selectedTemplate = value;
                _selectedTemplate?.SelectedItem?.SetSelectableItems();
                RaisePropertyChanged();
            }
        }

        public string CombinatieTemplatesFileLocation
        {
            get => _plugin.TemplatesLocation;
            set
            {
                _plugin.TemplatesLocation = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(HasTemplates));
            }
        }

        public bool HasTemplates => !string.IsNullOrWhiteSpace(CombinatieTemplatesFileLocation) && File.Exists(CombinatieTemplatesFileLocation);

        #endregion // Properties

        #region Commands

        public ICommand AddCombinatieTemplateCommand => _addCombinatieTemplateCommand ?? (_addCombinatieTemplateCommand = new RelayCommand(() =>
        {
            var vm = new CombinatieTemplateViewModel(new CombinatieTemplateModel { Name = "Nieuw template" });
            vm.Items.Add(new CombinatieTemplateItemViewModel(new CombinatieTemplateItemModel
            {
                ObjectJson = JsonConvert.SerializeObject(new DetectorModel { Naam = "d1" }, Formatting.Indented),
                Description = "Detector 1"
            }));
            CombinatieTemplates.Add(vm);
            SelectedTemplate = vm;
            vm.SelectedItem = vm.Items.First();
        }));

        public ICommand RemoveCombinatieTemplateCommand => _removeCombinatieTemplateCommand ?? (_removeCombinatieTemplateCommand = new RelayCommand(() =>
        {
            var i = CombinatieTemplates.IndexOf(SelectedTemplate);
            CombinatieTemplates.Remove(SelectedTemplate);
            SelectedTemplate = null;
            if (CombinatieTemplates.Any())
            {
                if (i >= CombinatieTemplates.Count) SelectedTemplate = CombinatieTemplates.Last();
                else if (i >= 0 && i < CombinatieTemplates.Count) SelectedTemplate = CombinatieTemplates[i];
            }
        },
        () => SelectedTemplate != null));

        #endregion // Commands

        #region Constructor

        public CombinatieTemplatesSettingsWindowViewModel(ObservableCollection<CombinatieTemplateViewModel> combinatieTemplates, TLCGenToolsPlugin plugin)
        {
            CombinatieTemplates = combinatieTemplates;
            _plugin = plugin;
        }

        #endregion // Constructor
    }
}
