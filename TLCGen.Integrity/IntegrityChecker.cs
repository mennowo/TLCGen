using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;
using TLCGen.Settings;

namespace TLCGen.Integrity
{
    public static class IntegrityChecker
    {
        /// <summary>
        /// Checks the integrity of the data in the instance of ControllerModel that is parsed in
        /// </summary>
        /// <param name="c">The instance of ControllerModel to check for integrity</param>
        /// <returns></returns>
        public static string IsControllerDataOK(ControllerModel c)
        {
            string s = IsConflictMatrixOK(c);
            if (!string.IsNullOrEmpty(s))
            {
                return s;
            }
            return null;
        }

        /// <summary>
        /// Checks if the ConflictMatrix is symmetrical.
        /// </summary>
        /// <returns>null if succesfull, otherwise a string stating the first error found.</returns>
        public static string IsConflictMatrixOK(ControllerModel c)
        {
            foreach (FaseCyclusModel fcm in c.Fasen)
            {
                foreach (ConflictModel cm in fcm.Conflicten)
                {
                    bool Found = false;
                    foreach (FaseCyclusModel fcm2 in c.Fasen)
                    {
                        if (fcm == fcm2)
                            continue;

                        if (cm.FaseNaar == fcm2.Define)
                        {
                            foreach (ConflictModel cm2 in fcm2.Conflicten)
                            {
                                if (cm2.FaseNaar == fcm.Define)
                                {
                                    // Check againt guaranteed timings
                                    Found = true;
                                    switch (cm.SerializedWaarde)
                                    {
                                        case "FK":
                                            if (cm2.SerializedWaarde != "FK")
                                                return "Conflict matrix niet symmetrisch:\nFK van " + fcm.Naam + " naar " + fcm2.Naam + " maar niet andersom.";
                                            break;
                                        case "GK":
                                            if (cm2.SerializedWaarde != "GK" && cm2.SerializedWaarde != "GKL")
                                                return "Conflict matrix niet symmetrisch:\nGK van " + fcm.Naam + " naar " + fcm2.Naam + " maar niet andersom.";
                                            continue;
                                        case "GKL":
                                            if (cm2.SerializedWaarde != "GK")
                                                return "Conflict matrix niet symmetrisch:\nGKL van " + fcm.Naam + " naar " + fcm2.Naam + " maar andersom geen GK.";
                                            continue;
                                        default:
                                            int co;
                                            if (Int32.TryParse(cm.SerializedWaarde, out co))
                                            {
                                                // Check againt guaranteed timings
                                                if (c.Data.Instellingen.GarantieOntruimingsTijden)
                                                {
                                                    if (cm.GarantieWaarde == null)
                                                        return "Ontbrekende garantie ontruimingstijd van " + fcm.Naam + " naar " + fcm2.Naam + ".";
                                                    else if (co < cm.GarantieWaarde)
                                                        return "Ontruimingstijd van " + fcm.Naam + " naar " + fcm2.Naam + " lager dan garantie ontruimmingstijd.";
                                                }

                                                if (Int32.TryParse(cm2.SerializedWaarde, out co))
                                                {
                                                    break;
                                                }
                                                else
                                                {
                                                    return "Conflict matrix niet symmetrisch:\nwaarde van " + fcm.Naam + " naar " + fcm2.Naam + " ontbrekend of onjuist (niet numeriek, FK, GK of GKL).";
                                                }
                                            }
                                            else
                                            {
                                                return "Conflict matrix not symmetrical:\nwaarde van " + fcm.Naam + " naar " + fcm2.Naam + " onjuist (niet numeriek, FK, GK of GKL).";
                                            }
                                    }
                                }
                                if (Found) break;
                            }
                            if (Found) break;
                        }
                    }
                    if (!Found)
                    {
                        return "Conflict matrix niet symmetrisch:\nconflict van " + fcm.Define + " naar " + cm.FaseNaar + " niet symmetrisch.";
                    }
                }
            }
            return null;
        }
    }
}
