using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Input;

namespace TLCGen.Plugins.Tools
{
    public class CombinatieTemplateOptieViewModel : ViewModelBase
    {
        #region Fields

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
