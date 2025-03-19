using System;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;


namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 7, type: TabItemTypeEnum.SpecialsTab)]
    public class StarTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private AddRemoveItemsManager<StarProgrammaViewModel, StarProgrammaModel, string> _programmaManager;

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
                OnPropertyChanged(broadcast: true);
            }
        }

        public string DefaultProgramma
        {
            get => Controller.StarData.DefaultProgramma;
            set
            {
                Controller.StarData.DefaultProgramma = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool ProgrammaSturingViaKlok
        {
            get => Controller.StarData.ProgrammaSturingViaKlok;
            set
            {
                Controller.StarData.ProgrammaSturingViaKlok = value;
                if (!value) ProgrammaSturingViaParameter = true;
                OnPropertyChanged(broadcast: true);
            }
        }
        
        public bool ProgrammaSturingViaParameter
        {
            get => Controller.StarData.ProgrammaSturingViaParameter;
            set
            {
                Controller.StarData.ProgrammaSturingViaParameter = value;
                if (!value)
                {
                    ProgrammaSturingViaKlok = true;
                }
                OnPropertyChanged(broadcast: true);
            }
        }
        
        public bool IngangAlsVoorwaarde
        {
            get => Controller.StarData.IngangAlsVoorwaarde;
            set
            {
                Controller.StarData.IngangAlsVoorwaarde = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public StarPeriodeDataViewModel SelectedPeriode { get; set; }

        public bool ProgrammaTijdenInParameters
        {
            get => Controller.StarData.ProgrammaTijdenInParameters;
            set
            {
                Controller.StarData.ProgrammaTijdenInParameters = value;
                OnPropertyChanged(broadcast: true);
            }
        }

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
                            } while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.StarProgramma, newname));

                            var prog = new StarProgrammaModel {Naam = newname};
                            foreach (var fc in Controller.Fasen) prog.Fasen.Add(new StarProgrammaFase {FaseCyclus = fc.Naam});
                            return new StarProgrammaViewModel(prog);
                        }, null) {SelectedItem = Programmas.FirstOrDefault()};
                }
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

        private void OnFasenChanged(object sender, FasenChangedMessage message)
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

        private void OnPeriodenChanged(object sender, PeriodenChangedMessage message)
        { 
            UpdatePeriodenData();
        }

        private void OnNameChanged(object sender, NameChangedMessage message)
        {
            foreach (var pr in Programmas)
            {
                pr.OnPropertyChanged("");
                foreach (var f in pr.Fasen)
                {
                    f.OnPropertyChanged("");
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
            WeakReferenceMessenger.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessenger.Default.Register<PeriodenChangedMessage>(this, OnPeriodenChanged);
            WeakReferenceMessenger.Default.Register<NameChangedMessage>(this, OnNameChanged);
        }

        #endregion // Constructor
    }
}
