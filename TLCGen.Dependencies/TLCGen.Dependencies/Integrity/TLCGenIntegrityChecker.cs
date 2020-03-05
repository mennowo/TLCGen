using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Integrity
{
    public static class TLCGenIntegrityChecker
    {
        public static int CompareDetectors(string d1name, string d2name, string d1fcname, string d2fcname)
        {
            var myName = d1fcname == null ? d1name : d1name.Replace(d1fcname, "");
            var hisName = d2fcname == null ? d2name : d2name.Replace(d2fcname, "");
            if (Regex.IsMatch(myName, @".*[a-zA-Z]$"))
            {
                if (!Regex.IsMatch(hisName, @".*[a-zA-Z]$"))
                {
                    hisName = hisName + "0";
                }
            }
            if (Regex.IsMatch(hisName, @".*[a-zA-Z]$"))
            {
                if (!Regex.IsMatch(myName, @".*[a-zA-Z]$"))
                {
                    myName = myName + "0";
                }
            }

            if (Regex.IsMatch(myName, @"^[0-9]+") && myName.Length < hisName.Length) myName = myName.PadLeft(hisName.Length, '0');
            else if (Regex.IsMatch(hisName, @"^[0-9]+") && hisName.Length < myName.Length) hisName = hisName.PadLeft(myName.Length, '0');
            return string.Compare(
                d1fcname == null ? myName : d1fcname + myName,
                d1fcname == null ? hisName : d2fcname + hisName,
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Checks the integrity of the data in the instance of ControllerModel that is parsed in
        /// </summary>
        /// <param name="Controller">The instance of ControllerModel to check for integrity</param>
        /// <returns></returns>
        public static string IsControllerDataOK(ControllerModel c)
        {
            if (c == null) return "Geen controller gevonden om op te slaan!";
            var s = IsConflictMatrixOK(c);
            if (!string.IsNullOrEmpty(s))
            {
                return s;
            }

            return null;
        }

        public static string IsGroentijdenSetDataOK(ControllerModel c)
        {
            if(string.IsNullOrWhiteSpace(c.PeriodenData.DefaultPeriodeGroentijdenSet))
            {
                return "Default groentijden set niet ingesteld.";
            }
            foreach(var per in c.PeriodenData.Perioden)
            {
                if (per.Type == PeriodeTypeEnum.Groentijden && string.IsNullOrWhiteSpace(per.GroentijdenSet))
                {
                    return $"Groentijden set niet ingesteld voor periode {per.Naam}.";
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if the ConflictMatrix is symmetrical.
        /// </summary>
        /// <returns>null if succesfull, otherwise a string stating the first error found.</returns>
        public static string IsConflictMatrixOK(ControllerModel c)
        {
            if (c == null)
                throw new NullReferenceException("Error with IsConflictMatrixOK: ControllerModel cannot be null");
            
            // Loop all conflicts
            foreach (var cm1 in c.InterSignaalGroep.Conflicten)
            {
                var Found = false;
                foreach (var cm2 in c.InterSignaalGroep.Conflicten)
                {
                    if (cm1.FaseVan == cm2.FaseNaar && cm1.FaseNaar == cm2.FaseVan)
                    {
                        Found = true;
                        switch (cm1.SerializedWaarde)
                        {
                            case "FK":
                                if (cm2.SerializedWaarde != "FK")
                                    return "Conflict matrix niet symmetrisch:\nFK van " + cm1.FaseVan + " naar " + cm1.FaseNaar + " maar niet andersom.";
                                break;
                            case "GK":
                                if (cm2.SerializedWaarde != "GK" && cm2.SerializedWaarde != "GKL")
                                    return "Conflict matrix niet symmetrisch:\nGK van " + cm1.FaseVan + " naar " + cm1.FaseNaar + " maar niet andersom.";
                                continue;
                            case "GKL":
                                if (cm2.SerializedWaarde != "GK")
                                    return "Conflict matrix niet symmetrisch:\nGKL van " + cm1.FaseVan + " naar " + cm1.FaseNaar + " maar andersom geen GK.";
                                continue;
                            default:
                                int co;
                                if (Int32.TryParse(cm1.SerializedWaarde, out co))
                                {
                                    // Check against guaranteed timings
                                    if (c.Data.GarantieOntruimingsTijden)
                                    {
                                        if (cm1.GarantieWaarde == null)
                                            return "Ontbrekende garantie ontruimingstijd van " + cm1.FaseVan + " naar " + cm1.FaseNaar + ".";
                                        else if (co < cm1.GarantieWaarde)
                                            return "Ontruimingstijd van " + cm1.FaseVan + " naar " + cm1.FaseNaar + " lager dan garantie ontruimmingstijd.";
                                    }

                                    if (Int32.TryParse(cm2.SerializedWaarde, out co))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        return "Conflict matrix niet symmetrisch:\nwaarde van " + cm2.FaseVan + " naar " + cm2.FaseNaar + " ontbrekend of onjuist (niet numeriek, FK, GK of GKL).";
                                    }
                                }
                                else
                                {
                                    return "Conflict matrix niet symmetrisch:\nwaarde van " + cm2.FaseVan + " naar " + cm2.FaseNaar + " ontbrekend of onjuist (niet numeriek, FK, GK of GKL).";
                                }
                        }

                        if (c.Data.GarantieOntruimingsTijden)
                        {
                            int out1;

                            if (Int32.TryParse(cm1.SerializedWaarde, out out1))
                            {
                                if (cm1.GarantieWaarde != null && cm1.GarantieWaarde >= 0)
                                {
                                    if (out1 < cm1.GarantieWaarde)
                                    {
                                        return "Ontruimingstijd van " + cm1.FaseVan + " naar " + cm1.FaseNaar + " lager dan garantie ontruimmingstijd.";
                                    }
                                }
                                else
                                {
                                    return "Ontbrekende garantie ontruimingstijd van " + cm1.FaseVan + " naar " + cm1.FaseNaar + ".";
                                }
                            }
                            else if (cm1.GarantieWaarde != null && cm1.GarantieWaarde >= 0)
                            {
                                return "Ontbrekende ontruimingstijd van " + cm1.FaseVan + " naar " + cm1.FaseNaar + ".";
                            }
                        }
                    }
                    if (Found) break;
                }
                if (!Found)
                {
                    return "Conflict matrix niet symmetrisch:\nconflict van " + cm1.FaseVan + " naar " + cm1.FaseNaar + " ontbreekt.";
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if an element's Name property is unique accross the ControllerModel
        /// </summary>
        /// <param name="naam">The Name property to check</param>
        /// <returns>True if unique, false if not</returns>
        public static bool IsElementNaamUnique(ControllerModel _Controller, string naam, TLCGenObjectTypeEnum type)
        {
            foreach (var pl in Plugins.TLCGenPluginManager.Default.ApplicationPlugins.Where(x => x.Item1.HasFlag(Plugins.TLCGenPluginElems.IOElementProvider)))
            {
                if (!((Plugins.ITLCGenElementProvider)pl.Item2).IsElementNameUnique(naam, type)) return false;
            }
            return IsElementNaamUnique((object)_Controller, naam, type);
        }

        private static bool IsElementNaamUnique(object obj, string naam, TLCGenObjectTypeEnum type)
        {
            if (obj == null || string.IsNullOrWhiteSpace(naam))
                return true;
            
            var objType = obj.GetType();
            var properties = objType.GetProperties();
            foreach (var property in properties)
            {
                var ignore = (TLCGenIgnoreAttributeAttribute)property.GetCustomAttribute(typeof(TLCGenIgnoreAttributeAttribute));
                if (ignore != null) continue;

                var propValue = property.GetValue(obj);

                if (property.PropertyType == typeof(string))
                {
                    var attr = property.GetCustomAttributes(typeof(ModelNameAttribute), true);
                    if (attr.Length != 1) continue;
                    var mnAttr = (ModelNameAttribute)attr.First();
                    var propString = (string) propValue;
                    if (propString == naam && mnAttr.Type == type)
                    {
                        return false;
                    }
                }
                else if (!property.PropertyType.IsValueType)
                {
                    if (propValue is IList elems)
                    {
                        if (elems.Cast<object>().Any(item => !IsElementNaamUnique(item, naam, type)))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if(!IsElementNaamUnique(propValue, naam, type))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if an element's VissimName property is unique accross the ControllerModel
        /// </summary>
        /// <param name="vissimnaam">The VissimName property to check</param>
        /// <returns>True if unique, false if not</returns>
        public static bool IsElementVissimNaamUnique(ControllerModel _Controller, string vissimnaam)
        {
            // Check detectie
            foreach (var fcvm in _Controller.Fasen)
            {
                foreach (var dvm in fcvm.Detectoren)
                {
                    if (dvm.VissimNaam == vissimnaam)
                        return false;
                }
            }
            foreach (var dvm in _Controller.Detectoren)
            {
                if (dvm.VissimNaam == vissimnaam)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if this phase conflicts with the one parsed
        /// </summary>
        public static bool IsFasenConflicting(ControllerModel controller, FaseCyclusModel fcm1, FaseCyclusModel fcm2)
        {
            if (controller == null)
                throw new NullReferenceException();

            foreach (var cm in controller.InterSignaalGroep.Conflicten)
            {
                if (cm.FaseVan == fcm1.Naam && cm.FaseNaar == fcm2.Naam)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if this phase conflicts with the one parsed
        /// </summary>
        public static bool IsFasenConflicting(ControllerModel controller, string define1, string define2)
        {
            if (controller == null)
                throw new NullReferenceException();

            foreach (var cm in controller.InterSignaalGroep.Conflicten)
            {
                if (cm.FaseVan == define1 && cm.FaseNaar == define2)
                    return true;
            }
            return false;
        }
    }
}
