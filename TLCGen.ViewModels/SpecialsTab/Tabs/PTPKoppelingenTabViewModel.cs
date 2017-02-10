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

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 0, type: TabItemTypeEnum.SpecialsTab)]
    public class PTPKoppelingenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private PTPKoppelingViewModel _SelectedPTPKoppeling;
        private ObservableCollection<PTPKoppelingViewModel> _PTPKoppelingen;

        #endregion // Fields

        #region Properties

        public string KruisingPTPNaam
        {
            get { return _Controller.PTPData.KruisingPTPNaam; }
            set
            {
                _Controller.PTPData.KruisingPTPNaam = value;
                OnMonitoredPropertyChanged("KruisingPTPNaam");
            }
        }

        public ObservableCollection<PTPKoppelingViewModel> PTPKoppelingen
        {
            get
            {
                if (_PTPKoppelingen == null)
                    _PTPKoppelingen = new ObservableCollection<PTPKoppelingViewModel>();
                return _PTPKoppelingen;
            }
        }

        public PTPKoppelingViewModel SelectedPTPKoppeling
        {
            get { return _SelectedPTPKoppeling; }
            set
            {
                _SelectedPTPKoppeling = value;
                OnPropertyChanged("SelectedPTPKoppeling");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddPTPKoppelingCommand;
        public ICommand AddPTPKoppelingCommand
        {
            get
            {
                if (_AddPTPKoppelingCommand == null)
                {
                    _AddPTPKoppelingCommand = new RelayCommand(AddPTPKoppelingCommand_Executed, AddPTPKoppelingCommand_CanExecute);
                }
                return _AddPTPKoppelingCommand;
            }
        }

        RelayCommand _RemovePTPKoppelingCommand;
        public ICommand RemovePTPKoppelingCommand
        {
            get
            {
                if (_RemovePTPKoppelingCommand == null)
                {
                    _RemovePTPKoppelingCommand = new RelayCommand(RemovePTPKoppelingCommand_Executed, RemovePTPKoppelingCommand_CanExecute);
                }
                return _RemovePTPKoppelingCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private bool AddPTPKoppelingCommand_CanExecute(object obj)
        {
            return true;
        }

        private void AddPTPKoppelingCommand_Executed(object obj)
        {
            PTPKoppelingModel ptp = new PTPKoppelingModel();
            PTPKoppelingen.Add(new PTPKoppelingViewModel(ptp));
        }

        private bool RemovePTPKoppelingCommand_CanExecute(object obj)
        {
            return SelectedPTPKoppeling != null;
        }

        private void RemovePTPKoppelingCommand_Executed(object obj)
        {
            PTPKoppelingen.Remove(SelectedPTPKoppeling);
            SelectedPTPKoppeling = null;
        }

        #endregion // Command Functionality

        #region Public methods

        #endregion // Public methods

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "PTP";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
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
                PTPKoppelingen.CollectionChanged -= PTPKoppelingen_CollectionChanged;
                PTPKoppelingen.Clear();
                foreach (PTPKoppelingModel ptp in _Controller.PTPData.PTPKoppelingen)
                {
                    PTPKoppelingen.Add(new PTPKoppelingViewModel(ptp));
                }
                PTPKoppelingen.CollectionChanged += PTPKoppelingen_CollectionChanged;
            }
        }

        #endregion // TabItem Overrides

        #region Collection Changed

        private void PTPKoppelingen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (PTPKoppelingViewModel ptp in e.NewItems)
                {
                    _Controller.PTPData.PTPKoppelingen.Add(ptp.PTPKoppeling);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (PTPKoppelingViewModel ptp in e.OldItems)
                {
                    _Controller.PTPData.PTPKoppelingen.Remove(ptp.PTPKoppeling);
                }
            };
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed
        
        #region Constructor

        public PTPKoppelingenTabViewModel() : base()
        {

        }

        #endregion // Constructor
    }
}
