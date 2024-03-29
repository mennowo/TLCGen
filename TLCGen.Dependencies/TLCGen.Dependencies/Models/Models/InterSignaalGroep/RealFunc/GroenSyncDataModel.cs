﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class GroenSyncDataModel
    {
        [XmlArrayItem(ElementName = "GroenSync")]
        public List<GroenSyncModel> GroenSyncFasen { get; set; }

        [XmlArrayItem(ElementName = "FictiefConflict")]
        public List<FictiefConflictModel> FictieveConflicten { get; set; }

        public GroenSyncDataModel()
        {
            GroenSyncFasen = new List<GroenSyncModel>();
            FictieveConflicten = new List<FictiefConflictModel>();
        }

        public static GroenSyncDataModel ConvertSyncFuncToRealFunc(ControllerModel c)
        {
            var result = new GroenSyncDataModel();

            foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
            {
                var naloop1 = c.InterSignaalGroep.Nalopen.FirstOrDefault(x => x.FaseVan == gs.FaseVan && x.FaseNaar == gs.FaseNaar);
                var naloop2 = c.InterSignaalGroep.Nalopen.FirstOrDefault(x => x.FaseVan == gs.FaseNaar && x.FaseNaar == gs.FaseVan);
                result.GroenSyncFasen.Add(new GroenSyncModel{ FaseVan = gs.FaseVan, FaseNaar = gs.FaseNaar, Waarde = naloop1?.MaximaleVoorstart ?? 0, Richting = 1, AanUit = gs.Schakelbaar });
                result.GroenSyncFasen.Add(new GroenSyncModel{ FaseVan = gs.FaseNaar, FaseNaar = gs.FaseVan, Waarde = naloop2?.MaximaleVoorstart ?? 0, Richting = 1, AanUit = gs.Schakelbaar });
                if (gs.DeelConflict)
                {
                    result.FictieveConflicten.Add(new FictiefConflictModel{ FaseVan = gs.FaseVan, FaseNaar = gs.FaseNaar, FictieveOntruimingsTijd = gs.GelijkstartOntruimingstijdFaseVan });
                    result.FictieveConflicten.Add(new FictiefConflictModel{ FaseVan = gs.FaseNaar, FaseNaar = gs.FaseVan, FictieveOntruimingsTijd = gs.GelijkstartOntruimingstijdFaseNaar });
                }
            }

            foreach (var vs in c.InterSignaalGroep.Voorstarten)
            {
                result.GroenSyncFasen.Add(new GroenSyncModel{ FaseVan = vs.FaseVan, FaseNaar = vs.FaseNaar, Waarde = vs.VoorstartTijd * -1, Richting = -1 });
                result.FictieveConflicten.Add(new FictiefConflictModel{ FaseVan = vs.FaseNaar, FaseNaar = vs.FaseVan, FictieveOntruimingsTijd = vs.VoorstartOntruimingstijd });
            }

            foreach (var lr in c.InterSignaalGroep.LateReleases)
            {
                result.GroenSyncFasen.Add(new GroenSyncModel{ FaseVan = lr.FaseVan, FaseNaar = lr.FaseNaar, Waarde = lr.LateReleaseTijd, Richting = 1 });
                result.FictieveConflicten.Add(new FictiefConflictModel{ FaseVan = lr.FaseNaar, FaseNaar = lr.FaseVan, FictieveOntruimingsTijd = lr.LateReleaseOntruimingstijd });
            }

            foreach (var nl in c.InterSignaalGroep.Nalopen)
            {
                if (nl.MaximaleVoorstart.HasValue &&
                    !result.GroenSyncFasen.Any(x => x.FaseVan == nl.FaseNaar && x.FaseNaar == nl.FaseVan))
                {
                    result.GroenSyncFasen.Add(new GroenSyncModel{ FaseVan = nl.FaseNaar, FaseNaar = nl.FaseVan, Waarde = nl.MaximaleVoorstart.Value, Richting = 1 });
                }
            }

            return result;
        }

        public static (List<GroenSyncModel> oneWay, List<(GroenSyncModel m1, GroenSyncModel m2, bool gelijkstart)> twoWay, List<(GroenSyncModel m1, GroenSyncModel m2, bool gelijkstart)> twoWayPedestrians) OrderSyncs(ControllerModel c, GroenSyncDataModel groenSyncData)
        {
            var oneWay = new List<GroenSyncModel>();
            var twoWay = new List<(GroenSyncModel m1, GroenSyncModel m2, bool gelijkstart)>();
            var twoWayPedestrians = new List<(GroenSyncModel m1, GroenSyncModel m2, bool gelijkstart)>();
            foreach (var first in groenSyncData.GroenSyncFasen)
            {
                if (twoWay.Any(x => x.m2.FaseVan == first.FaseVan && x.m2.FaseNaar == first.FaseNaar))
                {
                    continue;
                }

                var second =
                    groenSyncData.GroenSyncFasen.FirstOrDefault(
                        x => x.FaseVan == first.FaseNaar && x.FaseNaar == first.FaseVan);
                if (second == null) oneWay.Add(first);
                else
                {
                    twoWay.Add((first, second, first.Waarde == 0 && second.Waarde == 0));
                    var fc1 = c.Fasen.FirstOrDefault(x => x.Naam == first.FaseVan);
                    var fc2 = c.Fasen.FirstOrDefault(x => x.Naam == first.FaseNaar);
                    if (fc1 == null || fc2 == null) continue;

                    if (fc1.Type == FaseTypeEnum.Voetganger && fc2.Type == FaseTypeEnum.Voetganger)
                    {
                        twoWayPedestrians.Add((first, second, first.Waarde == 0 && second.Waarde == 0));
                    }
                }
            }

            return (oneWay, twoWay, twoWayPedestrians);
        }
    }
}
