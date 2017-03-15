using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1, type: TabItemTypeEnum.SpecialsTab)]
    public class FileTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private FileIngreepViewModel _SelectedFileIngreep;

        private List<string> _ControllerFasen;
        private List<string> _ControllerFileDetectoren;

        private RelayCommand _AddFileIngreepCommand;
        private RelayCommand _RemoveFileIngreepCommand;

        #endregion // Fields

        #region Properties

        public FileIngreepViewModel SelectedFileIngreep
        {
            get { return _SelectedFileIngreep; }
            set
            {
                _SelectedFileIngreep = value;
                if (_SelectedFileIngreep != null)
                {
                    _SelectedFileIngreep.OnSelected(_ControllerFasen, _ControllerFileDetectoren);
                }
                RaisePropertyChanged("SelectedFileIngreep");
            }
        }

        public ObservableCollectionAroundList<FileIngreepViewModel, FileIngreepModel> FileIngrepen
        {
            get;
            private set;
        }

        #endregion // Properties

        #region Commands

        public ICommand AddFileIngreepCommand
        {
            get
            {
                if (_AddFileIngreepCommand == null)
                {
                    _AddFileIngreepCommand = new RelayCommand(AddNewFileIngreepCommand_Executed, AddNewFileIngreepCommand_CanExecute);
                }
                return _AddFileIngreepCommand;
            }
        }

        public ICommand RemoveFileIngreepCommand
        {
            get
            {
                if (_RemoveFileIngreepCommand == null)
                {
                    _RemoveFileIngreepCommand = new RelayCommand(RemoveFileIngreepCommand_Executed, RemoveFileIngreepCommand_CanExecute);
                }
                return _RemoveFileIngreepCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewFileIngreepCommand_Executed(object prm)
        {
            FileIngreepModel fim = new FileIngreepModel();
            int i = FileIngrepen.Count + 1;
            fim.Naam = "File" + i.ToString();
            while(!Integrity.TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, fim.Naam))
            {
                ++i;
                fim.Naam = "File" + i.ToString();
            }
            FileIngreepViewModel fivm = new FileIngreepViewModel(fim);
            FileIngrepen.Add(fivm);

            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        bool AddNewFileIngreepCommand_CanExecute(object prm)
        {
            return true;
        }

        void RemoveFileIngreepCommand_Executed(object prm)
        {
            FileIngrepen.Remove(SelectedFileIngreep);
            SelectedFileIngreep = null;
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        bool RemoveFileIngreepCommand_CanExecute(object prm)
        {
            return SelectedFileIngreep != null;
        }

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen TabItem overrides

        public override string DisplayName
        {
            get { return "File"; }
        }

        public override void OnSelected()
        {
            _ControllerFasen = new List<string>();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                _ControllerFasen.Add(fcm.Naam);
            }
            _ControllerFileDetectoren = new List<string>();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                foreach(DetectorModel dm in fcm.Detectoren)
                {
                    if(dm.Type == Models.Enumerations.DetectorTypeEnum.File)
                        _ControllerFileDetectoren.Add(dm.Naam);
                }
            }
            foreach (DetectorModel dm in _Controller.Detectoren)
            {
                if(dm.Type == Models.Enumerations.DetectorTypeEnum.File)
                    _ControllerFileDetectoren.Add(dm.Naam);
            }

            if(_SelectedFileIngreep != null)
            {
                _SelectedFileIngreep.OnSelected(_ControllerFasen, _ControllerFileDetectoren);
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
                if (base.Controller != null)
                {
                    FileIngrepen = new ObservableCollectionAroundList<FileIngreepViewModel, FileIngreepModel>(_Controller.FileIngrepen);
                }
                else
                {
                    FileIngrepen = null;
                }
                RaisePropertyChanged("FileIngrepen");
            }
        }

        #endregion // TLCGen TabItem overrides

        #region TLCGen Events

        private void OnFasenChanged(FasenChangedMessage message)
        {
            FileIngrepen.Rebuild();
        }

        private void OnDetectorenChanged(DetectorenChangedMessage message)
        {
            FileIngrepen.Rebuild();
        }

        #endregion // TLCGen Events

        #region Constructor

        public FileTabViewModel() : base()
        {
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<DetectorenChangedMessage>(OnDetectorenChanged));
        }

        #endregion // Constructor
    }
}
