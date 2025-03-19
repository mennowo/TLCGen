using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class HardMeeverlengenFaseCyclusViewModel : ObservableObjectEx, IViewModelWithItem
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public HardMeeverlengenFaseCyclusModel HardMeeverlengenFaseCyclus { get; }

        public string FaseCyclus => HardMeeverlengenFaseCyclus.FaseCyclus;

        public HardMeevelengenTypeEnum Type
        {
            get => HardMeeverlengenFaseCyclus.Type;
            set
            {
                HardMeeverlengenFaseCyclus.Type = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool AanUit
        {
            get => HardMeeverlengenFaseCyclus.AanUit;
            set
            {
                HardMeeverlengenFaseCyclus.AanUit = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return HardMeeverlengenFaseCyclus;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public HardMeeverlengenFaseCyclusViewModel(HardMeeverlengenFaseCyclusModel hardMeeverlengenFaseCyclus)
        {
            HardMeeverlengenFaseCyclus = hardMeeverlengenFaseCyclus;
        }

        #endregion // Constructor

    }
}
