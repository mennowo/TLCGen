using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.ViewModels;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

namespace TLCGen.ViewModels
{
    public class StarProgrammaViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        #region Properties

        public ObservableCollectionAroundList<StarProgrammaFaseViewModel, StarProgrammaFase> Fasen { get; set; }

        public StarProgrammaModel StarProgramma { get; }

        public string Naam
        {
            get => StarProgramma.Naam;
            set
            {
                var oldName = StarProgramma.Naam;
                if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.StarProgramma, value));
                {
                    StarProgramma.Naam = value;
                    MessengerInstance.Send(new NameChangingMessage(TLCGenObjectTypeEnum.StarProgramma, oldName, StarProgramma.Naam));
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Cyclustijd
        {
            get => StarProgramma.Cyclustijd;
            set
            {
                StarProgramma.Cyclustijd = value; 
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem() => StarProgramma;

        #endregion // IViewModelWithItem

        #region Constructor

        public StarProgrammaViewModel(StarProgrammaModel starProgramma)
        {
            StarProgramma = starProgramma;
            Fasen = new ObservableCollectionAroundList<StarProgrammaFaseViewModel, StarProgrammaFase>(starProgramma.Fasen);
        }

        #endregion // Constructor

        public int CompareTo(object obj)
        {
            return string.Compare(Naam, ((StarProgrammaViewModel) obj).Naam, StringComparison.Ordinal);
        }
    }

    public class StarProgrammaFaseViewModel : ViewModelBase, IViewModelWithItem
    {
        public StarProgrammaFase Fase { get; }

        public string FaseNaam => Fase.FaseCyclus;

        public int Start1
        {
            get => Fase.Start1;
            set
            {
                Fase.Start1 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int? Start2
        {
            get => Fase.Start2;
            set
            {
                Fase.Start2 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Eind1
        {
            get => Fase.Eind1;
            set
            {
                Fase.Eind1 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int? Eind2
        {
            get => Fase.Eind2;
            set
            {
                Fase.Eind2 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public object GetItem() => Fase;

        public StarProgrammaFaseViewModel(StarProgrammaFase fase)
        {
            Fase = fase;
        }
    }

    public class StarPeriodeDataViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        #region Properties

        public StarPeriodeDataModel StarPeriode { get; }

        public string Periode => StarPeriode.Periode;

        public string StarProgramma
        {
            get => StarPeriode.StarProgramma;
            set
            {
                StarPeriode.StarProgramma = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem() => StarPeriode;

        #endregion // IViewModelWithItem

        #region IComparable

        public int CompareTo(object obj)
        {
            return string.Compare(Periode, ((StarPeriodeDataViewModel) obj).Periode, StringComparison.Ordinal);
        }

        #endregion // IComparable

        #region Constructor

        public StarPeriodeDataViewModel(StarPeriodeDataModel starPeriode)
        {
            StarPeriode = starPeriode;
        }

        #endregion // Constructor
    }

    [TLCGenTabItem(index: 7, type: TabItemTypeEnum.SpecialsTab)]
    public class StarTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private RelayCommand _addStarProgramma;
        private RelayCommand _removeStarProgramma;
        private StarProgrammaViewModel _selectedProgramma;

        #endregion // Fields

        #region Properties

        public ObservableCollectionAroundList<StarProgrammaViewModel, StarProgrammaModel> Programmas { get; private set; }
        public ObservableCollectionAroundList<StarPeriodeDataViewModel, StarPeriodeDataModel> Perioden { get; private set; }

        public bool ToepassenStar
        {
            get => Controller.StarData.ToepassenStar;
            set
            {
                Controller.StarData.ToepassenStar = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string DefaultProgramma
        {
            get => Controller.StarData.DefaultProgramma;
            set
            {
                Controller.StarData.DefaultProgramma = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool ProgrammaSturingViaKlok
        {
            get => Controller.StarData.ProgrammaSturingViaKlok;
            set
            {
                Controller.StarData.ProgrammaSturingViaKlok = value;
                if (!value) ProgrammaSturingViaParameter = true;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }
        
        public bool ProgrammaSturingViaParameter
        {
            get => Controller.StarData.ProgrammaSturingViaParameter;
            set
            {
                Controller.StarData.ProgrammaSturingViaParameter = value;
                if (!value) ProgrammaSturingViaKlok = true;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public StarPeriodeDataViewModel SelectedPeriode { get; set; }

        public bool ProgrammaTijdenInParameters
        {
            get => Controller.StarData.ProgrammaTijdenInParameters;
            set
            {
                Controller.StarData.ProgrammaTijdenInParameters = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        private AddRemoveItemsManager<StarProgrammaViewModel, StarProgrammaModel, string> _programmaManager;
        public AddRemoveItemsManager<StarProgrammaViewModel, StarProgrammaModel, string> ProgrammaManager
        {
            get
            {
                if (_programmaManager == null)
                {
                    _programmaManager = new AddRemoveItemsManager<StarProgrammaViewModel, StarProgrammaModel, string>(
                        Programmas,
                        (x) =>
                        {
                            var inext = 0;
                            string newname;
                            do
                            {
                                inext++;
                                newname = "star" + (inext < 10 ? "0" : "") + inext;
                            }
                            while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.StarProgramma, newname));

                            var prog = new StarProgrammaModel {Naam = newname};
                            foreach (var fc in Controller.Fasen) prog.Fasen.Add(new StarProgrammaFase{FaseCyclus = fc.Naam});
                            return new StarProgrammaViewModel(prog);
                        }, null);
                }
                _programmaManager.SelectedItem = Programmas.FirstOrDefault();
                return _programmaManager;
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen TabItem overrides

        public override string DisplayName => "Star regelen";

        public override void OnSelected()
        {
            Programmas.BubbleSort();
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                if (value != null)
                {
                    _programmaManager = null;
                    Programmas = new ObservableCollectionAroundList<StarProgrammaViewModel, StarProgrammaModel>(value.StarData.Programmas);
                    Perioden = new ObservableCollectionAroundList<StarPeriodeDataViewModel, StarPeriodeDataModel>(value.StarData.PeriodenData);
                    UpdatePeriodenData();
                }
            }
        }

        #endregion // TLCGen TabItem overrides

        #region TLCGen Events

        private void OnFasenChanged(FasenChangedMessage message)
        {
            foreach (var programma in Programmas)
            {
                if (message.RemovedFasen?.Any() == true)
                {
                    foreach (var fc in message.RemovedFasen)
                    {
                        var sfc = programma.StarProgramma.Fasen.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                        programma.StarProgramma.Fasen.Remove(sfc);
                    }
                    programma.Fasen.Rebuild();
                }
                if (message.AddedFasen?.Any() == true)
                {
                    foreach (var fc in message.AddedFasen)
                    {
                        programma.StarProgramma.Fasen.Add(new StarProgrammaFase{FaseCyclus = fc.Naam});
                    }
                }
                programma.Fasen.Rebuild();
            }
        }

        private void OnPeriodenChanged(PeriodenChangedMessage message)
        { 
            UpdatePeriodenData();
        }

        private void OnNameChanged(NameChangedMessage message)
        {
            foreach (var pr in Programmas)
            {
                pr.RaisePropertyChanged("");
                foreach (var f in pr.Fasen)
                {
                    f.RaisePropertyChanged("");
                }
            }
        }

        #endregion // TLCGen Events

        #region Private Methods

        private void UpdatePeriodenData()
        {
            foreach (var per in Controller.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.StarRegelen))
            {
                if (Perioden.All(x => x.Periode != per.Naam))
                {
                    Perioden.Add(new StarPeriodeDataViewModel(new StarPeriodeDataModel{Periode = per.Naam, StarProgramma = Programmas.FirstOrDefault()?.Naam}));
                }
            }

            var rems = Perioden
                .Where(x => Controller.PeriodenData.Perioden.All(x2 => x2.Type != PeriodeTypeEnum.StarRegelen || x2.Naam != x.Periode))
                .ToList();
            foreach (var r in rems) Perioden.Remove(r);

            Perioden.RebuildList();
            Perioden.BubbleSort();
        }

        #endregion // Private Methods

        #region Constructor

        public StarTabViewModel() : base()
        {
            MessengerInstance.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            MessengerInstance.Register(this, new Action<PeriodenChangedMessage>(OnPeriodenChanged));
            MessengerInstance.Register(this, new Action<NameChangedMessage>(OnNameChanged));
        }

        #endregion // Constructor
    }
}
