using System;
using System.Windows;
using System.Windows.Controls;
using TLCGen.Models.Enumerations;
using TLCGen.ViewModels;

namespace TLCGen.Views
{
    public class MeldingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RegularTemplate { get; set; }
        public DataTemplate RISTemplate { get; set; }
        public DataTemplate PelotonTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            switch (item)
            {
                case PrioIngreepPelotonMeldingViewModel _: 
                    return PelotonTemplate ?? throw new NullReferenceException();
                case PrioIngreepRISMeldingViewModel _: 
                    return RISTemplate ?? throw new NullReferenceException();
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding:
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector:
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                default: 
                    return RegularTemplate ?? throw new NullReferenceException();
            }

            throw new NullReferenceException();
        }
    }
}