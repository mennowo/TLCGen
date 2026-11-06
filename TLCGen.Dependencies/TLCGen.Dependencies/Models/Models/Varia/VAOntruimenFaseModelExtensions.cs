using System.Collections.Generic;
using System.Linq;

namespace TLCGen.Models
{
    public static class VAOntruimenFaseModelExtensions
    {
        /// <summary>
        /// All signal groups for which VA ontruimen applies: the hard conflicts of the fase followed by
        /// the manually added conflicts. Normally all detectors share the same lists; the union is used
        /// so a detector that is out of sync cannot hide a conflict.
        /// </summary>
        public static IReadOnlyList<VAOntruimenNaarFaseModel> GetAllConflicts(this VAOntruimenFaseModel fase)
        {
            var conflicts = new List<VAOntruimenNaarFaseModel>();
            foreach (var detector in fase.VADetectoren)
            {
                AddDistinct(conflicts, detector.ConflicterendeFasen);
            }
            foreach (var detector in fase.VADetectoren)
            {
                AddDistinct(conflicts, detector.ManualConflicts);
            }
            return conflicts;
        }

        /// <summary>
        /// The signal groups that were added manually, as a union over all detectors (the membership is
        /// fase-wide, the times are per detector).
        /// </summary>
        public static IReadOnlyList<string> GetManualConflicts(this VAOntruimenFaseModel fase)
        {
            var fases = new List<string>();
            foreach (var detector in fase.VADetectoren)
            {
                foreach (var manual in detector.ManualConflicts)
                {
                    if (!fases.Contains(manual.FaseCyclus))
                    {
                        fases.Add(manual.FaseCyclus);
                    }
                }
            }
            return fases;
        }

        /// <summary>
        /// The model holding the VA ontruimen time for one detector and conflict: the detector's own
        /// (automatically added) hard conflict if present, otherwise its manual conflict. Returns null
        /// when the conflict does not apply to the detector.
        /// </summary>
        public static VAOntruimenNaarFaseModel GetConflictForDetector(
            this VAOntruimenDetectorModel detector, string faseCyclus)
        {
            return detector.ConflicterendeFasen.FirstOrDefault(x => x.FaseCyclus == faseCyclus)
                ?? detector.ManualConflicts.FirstOrDefault(x => x.FaseCyclus == faseCyclus);
        }

        private static void AddDistinct(
            List<VAOntruimenNaarFaseModel> conflicts, IEnumerable<VAOntruimenNaarFaseModel> toAdd)
        {
            foreach (var conflict in toAdd)
            {
                if (conflicts.All(x => x.FaseCyclus != conflict.FaseCyclus))
                {
                    conflicts.Add(conflict);
                }
            }
        }
    }
}
