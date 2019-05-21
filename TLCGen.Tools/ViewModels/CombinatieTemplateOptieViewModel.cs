using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace TLCGen.Plugins.Tools
{
    public class CombinatieTemplateOptieViewModel : ViewModelBase
    {
        #region Fields

        private ObservableCollection<string> _fasen;
        
        #endregion // Fields
        
        #region Properties

        public CombinatieTemplateOptieModel Optie { get; }

        public string Description => Optie.Description;

        public CombinatieTemplateOptieTypeEnum Type => Optie.Type;

        public string Replace
        {
            get => Optie.Replace;
            set
            {
                if (value != null)
                {
                    Optie.Replace = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int ReplaceAsInt
        {
            get => int.Parse(Replace);
            set
            {
                Replace = value.ToString();
                RaisePropertyChanged();
            }
        }

        #endregion // Properties

        #region Public Methods

        public void SetReplaceToNull()
        {
            Replace = null;
        }

        #endregion // Public Methods

        #region Constructor

        public CombinatieTemplateOptieViewModel(CombinatieTemplateOptieModel optie)
        {
            Optie = optie;
        }

        #endregion // Constructor
    }
}
