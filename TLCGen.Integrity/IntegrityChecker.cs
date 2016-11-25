using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.DataAccess;
using TLCGen.Messaging;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Settings;

namespace TLCGen.Integrity
{
    public static class IntegrityChecker
    {
        /// <summary>
        /// Reference to the loaded Controller object. 
        /// This is set and held in the static DataProvider class.
        /// </summary>
        public static ControllerModel Controller
        {
            get { return DataProvider.Instance.Controller; }
        }
        
        public static string IsControllerDataOK()
        {
            return IsControllerDataOK(Controller);
        }

        public static string IsConflictMatrixOK()
        {
            return IsConflictMatrixOK(Controller);
        }
        /// <summary>
        /// Checks the integrity of the data in the instance of ControllerModel that is parsed in
        /// </summary>
        /// <param name="Controller">The instance of ControllerModel to check for integrity</param>
        /// <returns></returns>
        public static string IsControllerDataOK(ControllerModel _Controller)
        {
            string s = IsConflictMatrixOK(_Controller);
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
        public static string IsConflictMatrixOK(ControllerModel _Controller)
        {
            // Request to process all synchronisation data from matrix to model
            MessageManager.Instance.Send(new ProcessSynchronisationsRequest());

            // Loop all conflicts
            foreach (ConflictModel cm1 in _Controller.InterSignaalGroep.Conflicten)
            {
                bool Found = false;
                foreach (ConflictModel cm2 in _Controller.InterSignaalGroep.Conflicten)
                {

                    if(cm1.FaseVan == cm2.FaseNaar && cm1.FaseNaar == cm2.FaseVan)
                    {
                        Found = true;
                        switch (cm1.SerializedWaarde)
                        {
                            case "FK":
                                if (cm2.SerializedWaarde != "FK")
                                    return "Conflict matrix niet symmetrisch:\nFK van " + cm1.FaseVan + " naar " + cm2.FaseNaar + " maar niet andersom.";
                                break;
                            case "GK":
                                if (cm2.SerializedWaarde != "GK" && cm2.SerializedWaarde != "GKL")
                                    return "Conflict matrix niet symmetrisch:\nGK van " + cm1.FaseVan + " naar " + cm2.FaseNaar + " maar niet andersom.";
                                continue;
                            case "GKL":
                                if (cm2.SerializedWaarde != "GK")
                                    return "Conflict matrix niet symmetrisch:\nGKL van " + cm1.FaseVan + " naar " + cm2.FaseNaar + " maar andersom geen GK.";
                                continue;
                            default:
                                int co;
                                if (Int32.TryParse(cm1.SerializedWaarde, out co))
                                {
                                    // Check againt guaranteed timings
                                    if (_Controller.Data.Instellingen.GarantieOntruimingsTijden)
                                    {
                                        if (cm1.GarantieWaarde == null)
                                            return "Ontbrekende garantie ontruimingstijd van " + cm1.FaseVan + " naar " + cm2.FaseNaar + ".";
                                        else if (co < cm1.GarantieWaarde)
                                            return "Ontruimingstijd van " + cm1.FaseVan + " naar " + cm2.FaseNaar + " lager dan garantie ontruimmingstijd.";
                                    }

                                    if (Int32.TryParse(cm2.SerializedWaarde, out co))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        return "Conflict matrix niet symmetrisch:\nwaarde van " + cm1.FaseVan + " naar " + cm2.FaseNaar + " ontbrekend of onjuist (niet numeriek, FK, GK of GKL).";
                                    }
                                }
                                else
                                {
                                    return "Conflict matrix not symmetrical:\nwaarde van " + cm1.FaseVan + " naar " + cm2.FaseNaar + " onjuist (niet numeriek, FK, GK of GKL).";
                                }
                        }
                    }
                    if (Found) break;
                }
                if (!Found)
                {
                    return "Conflict matrix niet symmetrisch:\nconflict van " + cm1.FaseVan + " naar " + cm1.FaseNaar + " niet symmetrisch.";
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if an element's Define property is unique accross the ControllerModel
        /// </summary>
        /// <param name="naam">The Define property to check</param>
        /// <returns>True if unique, false if not</returns>
        public static bool IsElementDefineUnique(string define)
        {
            // Fasen
            foreach (FaseCyclusModel fcm in Controller.Fasen)
            {
                if (fcm.Define == define)
                    return false;
            }

            // Detectie
            foreach (FaseCyclusModel fcm in Controller.Fasen)
            {
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    if (dm.Define == define)
                        return false;
                }
            }
            foreach (DetectorModel dm in Controller.Detectoren)
            {
                if (dm.Define == define)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if an element's Name property is unique accross the ControllerModel
        /// </summary>
        /// <param name="naam">The Name property to check</param>
        /// <returns>True if unique, false if not</returns>
        public static bool IsElementNaamUnique(string naam)
        {
            // Check fasen
            foreach (FaseCyclusModel fcvm in Controller.Fasen)
            {
                if (fcvm.Naam == naam)
                    return false;
            }

            // Check detectie
            foreach (FaseCyclusModel fcvm in Controller.Fasen)
            {
                foreach (DetectorModel dvm in fcvm.Detectoren)
                {
                    if (dvm.Naam == naam)
                        return false;
                }
            }
            foreach (DetectorModel dvm in Controller.Detectoren)
            {
                if (dvm.Naam == naam)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if this phase conflicts with the one parsed
        /// </summary>
        public static bool IsFasenConflicting(FaseCyclusModel fcm1, FaseCyclusModel fcm2)
        {
            if (Controller == null)
                throw new NotImplementedException();

            foreach (ConflictModel cm in Controller.InterSignaalGroep.Conflicten)
            {
                if (cm.FaseVan == fcm1.Define && cm.FaseNaar == fcm2.Define)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if this phase conflicts with the one parsed
        /// </summary>
        public static bool IsFasenConflicting(string define1, string define2)
        {
            if (Controller == null)
                throw new NotImplementedException();

            foreach (ConflictModel cm in Controller.InterSignaalGroep.Conflicten)
            {
                if (cm.FaseVan == define1 && cm.FaseNaar == define2)
                    return true;
            }
            return false;
        }
    }
}
