using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 3, type: TabItemTypeEnum.PrioriteitTab)]
    public class HDIngrepenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<FaseCyclusViewModel> _Fasen;
        private FaseCyclusViewModel _SelectedFaseCyclus;
        private HDIngreepViewModel _SelectedHDIngreep;

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
            get => _SelectedFaseCyclus;
            set
            {
                _SelectedFaseCyclus = value;
                SelectedHDIngreep = null;
                if (_SelectedFaseCyclus != null)
                {
                    foreach (var hdm in _Controller.PrioData.HDIngrepen)
                    {
                        if (hdm.FaseCyclus == SelectedFaseCyclus.Naam)
                        {
                            SelectedHDIngreep = new HDIngreepViewModel(_Controller, hdm);
                            break;
                        }
                    }
                }

                OnPropertyChanged("");
            }
        }

        public bool SelectedFaseCyclusHDIngreep
        {
            get
            {
                if (SelectedFaseCyclus == null)
                {
                    return false;
                }
                else
                {
                    return SelectedFaseCyclus.HDIngreep;
                }
            }
            set
            {
                if (SelectedFaseCyclus != null)
                {
                    SelectedFaseCyclus.HDIngreep = value;
                    if (value)
                    {
                        var hd = new HDIngreepModel();
                        Settings.DefaultsProvider.Default.SetDefaultsOnModel(hd);
                        hd.FaseCyclus = SelectedFaseCyclus.Naam;
                        _Controller.PrioData.HDIngrepen.Add(hd);
                        _Controller.PrioData.HDIngrepen.BubbleSort();
                        SelectedHDIngreep = new HDIngreepViewModel(_Controller, hd);
                        /* Trick to add dummy detectors */
                        if (hd.KAR)
                        {
                            SelectedHDIngreep.KAR = true;
                        }
                    }
                    else
                    {
                        if (SelectedHDIngreep != null)
                        {
                            _Controller.PrioData.HDIngrepen.Remove(SelectedHDIngreep.HDIngreep);
                            SelectedHDIngreep = null;
                        }
                    }
                    WeakReferenceMessengerEx.Default.Send(new PrioIngrepenChangedMessage());
                    Integrity.TLCGenControllerModifier.Default.CorrectModel_AlteredHDIngrepen();
                }
                // TODO Check OK ?  OnPropertyChanged(nameof(SelectedFaseCyclusHDIngreep), null, null, true);
                OnPropertyChanged(broadcast: true);
            }
        }

        public HDIngreepViewModel SelectedHDIngreep
        {
            get => _SelectedHDIngreep;
            set
            {
                _SelectedHDIngreep = value;
                OnPropertyChanged();
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName => "HD ingrepen";

        public override bool CanBeEnabled()
        {
            return _Controller?.PrioData?.PrioIngreepType != Models.Enumerations.PrioIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {
            var temp = SelectedFaseCyclus;
            Fasen.Clear();
            SelectedFaseCyclus = null;
            foreach (var fcm in _Controller.Fasen)
            {
                var fcvm = new FaseCyclusViewModel(fcm);
                Fasen.Add(fcvm);
	            if (temp == null || fcvm.Naam != temp.Naam) continue;
	            SelectedFaseCyclus = fcvm;
	            temp = null;
            }
            if(SelectedFaseCyclus == null && Fasen.Count > 0)
            {
                SelectedFaseCyclus = Fasen[0];
            }
        }

        #endregion // TabItem Overrides

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public HDIngrepenTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
