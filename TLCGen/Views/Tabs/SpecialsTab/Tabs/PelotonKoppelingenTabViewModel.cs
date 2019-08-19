using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 6, type: TabItemTypeEnum.SpecialsTab)]
    public class PelotonKoppelingenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        RelayCommand _addPelotonKoppelingCommand;
        RelayCommand _removePelotonKoppelingCommand;
        RelayCommand _moveUpPelotonKoppelingCommand;
        RelayCommand _moveDownPelotonKoppelingCommand;
        private PelotonKoppelingViewModel _selectedPelotonKoppeling;
        private ObservableCollection<string> _interneKoppelingenUit;

        #endregion // Fields

        #region Properties

        public ObservableCollectionAroundList<PelotonKoppelingViewModel, PelotonKoppelingModel> PelotonKoppelingen
        {
            get; private set;
        }

        public ObservableCollection<string> InterneKoppelingenUit
        {
            get
            {
                if (_interneKoppelingenUit == null)
                {
                    _interneKoppelingenUit = new ObservableCollection<string>();
                }
                return _interneKoppelingenUit;
            }
        }

        public ObservableCollection<string> ControllerFasen { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> ControllerDetectoren { get; } = new ObservableCollection<string>();

        public PelotonKoppelingViewModel SelectedPelotonKoppeling
        {
            get { return _selectedPelotonKoppeling; }
            set
            {
                _selectedPelotonKoppeling = value;
                value?.UitgaandeDetectorenManager.UpdateSelectables(ControllerDetectoren);
                InterneKoppelingenUit.Clear();
                foreach (var k in PelotonKoppelingen.Where(x => x.IsInternUit)) InterneKoppelingenUit.Add(k.KoppelingNaam);
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<string> PTPKruisingenNames { get; } = new ObservableCollection<string>();

        #endregion // Properties

        #region Commands

        public ICommand AddPelotonKoppelingCommand
        {
            get
            {
                if (_addPelotonKoppelingCommand == null)
                {
                    _addPelotonKoppelingCommand = new RelayCommand(AddPelotonKoppelingCommand_Executed, AddPelotonKoppelingCommand_CanExecute);
                }
                return _addPelotonKoppelingCommand;
            }
        }

        public ICommand RemovePelotonKoppelingCommand
        {
            get
            {
                if (_removePelotonKoppelingCommand == null)
                {
                    _removePelotonKoppelingCommand = new RelayCommand(RemovePelotonKoppelingCommand_Executed, RemovePelotonKoppelingCommand_CanExecute);
                }
                return _removePelotonKoppelingCommand;
            }
        }

        public ICommand MoveUpPelotonKoppelingCommand
        {
            get
            {
                if (_moveUpPelotonKoppelingCommand == null)
                {
                    _moveUpPelotonKoppelingCommand = new RelayCommand(MoveUpPelotonKoppelingCommand_Executed, MoveUpPelotonKoppelingCommand_CanExecute);
                }
                return _moveUpPelotonKoppelingCommand;
            }
        }

        public ICommand MoveDownPelotonKoppelingCommand
        {
            get
            {
                if (_moveDownPelotonKoppelingCommand == null)
                {
                    _moveDownPelotonKoppelingCommand = new RelayCommand(MoveDownPelotonKoppelingCommand_Executed, MoveDownPelotonKoppelingCommand_CanExecute);
                }
                return _moveDownPelotonKoppelingCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private bool AddPelotonKoppelingCommand_CanExecute(object obj)
        {
            return true;
        }

        private void AddPelotonKoppelingCommand_Executed(object obj)
        {
            var Peloton = new PelotonKoppelingModel();
            if (ControllerFasen.Any()) Peloton.GekoppeldeSignaalGroep = ControllerFasen.First();
            if (PTPKruisingenNames.Any())
            {
                Peloton.PTPKruising = PTPKruisingenNames.First();
                Peloton.KoppelingNaam = PTPKruisingenNames.First();
            }
            else
            {
                Peloton.KoppelingNaam = "KOP1";
            }
            var vm = new PelotonKoppelingViewModel(Peloton);
            PelotonKoppelingen.Add(vm);
            SelectedPelotonKoppeling = vm;
        }

        private bool RemovePelotonKoppelingCommand_CanExecute(object obj)
        {
            return SelectedPelotonKoppeling != null;
        }

        private void RemovePelotonKoppelingCommand_Executed(object obj)
        {
            PelotonKoppelingen.Remove(SelectedPelotonKoppeling);
            SelectedPelotonKoppeling = PelotonKoppelingen.Any() ? PelotonKoppelingen[0] : null;
        }

        private bool MoveUpPelotonKoppelingCommand_CanExecute(object obj)
        {
            return SelectedPelotonKoppeling != null && PelotonKoppelingen != null && PelotonKoppelingen.IndexOf(SelectedPelotonKoppeling) > 0;
        }

        private void MoveUpPelotonKoppelingCommand_Executed(object obj)
        {
            PelotonKoppelingen.Move(PelotonKoppelingen.IndexOf(SelectedPelotonKoppeling), PelotonKoppelingen.IndexOf(SelectedPelotonKoppeling) - 1);
            PelotonKoppelingen.RebuildList();
        }

        private bool MoveDownPelotonKoppelingCommand_CanExecute(object obj)
        {
            return SelectedPelotonKoppeling != null && PelotonKoppelingen != null && PelotonKoppelingen.IndexOf(SelectedPelotonKoppeling) < (PelotonKoppelingen.Count - 1);
        }

        private void MoveDownPelotonKoppelingCommand_Executed(object obj)
        {
            PelotonKoppelingen.Move(PelotonKoppelingen.IndexOf(SelectedPelotonKoppeling), PelotonKoppelingen.IndexOf(SelectedPelotonKoppeling) + 1);
            PelotonKoppelingen.RebuildList();
        }

        #endregion // Command Functionality

        #region Public methods

        #endregion // Public methods

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Peloton\nKoppeling";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
            ControllerFasen.Clear();
            ControllerDetectoren.Clear();
            foreach (var fc in _Controller.Fasen) ControllerFasen.Add(fc.Naam);
            foreach (var d in _Controller.GetAllDetectors(x => !x.Dummy)) ControllerDetectoren.Add(d.Naam);

            if (SelectedPelotonKoppeling == null && PelotonKoppelingen.Any()) SelectedPelotonKoppeling = PelotonKoppelingen[0];
            else
            {
                SelectedPelotonKoppeling?.UitgaandeDetectorenManager.UpdateSelectables(ControllerDetectoren);
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

                    PelotonKoppelingen = new ObservableCollectionAroundList<PelotonKoppelingViewModel, PelotonKoppelingModel>(Controller.PelotonKoppelingenData.PelotonKoppelingen);
                    PTPKruisingenNames.Clear();
                    foreach (var kr in Controller.PTPData.PTPKoppelingen)
                    {
                        PTPKruisingenNames.Add(kr.TeKoppelenKruispunt);
                    }
                }
                else
                {
                    PelotonKoppelingen = null;
                }
            }
        }

        #endregion // TabItem Overrides

        #region Collection Changed

        private void PelotonKoppelingen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region TLCGen Events

        private void OnPTPKoppelingenChanged(PTPKoppelingenChangedMessage msg)
        {
            var rems = new List<string>();
            foreach (var ptpk in PTPKruisingenNames)
            {
                if (Controller.PTPData.PTPKoppelingen.All(x => x.TeKoppelenKruispunt != ptpk))
                {
                    rems.Add(ptpk);
                }
            }

            foreach (var r in rems)
            {
                PTPKruisingenNames.Remove(r);
                foreach (var k in PelotonKoppelingen)
                {
                    if (k.PTPKruising == r)
                    {
                        k.PTPKruising = "onbekend";
                    }
                }
            }

            foreach (var ptpkp in Controller.PTPData.PTPKoppelingen)
            {
                if (PTPKruisingenNames.All(x => x != ptpkp.TeKoppelenKruispunt))
                {
                    PTPKruisingenNames.Add(ptpkp.TeKoppelenKruispunt);
                }
            }
        }

        private void OnNameChanged(NameChangedMessage msg)
        {

            if (PTPKruisingenNames.Contains(msg.OldName))
            {
                PTPKruisingenNames.Add(msg.NewName);
                foreach (var k in PelotonKoppelingen)
                {
                    if (k.PTPKruising == msg.OldName)
                    {
                        k.PTPKruising = msg.NewName;
                    }
                }
                PTPKruisingenNames.Remove(msg.OldName);
            }
        }

        #endregion // TLCGen Events

        #region Constructor

        public PelotonKoppelingenTabViewModel() : base()
        {
            MessengerInstance.Register<PTPKoppelingenChangedMessage>(this, OnPTPKoppelingenChanged);
            MessengerInstance.Register<NameChangedMessage>(this, OnNameChanged);
        }

        #endregion // Constructor
    }
}
