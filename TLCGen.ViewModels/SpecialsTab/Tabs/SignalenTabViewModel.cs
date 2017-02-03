using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 3, type: TabItemTypeEnum.SpecialsTab)]
    public class SignalenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private WaarschuwingsGroepViewModel _SelectedWaarschuwingsGroep;

        private RelayCommand _AddWaarschuwingsGroepCommand;
        private RelayCommand _RemoveWaarschuwingsGroepCommand;

        #endregion // Fields

        #region Properties

        public WaarschuwingsGroepViewModel SelectedWaarschuwingsGroep
        {
            get { return _SelectedWaarschuwingsGroep; }
            set
            {
                _SelectedWaarschuwingsGroep = value;
                OnPropertyChanged("SelectedWaarschuwingsGroep");
            }
        }

        public ObservableCollectionAroundList<WaarschuwingsGroepViewModel, WaarschuwingsGroepModel> WaarschuwingsGroepen
        {
            get;
            private set;
        }

        #endregion // Properties

        #region Commands

        public ICommand AddWaarschuwingsGroepCommand
        {
            get
            {
                if (_AddWaarschuwingsGroepCommand == null)
                {
                    _AddWaarschuwingsGroepCommand = new RelayCommand(AddNewFileIngreepCommand_Executed, AddNewFileIngreepCommand_CanExecute);
                }
                return _AddWaarschuwingsGroepCommand;
            }
        }

        public ICommand RemoveWaarschuwingsGroepCommand
        {
            get
            {
                if (_RemoveWaarschuwingsGroepCommand == null)
                {
                    _RemoveWaarschuwingsGroepCommand = new RelayCommand(RemoveFileIngreepCommand_Executed, RemoveFileIngreepCommand_CanExecute);
                }
                return _RemoveWaarschuwingsGroepCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewFileIngreepCommand_Executed(object prm)
        {
            var grm = new WaarschuwingsGroepModel();
            int i = WaarschuwingsGroepen.Count + 1;
            grm.Naam = "groep" + i.ToString();
            while (!Integrity.IntegrityChecker.IsElementNaamUnique(_Controller, grm.Naam))
            {
                ++i;
                grm.Naam = "groep" + i.ToString();
            }
            WaarschuwingsGroepViewModel grvm = new WaarschuwingsGroepViewModel(grm);
            WaarschuwingsGroepen.Add(grvm);
        }

        bool AddNewFileIngreepCommand_CanExecute(object prm)
        {
            return true;
        }

        void RemoveFileIngreepCommand_Executed(object prm)
        {
            WaarschuwingsGroepen.Remove(SelectedWaarschuwingsGroep);
            SelectedWaarschuwingsGroep = null;
        }

        bool RemoveFileIngreepCommand_CanExecute(object prm)
        {
            return SelectedWaarschuwingsGroep != null;
        }

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen TabItem overrides

        public override string DisplayName
        {
            get { return "Signalen"; }
        }

        public override void OnSelected()
        {
            //_ControllerFasen = new List<string>();
            //foreach (FaseCyclusModel fcm in _Controller.Fasen)
            //{
            //    _ControllerFasen.Add(fcm.Naam);
            //}
            //_ControllerFileDetectoren = new List<string>();
            //foreach (FaseCyclusModel fcm in _Controller.Fasen)
            //{
            //    foreach (DetectorModel dm in fcm.Detectoren)
            //    {
            //        if (dm.Type == Models.Enumerations.DetectorTypeEnum.File)
            //            _ControllerFileDetectoren.Add(dm.Naam);
            //    }
            //}
            //foreach (DetectorModel dm in _Controller.Detectoren)
            //{
            //    if (dm.Type == Models.Enumerations.DetectorTypeEnum.File)
            //        _ControllerFileDetectoren.Add(dm.Naam);
            //}
            //
            //if (_SelectedWaarschuwingsGroep != null)
            //{
            //    _SelectedWaarschuwingsGroep.OnSelected(_ControllerFasen, _ControllerFileDetectoren);
            //}
        }

        #endregion // TLCGen TabItem overrides

        #region Constructor

        public SignalenTabViewModel(ControllerModel controller) : base(controller)
        {
            WaarschuwingsGroepen = new ObservableCollectionAroundList<WaarschuwingsGroepViewModel, WaarschuwingsGroepModel>(_Controller.WaarschuwingsGroepen);
        }

        #endregion // Constructor
    }
}
