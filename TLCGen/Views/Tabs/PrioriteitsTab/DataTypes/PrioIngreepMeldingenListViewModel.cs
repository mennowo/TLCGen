using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class PrioIngreepMeldingenListViewModel : ViewModelBase
    {
        private ObservableCollection<PrioIngreepInUitMeldingViewModel> _meldingen;
        public ObservableCollection<PrioIngreepInUitMeldingViewModel> Meldingen => _meldingen ?? (_meldingen = new ObservableCollection<PrioIngreepInUitMeldingViewModel>());
        
        [Browsable(false)]
        public string Naam { get; }

        [Browsable(false)]
        public bool IsInmeldingenList { get; }
        [Browsable(false)]
        public bool IsUitmeldingenList { get; }

        public PrioIngreepMeldingenListViewModel(string naam, PrioIngreepInUitMeldingTypeEnum type, List<PrioIngreepInUitMeldingViewModel> meldingen)
        {
            _meldingen = new ObservableCollection<PrioIngreepInUitMeldingViewModel>(meldingen);
            Naam = naam;

            IsInmeldingenList = type == PrioIngreepInUitMeldingTypeEnum.Inmelding;
            IsUitmeldingenList = type == PrioIngreepInUitMeldingTypeEnum.Uitmelding;
        }
    }
}