using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class KruispuntArmFaseCyclusViewModel : ObservableObjectEx, IViewModelWithItem
    {
        #region Properties

        public KruispuntArmFaseCyclusModel Model { get; }

        public string FaseCyclus
        {
            get => Model.FaseCyclus;
            set
            {
                Model.FaseCyclus = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public string KruispuntArm
        {
            get => Model.KruispuntArm;
            set
            {
                Model.KruispuntArm = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public string KruispuntArmVolg
        {
            get => Model.KruispuntArmVolg;
            set
            {
                Model.KruispuntArmVolg = value;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(HasVolgArm));
                OnPropertyChanged(nameof(HasVolgArmAndUseTime));
            }
        }

        public bool HasKruispuntArmVolgTijd
        {
            get => Model.HasKruispuntArmVolgTijd;
            set
            {
                Model.HasKruispuntArmVolgTijd = value;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(HasVolgArmAndUseTime));
            }
        }

        public int KruispuntArmVolgTijd
        {
            get => Model.KruispuntArmVolgTijd;
            set
            {
                Model.KruispuntArmVolgTijd = value;
                OnPropertyChanged(broadcast: true);
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