using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class PelotonKoppelingCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private string _huks;
        private string _hiks;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _tpelmeet;
        private CCOLGeneratorCodeStringSettingModel _tpelmaxhiaat;
        private CCOLGeneratorCodeStringSettingModel _prmpelgrens;
        private CCOLGeneratorCodeStringSettingModel _mpelvtg;
        private CCOLGeneratorCodeStringSettingModel _mpelin;
        private CCOLGeneratorCodeStringSettingModel _hpelin;
        private CCOLGeneratorCodeStringSettingModel _uspelin;
        private CCOLGeneratorCodeStringSettingModel _tpelrw;
        private CCOLGeneratorCodeStringSettingModel _tpelrwmax;
        private CCOLGeneratorCodeStringSettingModel _tpelstartrw;
        private CCOLGeneratorCodeStringSettingModel _schpelrw;
        private CCOLGeneratorCodeStringSettingModel _schpelmk;
        private CCOLGeneratorCodeStringSettingModel _tpela;
        private CCOLGeneratorCodeStringSettingModel _schpela;
        private CCOLGeneratorCodeStringSettingModel _schpk;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            _myBitmapOutputs = new List<CCOLIOElement>();

            foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Uitgaand && x.Detectoren.Any()))
            {
                CCOLElementCollector.AddKoppelSignaal(1, $"{pk.KruisingNaam}g{pk.GekoppeldeSignaalGroep}", CCOLKoppelSignaalRichtingEnum.Uit);
                foreach(var d in pk.Detectoren)
                {
                    CCOLElementCollector.AddKoppelSignaal(1, $"{pk.KruisingNaam}d{d.DetectorNaam}", CCOLKoppelSignaalRichtingEnum.Uit);
                }
            }
            foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend && x.Detectoren.Any()))
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpelmeet}{pk.GekoppeldeSignaalGroep}", pk.Meetperiode,
                            CCOLElementTimeTypeEnum.TE_type, _tpelmeet, pk.GekoppeldeSignaalGroep));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpelmaxhiaat}{pk.GekoppeldeSignaalGroep}", pk.MaximaalHiaat,
                            CCOLElementTimeTypeEnum.TE_type, _tpelmaxhiaat, pk.GekoppeldeSignaalGroep));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpelgrens}{pk.GekoppeldeSignaalGroep}", pk.MinimaalAantalVoertuigen,
                            CCOLElementTimeTypeEnum.TE_type, _prmpelgrens, pk.GekoppeldeSignaalGroep));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mpelvtg}{pk.GekoppeldeSignaalGroep}", _mpelvtg, pk.GekoppeldeSignaalGroep));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpelin}{pk.GekoppeldeSignaalGroep}", _hpelin, pk.GekoppeldeSignaalGroep));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mpelin}{pk.GekoppeldeSignaalGroep}", _mpelin, pk.GekoppeldeSignaalGroep));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uspelin}{pk.GekoppeldeSignaalGroep}", _uspelin, pk.GekoppeldeSignaalGroep));

                if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Nooit)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpelrw}{pk.GekoppeldeSignaalGroep}", pk.TijdRetourWachtgroen, CCOLElementTimeTypeEnum.TE_type,  _tpelrw, pk.GekoppeldeSignaalGroep));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpelrwmax}{pk.GekoppeldeSignaalGroep}", pk.MaxTijdToepassenRetourWachtgroen, CCOLElementTimeTypeEnum.TE_type,  _tpelrwmax, pk.GekoppeldeSignaalGroep));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpelstartrw}{pk.GekoppeldeSignaalGroep}", pk.TijdTotRetourWachtgroen, CCOLElementTimeTypeEnum.TE_type, _tpelstartrw, pk.GekoppeldeSignaalGroep));
                    if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Altijd)
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpelrw}{pk.GekoppeldeSignaalGroep}", pk.ToepassenRetourWachtgroen == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schpelrw, pk.GekoppeldeSignaalGroep));
                    if (pk.ToepassenMeetkriterium != NooitAltijdAanUitEnum.Altijd && pk.ToepassenMeetkriterium != NooitAltijdAanUitEnum.Nooit)
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpelmk}{pk.GekoppeldeSignaalGroep}", pk.ToepassenMeetkriterium == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schpelmk, pk.GekoppeldeSignaalGroep));
                }

                if (pk.ToepassenAanvraag != NooitAltijdAanUitEnum.Nooit)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpela}{pk.GekoppeldeSignaalGroep}", pk.TijdTotAanvraag, CCOLElementTimeTypeEnum.TE_type, _tpela, pk.GekoppeldeSignaalGroep));
                    if (pk.ToepassenAanvraag != NooitAltijdAanUitEnum.Altijd)
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpela}{pk.GekoppeldeSignaalGroep}", pk.ToepassenAanvraag == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schpela, pk.GekoppeldeSignaalGroep));
                }
                _myBitmapOutputs.Add(new CCOLIOElement(pk.InkomendVerklikking, $"{_uspf}{_uspelin}{pk.GekoppeldeSignaalGroep}"));

                if (pk.AutoIngangsSignalen)
                {
                    CCOLElementCollector.AddKoppelSignaal(1, $"{pk.KruisingNaam}g{pk.GekoppeldeSignaalGroep}", CCOLKoppelSignaalRichtingEnum.In);
                    foreach (var d in pk.Detectoren)
                    {
                        CCOLElementCollector.AddKoppelSignaal(1, $"{pk.KruisingNaam}d{d.DetectorNaam}", CCOLKoppelSignaalRichtingEnum.In);
                    }
                }
                else
                {
                    CCOLElementCollector.AddKoppelSignaal(1, pk.IngangsSignaalFG, $"{pk.KruisingNaam}g{pk.GekoppeldeSignaalGroep}", CCOLKoppelSignaalRichtingEnum.In);
                    foreach (var d in pk.Detectoren)
                    {
                        CCOLElementCollector.AddKoppelSignaal(1, d.KoppelSignaal, $"{pk.KruisingNaam}d{d.DetectorNaam}", CCOLKoppelSignaalRichtingEnum.In);
                    }
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override bool HasCCOLBitmapOutputs() => true;

        public override bool HasCodeForController(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCTop:
                    return c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Type == PelotonKoppelingType.RHDHV);
                case CCOLCodeTypeEnum.RegCPreApplication:
                    return c.PelotonKoppelingenData.PelotonKoppelingen.Any();
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    return c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.IsInkomend);
                default:
                    return false;
            }
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCTop:
                    return 70;
                case CCOLCodeTypeEnum.RegCPreApplication:
                    return 50;
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    return 30;
                case CCOLCodeTypeEnum.RegCPostApplication:
                    return 40;
                case CCOLCodeTypeEnum.RegCWachtgroen:
                    return 30;
            }
            return 0;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCTop:
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend && x.Detectoren.Any()))
                    {
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend && x.Detectoren.Any()))
                        {
                            if (pk.Type == PelotonKoppelingType.RHDHV)
                            {
                                sb.AppendLine($"/* Variabelen tbv vrije koppeling met {pk.KruisingNaam} */");
                                sb.AppendLine($"int iaTime{pk.KruisingNaam}[MAX_VK_ARRAY];");
                                sb.AppendLine($"int iaStatus{pk.KruisingNaam}[MAX_VK_ARRAY];");
                                sb.AppendLine($"int iSizeOfArray{pk.KruisingNaam};");
                                sb.AppendLine($"int iCount{pk.KruisingNaam};");
                                sb.AppendLine($"int iCounterVkop;");
                                sb.AppendLine($"int iOldRW{pk.KruisingNaam};");
                                sb.AppendLine($"bool bSingleRW{pk.KruisingNaam};");
                            }
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPreApplication:
                    var f = false;
                    if(c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend && x.Type == PelotonKoppelingType.RHDHV))
                    {
                        sb.AppendLine($"{ts}/* Vrije koppelingen */");
                        sb.AppendLine($"{ts}/* ================= */");
                        sb.AppendLine($"{ts}if (TS) iCounterVkop = CIF_KLOK[CIF_MINUUT] * 60 + CIF_KLOK[CIF_SECONDE];");
                        f = true;
                    }
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Uitgaand && x.Detectoren.Any()))
                    {
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Uitgaand && x.Detectoren.Any()))
                        {
                            var sgWithD = new Dictionary<FaseCyclusModel, List<string>>();
                            foreach (var d in pk.Detectoren)
                            {
                                var sg = c.Fasen.FirstOrDefault(x => x.Detectoren.Any(x2 => x2.Naam == d.DetectorNaam));
                                if (sg != null)
                                {
                                    if (!sgWithD.ContainsKey(sg))
                                    {
                                        sgWithD.Add(sg, new List<string>());
                                    }
                                    sgWithD[sg].Add(d.DetectorNaam);
                                }
                            }
                            if (!sgWithD.Any()) continue;

                            // TODO: den haag!
                            if (pk.Type == PelotonKoppelingType.DenHaag)
                            {
                                if (f) sb.AppendLine();
                                sb.AppendLine($"{ts}/* Uitgaande peloton koppeling naar {pk.KruisingNaam} */");
                                var ipl = CCOLElementCollector.GetKoppelSignaalCount($"{pk.KruisingNaam}g{pk.GekoppeldeSignaalGroep}", CCOLKoppelSignaalRichtingEnum.Uit);
                                sb.AppendLine($"{ts}IH[{_hpf}{pk.PTPKruising}{_huks}{ipl:00}] = SG[{_fcpf}{pk.GekoppeldeSignaalGroep}] || FG[{_fcpf}{pk.GekoppeldeSignaalGroep}];");
                                foreach (var d in pk.Detectoren)
                                {
                                    ipl = CCOLElementCollector.GetKoppelSignaalCount($"{pk.KruisingNaam}d{d.DetectorNaam}", CCOLKoppelSignaalRichtingEnum.Uit);
                                    sb.AppendLine($"{ts}if (G[{_fcpf}{pk.GekoppeldeSignaalGroep}] && ED[{_dpf}{d.DetectorNaam}]) IH[{_hpf}{pk.PTPKruising}{_huks}{ipl:00}] = !IH[{_hpf}{pk.PTPKruising}{_huks}{ipl:00}];");
                                }
                            }
                            else // RHDHV 
                            {
                                // TODO: dit telt geen voertuigen
                                var ipl = CCOLElementCollector.GetKoppelSignaalCount($"{pk.KruisingNaam}g{pk.GekoppeldeSignaalGroep}", CCOLKoppelSignaalRichtingEnum.Uit);
                                sb.AppendLine($"{ts}/* Uitgaande vrije koppeling {c.Data.Naam} => {pk.KruisingNaam} */");
                                sb.AppendLine($"{ts}/* ---------------------------------------- */");
                                sb.Append($"{ts}IH[{_hpf}{pk.PTPKruising}{_huks}{ipl:00}] =");
                                
                                var firstsg = true;
                                foreach (var sg in sgWithD)
                                {
                                    if (!firstsg) sb.AppendLine($" || ");
                                    firstsg = false;
                                    sb.Append($"SCH[{_schpf}{_schpk}{sg.Key.Naam}] && G[fc{sg.Key.Naam}] && (");
                                    foreach (var d in sg.Value)
                                    {
                                        var firstd = true;
                                        {
                                            if (!firstd) sb.Append($" || ");
                                            firstd = false;
                                            sb.Append($"TDH[{_dpf}{d}] && (CIF_IS[{_dpf}{d}] < CIF_DET_STORING)");
                                        }
                                    }
                                    sb.Append($")");
                                }
                            }
                            f = true;
                        }
                    }
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend && x.Detectoren.Any()))
                    {
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend && x.Detectoren.Any()))
                        {
                            var isg = CCOLElementCollector.GetKoppelSignaalCount($"{pk.KruisingNaam}g{pk.GekoppeldeSignaalGroep}", CCOLKoppelSignaalRichtingEnum.In);

                            if (f) sb.AppendLine();
                            if (pk.Type == PelotonKoppelingType.DenHaag)
                            {
                                sb.AppendLine($"{ts}/* Inkomende peloton koppeling van {pk.KruisingNaam} */");
                                sb.Append($"{ts}IH[{_hpf}{_hpelin}{pk.GekoppeldeSignaalGroep}] = proc_pel_in_V1({_hpf}{pk.PTPKruising}{_hiks}{isg:00}, {_tpf}{_tpelmeet}{pk.GekoppeldeSignaalGroep}, {_tpf}{_tpelmaxhiaat}{pk.GekoppeldeSignaalGroep}, {_prmpf}{_prmpelgrens}{pk.GekoppeldeSignaalGroep}, {_mpf}{_mpelvtg}{pk.GekoppeldeSignaalGroep}, {_mpf}{_mpelin}{pk.GekoppeldeSignaalGroep}, ");
                                foreach (var d in pk.Detectoren)
                                {
                                    var id = CCOLElementCollector.GetKoppelSignaalCount($"{pk.KruisingNaam}d{d.DetectorNaam}", CCOLKoppelSignaalRichtingEnum.In);
                                    sb.Append($"{_hpf}{pk.PTPKruising}{_hiks}{id:00}, ");
                                }
                                sb.AppendLine("END);");
                            }
                            else // RHDHV
                            {
                                sb.AppendLine($"{ts}/* Inkomende vrije koppeling KP017 => KP018 */");
                                sb.AppendLine($"{ts}/* ---------------------------------------- */");
                                sb.AppendLine($"{ts}if (iSizeOfArray{pk.KruisingNaam} > 0)");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}if ((iaTime{pk.KruisingNaam}[0] <= iCounterVkop) &&");
                                sb.AppendLine($"{ts}{ts}    (abs(iaTime{pk.KruisingNaam}[0] - iCounterVkop) < 1800))   /* tbv overgang 3600sec. */");
                                sb.AppendLine($"{ts}{ts}{{");
                                sb.AppendLine($"{ts}{ts}{ts}/* hertarten naloop */");
                                sb.AppendLine($"{ts}{ts}{ts}if (iaStatus{pk.KruisingNaam}[0] == 1) RT[{_tpf}{_tpelrw}{pk.GekoppeldeSignaalGroep}] = TRUE;  /* Verkeer onderweg */");
                                sb.AppendLine($"{ts}{ts}{ts}else                                   RT[{_tpf}{_tpelrw}{pk.GekoppeldeSignaalGroep}] = FALSE; /* Geen verkeer */");
                                sb.AppendLine($"");
                                sb.AppendLine($"{ts}{ts}{ts}for (iCount{pk.KruisingNaam} = 1; iCount{pk.KruisingNaam} < iSizeOfArray{pk.KruisingNaam} && iCount{pk.KruisingNaam} < MAX_VK_ARRAY; iCount{pk.KruisingNaam}++)");
                                sb.AppendLine($"{ts}{ts}{ts}{{");
                                sb.AppendLine($"{ts}{ts}{ts}{ts}iaTime{pk.KruisingNaam}[iCount{pk.KruisingNaam} - 1] = iaTime{pk.KruisingNaam}[iCount{pk.KruisingNaam}];");
                                sb.AppendLine($"{ts}{ts}{ts}{ts}iaTime{pk.KruisingNaam}[iCount{pk.KruisingNaam}] = 0;");
                                sb.AppendLine($"{ts}{ts}{ts}{ts}iaStatus{pk.KruisingNaam}[iCount{pk.KruisingNaam} - 1] = iaStatus{pk.KruisingNaam}[iCount{pk.KruisingNaam}];");
                                sb.AppendLine($"{ts}{ts}{ts}{ts}iaStatus{pk.KruisingNaam}[iCount{pk.KruisingNaam}] = 0;");
                                sb.AppendLine($"{ts}{ts}{ts}}}");
                                sb.AppendLine($"{ts}{ts}{ts}iSizeOfArray{pk.KruisingNaam}--;");
                                sb.AppendLine($"{ts}{ts}}}");
                                sb.AppendLine($"{ts}}}");
                                sb.AppendLine($"{ts}else");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}iaTime{pk.KruisingNaam}[0] = iaStatus{pk.KruisingNaam}[0] = 0;");
                                sb.AppendLine($"{ts}}}");
                                sb.AppendLine($"");
                                sb.AppendLine($"{ts}/* starttijd noteren bij start hulpelement alleen wanneer het peloton het */");
                                sb.AppendLine($"{ts}/* TXD-moment of maximumgroen nog kan halen, anders krijgt starttijd status 0 */");
                                sb.AppendLine($"");
                                sb.AppendLine($"{ts}if (SH[{_hpf}{pk.PTPKruising}{_hiks}{isg:00}] && (iSizeOfArray{pk.KruisingNaam} < (MAX_VK_ARRAY - 2)))");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}iaTime{pk.KruisingNaam}[iSizeOfArray{pk.KruisingNaam}] = (CIF_KLOK[CIF_MINUUT] * 60 + CIF_KLOK[CIF_SECONDE] + T_max[tvsvki05]) % 3600;");
                                sb.AppendLine($"");
                                sb.AppendLine($"{ts}{ts}if ((TVG_max[{_fcpf}{pk.GekoppeldeSignaalGroep}] - TVG_timer[{_fcpf}{pk.GekoppeldeSignaalGroep}]) < (T_max[tvsvki05] + T_max[tnlvki05]) * 10)");
                                sb.AppendLine($"{ts}{ts}{{");
                                sb.AppendLine($"{ts}{ts}{ts}iaStatus{pk.KruisingNaam}[iSizeOfArray{pk.KruisingNaam}] = 0;");
                                sb.AppendLine($"{ts}{ts}{ts}iSizeOfArray{pk.KruisingNaam}++;");
                                sb.AppendLine($"{ts}{ts}}}");
                                sb.AppendLine($"{ts}{ts}else");
                                sb.AppendLine($"{ts}{ts}{{");
                                sb.AppendLine($"{ts}{ts}{ts}iaStatus{pk.KruisingNaam}[iSizeOfArray{pk.KruisingNaam}] = 1;");
                                sb.AppendLine($"{ts}{ts}{ts}iSizeOfArray{pk.KruisingNaam}++;");
                                sb.AppendLine($"{ts}{ts}}}");
                                sb.AppendLine($"{ts}}}");
                                sb.AppendLine($"");
                                sb.AppendLine($"{ts}/* eindtijd noteren bij eind hulpelement alleen als er nog een starttijd is */");
                                sb.AppendLine($"{ts}/* en het verschil meer is dan x, anders krijgt starttijd status 0 */");
                                sb.AppendLine($"{ts}if (EH[{_hpf}{pk.PTPKruising}{_hiks}{isg:00}] && iSizeOfArray{pk.KruisingNaam} < (MAX_VK_ARRAY - 1))");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}iaTime{pk.KruisingNaam}[iSizeOfArray{pk.KruisingNaam}] = (CIF_KLOK[CIF_MINUUT] * 60 + CIF_KLOK[CIF_SECONDE] + T_max[tvsvki05]) % 3600;");
                                sb.AppendLine($"");
                                sb.AppendLine($"{ts}{ts}if (iSizeOfArray{pk.KruisingNaam} > 0)");
                                sb.AppendLine($"{ts}{ts}{{");
                                sb.AppendLine($"{ts}{ts}{ts}if (abs(iaTime{pk.KruisingNaam}[iSizeOfArray{pk.KruisingNaam}] - iaTime{pk.KruisingNaam}[iSizeOfArray{pk.KruisingNaam} - 1]) < T_max[tdbvki05])");
                                sb.AppendLine($"{ts}{ts}{ts}{{");
                                sb.AppendLine($"{ts}{ts}{ts}{ts}iaStatus{pk.KruisingNaam}[iSizeOfArray{pk.KruisingNaam} - 1] = 0;");
                                sb.AppendLine($"{ts}{ts}{ts}}}");
                                sb.AppendLine($"{ts}{ts}}}");
                                sb.AppendLine($"");
                                sb.AppendLine($"{ts}{ts}iaStatus{pk.KruisingNaam}[iSizeOfArray{pk.KruisingNaam}] = 0;");
                                sb.AppendLine($"{ts}{ts}iSizeOfArray{pk.KruisingNaam}++;");
                                sb.AppendLine($"{ts}}}");

                            }
                            f = true;
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend && x.Detectoren.Any()))
                    {
                        var ff = false;
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend && x.Detectoren.Any()))
                        {
                            if (ff) sb.AppendLine();
                            if (pk.Type == PelotonKoppelingType.DenHaag)
                            {
                                sb.AppendLine($"{ts}/* Inkomende peloton koppeling van {pk.GekoppeldeSignaalGroep} van kruising {pk.KruisingNaam} */");
                                if (pk.ToepassenAanvraag != NooitAltijdAanUitEnum.Nooit)
                                {
                                    sb.AppendLine($"{ts}/* timer resetten om aanvraag te zetten */");
                                    sb.AppendLine($"{ts}RT[{_tpf}{_tpela}{pk.GekoppeldeSignaalGroep}] = IH[{_hpf}{_hpelin}{pk.GekoppeldeSignaalGroep}] && !T[{_tpf}{_tpela}{pk.GekoppeldeSignaalGroep}];");
                                }
                                if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Nooit)
                                {
                                    sb.AppendLine($"{ts}/* timer resetten maximale tijd toepassen RW vanaf SG */");
                                    sb.AppendLine($"{ts}RT[{_tpf}{_tpelrwmax}{pk.GekoppeldeSignaalGroep}] = SG[{_fcpf}{pk.GekoppeldeSignaalGroep}];");
                                    sb.AppendLine($"{ts}/* timer resetten om gebied open te houden */");
                                    sb.AppendLine($"{ts}RT[{_tpf}{_tpelstartrw}{pk.GekoppeldeSignaalGroep}] = IH[{_hpf}{_hpelin}{pk.GekoppeldeSignaalGroep}] && !T[{_tpf}{_tpelstartrw}{pk.GekoppeldeSignaalGroep}];");
                                    sb.AppendLine();
                                }
                                if (pk.ToepassenAanvraag != NooitAltijdAanUitEnum.Nooit)
                                {
                                    sb.AppendLine($"{ts}/* zet aanvraag als timer is afgelopen */");
                                    sb.Append($"{ts}if (ET[{_tpf}{_tpela}{pk.GekoppeldeSignaalGroep}]");
                                    if (pk.ToepassenAanvraag != NooitAltijdAanUitEnum.Altijd)
                                    {
                                        sb.Append($" && SCH[{_schpf}{_schpela}{pk.GekoppeldeSignaalGroep}]");
                                    }
                                    sb.AppendLine($") A[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT12;");
                                    sb.AppendLine();
                                }
                                if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Nooit)
                                {
                                    sb.AppendLine($"{ts}/* start vasthoud timer bij einde timer en als de timer nog niet loopt */");
                                    sb.AppendLine($"{ts}RT[{_tpf}{_tpelrw}{pk.GekoppeldeSignaalGroep}] = ET[{_tpf}{_tpelstartrw}{pk.GekoppeldeSignaalGroep}] && !T[{_tpf}{_tpelrw}{pk.GekoppeldeSignaalGroep}];");
                                    sb.AppendLine();
                                }
                                sb.AppendLine($"{ts}MK[{_fcpf}{pk.GekoppeldeSignaalGroep}] &= ~BIT12;");
                                sb.AppendLine();
                                if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Nooit && pk.ToepassenMeetkriterium != NooitAltijdAanUitEnum.Nooit)
                                {
                                    sb.AppendLine($"{ts}/* zet meetkriterium als de vasthoudperiode loopt */");
                                    sb.Append($"{ts}if (T[{_tpf}{_tpelrw}{pk.GekoppeldeSignaalGroep}]");
                                    if (pk.ToepassenMeetkriterium != NooitAltijdAanUitEnum.Altijd)
                                    {
                                        sb.Append($" && SCH[{_schpf}{_schpelmk}{pk.GekoppeldeSignaalGroep}]");
                                    }
                                    sb.AppendLine(")");
                                    sb.AppendLine($"{ts}{{");
                                    sb.AppendLine($"{ts}{ts}MK[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT2 | BIT12;");
                                    sb.AppendLine($"{ts}}}");
                                    sb.AppendLine();
                                }
                                if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Nooit)
                                {
                                    sb.AppendLine($"{ts}/* houd groen vast als de vasthoudperiode loopt,");
                                    sb.AppendLine($"{ts}   de maximale wachttijd nog niet bereikt is,");
                                    sb.AppendLine($"{ts}   tenzij de timer al loopt (besluit wordt niet teruggenomen) */");
                                    sb.Append($"{ts}if (T[{_tpf}{_tpelrw}{pk.GekoppeldeSignaalGroep}]");
                                    if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Altijd)
                                    {
                                        sb.Append($" && SCH[{_schpf}{_schpelrw}{pk.GekoppeldeSignaalGroep}]");
                                    }
                                    sb.AppendLine($" && T[{_tpf}{_tpelrwmax}{pk.GekoppeldeSignaalGroep}])");
                                    sb.AppendLine($"{ts}{{");
                                    sb.AppendLine($"{ts}{ts}RW[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT12;");
                                    sb.AppendLine($"{ts}{ts}PP[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT12;");
                                    sb.AppendLine($"{ts}{ts}PAR[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT12;");
                                    sb.AppendLine($"{ts}}}");
                                    sb.AppendLine($"{ts}else");
                                    sb.AppendLine($"{ts}{{");
                                    sb.AppendLine($"{ts}{ts}PP[{_fcpf}{pk.GekoppeldeSignaalGroep}] &= ~BIT12;");
                                    sb.AppendLine($"{ts}}}");
                                    sb.Append($"{ts}if (!(T[{_tpf}{_tpelrw}{pk.GekoppeldeSignaalGroep}]");
                                    if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Altijd)
                                    {
                                        sb.Append($" && SCH[{_schpf}{_schpelrw}{pk.GekoppeldeSignaalGroep}]");
                                    }
                                    sb.AppendLine($"))");
                                    sb.AppendLine($"{ts}{{");
                                    sb.AppendLine($"{ts}{ts}RW[{_fcpf}{pk.GekoppeldeSignaalGroep}] &= ~BIT12;");
                                    sb.AppendLine($"{ts}}}");
                                }
                            }
                            else // RHDHV
                            {
                                sb.AppendLine($"{ts}/* extra verlengen obv vrije koppeling */");
                                sb.AppendLine($"{ts}meetkriterium_exp((count){_fcpf}{pk.GekoppeldeSignaalGroep}, (bool)(SCH[{_schpf}{_schpelmk}{pk.GekoppeldeSignaalGroep}] && T_max[{_tpf}{_tpelmeet}{pk.GekoppeldeSignaalGroep}] > 0 && T[{_tpf}{_tpelmeet}{pk.GekoppeldeSignaalGroep}]));");
                            }
                            ff = true;
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCWachtgroen:
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend && x.Detectoren.Any()))
                    {
                        var ff = false;
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend && x.Detectoren.Any()))
                        {
                            sb.AppendLine($"{ts}/* Vasthouden {_fcpf}{pk.GekoppeldeSignaalGroep} */");
                            sb.AppendLine($"{ts}IH[{_hpf}{_hpelin}{pk.GekoppeldeSignaalGroep}] = T_max[{_tpf}{_tpelrw}{pk.GekoppeldeSignaalGroep}] > 0 && T[{_tpf}{_tpelrw}{pk.GekoppeldeSignaalGroep}] &&");
                            sb.AppendLine($"{ts}SCH[{_schpf}{_schpelrw}{pk.GekoppeldeSignaalGroep}];");
                            sb.AppendLine($"");
                            sb.AppendLine($"{ts}/* Bewaken RW duur */");
                            sb.AppendLine($"{ts}RT[{_tpf}{_tpelrwmax}{pk.GekoppeldeSignaalGroep}] = SH[{_hpf}{_hpelin}{pk.GekoppeldeSignaalGroep}] && eenkeerRW;");
                            sb.AppendLine($"");
                            sb.AppendLine($"{ts}/* RW opzetten */");
                            sb.AppendLine($"{ts}if (bSingleRW{pk.KruisingNaam} && IH[{_hpf}{_hpelin}{pk.GekoppeldeSignaalGroep}] && !fkaa({_fcpf}{pk.GekoppeldeSignaalGroep}) && !Z[{_fcpf}{pk.GekoppeldeSignaalGroep}] && (T[{_tpf}{_tpelrwmax}{pk.GekoppeldeSignaalGroep}] || RT[{_tpf}{_tpelrwmax}{pk.GekoppeldeSignaalGroep}])) RW[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT14;");
                            sb.AppendLine($"{ts}else/* if (!H[hvki05])*/ RW[{_fcpf}{pk.GekoppeldeSignaalGroep}] &= ~BIT14;");
                            sb.AppendLine($"");
                            sb.AppendLine($"{ts}/* Bewaken eenmalig opzetten RW */");
                            sb.AppendLine($"{ts}if (!(RW[{_fcpf}{pk.GekoppeldeSignaalGroep}] & BIT14) && (iOldRW{pk.KruisingNaam} & BIT14)/* && SCH[schvki05eenkeerRW]*/) bSingleRW{pk.KruisingNaam} = FALSE;");
                            sb.AppendLine($"{ts}if (EG[{_fcpf}{pk.GekoppeldeSignaalGroep}]) bSingleRW{pk.KruisingNaam} = TRUE;");
                            sb.AppendLine($"{ts}iOldRW{pk.KruisingNaam} = RW[{_fcpf}{pk.GekoppeldeSignaalGroep}];");
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPostApplication:
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend && x.Detectoren.Any()))
                    {
                        sb.AppendLine("/* Verklikken inkomende pelotons */");
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend && x.Detectoren.Any()))
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uspelin}{pk.GekoppeldeSignaalGroep}] = IH[{_hpf}{_hpelin}{pk.GekoppeldeSignaalGroep}];");
                        }
                    }
                    return sb.ToString();
            }

            return sb.ToString();
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _huks = CCOLGeneratorSettingsProvider.Default.GetElementName("huks");
            _hiks = CCOLGeneratorSettingsProvider.Default.GetElementName("hiks");

            return base.SetSettings(settings);
        }
    }
}
