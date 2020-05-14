using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Integrity
{
    public static class TLCGenControllerChecker
    {
        /// <summary>
        /// Determines if this phase conflicts with the one parsed
        /// </summary>
        public static bool IsFasenConflicting(ControllerModel controller, string fase1, string fase2)
        {
            if (controller == null)
                throw new NullReferenceException();

            foreach (var cm in controller.InterSignaalGroep.Conflicten)
            {
                if (cm.FaseVan == fase1 && cm.FaseNaar == fase2)
                    return true;
            }
            return false;
        }

        public static List<ConflictModel> GetFaseConflicts(ControllerModel controller, string fase)
        {
            var cfs = new List<ConflictModel>();
            foreach(var c in controller.InterSignaalGroep.Conflicten)
            {
                if(c.FaseVan == fase)
                {
                    cfs.Add(c);
                }
            }
            return cfs;
        }
    }
}
