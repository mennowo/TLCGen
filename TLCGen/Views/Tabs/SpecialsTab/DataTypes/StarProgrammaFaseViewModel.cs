using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
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
}