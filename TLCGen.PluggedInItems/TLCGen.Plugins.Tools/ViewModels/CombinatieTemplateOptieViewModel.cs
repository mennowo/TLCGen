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

        public string Description
        {
            get => Optie.Description;
            set
            {
                Optie.Description = value;
                RaisePropertyChanged();
            }
        }

        public CombinatieTemplateOptieTypeEnum Type
        {
            get => Optie.Type;
            set
            {
                Optie.Type = value;
                RaisePropertyChanged();
            }
        }

        public string Search
        {
            get => Optie.Search;
            set
            {
                Optie.Search = value;
                RaisePropertyChanged();
            }
        }

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
            get
            {
                if(int.TryParse(Replace, out var i))
                {
                    return i;
                }
                return int.MinValue;
            }
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
