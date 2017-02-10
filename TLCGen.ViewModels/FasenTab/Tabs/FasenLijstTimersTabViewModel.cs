using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 2, type: TabItemTypeEnum.FasenTab)]
    public class FasenLijstTimersTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<FaseCyclusViewModel> _Fasen;
        private FaseCyclusViewModel _SelectedFaseCyclus;
        private IList _SelectedFaseCycli = new ArrayList();

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
                OnPropertyChanged("SelectedFaseCyclus");
            }
        }

        public IList SelectedFaseCycli
        {
            get { return _SelectedFaseCycli; }
            set
            {
                _SelectedFaseCycli = value;
                _SettingMultiple = false;
                OnPropertyChanged("SelectedFaseCycli");
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Tijden";
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
#warning Consider to change this, so that the VM reacts to a message instead.

            foreach (var fcvm in Fasen)
            {
                fcvm.PropertyChanged -= FaseCyclus_PropertyChanged;
            }

            Fasen.Clear();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                var fcvm = new FaseCyclusViewModel(fcm);
                if (sel != null && fcvm.Naam == sel.Naam)
                    SelectedFaseCyclus = fcvm;
                Fasen.Add(fcvm);
                fcvm.PropertyChanged += FaseCyclus_PropertyChanged;
            }
        }

        #endregion // TabItem Overrides

        #region TLCGen Event handling

        #endregion // TLCGen Event handling

        #region Event Handling

        private bool _SettingMultiple = false;
        private void FaseCyclus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_SettingMultiple || string.IsNullOrEmpty(e.PropertyName))
                return;

            if (SelectedFaseCycli != null && SelectedFaseCycli.Count > 1)
            {
                _SettingMultiple = true;
                MultiPropertySetter.SetPropertyForAllItems<FaseCyclusViewModel>(sender, e.PropertyName, SelectedFaseCycli);
            }
            _SettingMultiple = false;
        }

        #endregion // Event Handling

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public FasenLijstTimersTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
