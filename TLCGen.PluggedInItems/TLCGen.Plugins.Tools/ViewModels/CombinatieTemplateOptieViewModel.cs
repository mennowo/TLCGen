using CommunityToolkit.Mvvm.ComponentModel;

namespace TLCGen.Plugins.Tools
{
    public class CombinatieTemplateOptieViewModel : ObservableObject
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
                OnPropertyChanged();
            }
        }

        public CombinatieTemplateOptieTypeEnum Type
        {
            get => Optie.Type;
            set
            {
                Optie.Type = value;
                OnPropertyChanged();
            }
        }

        public string Search
        {
            get => Optie.Search;
            set
            {
                Optie.Search = value;
                OnPropertyChanged();
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
                    OnPropertyChanged();
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
                OnPropertyChanged();
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
