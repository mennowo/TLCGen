using GalaSoft.MvvmLight;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class OVIngreepRitCategorieViewModel : ViewModelBase
    {
        #region Fields

        private OVIngreepRitCategorieModel _RitCategorie;

        #endregion // Fields

        #region Properties

        public OVIngreepRitCategorieModel RitCategorie
        {
            get { return _RitCategorie; }
            set
            {
                _RitCategorie = value;
                RaisePropertyChanged<object>("_RitCategorie", broadcast: true);
            }
        }

        public string Nummer
        {
            get { return _RitCategorie.Nummer; }
            set
            {
                _RitCategorie.Nummer = value;
                RaisePropertyChanged<object>("Nummer", broadcast: true);
            }
        }

        #endregion // Properties

        #region Constructor

        public OVIngreepRitCategorieViewModel(OVIngreepRitCategorieModel model)
        {
            _RitCategorie = model;
        }

        #endregion // Constructor
    }
}
