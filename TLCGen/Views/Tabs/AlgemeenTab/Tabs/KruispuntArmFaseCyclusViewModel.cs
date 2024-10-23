using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class KruispuntArmFaseCyclusViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Properties

        public KruispuntArmFaseCyclusModel Model { get; }

        public string FaseCyclus
        {
            get => Model.FaseCyclus;
            set
            {
                Model.FaseCyclus = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string KruispuntArm
        {
            get => Model.KruispuntArm;
            set
            {
                Model.KruispuntArm = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string KruispuntArmVolg
        {
            get => Model.KruispuntArmVolg;
            set
            {
                Model.KruispuntArmVolg = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasVolgArm));
                RaisePropertyChanged(nameof(HasVolgArmAndUseTime));
            }
        }

        public bool HasKruispuntArmVolgTijd
        {
            get => Model.HasKruispuntArmVolgTijd;
            set
            {
                Model.HasKruispuntArmVolgTijd = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasVolgArmAndUseTime));
            }
        }

        public int KruispuntArmVolgTijd
        {
            get => Model.KruispuntArmVolgTijd;
            set
            {
                Model.KruispuntArmVolgTijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool HasVolgArm => KruispuntArmVolg != null && KruispuntArmVolg != "NG";
        public bool HasVolgArmAndUseTime => HasVolgArm && HasKruispuntArmVolgTijd;

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return Model;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public KruispuntArmFaseCyclusViewModel(KruispuntArmFaseCyclusModel model)
        {
            Model = model;
        }

        #endregion // Constructor
    }
}