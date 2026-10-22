using System;
using System.Collections.Generic;
using System.Linq;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    /// <summary>
    /// The vehicle types that can be chosen for a priority ingreep.
    /// </summary>
    public static class PrioVoertuigTypes
    {
        /// <summary>
        /// The vehicle types allowed for an ingreep on a fase of the given type. Hulpdiensten (emergency vehicles)
        /// do not use langzaam verkeer (fiets, voetganger) fasen, so Hulpdienst is not allowed there.
        /// </summary>
        public static IReadOnlyList<PrioIngreepVoertuigTypeEnum> GetAllowed(FaseTypeEnum faseType)
        {
            return Enum.GetValues(typeof(PrioIngreepVoertuigTypeEnum))
                .Cast<PrioIngreepVoertuigTypeEnum>()
                .Where(x => IsAllowed(x, faseType))
                .ToList();
        }

        public static bool IsAllowed(PrioIngreepVoertuigTypeEnum vehicleType, FaseTypeEnum faseType)
        {
            return vehicleType != PrioIngreepVoertuigTypeEnum.Hulpdienst ||
                   faseType != FaseTypeEnum.Fiets && faseType != FaseTypeEnum.Voetganger;
        }
    }
}
