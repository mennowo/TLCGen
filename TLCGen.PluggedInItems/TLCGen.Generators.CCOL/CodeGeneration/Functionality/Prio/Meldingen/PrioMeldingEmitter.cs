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
            /// Elements specific to the melding type, declared before the helper element, timer and switch of the melding.
            /// </summary>
            public virtual void AddElements(PrioMeldingContext m, List<CCOLElement> elements)
            {
            }

            /// <summary>
            /// Elements added straight to the elements of the generator, so before the elements of the melding itself
            /// (and after those of its melding bij storing).
            /// </summary>
            public virtual void AddElementsBeforeMelding(PrioMeldingContext m, List<CCOLElement> elements)
            {
            }

            /// <summary>
            /// Whether the melding only applies when the ingreep has no active inmelding (!C[cvc]), regardless of
            /// AlleenIndienGeenInmelding.
            /// </summary>
            public virtual bool RequiresNoActiveInmelding(PrioMeldingContext m) => false;

            /// <summary>
            /// Whether the code of the melding is enclosed in #ifndef NO_RIS.
            /// </summary>
            public virtual bool WrapInNoRis => false;

            /// <summary>
            /// Whether the condition checks the anti-jutter timers (!T[timer]) of the melding and of all meldingen.
            /// </summary>
            public virtual bool ChecksAntiJutterTimers => true;

            /// <summary>
            /// Whether the condition restarts the anti-jutter timer of the melding (RT[timer]).
            /// </summary>
            public virtual bool RestartsAntiJutterTimer => true;

            /// <summary>
            /// Condition appended to "if (SCH[switch]", eg. to skip the melding on detector failure.
            /// </summary>
            public virtual string GetStoringCondition(PrioMeldingContext m) => "";

            /// <summary>
            /// The melding condition, appended to "IH[hulp] = [common conditions] ". Ends the statement.
            /// </summary>
            public abstract void AppendCondition(PrioMeldingContext m, StringBuilder sb);

            /// <summary>
            /// Code after the condition, inside the if-block of the melding.
            /// </summary>
            /// <param name="hulpElements">Helper elements of the melding so far (empty for a melding bij storing).</param>
            public virtual void AppendAfterCondition(PrioMeldingContext m, StringBuilder sb, IReadOnlyList<string> hulpElements)
            {
            }
        }

        private Dictionary<PrioIngreepInUitMeldingVoorwaardeTypeEnum, PrioMeldingEmitter> _meldingEmitters;

        private PrioMeldingEmitter GetMeldingEmitter(PrioIngreepInUitMeldingVoorwaardeTypeEnum type)
        {
            _meldingEmitters ??= new Dictionary<PrioIngreepInUitMeldingVoorwaardeTypeEnum, PrioMeldingEmitter>
            {
                { PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding, new KarMeldingEmitter(this) },
                { PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, new DetectorMeldingEmitter(this, false) },
                { PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang, new DetectorMeldingEmitter(this, true) },
                { PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector, new VecomMeldingEmitter(this) },
                { PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector, new SelectieveDetectorMeldingEmitter(this) },
                { PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton, new FietsMeldingEmitter(this) },
                { PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde, new RisMeldingEmitter(this) },
                { PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom, new OpticomMeldingEmitter(this) },
                { PrioIngreepInUitMeldingVoorwaardeTypeEnum.AutoMassaPeloton, new AutoPelotonMeldingEmitter(this) },
            };
            // Every melding type has an emitter; a new type without one fails here rather than generating partial code
            return _meldingEmitters[type];
        }

        private FietsMeldingEmitter FietsEmitter => (FietsMeldingEmitter)GetMeldingEmitter(PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton);

        private PrioMeldingContext CreateMeldingContext(ControllerModel c, PrioIngreepModel prio, PrioIngreepInUitMeldingModel melding)
        {
            return new PrioMeldingContext(c, prio, melding, GetMeldingNames(c, prio, melding), CCOLCodeHelper.GetPriorityName(c, prio));
        }
    }
}
