using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TLCGen.Plugins.Tools
{
    public class CombinatieTemplateViewModel : ViewModelBase
    {
        #region Fields
        
        private ObservableCollection<string> _fasen;
        
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
            foreach (var o in template.Opties)
            {
                Opties.Add(new CombinatieTemplateOptieViewModel(o));
            }
        }

        #endregion // Constructor
    }
}
