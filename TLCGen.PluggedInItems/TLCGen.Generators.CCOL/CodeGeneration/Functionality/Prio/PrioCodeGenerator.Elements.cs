using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: CCOL elements (outputs, helper elements, timers, counters, switches, parameters) and dummy detectors.
    // Hook: CollectCCOLElements. Element names of meldingen come from GetMeldingElements (Meldingen.cs);
    // elements of legacy HD ingrepen from AddAllHDIngreepElements (HDLegacy.cs).
    public partial class PrioCodeGenerator
    {
        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            _myDetectors = new List<DetectorModel>();

            if (c.PrioData.PrioIngreepType != PrioIngreepTypeEnum.GeneriekePrioriteit) return;

            if (c.PrioData.VerklikkenPrioTellerUber == NooitAltijdAanUitEnum.SchAan || c.PrioData.VerklikkenPrioTellerUber == NooitAltijdAanUitEnum.SchUit)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schcovuber}", c.PrioData.VerklikkenPrioTellerUber == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schcovuber));
            }

            if (c.HasPT() || c.HasHD())
            {
                /* Variables independent of signal groups */
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmwta}", c.PrioData.MaxWachttijdAuto, CCOLElementTimeTypeEnum.TS_type, _prmmwta));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmwtfts}", c.PrioData.MaxWachttijdFiets, CCOLElementTimeTypeEnum.TS_type, _prmmwtfts));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmwtvtg}", c.PrioData.MaxWachttijdVoetganger, CCOLElementTimeTypeEnum.TS_type, _prmmwtvtg));

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usmaxwt}", _usmaxwt, c.PrioData.MaximaleWachttijdOverschredenBitmapData));

                if (c.HasKAR())
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uskarmelding}", _uskarmelding, c.PrioData.KARMeldingBitmapData));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uskarog}", _uskarog, c.PrioData.KAROnderGedragBitmapData));

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tkarmelding}", 15, CCOLElementTimeTypeEnum.TE_type, _tkarmelding));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tkarog}", 1440, CCOLElementTimeTypeEnum.TM_type, _tkarog));
                }
            }
            if (c.HasPT())
            {
                /* Variables independent of signal groups */
                if (c.HasDSI())
                {
                    var prmtest1 = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmtestdsivert}", 120, CCOLElementTimeTypeEnum.TS_type, _prmtestdsivert);
                    var prmtest2 = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmtestdsilyn}", 0, CCOLElementTimeTypeEnum.None, _prmtestdsilyn);
                    var prmtest3 = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmtestdsicat}", 10, CCOLElementTimeTypeEnum.None, _prmtestdsicat);
                    prmtest1.Dummy = true;
                    prmtest2.Dummy = true;
                    prmtest3.Dummy = true;
                    _myElements.Add(prmtest1);
                    _myElements.Add(prmtest2);
                    _myElements.Add(prmtest3);
                }

                foreach (var d in c.GetAllDetectors(x => x.Type == DetectorTypeEnum.WisselStandDetector))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schgeenwissel}{d.Naam}", 0, CCOLElementTimeTypeEnum.SCH_type, _schgeenwissel, d.Naam));
                }
                foreach (var d in c.Ingangen.Where(x => x.Type == IngangTypeEnum.WisselContact))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schgeenwissel}{d.Naam}", 0, CCOLElementTimeTypeEnum.SCH_type, _schgeenwissel, d.Naam));
                }
                foreach (var d in c.GetAllDetectors(x => x.Type == DetectorTypeEnum.WisselStandDetector))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schwisselpol}{d.Naam}", 0, CCOLElementTimeTypeEnum.SCH_type, _schwisselpol, d.Naam));
                }
                foreach (var d in c.Ingangen.Where(x => x.Type == IngangTypeEnum.WisselContact))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schwisselpol}{d.Naam}", 0, CCOLElementTimeTypeEnum.SCH_type, _schwisselpol, d.Naam));
                }

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schcheckdstype}", c.PrioData.CheckOpDSIN ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schcheckdstype));

                /* Variables for conflicting signal groups */
                if (!c.PrioData.PrioIngreepSignaalGroepParametersHard)
                {
                    foreach (var ovfc in c.PrioData.PrioIngreepSignaalGroepParameters)
                    {
                        if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, ovfc.FaseCyclus))
                        {
                            continue;
                        }

                        var fct = c.Fasen.First(x => x.Naam == ovfc.FaseCyclus).Type;

                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpmgt}{ovfc.FaseCyclus}", ovfc.PercMaxGroentijdVoorTerugkomen, CCOLElementTimeTypeEnum.None, _prmpmgt, ovfc.FaseCyclus));
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmognt}{ovfc.FaseCyclus}", ovfc.OndergrensNaTerugkomen, CCOLElementTimeTypeEnum.TE_type, _prmognt, ovfc.FaseCyclus));
                        if (fct != FaseTypeEnum.Voetganger)
                        {
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmnofm}{ovfc.FaseCyclus}", ovfc.AantalKerenNietAfkappen, CCOLElementTimeTypeEnum.TE_type, _prmnofm, ovfc.FaseCyclus));
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmgcov}{ovfc.FaseCyclus}", ovfc.MinimumGroentijdConflictOVRealisatie, CCOLElementTimeTypeEnum.TE_type, _prmmgcov, ovfc.FaseCyclus));
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpmgcov}{ovfc.FaseCyclus}", ovfc.PercMaxGroentijdConflictOVRealisatie, CCOLElementTimeTypeEnum.None, _prmpmgcov, ovfc.FaseCyclus));
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmohpmg}{ovfc.FaseCyclus}", ovfc.OphoogpercentageNaAfkappen, CCOLElementTimeTypeEnum.None, _prmohpmg, ovfc.FaseCyclus));
                        }
                    }
                }
            }

            if (c.PrioData.PrioUitgangPerFase)
            {
                foreach (var sg in c.Fasen.Where(x => x.PrioIngreep))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usovinm}{sg.Naam}", _usovinm, sg.PrioIngreepBitmapData, sg.Naam, ""));
                }
            }

            /* Variables for OV */
            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                AddAllIngreepElements(prio, c.PrioData.PrioUitgangPerFase, c);
            }

            /* Variables for HD */
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                AddAllHDIngreepElements(hd, c);
            }

            // RIS prms per fase
            var ovRis = c.PrioData.PrioIngrepen
                .Where(x => x.MeldingenData.Inmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde) ||
                            x.MeldingenData.Uitmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))
                .ToList();
            if (ovRis.Any())
            {
                foreach (var ov in ovRis)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tris}{ov.FaseCyclus}{ov.Naam}", 0, CCOLElementTimeTypeEnum.TE_type, _tris, ov.FaseCyclus));
                }
            }

            if (c.PrioData.KARSignaalGroepNummersInParameters)
            {
                var karSgParms = new List<string>();
                foreach (var sg in c.Fasen)
                {
                    var hasOvKar = c.PrioData.PrioIngrepen.Any(x => x.FaseCyclus == sg.Naam && x.HasPrioIngreepKAR());
                    if (hasOvKar)
                    {
                        if (!int.TryParse(sg.Naam, out var iFc)) continue;
                        if (c.PrioData.VerlaagHogeSignaalGroepNummers && iFc > 200) iFc -= 200;
                        var parm = $"{_prmkarsg}{sg.Naam}";
                        if (!karSgParms.Contains(parm))
                        {
                            karSgParms.Add(parm);
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                                parm, iFc, CCOLElementTimeTypeEnum.None, _prmkarsg, sg.Naam));
                        }
                    }
                }
                foreach (var prio in c.PrioData.PrioIngrepen.Where(x => x.InmeldingOokDoorToepassen && x.InmeldingOokDoorFase > 0))
                {
                    var priofcnr = GetOokDoorFaseName(prio.InmeldingOokDoorFase);
                    var parm = $"{_prmkarsg}{priofcnr}";
                    if (!karSgParms.Contains(parm))
                    {
                        karSgParms.Add(parm);
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                            parm, prio.InmeldingOokDoorFase, CCOLElementTimeTypeEnum.None, _prmkarsg, priofcnr));
                    }
                }
                foreach (var hd in c.PrioData.HDIngrepen.Where(x => x.KAR || x.Opticom))
                {
                    if (!int.TryParse(hd.FaseCyclus, out var iFc)) continue;
                    if (c.PrioData.VerlaagHogeSignaalGroepNummers && iFc > 200) iFc -= 200;
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmkarsghd}{hd.FaseCyclus}", iFc, CCOLElementTimeTypeEnum.None, _prmkarsghd, hd.FaseCyclus));

                    if (hd.InmeldingOokDoorToepassen && hd.InmeldingOokDoorFase > 0)
                    {
                        var hdpriofcnr = GetOokDoorFaseName(hd.InmeldingOokDoorFase);
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmkarsghd}{hdpriofcnr}", hd.InmeldingOokDoorFase, CCOLElementTimeTypeEnum.None, _prmkarsghd, hdpriofcnr));
                    }
                }
            }
        }

        private void AddAllIngreepElements(PrioIngreepModel prio, bool prioUitgangPerFase, ControllerModel c)
        {
            var prioName = CCOLCodeHelper.GetPriorityName(c, prio);

            // Verklikken prio
            if (!prioUitgangPerFase)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usovinm}{prioName}", _usovinm, prio.PrioInmeldingBitmapData, prio.FaseCyclus, prio.Type.GetDescription()));
            }

            // Hulp elementen prio
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hprio}{prioName}", _hprio, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hprioin}{prioName}", _hprioin, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpriouit}{prioName}", _hpriouit, prio.FaseCyclus, prio.Type.GetDescription()));

            // Elementen voor in- en uitmeldingen
            foreach (var melding in prio.MeldingenData.Inmeldingen.Concat(prio.MeldingenData.Uitmeldingen))
            {
                _myElements.AddRange(GetMeldingElements(c, prio, melding, true));
            }

            // Anti-jutter timers indien actief voor alle in- of uitmeldingen gezamenlijk
            if (prio.MeldingenData.AntiJutterVoorAlleInmeldingen && prio.MeldingenData.Inmeldingen.Any())
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tprioin}{prioName}", prio.MeldingenData.AntiJutterTijdVoorAlleInmeldingen, CCOLElementTimeTypeEnum.TE_type, _tprioin, prio.FaseCyclus, prio.Type.GetDescription()));
            }
            if (prio.MeldingenData.AntiJutterVoorAlleUitmeldingen && prio.MeldingenData.Uitmeldingen.Any())
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpriouit}{prioName}", prio.MeldingenData.AntiJutterTijdVoorAlleUitmeldingen, CCOLElementTimeTypeEnum.TE_type, _tpriouit, prio.FaseCyclus, prio.Type.GetDescription()));
            }

            // Generieke elementen: voor elke ingreep
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tbtovg}{prioName}", prio.BezettijdPrioGehinderd, CCOLElementTimeTypeEnum.TE_type, _tbtovg, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_trt}{prioName}", 0, CCOLElementTimeTypeEnum.TE_type, _trt, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_cvc}{prioName}", 999, CCOLElementTimeTypeEnum.CT_type, _cvc, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tgb}{prioName}", prio.GroenBewaking, CCOLElementTimeTypeEnum.TE_type, _tgb, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrto}{prioName}", prio.RijTijdOngehinderd, CCOLElementTimeTypeEnum.TE_type, _prmrto, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrtbg}{prioName}", prio.RijTijdBeperktgehinderd, CCOLElementTimeTypeEnum.TE_type, _prmrtbg, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrtg}{prioName}", prio.RijTijdGehinderd, CCOLElementTimeTypeEnum.TE_type, _prmrtg, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmomx}{prioName}", prio.OnderMaximum, CCOLElementTimeTypeEnum.TE_type, _prmomx, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tblk}{prioName}", prio.BlokkeertijdNaPrioIngreep, CCOLElementTimeTypeEnum.TE_type, _tblk, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmupinagb}{prioName}", 0, CCOLElementTimeTypeEnum.None, _prmupinagb, prio.FaseCyclus, prio.Type.GetDescription()));
            if (prio.CheckOpSirene && prio.Type == PrioIngreepVoertuigTypeEnum.Hulpdienst)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schchecksirene}{prioName}", prio.CheckOpSirene ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schchecksirene, prio.FaseCyclus, prio.Type.GetDescription()));
            }
            if ((prio.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.SchAan ||
                 prio.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.SchUit) &&
                !string.IsNullOrWhiteSpace(prio.Koplus) && prio.Koplus != "NG")
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schvi}{prioName}", prio.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schvi, prio.FaseCyclus, prio.Type.GetDescription()));
            }

            if (prio.MeldingenData.Inmeldingen.Any(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding || x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector) ||
                prio.MeldingenData.Uitmeldingen.Any(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding || x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector))
            {
                var vtgtype = prio.Type
                    switch
                {
                    PrioIngreepVoertuigTypeEnum.Bus => 1,
                    PrioIngreepVoertuigTypeEnum.Tram => 2,
                    _ => 0
                };
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmvtgcat}{prioName}", vtgtype, CCOLElementTimeTypeEnum.None, _prmvtgcat, prioName));
            }

            var opties = 0;
            string sopties;
            if (prio.Type == PrioIngreepVoertuigTypeEnum.Hulpdienst)
            {
                opties = 9005;
                sopties = opties.ToString();
            }
            else
            {
                if (prio.AfkappenConflicten || prio.AfkappenConflictenPrio) opties += 100;
                if (prio.AfkappenConflictenPrio) opties += 300;
                if (prio.TussendoorRealiseren) opties += 3;
                if (prio.VasthoudenGroen) opties += 20;
                sopties = opties == 0 ? "0" : opties.ToString().Replace("0", "");
                if (prio.PrioriteitsNiveau > 0)
                {
                    sopties = prio.PrioriteitsNiveau.ToString() +
                        (sopties.Length == 1 
                            ? "00" 
                            : sopties.Length == 2 
                                ? "0" 
                                : "") + sopties;

                }
            }
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmprio}{prioName}", int.Parse(sopties), CCOLElementTimeTypeEnum.None, _prmprio, prio.FaseCyclus, prio.Type.GetDescription()));

            if (prio.CheckLijnNummer)
            {
                // !!! NOTE !!! "allelijnen" must alway be DIRECTLY above the line prms, cause of the way these prms are used in code
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmallelijnen}{prioName}", prio.AlleLijnen == true ? 1 : 0, CCOLElementTimeTypeEnum.None, _prmallelijnen, prio.FaseCyclus, prio.Type.GetDescription()));
                var n = 1;
                foreach (var l in prio.LijnNummers)
                {
                    if (!int.TryParse(l.Nummer, out var num)) continue;
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmlijn}{prioName}_{n:00}", num, CCOLElementTimeTypeEnum.None, _prmlijn, n.ToString(), prio.FaseCyclus, prio.Type.GetDescription()));
                    ++n;
                }

                if (prio.CheckRitCategorie)
                {
                    n = 1;
                    foreach (var l in prio.LijnNummers)
                    {
                        if (!int.TryParse(l.RitCategorie, out var ritcat)) continue;
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmritcat}{prioName}_{n:00}", ritcat, CCOLElementTimeTypeEnum.None, _prmritcat, n.ToString(), prio.FaseCyclus, prio.Type.GetDescription()));
                        ++n;
                    }
                }
            }

            // Help elements to store wissel condition
            if (prio.HasOVIngreepWissel())
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hwissel}{prioName}", _hwissel, prio.FaseCyclus, prio.Type.GetDescription()));
            }

            if (prio.HasPrioIngreepKAR())
            {
                _myDetectors.AddRange(prio.GetDummyInDetectors());
                _myDetectors.AddRange(prio.GetDummyUitDetectors());
            }

            if (prio.MeldingenData.Inmeldingen.Any(x => x.AlleenIndienRood) || prio.NoodaanvraagKoplus)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tovminrood}{prioName}", prio.MinimaleRoodtijd, CCOLElementTimeTypeEnum.TE_type, _tovminrood, prio.FaseCyclus, prio.Type.GetDescription()));
            }

            var inRis = prio.MeldingenData.Inmeldingen.Where(x =>
                x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde ||
                x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton && x.FietsPrioriteitGebruikRIS).ToList();
            var uitRis = prio.MeldingenData.Uitmeldingen.Where(x =>
                x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde).ToList();

            if (inRis.Any() || uitRis.Any())
            {
                foreach (var inR in inRis)
                {
                    var fcRis = c.RISData.RISFasen.FirstOrDefault(x => x.FaseCyclus == prio.FaseCyclus);

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisstart}{prioName}", inR.RisStart, CCOLElementTimeTypeEnum.None, _prmrisstart, prio.FaseCyclus));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisend}{prioName}", inR.RisEnd, CCOLElementTimeTypeEnum.None, _prmrisend, prio.FaseCyclus));
                    if (inR.RisEta.HasValue)
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmriseta}{prioName}", inR.RisEta.Value, CCOLElementTimeTypeEnum.TE_type, _prmriseta, prio.FaseCyclus));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisrole}{prioName}", (int)inR.RisRole, CCOLElementTimeTypeEnum.None, _prmrisrole, prio.FaseCyclus));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrissubrole}{prioName}", (int)inR.RisSubrole, CCOLElementTimeTypeEnum.None, _prmrissubrole, prio.FaseCyclus));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisimportance}{prioName}", (int)inR.RisImportance, CCOLElementTimeTypeEnum.None, _prmrisimportance, prio.FaseCyclus));

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisstationtype}{prioName}", 0, CCOLElementTimeTypeEnum.None, _prmrisstationtype, prio.FaseCyclus));

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisgrenspriotype}{prioName}", 9, CCOLElementTimeTypeEnum.None, _prmrisgrenspriotype, prio.FaseCyclus));

                    // we only need these elements if they differ from the elements already created for signalgroups
                    if (fcRis != null && prioName != prio.FaseCyclus)
                    {
                        var approach = $"{_prmrisapproachid}{prioName}";
                        if (_myElements.All(x => x.Naam != approach))
                        {
                            _myElements.Add(
                                CCOLGeneratorSettingsProvider.Default.CreateElement(
                                    approach, 999, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "",
                                    PrioCodeGeneratorHelper.CAT_Prioriteren, PrioCodeGeneratorHelper.SUBCAT_OpenbaarVervoer));
                        }
                        foreach (var lane in fcRis.LaneData)
                        {
                            _myElements.Add(
                                CCOLGeneratorSettingsProvider.Default.CreateElement(
                                    $"{_prmrislaneid}{prioName}_{lane.RijstrookIndex}", 
                                    lane.LaneID, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "",
                                    PrioCodeGeneratorHelper.CAT_Prioriteren, PrioCodeGeneratorHelper.SUBCAT_OpenbaarVervoer));
                        }
                    }
                }
            }

            if (prio.Type == PrioIngreepVoertuigTypeEnum.Hulpdienst && prio.MeerealiserendeIngrepen.Any())
            {
                foreach (var mrPrio in prio.MeerealiserendeIngrepen)
                {
                    var mrIngreep = c.GetPrioIngreep(mrPrio);
                    if (mrIngreep == null) continue;
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_schpriomr}{CCOLCodeHelper.GetPriorityName(c, mrIngreep)}met{prioName}", 1,
                            CCOLElementTimeTypeEnum.SCH_type, _schpriomr, CCOLCodeHelper.GetPriorityName(c, mrIngreep), prio.Type.GetDescription()));
                }
            }
        }
    }
}
