using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers.Settings;
using TLCGen.Models;
using TLCGen.Models.Settings;

namespace TLCGen.ViewModels
{
    public class ControllerDataViewModel : ViewModelBase
    {
        #region Fields

        private ControllerViewModel _ControllerVM;
        private ControllerDataModel _ControllerData;

        #endregion // Fields

        #region Properties

        public string Naam
        {
            get { return _ControllerData.Naam; }
            set
            {
                _ControllerData.Naam = value;
                OnMonitoredPropertyChanged("Naam", _ControllerVM);
            }
        }

        public string Stad
        {
            get { return _ControllerData.Stad; }
            set
            {
                _ControllerData.Stad = value;
                OnMonitoredPropertyChanged("Stad", _ControllerVM);
            }
        }

        public string Straat1
        {
            get { return _ControllerData.Straat1; }
            set
            {
                _ControllerData.Straat1 = value;
                OnMonitoredPropertyChanged("Straat1", _ControllerVM);
            }
        }

        public string Straat2
        {
            get { return _ControllerData.Straat2; }
            set
            {
                _ControllerData.Straat2 = value;
                OnMonitoredPropertyChanged("Straat2", _ControllerVM);
            }
        }

        public DefinePrefixSettings PrefixSettings
        {
            get { return _ControllerData.Instellingen.PrefixSettings; }
        }

        private ObservableCollection<SettingModelBase> _PrefixSettingsList;
        public ObservableCollection<SettingModelBase> PrefixSettingsList
        {
            get
            {
                if(_PrefixSettingsList == null)
                {
                    _PrefixSettingsList = new ObservableCollection<SettingModelBase>();
                }
                return _PrefixSettingsList;
            }
        }

        #endregion // Properties

        #region Public methods

        #endregion // Public methods

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public ControllerDataViewModel(ControllerViewModel controllervm, ControllerDataModel controllerdata)
        {
            _ControllerVM = controllervm;
            _ControllerData = controllerdata;

            foreach(SettingModelBase setting in SettingsProvider.AppSettings.PrefixSettingsList)
            {
                PrefixSettingsList.Add(setting);
            }
        }

        #endregion // Constructor
    }
}
