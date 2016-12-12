using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 3, type: TabItemTypeEnum.FasenTab)]
    public class FasenOVTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<FaseCyclusViewModel> _Fasen;
        private FaseCyclusViewModel _SelectedFaseCyclus;
        private OVIngreepViewModel _SelectedOVIngreep;
        private HDIngreepViewModel _SelectedHDIngreep;
        private OVIngreepSGInstellingenLijstViewModel _OVIngreepSGInstellingenLijstVM;

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

        public OVIngreepSGInstellingenLijstViewModel OVIngreepSGInstellingenLijstVM
        {
            get
            {
                return _OVIngreepSGInstellingenLijstVM;
            }
        }

        public FaseCyclusViewModel SelectedFaseCyclus
        {
            get { return _SelectedFaseCyclus; }
            set
            {
                _SelectedFaseCyclus = value;
                SelectedOVIngreep = null;
                foreach(OVIngreepModel ovm in _Controller.OVData.OVIngrepen)
                {
                    if(ovm.FaseCyclus == SelectedFaseCyclus.Define)
                    {
                        SelectedOVIngreep = new OVIngreepViewModel(ovm);
                        break;
                    }
                }

                foreach (HDIngreepModel hdm in _Controller.OVData.HDIngrepen)
                {
                    if (hdm.FaseCyclus == SelectedFaseCyclus.Define)
                    {
                        SelectedHDIngreep = new HDIngreepViewModel(_Controller, hdm);
                        break;
                    }
                }

                OnPropertyChanged("SelectedFaseCyclus");
                OnMonitoredPropertyChanged("SelectedFaseCyclusOVIngreep");
                OnMonitoredPropertyChanged("SelectedFaseCyclusHDIngreep");
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
                        ov.FaseCyclus = SelectedFaseCyclus.Define;
                        _Controller.OVData.OVIngrepen.Add(ov);
                        SelectedOVIngreep = new OVIngreepViewModel(ov);
                    }
                    else
                    {
                        if (SelectedOVIngreep != null)
                        {
                            _Controller.OVData.OVIngrepen.Remove(SelectedOVIngreep.OVIngreep);
                        }
                    }
                }
                OnMonitoredPropertyChanged("SelectedFaseCyclusOVIngreep");
            }
        }

        public OVIngreepViewModel SelectedOVIngreep
        {
            get { return _SelectedOVIngreep; }
            set
            {
                _SelectedOVIngreep = value;
                OnPropertyChanged("SelectedOVIngreep");
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
                        hd.FaseCyclus = SelectedFaseCyclus.Define;
                        _Controller.OVData.HDIngrepen.Add(hd);
                        SelectedHDIngreep = new HDIngreepViewModel(_Controller, hd);
                    }
                    else
                    {
                        if (SelectedHDIngreep != null)
                        {
                            _Controller.OVData.HDIngrepen.Remove(SelectedHDIngreep.HDIngreep);
                        }
                    }
                }
                OnMonitoredPropertyChanged("SelectedFaseCyclusHDIngreep");
            }
        }

        public HDIngreepViewModel SelectedHDIngreep
        {
            get { return _SelectedHDIngreep; }
            set
            {
                _SelectedHDIngreep = value;
                OnPropertyChanged("SelectedHDIngreep");
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "OV/HD";
            }
        }

        public override bool CanBeEnabled()
        {
            return _Controller.Data.OVIngreep != Models.Enumerations.OVIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {
            Fasen.Clear();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                Fasen.Add(new FaseCyclusViewModel(fcm));
            }

            if(Fasen.Count > 0)
            {
                SelectedFaseCyclus = Fasen[0];
            }
        }

        #endregion // TabItem Overrides

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public FasenOVTabViewModel(ControllerModel controller) : base(controller)
        {
            _OVIngreepSGInstellingenLijstVM = new OVIngreepSGInstellingenLijstViewModel(controller);
        }

        #endregion // Constructor
    }
}
