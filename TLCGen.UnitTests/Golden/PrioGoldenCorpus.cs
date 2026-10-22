using System;
using System.Collections.Generic;
using System.Linq;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.UnitTests.Golden
{
    /// <summary>
    /// Synthetic controllers for the prio golden-master tests. Together they cover every vehicle type, every
    /// melding type and the prio options with their own code paths in PrioCodeGenerator.
    /// The controllers only need to exercise the generator; they are not meant to be sensible junctions.
    /// Changing a controller here changes its baseline: regenerate it (see PrioGoldenMasterTests).
    /// </summary>
    internal static class PrioGoldenCorpus
    {
        public static readonly IReadOnlyDictionary<string, Func<ControllerModel>> Cases = new Dictionary<string, Func<ControllerModel>>
        {
            ["VehicleTypesKar"] = VehicleTypesKar,
            ["VehicleTypesKarCcol9"] = () =>
            {
                var c = VehicleTypesKar();
                c.Data.CCOLVersie = CCOLVersieEnum.CCOL9;
                c.Data.Intergroen = false;
                return c;
            },
            ["MeldingTypesBus"] = MeldingTypesBus,
            ["AntiJutterWisselLijnnummers"] = AntiJutterWisselLijnnummers,
            ["Fiets"] = Fiets,
            ["Ris"] = Ris,
            ["HulpdienstGeneriek"] = HulpdienstGeneriek,
            ["HulpdienstLegacy"] = HulpdienstLegacy,
            ["HulpdienstBlokkerenAlle"] = () => HulpdienstBlokkeren(false),
            ["HulpdienstBlokkerenLangzaamVerkeer"] = () => HulpdienstBlokkeren(true),
            ["KarSgInParametersUitgangPerFase"] = () => KarSgHogeNummers(true),
            ["KarSgHogeNummers"] = () => KarSgHogeNummers(false),
            ["NaloopMetPrio"] = NaloopMetPrio,
            ["Randgevallen"] = Randgevallen,
        };

        #region Cases

        // one ingreep with KAR in- and uitmelding for every vehicle type
        private static ControllerModel VehicleTypesKar()
        {
            var c = CreateController(("02", FaseTypeEnum.Auto), ("05", FaseTypeEnum.Auto), ("22", FaseTypeEnum.Fiets),
                ("41", FaseTypeEnum.OV), ("42", FaseTypeEnum.OV), ("61", FaseTypeEnum.Auto));

            AddIngreep(c, "41", "tram", PrioIngreepVoertuigTypeEnum.Tram, Kar(true), Kar(false));
            AddIngreep(c, "41", "bus", PrioIngreepVoertuigTypeEnum.Bus, Kar(true), Kar(false));
            AddIngreep(c, "42", "bus", PrioIngreepVoertuigTypeEnum.Bus, Kar(true), Kar(false));
            AddIngreep(c, "22", "fiets", PrioIngreepVoertuigTypeEnum.Fiets, Kar(true), Kar(false));
            AddIngreep(c, "02", "vrw", PrioIngreepVoertuigTypeEnum.Vrachtwagen, Kar(true), Kar(false));
            AddIngreep(c, "05", "auto", PrioIngreepVoertuigTypeEnum.Auto, Kar(true), Kar(false));
            AddIngreep(c, "05", "ng", PrioIngreepVoertuigTypeEnum.NG, Kar(true), Kar(false));
            AddIngreep(c, "61", "hd", PrioIngreepVoertuigTypeEnum.Hulpdienst, Kar(true), Kar(false));

            return c;
        }

        // every melding type (and input type) as in- and uitmelding on a bus ingreep, with opvang storing
        private static ControllerModel MeldingTypesBus()
        {
            var c = CreateController(("02", FaseTypeEnum.Auto), ("05", FaseTypeEnum.Auto), ("41", FaseTypeEnum.OV), ("42", FaseTypeEnum.OV));
            var fc41 = c.Fasen.Single(x => x.Naam == "41");
            fc41.Detectoren.Add(new DetectorModel { Naam = "413", Type = DetectorTypeEnum.Overig });
            fc41.Detectoren.Add(new DetectorModel { Naam = "414", Type = DetectorTypeEnum.Overig });
            c.Ingangen.Add(new IngangModel { Naam = "ov41in", Type = IngangTypeEnum.OVmelding });
            c.Ingangen.Add(new IngangModel { Naam = "ov41uit", Type = IngangTypeEnum.OVmelding });
            c.SelectieveDetectoren.Add(new SelectieveDetectorModel { Naam = "sd41in", SdType = SelectieveDetectorTypeEnum.VECOM });
            c.SelectieveDetectoren.Add(new SelectieveDetectorModel { Naam = "sd41uit", SdType = SelectieveDetectorTypeEnum.VECOM });

            var karIn = Kar(true);
            karIn.AntiJutterTijdToepassen = true;
            karIn.AntiJutterTijd = 50;
            karIn.OpvangStoring = true;
            karIn.MeldingBijstoring = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, "411", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie);

            var detInTwee = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, "412", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieOp);
            detInTwee.TweedeInput = true;
            detInTwee.RelatedInput2 = "413";
            detInTwee.RelatedInput2Type = PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet;
            detInTwee.AlleenIndienGeenInmelding = true;

            var detInOpvang = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, "413", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectieBezet);
            detInOpvang.AntiJutterTijdToepassen = true;
            detInOpvang.AntiJutterTijd = 30;
            detInOpvang.OpvangStoring = true;
            detInOpvang.MeldingBijstoring = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, "414", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectieHiaat);

            var ingInTwee = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang, "ov41in", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.IngangHoog);
            ingInTwee.TweedeInput = true;
            ingInTwee.RelatedInput2 = "ov41uit";
            ingInTwee.RelatedInput2Type = PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartIngang;

            var opticomIn = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom, "414", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet);
            opticomIn.AntiJutterTijdToepassen = true;
            opticomIn.AntiJutterTijd = 20;

            var sdUit = Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector, "sd41uit", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie);
            sdUit.CheckAltijdOpDsinBijVecom = true;

            var detUitTwee = Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, "412", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie);
            detUitTwee.TweedeInput = true;
            detUitTwee.RelatedInput2 = "413";
            detUitTwee.RelatedInput2Type = PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectieHiaat;
            // inmelding-only options, set on an uitmelding (could be set in the UI by mistake): must be ignored
            detUitTwee.AntiJutterTijdToepassen = true;
            detUitTwee.AntiJutterTijd = 30;
            detUitTwee.AlleenIndienGeenInmelding = true;
            detUitTwee.AlleenIndienRood = true;

            var ingUitTwee = Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang, "ov41uit", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeIngang);
            ingUitTwee.TweedeInput = true;
            ingUitTwee.RelatedInput2 = "ov41in";
            ingUitTwee.RelatedInput2Type = PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.IngangHoog;

            AddIngreep(c, "41", "bus", PrioIngreepVoertuigTypeEnum.Bus,
                karIn,
                Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector, "sd41in", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie),
                detInTwee,
                detInOpvang,
                Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang, "ov41in", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartIngang),
                ingInTwee,
                Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector, "411", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie),
                opticomIn,
                Kar(false),
                sdUit,
                Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, "411", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieOp),
                detUitTwee,
                Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang, "ov41in", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeIngang),
                ingUitTwee,
                Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector, "414", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie),
                Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom, "414", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet));

            // opticom only, without anti-jutter; two uitmeldingen, each on its own input, one with (ignored) anti-jutter
            var opticomUit = Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom, "422", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet);
            opticomUit.AntiJutterTijdToepassen = true;
            opticomUit.AntiJutterTijd = 25;
            AddIngreep(c, "42", "bus", PrioIngreepVoertuigTypeEnum.Bus,
                Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom, "421", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet),
                opticomUit,
                Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom, "423", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet));

            return c;
        }

        // anti-jutter for all meldingen, wissel 1 (ingang) and 2 (detector), koplus, lijnnummers and ritcategorie,
        // prio options and geconditioneerde prioriteit
        private static ControllerModel AntiJutterWisselLijnnummers()
        {
            var c = CreateController(("02", FaseTypeEnum.Auto), ("05", FaseTypeEnum.Auto), ("41", FaseTypeEnum.OV), ("42", FaseTypeEnum.OV));
            c.Detectoren.Add(new DetectorModel { Naam = "wis1", Type = DetectorTypeEnum.WisselStandDetector });
            c.Ingangen.Add(new IngangModel { Naam = "wisc1", Type = IngangTypeEnum.WisselContact });
            c.SelectieveDetectoren.Add(new SelectieveDetectorModel { Naam = "sd42", SdType = SelectieveDetectorTypeEnum.VECOM });

            var karIn = Kar(true);
            karIn.KijkNaarWisselStand = true;
            var detIn = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, "411", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie);
            detIn.AlleenIndienRood = true;
            detIn.KijkNaarWisselStand = true;
            detIn.AntiJutterTijdToepassen = true;
            detIn.AntiJutterTijd = 40;

            var tram = AddIngreep(c, "41", "tram", PrioIngreepVoertuigTypeEnum.Tram,
                karIn, detIn, Kar(false),
                Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, "412", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie));
            tram.MeldingenData.AntiJutterVoorAlleInmeldingen = true;
            tram.MeldingenData.AntiJutterTijdVoorAlleInmeldingen = 100;
            tram.MeldingenData.AntiJutterVoorAlleUitmeldingen = true;
            tram.MeldingenData.AntiJutterTijdVoorAlleUitmeldingen = 50;
            tram.MeldingenData.Wissel1 = true;
            tram.MeldingenData.Wissel1Type = PrioIngreepInUitDataWisselTypeEnum.Ingang;
            tram.MeldingenData.Wissel1Input = "wisc1";
            tram.MeldingenData.Wissel1InputVoorwaarde = true;
            tram.MeldingenData.Wissel2 = true;
            tram.MeldingenData.Wissel2Type = PrioIngreepInUitDataWisselTypeEnum.Detector;
            tram.MeldingenData.Wissel2Detector = "wis1";
            tram.MeldingenData.Wissel2InputVoorwaarde = false;
            tram.Koplus = "411";
            tram.VersneldeInmeldingKoplus = NooitAltijdAanUitEnum.SchAan;
            tram.KoplusKijkNaarWisselstand = true;
            tram.NoodaanvraagKoplus = true;
            tram.MinimaleRoodtijd = 30;
            tram.CheckLijnNummer = true;
            tram.CheckRitCategorie = true;
            tram.CheckWagenNummer = true;
            tram.LijnNummers.Add(new OVIngreepLijnNummerModel { Nummer = "1", RitCategorie = "10" });
            tram.LijnNummers.Add(new OVIngreepLijnNummerModel { Nummer = "12", RitCategorie = "20" });
            tram.PrioriteitsNiveau = 2;
            tram.AfkappenConflicten = true;
            tram.VasthoudenGroen = true;
            tram.TussendoorRealiseren = true;
            tram.GeconditioneerdePrioriteit = NooitAltijdAanUitEnum.SchAan;
            tram.GeconditioneerdePrioTeVroeg = 30;
            tram.GeconditioneerdePrioOpTijd = 20;
            tram.GeconditioneerdePrioTeLaat = 10;
            tram.CheckPeriode = true;
            tram.GerelateerdePerioden.Add(new OVIngreepPeriodeModel { Periode = "ochtend" });

            var bus = AddIngreep(c, "42", "bus", PrioIngreepVoertuigTypeEnum.Bus,
                Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector, "sd42", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie),
                Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector, "sd42", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie));
            bus.MeldingenData.AntiJutterVoorAlleInmeldingen = true;
            bus.MeldingenData.AntiJutterTijdVoorAlleInmeldingen = 60;
            bus.CheckLijnNummer = true;
            bus.CheckRitCategorie = true;
            bus.AlleLijnen = true;
            bus.LijnNummers.Add(new OVIngreepLijnNummerModel { Nummer = "300", RitCategorie = "10" });
            bus.PrioriteitsNiveau = 1;
            bus.AfkappenConflictenPrio = true;
            bus.GeenEigenVerklikking = true;

            return c;
        }

        // fiets prioriteit with and without loop
        private static ControllerModel Fiets()
        {
            var c = CreateController(("02", FaseTypeEnum.Auto), ("22", FaseTypeEnum.Fiets), ("24", FaseTypeEnum.Fiets), ("31", FaseTypeEnum.Voetganger));

            var lusIn = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton, "221", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie);
            lusIn.FietsPrioriteitGebruikLus = true;
            lusIn.FietsPrioriteitBlok = 1;
            lusIn.FietsPrioriteitAantalKeerPerCyclus = 1;
            lusIn.FietsPrioriteitMinimumAantalVoertuigen = 3;
            lusIn.FietsPrioriteitMinimumWachttijdVoorPrioriteit = 10;
            AddIngreep(c, "22", "fiets", PrioIngreepVoertuigTypeEnum.Fiets, lusIn,
                Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton, "221", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie));

            var zonderLusIn = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton, null, PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie);
            zonderLusIn.FietsPrioriteitBlok = 2;
            zonderLusIn.FietsPrioriteitAantalKeerPerCyclus = 2;
            zonderLusIn.FietsPrioriteitMinimumWachttijdVoorPrioriteit = 20;
            AddIngreep(c, "24", "fiets", PrioIngreepVoertuigTypeEnum.Fiets, zonderLusIn,
                Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, "241", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie));

            return c;
        }

        // RIS meldingen (with and without eta), fiets prioriteit via RIS, multiple ingrepen per fase and
        // leaving out the ingreep name for a single ingreep per fase
        private static ControllerModel Ris()
        {
            var c = CreateController(("02", FaseTypeEnum.Auto), ("22", FaseTypeEnum.Fiets), ("41", FaseTypeEnum.OV), ("42", FaseTypeEnum.OV));
            c.Data.CCOLVersie = CCOLVersieEnum.CCOL110;
            c.PrioData.WeglatenIngreepNaamBijEnkeleIngreepPerFase = true;
            c.RISData.RISToepassen = true;
            c.RISData.SystemITF = "ITF1";
            AddRisFase(c, "41", 1, 1, 2);
            AddRisFase(c, "42", 2, 3);
            AddRisFase(c, "22", 3, 4);
            c.RISData.RISPelotonLanes.Add(new RISLanePelotonDataModel
            {
                SignalGroupName = "22", RijstrookIndex = 1, Type = RISStationTypeEnum.CYCLIST, PelotonBepalingStart = 0, PelotonBepalingEnd = 100
            });

            AddIngreep(c, "41", "bus", PrioIngreepVoertuigTypeEnum.Bus,
                RisMelding(true, 10, RISVehicleRole.PUBLICTRANSPORT, RISVehicleSubrole.BUS), RisMelding(false, 10, RISVehicleRole.PUBLICTRANSPORT, RISVehicleSubrole.BUS));
            AddIngreep(c, "41", "tram", PrioIngreepVoertuigTypeEnum.Tram,
                RisMelding(true, null, RISVehicleRole.PUBLICTRANSPORT, RISVehicleSubrole.TRAM), Kar(true),
                RisMelding(false, null, RISVehicleRole.PUBLICTRANSPORT, RISVehicleSubrole.TRAM), Kar(false));
            AddIngreep(c, "42", "bus", PrioIngreepVoertuigTypeEnum.Bus,
                RisMelding(true, 5, RISVehicleRole.PUBLICTRANSPORT, RISVehicleSubrole.BUS), RisMelding(false, 5, RISVehicleRole.PUBLICTRANSPORT, RISVehicleSubrole.BUS));

            var fietsRis = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton, "221", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie);
            fietsRis.FietsPrioriteitGebruikLus = true;
            fietsRis.FietsPrioriteitGebruikRIS = true;
            fietsRis.FietsPrioriteitMinimumAantalVoertuigen = 3;
            fietsRis.FietsPrioriteitMinimumAantalVoertuigenRIS = 2;
            fietsRis.RisStart = 0;
            fietsRis.RisEnd = 150;
            fietsRis.RisRole = RISVehicleRole.DEFAULT;
            fietsRis.RisSubrole = RISVehicleSubrole.UNKNOWN;
            AddIngreep(c, "22", "fiets", PrioIngreepVoertuigTypeEnum.Fiets, fietsRis,
                Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton, "221", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie));

            return c;
        }

        // generic hulpdienst ingrepen: sirene, inmelding ook door, meerealiserend, opticom
        private static ControllerModel HulpdienstGeneriek()
        {
            var c = CreateController(("02", FaseTypeEnum.Auto), ("05", FaseTypeEnum.Auto), ("08", FaseTypeEnum.Auto), ("41", FaseTypeEnum.OV));
            c.PrioData.BlokkeerNietConflictenBijHDIngreep = true;
            c.PrioData.BlokkeerNietConflictenAlleenLangzaamVerkeer = true;

            var hd02 = AddIngreep(c, "02", "hd", PrioIngreepVoertuigTypeEnum.Hulpdienst, Kar(true), Kar(false));
            hd02.CheckOpSirene = true;
            hd02.InmeldingOokDoorToepassen = true;
            hd02.InmeldingOokDoorFase = 3;
            hd02.MeerealiserendeIngrepen.Add(new PrioIngreepMeerealiserendeIngreepModel { FaseCyclus = "05", PrioIngreep = "hd" });

            var hd05 = AddIngreep(c, "05", "hd", PrioIngreepVoertuigTypeEnum.Hulpdienst, Kar(true),
                Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom, "051", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet),
                Kar(false),
                Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom, "052", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet));
            hd05.MeerealiserendeIngrepen.Add(new PrioIngreepMeerealiserendeIngreepModel { FaseCyclus = "02", PrioIngreep = "hd" });

            AddIngreep(c, "08", "hd", PrioIngreepVoertuigTypeEnum.Hulpdienst,
                Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, "081", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie),
                Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, "082", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie));

            AddIngreep(c, "41", "bus", PrioIngreepVoertuigTypeEnum.Bus, Kar(true), Kar(false));

            return c;
        }

        // legacy HD ingrepen (c.PrioData.HDIngrepen) with KAR, RIS and Opticom
        private static ControllerModel HulpdienstLegacy()
        {
            var c = CreateController(("02", FaseTypeEnum.Auto), ("05", FaseTypeEnum.Auto), ("08", FaseTypeEnum.Auto), ("41", FaseTypeEnum.OV));
            c.Data.CCOLVersie = CCOLVersieEnum.CCOL110;
            c.RISData.RISToepassen = true;
            c.RISData.SystemITF = "ITF1";
            AddRisFase(c, "08", 1, 1, 2);

            var hd02 = AddHDIngreep(c, "02");
            hd02.KAR = true;
            hd02.KARInmeldingFilterTijd = 20;
            hd02.KARUitmeldingFilterTijd = 25;
            hd02.Sirene = true;
            hd02.InmeldingOokDoorToepassen = true;
            hd02.InmeldingOokDoorFase = 5;
            hd02.MeerealiserendeFaseCycli.Add(new HDIngreepMeerealiserendeFaseCyclusModel { FaseCyclus = "05" });

            var hd05 = AddHDIngreep(c, "05");
            hd05.KAR = true;
            hd05.KARToepassenFilterTijden = false;
            hd05.Opticom = true;
            hd05.OpticomRelatedInput = "051";
            hd05.OpticomInmeldingFilterTijd = 30;
            hd05.MeerealiserendeFaseCycli.Add(new HDIngreepMeerealiserendeFaseCyclusModel { FaseCyclus = "02" });

            var hd08 = AddHDIngreep(c, "08");
            hd08.RIS = true;
            hd08.RisStart = 0;
            hd08.RisEnd = 200;
            hd08.RisEta = null;
            hd08.RisImportance = RISVehicleImportance.I02;

            AddIngreep(c, "41", "bus", PrioIngreepVoertuigTypeEnum.Bus, Kar(true), Kar(false));

            return c;
        }

        // edge cases: KAR on a non-numeric fase (bus and hulpdienst), fiets without lus but with a left-over input,
        // a left-over fiets RIS flag on other melding types, opvang storing on a melding type that does not support it
        private static ControllerModel Randgevallen()
        {
            var c = CreateController(("02", FaseTypeEnum.Auto), ("22", FaseTypeEnum.Fiets), ("41", FaseTypeEnum.OV), ("61a", FaseTypeEnum.Auto));

            AddIngreep(c, "61a", "bus", PrioIngreepVoertuigTypeEnum.Bus, Kar(true), Kar(false));
            AddIngreep(c, "61a", "hd", PrioIngreepVoertuigTypeEnum.Hulpdienst, Kar(true), Kar(false));

            var fietsZonderLus = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton, "221", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie);
            fietsZonderLus.FietsPrioriteitGebruikLus = false;
            AddIngreep(c, "22", "fiets", PrioIngreepVoertuigTypeEnum.Fiets, fietsZonderLus,
                Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton, null, PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie));

            var detMetFietsRis = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, "411", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie);
            detMetFietsRis.FietsPrioriteitGebruikRIS = true;
            var ingangOpvang = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang, "ov41in", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartIngang);
            ingangOpvang.OpvangStoring = true;
            ingangOpvang.MeldingBijstoring = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, "413", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie);
            // a melding bij storing of another type than Detector (used to crash the generator for RIS): ignored
            var vecomOpvangRis = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector, "414", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie);
            vecomOpvangRis.OpvangStoring = true;
            vecomOpvangRis.MeldingBijstoring = Melding(true, PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde, null, PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie);
            AddIngreep(c, "41", "bus", PrioIngreepVoertuigTypeEnum.Bus, detMetFietsRis, ingangOpvang, vecomOpvangRis,
                Melding(false, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, "412", PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie));

            return c;
        }

        // blocking non-conflicts during a HD ingreep (all fasen, or langzaam verkeer only), with generic HD ingrepen
        // on two auto fasen and a legacy HD ingreep; langzaam verkeer (22, 31) has no HD ingrepen (PrioVoertuigTypes)
        private static ControllerModel HulpdienstBlokkeren(bool alleenLangzaamVerkeer)
        {
            var c = CreateController(("02", FaseTypeEnum.Auto), ("05", FaseTypeEnum.Auto), ("08", FaseTypeEnum.Auto),
                ("22", FaseTypeEnum.Fiets), ("31", FaseTypeEnum.Voetganger), ("41", FaseTypeEnum.OV));
            c.PrioData.BlokkeerNietConflictenBijHDIngreep = true;
            c.PrioData.BlokkeerNietConflictenAlleenLangzaamVerkeer = alleenLangzaamVerkeer;

            AddIngreep(c, "02", "hd", PrioIngreepVoertuigTypeEnum.Hulpdienst, Kar(true), Kar(false));
            AddIngreep(c, "05", "hd", PrioIngreepVoertuigTypeEnum.Hulpdienst, Kar(true), Kar(false));
            AddHDIngreep(c, "08").KAR = true;
            AddIngreep(c, "41", "bus", PrioIngreepVoertuigTypeEnum.Bus, Kar(true), Kar(false));

            return c;
        }

        // signal group numbers above 200, KAR signal group numbers in parameters (or not),
        // prio output per fase, uber counter and hard signal group parameters
        private static ControllerModel KarSgHogeNummers(bool inParameters)
        {
            var c = CreateController(("02", FaseTypeEnum.Auto), ("05", FaseTypeEnum.Auto), ("41", FaseTypeEnum.OV), ("241", FaseTypeEnum.OV));
            c.PrioData.KARSignaalGroepNummersInParameters = inParameters;
            c.PrioData.VerlaagHogeSignaalGroepNummers = true;
            c.PrioData.PrioUitgangPerFase = inParameters;
            c.PrioData.VerklikkenPrioTellerUber = NooitAltijdAanUitEnum.SchAan;
            c.PrioData.CheckOpDSIN = true;
            c.PrioData.PrioIngreepSignaalGroepParametersHard = inParameters;
            foreach (var fc in c.Fasen.Where(x => x.Naam is "41" or "241")) fc.PrioIngreep = true;

            var bus241 = AddIngreep(c, "241", "bus", PrioIngreepVoertuigTypeEnum.Bus, Kar(true), Kar(false));
            bus241.InmeldingOokDoorToepassen = true;
            bus241.InmeldingOokDoorFase = 41;
            AddIngreep(c, "41", "bus", PrioIngreepVoertuigTypeEnum.Bus, Kar(true), Kar(false));

            var hd05 = AddIngreep(c, "05", "hd", PrioIngreepVoertuigTypeEnum.Hulpdienst, Kar(true), Kar(false));
            hd05.InmeldingOokDoorToepassen = true;
            hd05.InmeldingOokDoorFase = 2;

            var hd02 = AddHDIngreep(c, "02");
            hd02.KAR = true;
            hd02.InmeldingOokDoorToepassen = true;
            hd02.InmeldingOokDoorFase = 5;

            return c;
        }

        // nalopen (einde groen and cyclisch verlenggroen) whose naloop fase conflicts with a prio fase
        private static ControllerModel NaloopMetPrio()
        {
            var c = CreateController(("02", FaseTypeEnum.Auto), ("05", FaseTypeEnum.Auto), ("08", FaseTypeEnum.Auto), ("09", FaseTypeEnum.Auto), ("41", FaseTypeEnum.OV));
            RemoveConflict(c, "02", "08");
            RemoveConflict(c, "05", "09");
            c.InterSignaalGroep.Nalopen.Add(new NaloopModel
            {
                FaseVan = "02", FaseNaar = "08", Type = NaloopTypeEnum.EindeGroen,
                Tijden = { new NaloopTijdModel { Type = NaloopTijdTypeEnum.EindeGroen, Waarde = 50 } }
            });
            c.InterSignaalGroep.Nalopen.Add(new NaloopModel
            {
                FaseVan = "05", FaseNaar = "09", Type = NaloopTypeEnum.CyclischVerlengGroen,
                Tijden = { new NaloopTijdModel { Type = NaloopTijdTypeEnum.EindeVerlengGroen, Waarde = 30 } }
            });

            AddIngreep(c, "41", "bus", PrioIngreepVoertuigTypeEnum.Bus, Kar(true), Kar(false));

            return c;
        }

        #endregion // Cases

        #region Helpers

        private static ControllerModel CreateController(params (string naam, FaseTypeEnum type)[] fasen)
        {
            var c = new ControllerModel();
            c.Data.SetSegmentOutputs();
            c.Data.Naam = "GM";
            c.Data.CCOLVersie = CCOLVersieEnum.CCOL100;
            c.Data.Intergroen = true;
            c.PrioData.PrioIngreepType = PrioIngreepTypeEnum.GeneriekePrioriteit;

            var mg1 = new GroentijdenSetModel { Type = GroentijdenTypeEnum.MaxGroentijden, Naam = "MG1" };
            var mg2 = new GroentijdenSetModel { Type = GroentijdenTypeEnum.MaxGroentijden, Naam = "MG2" };
            c.GroentijdenSets.Add(mg1);
            c.GroentijdenSets.Add(mg2);
            c.PeriodenData.DefaultPeriodeNaam = "dal";
            c.PeriodenData.DefaultPeriodeGroentijdenSet = "MG1";
            c.PeriodenData.Perioden.Add(new PeriodeModel
            {
                StartTijd = new TimeSpan(6, 0, 0), EindTijd = new TimeSpan(9, 0, 0), Naam = "ochtend",
                GroentijdenSet = "MG2", Type = PeriodeTypeEnum.Groentijden, DagCode = PeriodeDagCodeEnum.Werkdagen
            });

            c.ModuleMolen.LangstWachtendeAlternatief = true;
            c.ModuleMolen.WachtModule = "ML1";

            foreach (var (naam, type) in fasen)
            {
                var fc = new FaseCyclusModel { Naam = naam, Type = type };
                switch (type)
                {
                    case FaseTypeEnum.Auto:
                    case FaseTypeEnum.OV:
                        fc.Detectoren.Add(new DetectorModel { Naam = naam + "1", Type = DetectorTypeEnum.Kop });
                        fc.Detectoren.Add(new DetectorModel { Naam = naam + "2", Type = DetectorTypeEnum.Lang });
                        break;
                    case FaseTypeEnum.Fiets:
                        fc.Detectoren.Add(new DetectorModel { Naam = naam + "1", Type = DetectorTypeEnum.Kop });
                        fc.Detectoren.Add(new DetectorModel { Naam = naam + "2", Type = DetectorTypeEnum.Verweg });
                        fc.Detectoren.Add(new DetectorModel { Naam = "k" + naam, Type = DetectorTypeEnum.Knop });
                        break;
                    case FaseTypeEnum.Voetganger:
                        fc.Detectoren.Add(new DetectorModel { Naam = naam + "1", Type = DetectorTypeEnum.KnopBuiten });
                        fc.Detectoren.Add(new DetectorModel { Naam = naam + "2", Type = DetectorTypeEnum.KnopBinnen });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
                c.Fasen.Add(fc);

                c.ModuleMolen.Modules.Add(new ModuleModel
                {
                    Naam = $"ML{c.ModuleMolen.Modules.Count + 1}",
                    Fasen = { new ModuleFaseCyclusModel { FaseCyclus = naam } }
                });
                c.ModuleMolen.FasenModuleData.Add(new FaseCyclusModuleDataModel { FaseCyclus = naam });
                mg1.Groentijden.Add(new GroentijdModel { FaseCyclus = naam, Waarde = 300 });
                mg2.Groentijden.Add(new GroentijdModel { FaseCyclus = naam, Waarde = 200 });
                c.PrioData.PrioIngreepSignaalGroepParameters.Add(new PrioIngreepSignaalGroepParametersModel { FaseCyclus = naam });
            }

            // every fase conflicts with every other fase; cases remove conflicts where needed
            foreach (var van in c.Fasen)
            {
                foreach (var naar in c.Fasen.Where(x => x != van))
                {
                    c.InterSignaalGroep.Conflicten.Add(new ConflictModel { FaseVan = van.Naam, FaseNaar = naar.Naam, Waarde = 30, GarantieWaarde = 30 });
                }
            }

            return c;
        }

        private static void RemoveConflict(ControllerModel c, string fc1, string fc2)
        {
            c.InterSignaalGroep.Conflicten.RemoveAll(x => x.FaseVan == fc1 && x.FaseNaar == fc2 || x.FaseVan == fc2 && x.FaseNaar == fc1);
        }

        private static void AddRisFase(ControllerModel c, string fc, int approachId, params int[] laneIds)
        {
            var risFc = new RISFaseCyclusDataModel { FaseCyclus = fc, ApproachID = approachId };
            for (var i = 0; i < laneIds.Length; i++)
            {
                risFc.LaneData.Add(new RISFaseCyclusLaneDataModel
                {
                    SignalGroupName = fc, RijstrookIndex = i + 1, LaneID = laneIds[i], SystemITF = c.RISData.SystemITF
                });
            }
            c.RISData.RISFasen.Add(risFc);
        }

        private static PrioIngreepModel AddIngreep(ControllerModel c, string fc, string naam, PrioIngreepVoertuigTypeEnum type,
            params PrioIngreepInUitMeldingModel[] meldingen)
        {
            var prio = new PrioIngreepModel
            {
                FaseCyclus = fc, Naam = naam, Type = type,
                RijTijdOngehinderd = 150, RijTijdBeperktgehinderd = 200, RijTijdGehinderd = 300,
                OnderMaximum = 50, GroenBewaking = 600, BlokkeertijdNaPrioIngreep = 200, BezettijdPrioGehinderd = 30
            };
            foreach (var m in meldingen)
            {
                (m.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? prio.MeldingenData.Inmeldingen : prio.MeldingenData.Uitmeldingen).Add(m);
            }
            prio.EnsureDummyKARDetectors();
            c.PrioData.PrioIngrepen.Add(prio);
            return prio;
        }

        private static HDIngreepModel AddHDIngreep(ControllerModel c, string fc)
        {
            var hd = new HDIngreepModel
            {
                FaseCyclus = fc, RijTijdOngehinderd = 150, RijTijdBeperktgehinderd = 200, RijTijdGehinderd = 300, GroenBewaking = 600
            };
            hd.DummyKARInmelding.Naam = "dummyhdkarin" + fc;
            hd.DummyKARUitmelding.Naam = "dummyhdkaruit" + fc;
            c.PrioData.HDIngrepen.Add(hd);
            return hd;
        }

        private static PrioIngreepInUitMeldingModel Kar(bool inmelding) =>
            Melding(inmelding, PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding, null, PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie);

        private static PrioIngreepInUitMeldingModel RisMelding(bool inmelding, int? eta, RISVehicleRole role, RISVehicleSubrole subrole)
        {
            var m = Melding(inmelding, PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde, null, PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie);
            m.RisStart = 0;
            m.RisEnd = 300;
            m.RisEta = eta;
            m.RisRole = role;
            m.RisSubrole = subrole;
            m.RisImportance = RISVehicleImportance.I01;
            return m;
        }

        private static PrioIngreepInUitMeldingModel Melding(bool inmelding, PrioIngreepInUitMeldingVoorwaardeTypeEnum type,
            string input, PrioIngreepInUitMeldingVoorwaardeInputTypeEnum inputType)
        {
            return new PrioIngreepInUitMeldingModel
            {
                InUit = inmelding ? PrioIngreepInUitMeldingTypeEnum.Inmelding : PrioIngreepInUitMeldingTypeEnum.Uitmelding,
                Type = type,
                RelatedInput1 = input,
                RelatedInput1Type = inputType
            };
        }

        #endregion // Helpers
    }
}
