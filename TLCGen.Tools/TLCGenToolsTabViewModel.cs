using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TLCGen.Models;

namespace TLCGen.Plugins.Tools
{
    public class TLCGenToolsTabViewModel : ViewModelBase
    {
        #region Fields
        #endregion // Fields

        #region Properties

        public ControllerModel Controller { get; set; }

        public List<CombinatieTemplateViewModel> CombinatieTemplates { get; }

        public CombinatieTemplateViewModel SelectedTemplate
        {
            get => _selectedTemplate;
            set
            {
                _selectedTemplate = value;
                RaisePropertyChanged();
            }
        }

        #endregion // Properties

        #region Commands

        private RelayCommand _applyTemplateCommand;
        private CombinatieTemplateViewModel _selectedTemplate;

        public ICommand ApplyTemplateCommand => _applyTemplateCommand ?? (_applyTemplateCommand = new RelayCommand(() =>
            {
                CombinatieTemplateProvider.ApplyCombinatieTemplate(Controller, SelectedTemplate.Template);
            },
            () => SelectedTemplate != null));

        #endregion // Commands

        #region Constructor

        public TLCGenToolsTabViewModel(List<CombinatieTemplateViewModel> combinatieTemplates)
        {
            CombinatieTemplates = combinatieTemplates;
        }

        #endregion // Constructor
    }

    public class CombinatieTemplateViewModel : ViewModelBase
    {
        #region Fields
        #endregion // Fields

        #region Properties

        public CombinatieTemplateModel Template { get; }

        public string Name => Template.Name;

        public List<CombinatieTemplateOptieViewModel> Opties { get; }

        #endregion // Properties

        #region Constructor

        public CombinatieTemplateViewModel(CombinatieTemplateModel template)
        {
            Template = template;
            Opties = new List<CombinatieTemplateOptieViewModel>();
            foreach(var o in template.Opties)
            {
                Opties.Add(new CombinatieTemplateOptieViewModel(o));
            }
        }

        #endregion // Constructor
    }

    public class CombinatieTemplateOptieViewModel : ViewModelBase
    {
        public CombinatieTemplateOptieModel Optie { get; }

        public string Description => Optie.Description;

        public string Replace
        {
            get => Optie.Replace;
            set
            {
                Optie.Replace = value;
                RaisePropertyChanged();
            }
        }
        public CombinatieTemplateOptieViewModel(CombinatieTemplateOptieModel optie)
        {
            Optie = optie;
        }
    }
}
