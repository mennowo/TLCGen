using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Extensions;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1, type: TabItemTypeEnum.OVTab)]
    public class OVFasenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<FaseCyclusViewModel> _Fasen;
        private FaseCyclusViewModel _SelectedFaseCyclus;
        private OVIngreepViewModel _SelectedOVIngreep;
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
            get { return _SelectedFaseCyclus; }
            set
            {
                _SelectedFaseCyclus = value;
                SelectedOVIngreep = null;
                SelectedHDIngreep = null;
                if (_SelectedFaseCyclus != null)
                {
                    foreach (OVIngreepModel ovm in _Controller.OVData.OVIngrepen)
                    {
                        if (ovm.FaseCyclus == SelectedFaseCyclus.Naam)
                        {
                            SelectedOVIngreep = new OVIngreepViewModel(ovm);
                            break;
                        }
                    }

                    foreach (HDIngreepModel hdm in _Controller.OVData.HDIngrepen)
                    {
                        if (hdm.FaseCyclus == SelectedFaseCyclus.Naam)
                        {
                            SelectedHDIngreep = new HDIngreepViewModel(_Controller, hdm);
                            break;
                        }
                    }
                }

                RaisePropertyChanged(null);
            }
        }

        public bool SelectedFaseCyclusOVIngreep
        {
            get
            {
                if (SelectedFaseCyclus == null)
                {
                    return false;
                }
                else
                {
                    return SelectedFaseCyclus.OVIngreep;
                }
            }
            set
            {
                if(SelectedFaseCyclus != null)
                {
                    SelectedFaseCyclus.OVIngreep = value;
                    if (value)
                    {
                        OVIngreepModel ov = new OVIngreepModel();
                        Settings.DefaultsProvider.Default.SetDefaultsOnModel(ov);
                        ov.FaseCyclus = SelectedFaseCyclus.Naam;
                        _Controller.OVData.OVIngrepen.Add(ov);
                        _Controller.OVData.OVIngrepen.BubbleSort();
                        SelectedOVIngreep = new OVIngreepViewModel(ov);
                        /* Trick to add dummy detectors */
                        if (ov.KAR)
                        {
                            SelectedOVIngreep.KAR = true;
                        }
                        if(ov.Vecom)
                        {
                            SelectedOVIngreep.Vecom = true;
                        }
                    }
                    else
                    {
                        if (SelectedOVIngreep != null)
                        {
                            _Controller.OVData.OVIngrepen.Remove(SelectedOVIngreep.OVIngreep);
                            SelectedOVIngreep = null;
                        }
                    }
                    MessengerInstance.Send(new OVIngrepenChangedMessage());
                }
                RaisePropertyChanged<object>("SelectedFaseCyclusOVIngreep", null, null, true);
            }
        }

        public OVIngreepViewModel SelectedOVIngreep
        {
            get { return _SelectedOVIngreep; }
            set
            {
                _SelectedOVIngreep = value;
                RaisePropertyChanged("SelectedOVIngreep");
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
                        HDIngreepModel hd = new HDIngreepModel();
                        Settings.DefaultsProvider.Default.SetDefaultsOnModel(hd);
                        hd.FaseCyclus = SelectedFaseCyclus.Naam;
                        _Controller.OVData.HDIngrepen.Add(hd);
                        _Controller.OVData.HDIngrepen.BubbleSort();
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
                            _Controller.OVData.HDIngrepen.Remove(SelectedHDIngreep.HDIngreep);
                            SelectedHDIngreep = null;
                        }
                    }
                    MessengerInstance.Send(new OVIngrepenChangedMessage());
                    Integrity.TLCGenControllerModifier.Default.CorrectModel_AlteredHDIngrepen();
                }
                RaisePropertyChanged<object>("SelectedFaseCyclusHDIngreep", null, null, true);
            }
        }

        public HDIngreepViewModel SelectedHDIngreep
        {
            get { return _SelectedHDIngreep; }
            set
            {
                _SelectedHDIngreep = value;
                RaisePropertyChanged("SelectedHDIngreep");
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Ingrepen";
            }
        }

        public override bool CanBeEnabled()
        {
            return _Controller?.OVData?.OVIngreepType != Models.Enumerations.OVIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {
            var temp = SelectedFaseCyclus;
            Fasen.Clear();
            SelectedFaseCyclus = null;
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                var fcvm = new FaseCyclusViewModel(fcm);
                Fasen.Add(fcvm);
                if(temp != null && fcvm.Naam == temp.Naam)
                {
                    SelectedFaseCyclus = fcvm;
                    temp = null;
                }
            }
            if(SelectedFaseCyclus == null && Fasen.Count > 0)
            {
                SelectedFaseCyclus = Fasen[0];
            }
        }

        public override ControllerModel Controller
        {
            get
            {
                return base.Controller;
            }

            set
            {
                base.Controller = value;
            }
        }

        #endregion // TabItem Overrides

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public OVFasenTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
