using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: the element names of a single in- or uitmelding. Single source for declaring the elements
    // (GetMeldingElements) and using them (GetMeldingCode), so the two cannot drift apart.
    public partial class PrioCodeGenerator
    {
        /// <summary>
        /// Element names of a melding, without the CCOL prefixes (h, t, sch, ...).
        /// </summary>
        /// <param name="Base">Priority name plus the short code of the melding type, eg. "41buskar".</param>
        /// <param name="Hulp">Helper element (hprioin/hpriouit) of the melding.</param>
        /// <param name="Timer">Anti-jutter timer (tprioin/tpriouit) of the melding.</param>
        /// <param name="Switch">Switch (schprioin/schpriouit) enabling the melding.</param>
        private sealed record PrioMeldingNames(string Base, string Hulp, string Timer, string Switch);

        private PrioMeldingNames GetMeldingNames(ControllerModel c, PrioIngreepModel prio, PrioIngreepInUitMeldingModel melding)
        {
            var inmelding = melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding;
            var @base = $"{CCOLCodeHelper.GetPriorityName(c, prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}";

            var he = (inmelding ? _hprioin.ToString() : _hpriouit.ToString()) + @base;
            var ti = (inmelding ? _tprioin.ToString() : _tpriouit.ToString()) + @base;
            var sw = (inmelding ? _schprioin.ToString() : _schpriouit.ToString()) + @base;

            // KAR and RIS meldingen have no input; other meldingen include their input(s) in the names
            if (melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding &&
                melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                he += melding.RelatedInput1;
                ti += melding.RelatedInput1;
                sw += melding.RelatedInput1;
                if (melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton)
                {
                    sw += PrioCodeGeneratorHelper.GetDetectorTypeSCHString(melding.RelatedInput1Type);
                }
                if ((melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector || melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang) && melding.TweedeInput)
                {
                    he += melding.RelatedInput2;
                    ti += melding.RelatedInput2;
                    sw = sw + melding.RelatedInput2 + PrioCodeGeneratorHelper.GetDetectorTypeSCHString(melding.RelatedInput2Type);
                }
            }

            return new PrioMeldingNames(@base, he, ti, sw);
        }
    }
}
