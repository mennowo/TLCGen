using System.Collections.Generic;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: base class and context of the per melding type emitters (Prio/Meldingen/*MeldingEmitter.cs).
    // GetMeldingElements and GetMeldingCode (Meldingen.cs) handle what all meldingen share and ask the emitter
    // of the melding type for the type specific parts. Emitters are nested in PrioCodeGenerator so they can read
    // its element name settings; they are NOT registered generators (settings are keyed by generator class name).
    public partial class PrioCodeGenerator
    {
        /// <summary>
        /// A single melding of an ingreep, as passed to an emitter.
        /// </summary>
        private sealed record PrioMeldingContext(
            ControllerModel C,
            PrioIngreepModel Prio,
            PrioIngreepInUitMeldingModel Melding,
            PrioMeldingNames Names,
            string PrioName)
        {
            public bool IsInmelding => Melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding;

            // Code generation only
            public string Ts { get; init; }
            /// <summary>Indentation inside the if-block of the melding.</summary>
            public string Tts { get; init; }
            /// <summary>Vehicle type parameter of the ingreep, eg. PRM[prmvtgcat41bus].</summary>
            public string VtgType { get; init; }
            /// <summary>Signal group number of the ingreep, -1 if not numeric.</summary>
            public int FcNmr { get; init; }
            /// <summary>Anti-jutter timer of the melding, with prefix.</summary>
            public string Timer { get; init; }
            /// <summary>Extra DSI conditions (line numbers, wagennummer, sirene), "TRUE" if none.</summary>
            public string Extra { get; init; }
        }

        private abstract class PrioMeldingEmitter
        {
            protected PrioMeldingEmitter(PrioCodeGenerator generator)
            {
                G = generator;
            }

            /// <summary>The generator, for its element name settings and prefixes.</summary>
            protected PrioCodeGenerator G { get; }

            /// <summary>
            /// Condition appended to "if (SCH[switch]", eg. to skip the melding on detector failure.
            /// </summary>
            public virtual string GetStoringCondition(PrioMeldingContext m) => "";

            /// <summary>
            /// The melding condition, appended to "IH[hulp] = [common conditions] ". Ends the statement.
            /// </summary>
            public abstract void AppendCondition(PrioMeldingContext m, StringBuilder sb);
        }

        private Dictionary<PrioIngreepInUitMeldingVoorwaardeTypeEnum, PrioMeldingEmitter> _meldingEmitters;

        private PrioMeldingEmitter GetMeldingEmitter(PrioIngreepInUitMeldingVoorwaardeTypeEnum type)
        {
            _meldingEmitters ??= new Dictionary<PrioIngreepInUitMeldingVoorwaardeTypeEnum, PrioMeldingEmitter>
            {
                { PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, new DetectorMeldingEmitter(this, false) },
                { PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang, new DetectorMeldingEmitter(this, true) },
            };
            return _meldingEmitters.TryGetValue(type, out var emitter) ? emitter : null;
        }
    }
}
