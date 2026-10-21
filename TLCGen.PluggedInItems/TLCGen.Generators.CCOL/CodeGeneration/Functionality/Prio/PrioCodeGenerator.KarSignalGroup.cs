using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: the signal group number passed to KAR DSI meldingen (generic and legacy HD ingrepen).
    public partial class PrioCodeGenerator
    {
        /// <summary>
        /// Signal group in a KAR DSI melding: the parameter (prmkarsg/prmkarsghd + name) when
        /// KARSignaalGroepNummersInParameters is set, else the number itself, minus 200 for high numbers
        /// when VerlaagHogeSignaalGroepNummers is set.
        /// </summary>
        private string GetKarSignalGroup(ControllerModel c, CCOLGeneratorCodeStringSettingModel prm, string name, int number)
        {
            return c.PrioData.KARSignaalGroepNummersInParameters
                ? $"PRM[{_prmpf}{prm}{name}]"
                : number > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers ? (number - 200).ToString() : number.ToString();
        }

        /// <summary>
        /// Name of the signal group of "inmelding ook door" (InmeldingOokDoorFase): the number, padded to two digits.
        /// </summary>
        private static string GetOokDoorFaseName(int fase)
        {
            return fase.ToString().Length < 2 ? "0" + fase : fase.ToString();
        }
    }
}
