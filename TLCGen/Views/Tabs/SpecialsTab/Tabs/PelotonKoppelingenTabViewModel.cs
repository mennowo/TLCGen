using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private PelotonKoppelingViewModel _selectedPelotonKoppeling;
        private ObservableCollection<PelotonKoppelingViewModel> _pelotonKoppelingen;
        private ObservableCollection<string> _controllerFasen;
        private string _selectedControllerDetector;

        #endregion // Fields

        #region Properties

        public ObservableCollection<PelotonKoppelingViewModel> PelotonKoppelingen
        {
            get
            {
                if (_pelotonKoppelingen == null)
                    _pelotonKoppelingen = new ObservableCollection<PelotonKoppelingViewModel>();
                return _pelotonKoppelingen;
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
                Peloton.KruisingNaam = PTPKruisingenNames.First();
            }
            else
            {
                Peloton.KruisingNaam = "KOP1";
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

                    PelotonKoppelingen.CollectionChanged -= PelotonKoppelingen_CollectionChanged;
                    PelotonKoppelingen.Clear();
                    foreach (PelotonKoppelingModel Peloton in _Controller.PelotonKoppelingenData.PelotonKoppelingen)
                    {
                        PelotonKoppelingen.Add(new PelotonKoppelingViewModel(Peloton));
                    }
                    PelotonKoppelingen.CollectionChanged += PelotonKoppelingen_CollectionChanged;
                    PTPKruisingenNames.Clear();
                    foreach (var kr in Controller.PTPData.PTPKoppelingen)
                    {
                        PTPKruisingenNames.Add(kr.TeKoppelenKruispunt);
                    }
                }
                else
                {
                    PelotonKoppelingen.CollectionChanged -= PelotonKoppelingen_CollectionChanged;
                    PelotonKoppelingen.Clear();
                }
            }
        }

        #endregion // TabItem Overrides

        #region Collection Changed

        private void PelotonKoppelingen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (PelotonKoppelingViewModel Peloton in e.NewItems)
                {
                    _Controller.PelotonKoppelingenData.PelotonKoppelingen.Add(Peloton.PelotonKoppeling);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (PelotonKoppelingViewModel Peloton in e.OldItems)
                {
                    _Controller.PelotonKoppelingenData.PelotonKoppelingen.Remove(Peloton.PelotonKoppeling);
                }
            };
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
