using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class OVIngreepRitCategorieViewModel : ObservableObjectEx, IViewModelWithItem
    {
        #region Fields

        private OVIngreepRitCategorieModel _RitCategorie;

        #endregion // Fields

        #region Properties

        public OVIngreepRitCategorieModel RitCategorie
        {
            get => _RitCategorie;
            set
            {
                _RitCategorie = value;
                OnPropertyChanged("_RitCategorie", broadcast: true);
            }
        }

        public string Nummer
        {
            get => _RitCategorie.Nummer;
            set
            {
                _RitCategorie.Nummer = value;
                OnPropertyChanged(nameof(Nummer), broadcast: true);
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return _RitCategorie;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public OVIngreepRitCategorieViewModel(OVIngreepRitCategorieModel model)
        {
            _RitCategorie = model;
        }

        #endregion // Constructor
    }
}
