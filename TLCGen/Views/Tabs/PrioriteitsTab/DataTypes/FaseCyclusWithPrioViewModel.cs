using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class FaseCyclusWithPrioViewModel : ViewModelBase
    {
        [Browsable(false)]
        public bool HasBus => Ingrepen.Any(x => x.FaseCyclus == Naam && x.Type == PrioIngreepVoertuigTypeEnum.Bus);
        [Browsable(false)]
        public bool HasTram => Ingrepen.Any(x => x.FaseCyclus == Naam && x.Type == PrioIngreepVoertuigTypeEnum.Tram);
        [Browsable(false)]
        public bool HasBicycle => Ingrepen.Any(x => x.FaseCyclus == Naam && x.Type == PrioIngreepVoertuigTypeEnum.Fiets);
        [Browsable(false)]
        public bool HasTruck => Ingrepen.Any(x => x.FaseCyclus == Naam && x.Type == PrioIngreepVoertuigTypeEnum.Vrachtwagen);

        [Browsable(false)]
        public string Naam { get; }

        [Browsable(false)]
        public List<PrioIngreepViewModel> Ingrepen { get; }

        public void UpdateTypes()
        {
            RaisePropertyChanged(nameof(HasBus));
            RaisePropertyChanged(nameof(HasTram));
            RaisePropertyChanged(nameof(HasBicycle));
            RaisePropertyChanged(nameof(HasTruck));
        }

        public FaseCyclusWithPrioViewModel(string naam, List<PrioIngreepViewModel> ingrepen)
        {
            Naam = naam;
            Ingrepen = ingrepen;
        }
    }
}