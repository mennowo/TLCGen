using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
            get => _selectedPelotonKoppeling;
            set
            {
                _selectedPelotonKoppeling = value;
                value?.UitgaandeDetectorenManager.UpdateSelectables(ControllerDetectoren);
                InterneKoppelingenUit.Clear();
                if(PelotonKoppelingen != null)
                {
                    foreach (var k in PelotonKoppelingen.Where(x => x.IsInternUit)) InterneKoppelingenUit.Add(k.KoppelingNaam);
                }
                OnPropertyChanged(); 
                _removePelotonKoppelingCommand?.NotifyCanExecuteChanged();
                _moveUpPelotonKoppelingCommand?.NotifyCanExecuteChanged();
                _moveDownPelotonKoppelingCommand?.NotifyCanExecuteChanged();
            }
        }

        public ObservableCollection<string> PTPKruisingenNames { get; } = new ObservableCollection<string>();

        #endregion // Properties

        #region Commands

        public ICommand AddPelotonKoppelingCommand => _addPelotonKoppelingCommand ??= new RelayCommand(() =>
        {
            var Peloton = new PelotonKoppelingModel();
            if (ControllerFasen.Any()) Peloton.GekoppeldeSignaalGroep = ControllerFasen.First();
            if (PTPKruisingenNames.Any()) Peloton.PTPKruising = PTPKruisingenNames.First();
            Peloton.KoppelingNaam = "KOP1";
            var i = 1;
            while(!TLCGen.Integrity.TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, Peloton.KoppelingNaam, Models.Enumerations.TLCGenObjectTypeEnum.PelotonKoppeling))
            {
                ++i;
                Peloton.KoppelingNaam = "KOP" + i;
            }
            var vm = new PelotonKoppelingViewModel(Peloton);
            PelotonKoppelingen.Add(vm);
            SelectedPelotonKoppeling = vm;
        });

        public ICommand RemovePelotonKoppelingCommand => _removePelotonKoppelingCommand ??= new RelayCommand(() =>
        {
            PelotonKoppelingen.Remove(SelectedPelotonKoppeling);
            SelectedPelotonKoppeling = PelotonKoppelingen.Any() ? PelotonKoppelingen[0] : null;
        }, () => SelectedPelotonKoppeling != null);

        public ICommand MoveUpPelotonKoppelingCommand => _moveUpPelotonKoppelingCommand ??= new RelayCommand(() =>
        {
            PelotonKoppelingen.Move(PelotonKoppelingen.IndexOf(SelectedPelotonKoppeling), PelotonKoppelingen.IndexOf(SelectedPelotonKoppeling) - 1);
            PelotonKoppelingen.RebuildList();
        }, () => SelectedPelotonKoppeling != null && PelotonKoppelingen != null && PelotonKoppelingen.IndexOf(SelectedPelotonKoppeling) > 0);

        public ICommand MoveDownPelotonKoppelingCommand => _moveDownPelotonKoppelingCommand ??= new RelayCommand(() =>
            {
                PelotonKoppelingen.Move(PelotonKoppelingen.IndexOf(SelectedPelotonKoppeling), PelotonKoppelingen.IndexOf(SelectedPelotonKoppeling) + 1);
                PelotonKoppelingen.RebuildList();
            },
                () => SelectedPelotonKoppeling != null && PelotonKoppelingen != null && PelotonKoppelingen.IndexOf(SelectedPelotonKoppeling) < (PelotonKoppelingen.Count - 1));

        #endregion // Commands

        #region Public methods

        #endregion // Public methods

        #region TabItem Overrides

        public override string DisplayName => "Peloton\nKoppeling";

        public override bool IsEnabled
        {
            get => true;
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
            get => base.Controller;

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
                    PTPKruisingenNames.Add("INTERN");
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
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region TLCGen Events

        private void OnPTPKoppelingenChanged(object sender, PTPKoppelingenChangedMessage msg)
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

        private void OnNameChanged(object sender, NameChangedMessage msg)
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
            WeakReferenceMessengerEx.Default.Register<PTPKoppelingenChangedMessage>(this, OnPTPKoppelingenChanged);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
        }

        #endregion // Constructor
    }
}
