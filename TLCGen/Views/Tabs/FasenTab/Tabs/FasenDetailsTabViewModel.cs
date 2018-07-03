using System.Collections.ObjectModel;
using System.Linq;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1, type: TabItemTypeEnum.FasenTab)]
    public class FasenDetailsTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<FaseCyclusViewModel> _Fasen;
        private FaseCyclusViewModel _SelectedFaseCyclus;

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusViewModel> Fasen
        {
            get
            {
                if (_Fasen == null)
                {
                    _Fasen = new ObservableCollection<FaseCyclusViewModel>();
                }
                return _Fasen;
            }
        }

        public FaseCyclusViewModel SelectedFaseCyclus
        {
            get { return _SelectedFaseCyclus; }
            set
            {
                _SelectedFaseCyclus = value;
                RaisePropertyChanged("SelectedFaseCyclus");
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Details";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
            var sel = SelectedFaseCyclus;
            Fasen.Clear();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                var fcvm = new FaseCyclusViewModel(fcm);
                if (sel != null && fcvm.Naam == sel.Naam)
                {
                    sel = null;
                    SelectedFaseCyclus = fcvm;
                }
                Fasen.Add(fcvm);
            }
            if(SelectedFaseCyclus == null && Fasen.Any())
            {
                SelectedFaseCyclus = Fasen[0];
            }
        }

        #endregion // TabItem Overrides
        
        #region Constructor

        public FasenDetailsTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
