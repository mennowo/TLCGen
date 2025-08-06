﻿using System.Collections.Generic;
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
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _tpelmeet;
        private CCOLGeneratorCodeStringSettingModel _tpelmaxhiaat;
        private CCOLGeneratorCodeStringSettingModel _prmpelgrens;
        private CCOLGeneratorCodeStringSettingModel _mpelvtg;
        private CCOLGeneratorCodeStringSettingModel _mpelin;
        private CCOLGeneratorCodeStringSettingModel _hpelin;
        private CCOLGeneratorCodeStringSettingModel _uspelin;
        private CCOLGeneratorCodeStringSettingModel _tpelnl;
        private CCOLGeneratorCodeStringSettingModel _tpelrw;
        private CCOLGeneratorCodeStringSettingModel _prmpelverschuif;
        private CCOLGeneratorCodeStringSettingModel _tpelrwmax;
        private CCOLGeneratorCodeStringSettingModel _tpelstartrw;
        private CCOLGeneratorCodeStringSettingModel _schpelrw;
        private CCOLGeneratorCodeStringSettingModel _schpelmk;
        private CCOLGeneratorCodeStringSettingModel _tpela;
        private CCOLGeneratorCodeStringSettingModel _hpeltegenh;
        private CCOLGeneratorCodeStringSettingModel _schpela;
        private CCOLGeneratorCodeStringSettingModel _schpku;
        private CCOLGeneratorCodeStringSettingModel _hpku;
        private CCOLGeneratorCodeStringSettingModel _hpkud;
#pragma warning restore 0649
        private string _hplact;
        private string _huks;
        private string _hiks;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen)
            {
                if (!pk.Detectoren.Any() && (/*pk.Type == PelotonKoppelingTypeEnum.DenHaag ||*/ pk.Richting == PelotonKoppelingRichtingEnum.Uitgaand)) continue;
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
                if (!pk.IsIntern)
                {
                    var signals = ((IHaveKoppelSignalen)pk).UpdateKoppelSignalen();
                    foreach (var s in signals)
                    {
                        CCOLElementCollector.AddKoppelSignaal(pk.PTPKruising, s.Count, s.Name, s.Richting);
                    }
                }
                switch (pk.Type)
                {
                    case PelotonKoppelingTypeEnum.DenHaag:
                        switch (pk.Richting)
                        {
                            case PelotonKoppelingRichtingEnum.Uitgaand:
                                // schakelaar voor elke fase met detectie die mee doet aan de koppeling
                                if (pk.IsIntern)
                                {
                                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpku}{pk.KoppelingNaam}", _hpku, pk.KoppelingNaam));
                                }
                                foreach (var sg in sgWithD)
                                {
                                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpku}{pk.KoppelingNaam}", 1, CCOLElementTimeTypeEnum.SCH_type, _schpku, pk.KoppelingNaam, sg.Key.Naam));
                                    foreach (var d in sg.Value)
                                    {
                                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpkud}{pk.KoppelingNaam}", _hpkud, d, pk.KoppelingNaam));
                                    }
                                }
                                break;
                            case PelotonKoppelingRichtingEnum.Inkomend:
                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpelmeet}{pk.KoppelingNaam}", pk.Meetperiode,
                                    CCOLElementTimeTypeEnum.TE_type, _tpelmeet, pk.KoppelingNaam, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpelmaxhiaat}{pk.KoppelingNaam}", pk.MaximaalHiaat,
                                    CCOLElementTimeTypeEnum.TE_type, _tpelmaxhiaat, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpelgrens}{pk.KoppelingNaam}", pk.MinimaalAantalVoertuigen,
                                    CCOLElementTimeTypeEnum.None, _prmpelgrens, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mpelvtg}{pk.KoppelingNaam}", _mpelvtg, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpelin}{pk.KoppelingNaam}", _hpelin, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpeltegenh}{pk.KoppelingNaam}", _hpeltegenh, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mpelin}{pk.KoppelingNaam}", _mpelin, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uspelin}{pk.KoppelingNaam}", _uspelin, pk.InkomendVerklikking, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));

                                if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Nooit)
                                {
                                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpelrw}{pk.KoppelingNaam}", pk.TijdRetourWachtgroen, CCOLElementTimeTypeEnum.TE_type, _tpelrw, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpelrwmax}{pk.KoppelingNaam}", pk.MaxTijdToepassenRetourWachtgroen, CCOLElementTimeTypeEnum.TE_type, _tpelrwmax, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpelstartrw}{pk.KoppelingNaam}", pk.TijdTotRetourWachtgroen, CCOLElementTimeTypeEnum.TE_type, _tpelstartrw, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                    if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Altijd)
                                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpelrw}{pk.KoppelingNaam}", pk.ToepassenRetourWachtgroen == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schpelrw, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                    if (pk.ToepassenMeetkriterium != NooitAltijdAanUitEnum.Altijd && pk.ToepassenMeetkriterium != NooitAltijdAanUitEnum.Nooit)
                                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpelmk}{pk.KoppelingNaam}", pk.ToepassenMeetkriterium == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schpelmk, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                }

                                if (pk.ToepassenAanvraag != NooitAltijdAanUitEnum.Nooit)
                                {
                                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpela}{pk.KoppelingNaam}", pk.TijdTotAanvraag, CCOLElementTimeTypeEnum.TE_type, _tpela, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                    if (pk.ToepassenAanvraag != NooitAltijdAanUitEnum.Altijd)
                                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpela}{pk.KoppelingNaam}", pk.ToepassenAanvraag == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schpela, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                }
                                break;
                        }
                        break;
                    case PelotonKoppelingTypeEnum.RHDHV:
                        switch (pk.Richting)
                        {
                            case PelotonKoppelingRichtingEnum.Uitgaand:
                                // schakelaar voor elke fase met detectie die mee doet aan de koppeling
                                if (pk.IsIntern)
                                {
                                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpku}{pk.KoppelingNaam}", _hpku, pk.KoppelingNaam));
                                }
                                foreach (var sg in sgWithD)
                                {
                                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpku}{pk.KoppelingNaam}", 1, CCOLElementTimeTypeEnum.SCH_type, _schpku, pk.KoppelingNaam, sg.Key.Naam));
                                }
                                break;
                            case PelotonKoppelingRichtingEnum.Inkomend:
                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uspelin}{pk.KoppelingNaam}", _uspelin, pk.InkomendVerklikking, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));

                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpelin}{pk.KoppelingNaam}", _hpelin, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpeltegenh}{pk.KoppelingNaam}", _hpeltegenh, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpelgrens}{pk.KoppelingNaam}", pk.MinimaalAantalVoertuigen,
                                    CCOLElementTimeTypeEnum.TS_type, _prmpelgrens, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpelverschuif}{pk.KoppelingNaam}", pk.Verschuiving,
                                    CCOLElementTimeTypeEnum.TS_type, _prmpelverschuif, pk.KoppelingNaam, pk.KoppelingNaam, c.Data.Naam, pk.KoppelingNaam));
                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpelnl}{pk.KoppelingNaam}", pk.TijdRetourWachtgroen, CCOLElementTimeTypeEnum.TS_type, _tpelnl, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                if (pk.ToepassenAanvraag != NooitAltijdAanUitEnum.Nooit && pk.ToepassenAanvraag != NooitAltijdAanUitEnum.Altijd)
                                {
                                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpela}{pk.KoppelingNaam}", pk.ToepassenAanvraag == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schpela, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                }
                                if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Nooit)
                                {
                                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpelrwmax}{pk.KoppelingNaam}", pk.MaxTijdToepassenRetourWachtgroen, CCOLElementTimeTypeEnum.TE_type, _tpelrwmax, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                    if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Altijd)
                                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpelrw}{pk.KoppelingNaam}", pk.ToepassenRetourWachtgroen == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schpelrw, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                    if (pk.ToepassenMeetkriterium != NooitAltijdAanUitEnum.Altijd && pk.ToepassenMeetkriterium != NooitAltijdAanUitEnum.Nooit)
                                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpelmk}{pk.KoppelingNaam}", pk.ToepassenMeetkriterium == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schpelmk, pk.KoppelingNaam, pk.GekoppeldeSignaalGroep));
                                }
                                break;
                        }
                        break;
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override bool HasCodeForController(ControllerModel c, CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCTop => c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.IsInkomend && x.Type == PelotonKoppelingTypeEnum.RHDHV),
                CCOLCodeTypeEnum.RegCPreApplication => c.PelotonKoppelingenData.PelotonKoppelingen.Any(),
                CCOLCodeTypeEnum.RegCAanvragen => c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.IsInkomend && x.Type == PelotonKoppelingTypeEnum.RHDHV),
                CCOLCodeTypeEnum.RegCMeetkriterium => c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.IsInkomend),
                CCOLCodeTypeEnum.RegCWachtgroen => c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.IsInkomend && x.Type == PelotonKoppelingTypeEnum.RHDHV),
                CCOLCodeTypeEnum.RegCPostApplication => c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.IsInkomend /*&& (x.Detectoren.Any() || x.Type == PelotonKoppelingTypeEnum.RHDHV)*/),
                _ => false
            };
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCTop => new []{70},
                CCOLCodeTypeEnum.RegCPreApplication => new []{50},
                CCOLCodeTypeEnum.RegCAanvragen => new []{70},
                CCOLCodeTypeEnum.RegCMeetkriterium => new []{30},
                CCOLCodeTypeEnum.RegCPostApplication => new []{40},
                CCOLCodeTypeEnum.RegCWachtgroen => new []{30},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();
            var ff = false;

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCTop:
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                    {
                        var first = true;
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                        {
                            if (pk.Type == PelotonKoppelingTypeEnum.RHDHV)
                            {
                                if (first)
                                {
                                    sb.AppendLine($"#define MAX_VK_ARRAY 10");
                                    first = false;
                                }
                                sb.AppendLine($"/* Variabelen tbv vrije koppeling met {pk.KoppelingNaam} */");
                                sb.AppendLine($"int iaTime{pk.KoppelingNaam}[MAX_VK_ARRAY];");
                                sb.AppendLine($"int iaStatus{pk.KoppelingNaam}[MAX_VK_ARRAY];");
                                sb.AppendLine($"int iSizeOfArray{pk.KoppelingNaam};");
                                sb.AppendLine($"int iCount{pk.KoppelingNaam};");
                                sb.AppendLine($"int iCounterVkop;");
                                if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Nooit)
                                {
                                    sb.AppendLine($"int iOldRW{pk.KoppelingNaam};");
                                    sb.AppendLine($"{c.GetBoolV()} bSingleRW{pk.KoppelingNaam} = TRUE;");
                                }
                            }
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPreApplication:
                    var f = false;
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend && x.Type == PelotonKoppelingTypeEnum.RHDHV))
                    {
                        sb.AppendLine($"{ts}/* Vrije koppelingen */");
                        sb.AppendLine($"{ts}/* ================= */");
                        sb.AppendLine($"{ts}if (TS) iCounterVkop = CIF_KLOK[CIF_MINUUT] * 60 + CIF_KLOK[CIF_SECONDE];");
                        f = true;
                    }
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                    {
                        if (f) sb.AppendLine();
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                        {
                            sb.AppendLine($"{ts}IH[{_hpf}{_hpeltegenh}{pk.KoppelingNaam}] = FALSE;");
                        }
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


                            if (pk.Type == PelotonKoppelingTypeEnum.DenHaag)
                            {
                                if (f) sb.AppendLine();
                                if (pk.IsIntern)
                                {
                                    sb.AppendLine($"{ts}/* Uitgaande peloton koppeling (intern) */");
                                    foreach (var sg in sgWithD)
                                    {
                                        sb.AppendLine($"{ts}IH[{_hpf}{_hpku}{pk.KoppelingNaam}] = SCH[{_schpf}{_schpku}{pk.KoppelingNaam}] && G[{_fcpf}{pk.GekoppeldeSignaalGroep}] && (CG[{_fcpf}{pk.GekoppeldeSignaalGroep}] < CG_WG);");
                                        foreach (var d in sg.Value)
                                        {
                                            sb.AppendLine($"{ts}if (G[{_fcpf}{sg.Key.Naam}] && ED[{_dpf}{d}]) IH[{_hpf}{_hpkud}{pk.KoppelingNaam}] = !IH[{_hpf}{_hpkud}{pk.KoppelingNaam}];");
                                        }
                                    }
                                }
                                else
                                {
                                    var ipl = CCOLElementCollector.GetKoppelSignaalCount(pk.PTPKruising, $"{pk.KoppelingNaam}g{pk.GekoppeldeSignaalGroep}", KoppelSignaalRichtingEnum.Uit);
                                    sb.AppendLine($"{ts}/* Uitgaande peloton koppeling naar {pk.KoppelingNaam} */");
                                    foreach (var sg in sgWithD)
                                    {
                                        sb.AppendLine($"{ts}IH[{_hpf}{pk.PTPKruising}{_huks}{ipl:00}] = SCH[{_schpf}{_schpku}{pk.KoppelingNaam}{sg.Key.Naam}] && G[{_fcpf}{sg.Key.Naam}] && (CG[{_fcpf}{sg.Key.Naam}] < CG_WG);");
                                        foreach (var d in sg.Value)
                                        {
                                            ipl = CCOLElementCollector.GetKoppelSignaalCount(pk.PTPKruising, $"{pk.KoppelingNaam}d{d}", KoppelSignaalRichtingEnum.Uit);
                                            sb.AppendLine($"{ts}if (G[{_fcpf}{sg.Key.Naam}] && ED[{_dpf}{d}]) IH[{_hpf}{pk.PTPKruising}{_huks}{ipl:00}] = !IH[{_hpf}{pk.PTPKruising}{_huks}{ipl:00}];");
                                        }
                                    }
                                }
                            }
                            else // RHDHV 
                            {
                                var st = "";
                                if (pk.IsIntern)
                                {
                                    sb.AppendLine($"{ts}/* Uitgaande vrije koppeling (intern) */");
                                    st = $"IH[{_hpf}{_hpku}{pk.KoppelingNaam}] = ";
                                    sb.Append($"{ts}IH[{_hpf}{_hpku}{pk.KoppelingNaam}] = ");
                                }
                                else
                                {
                                    var ipl = CCOLElementCollector.GetKoppelSignaalCount(pk.PTPKruising, $"{pk.KoppelingNaam}g{pk.GekoppeldeSignaalGroep}", KoppelSignaalRichtingEnum.Uit);
                                    sb.AppendLine($"{ts}/* Uitgaande vrije koppeling {c.Data.Naam} => {pk.KoppelingNaam} */");
                                    st = $"IH[{_hpf}{pk.PTPKruising}{_huks}{ipl:00}] = ";
                                    sb.Append($"{ts}IH[{_hpf}{pk.PTPKruising}{_huks}{ipl:00}] = ");
                                }

                                var firstsg = true;
                                foreach (var sg in sgWithD)
                                {
                                    if (!firstsg)
                                    {
                                        sb.AppendLine(" || ");
                                        sb.Append($"{ts}{"".PadRight(st.Length)}");
                                    }
                                    firstsg = false;
                                    sb.Append($"SCH[{_schpf}{_schpku}{pk.KoppelingNaam}] && G[{_fcpf}{sg.Key.Naam}] && (");
                                    var firstd = true;
                                    foreach (var d in sg.Value)
                                    {
                                        if (!firstd) sb.Append(" || ");
                                        firstd = false;
                                        sb.Append($"TDH[{_dpf}{d}] && (CIF_IS[{_dpf}{d}] < CIF_DET_STORING)");
                                    }
                                    sb.Append(")");
                                }
                                sb.AppendLine(";");
                            }
                            f = true;
                        }
                    }
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                    {
                        if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x =>
                            x.Richting == PelotonKoppelingRichtingEnum.Inkomend &&
                            x.Type == PelotonKoppelingTypeEnum.DenHaag /*&&
                            x.Detectoren.Any()*/))
                        {
                            sb.AppendLine($"{ts}/* Afzetten hulpelementen inkomende peloton koppelingen */");
                            foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                            {
                                if (pk.Type == PelotonKoppelingTypeEnum.DenHaag /*&& pk.Detectoren.Any()*/)
                                {
                                    sb.AppendLine($"{ts}IH[{_hpf}{_hpelin}{pk.KoppelingNaam}] = FALSE;");
                                }
                            }
                        }

                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                        {
                            if (f) sb.AppendLine();
                            if (pk.Type == PelotonKoppelingTypeEnum.DenHaag && (/*pk.Detectoren.Any() ||*/ pk.IsIntern))
                            {
                                if (pk.IsIntern)
                                {
                                    sb.AppendLine($"{ts}/* Inkomende peloton koppeling o.b.v. intern uitgaand koppelsignaal */");
                                    sb.Append($"{ts}IH[{_hpf}{_hpelin}{pk.KoppelingNaam}] |= proc_pel_in_V1({_hpf}{_hpku}{pk.GerelateerdePelotonKoppeling}, {_tpf}{_tpelmeet}{pk.KoppelingNaam}, {_tpf}{_tpelmaxhiaat}{pk.KoppelingNaam}, {_prmpf}{_prmpelgrens}{pk.KoppelingNaam}, {_mpf}{_mpelvtg}{pk.KoppelingNaam}, {_mpf}{_mpelin}{pk.KoppelingNaam}, ");
                                    sb.Append($"{_hpf}{_hpkud}{pk.GerelateerdePelotonKoppeling}, ");

                                    sb.AppendLine("END);");
                                }
                                else
                                {
                                    var isg = CCOLElementCollector.GetKoppelSignaalCount(pk.PTPKruising, $"{pk.KoppelingNaam}g{pk.GekoppeldeSignaalGroep}", KoppelSignaalRichtingEnum.In);
                                    sb.AppendLine($"{ts}/* Inkomende peloton koppeling van {pk.KoppelingNaam} */");
                                    sb.Append($"{ts}IH[{_hpf}{_hpelin}{pk.KoppelingNaam}] |= proc_pel_in_V1({_hpf}{pk.PTPKruising}{_hiks}{isg:00}, {_tpf}{_tpelmeet}{pk.KoppelingNaam}, {_tpf}{_tpelmaxhiaat}{pk.KoppelingNaam}, {_prmpf}{_prmpelgrens}{pk.KoppelingNaam}, {_mpf}{_mpelvtg}{pk.KoppelingNaam}, {_mpf}{_mpelin}{pk.KoppelingNaam}, ");
                                    foreach (var d in pk.Detectoren)
                                    {
                                        var id = CCOLElementCollector.GetKoppelSignaalCount(pk.PTPKruising, $"{pk.KoppelingNaam}d{d.DetectorNaam}", KoppelSignaalRichtingEnum.In);
                                        sb.Append($"{_hpf}{pk.PTPKruising}{_hiks}{id:00}, ");
                                    }
                                    sb.AppendLine("END);");
                                }
                            }
                            else if (pk.Type == PelotonKoppelingTypeEnum.RHDHV)
                            {
                                var isg = CCOLElementCollector.GetKoppelSignaalCount(pk.PTPKruising, $"{pk.KoppelingNaam}g{pk.GekoppeldeSignaalGroep}", KoppelSignaalRichtingEnum.In);
                                sb.AppendLine($"{ts}/* Inkomende vrije koppeling van {pk.KoppelingNaam} */");
                                sb.AppendLine($"{ts}if (iSizeOfArray{pk.KoppelingNaam} > 0)");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}if ((iaTime{pk.KoppelingNaam}[0] <= iCounterVkop) &&");
                                sb.AppendLine($"{ts}{ts}    (abs(iaTime{pk.KoppelingNaam}[0] - iCounterVkop) < 1800))   /* tbv overgang 3600sec. */");
                                sb.AppendLine($"{ts}{ts}{{");
                                sb.AppendLine($"{ts}{ts}{ts}/* hertarten naloop */");
                                sb.AppendLine($"{ts}{ts}{ts}if (iaStatus{pk.KoppelingNaam}[0] == 1) RT[{_tpf}{_tpelnl}{pk.KoppelingNaam}] = TRUE;  /* Verkeer onderweg */");
                                sb.AppendLine($"{ts}{ts}{ts}else                                   RT[{_tpf}{_tpelnl}{pk.KoppelingNaam}] = FALSE; /* Geen verkeer */");
                                sb.AppendLine($"");
                                sb.AppendLine($"{ts}{ts}{ts}for (iCount{pk.KoppelingNaam} = 1; iCount{pk.KoppelingNaam} < iSizeOfArray{pk.KoppelingNaam} && iCount{pk.KoppelingNaam} < MAX_VK_ARRAY; iCount{pk.KoppelingNaam}++)");
                                sb.AppendLine($"{ts}{ts}{ts}{{");
                                sb.AppendLine($"{ts}{ts}{ts}{ts}iaTime{pk.KoppelingNaam}[iCount{pk.KoppelingNaam} - 1] = iaTime{pk.KoppelingNaam}[iCount{pk.KoppelingNaam}];");
                                sb.AppendLine($"{ts}{ts}{ts}{ts}iaTime{pk.KoppelingNaam}[iCount{pk.KoppelingNaam}] = 0;");
                                sb.AppendLine($"{ts}{ts}{ts}{ts}iaStatus{pk.KoppelingNaam}[iCount{pk.KoppelingNaam} - 1] = iaStatus{pk.KoppelingNaam}[iCount{pk.KoppelingNaam}];");
                                sb.AppendLine($"{ts}{ts}{ts}{ts}iaStatus{pk.KoppelingNaam}[iCount{pk.KoppelingNaam}] = 0;");
                                sb.AppendLine($"{ts}{ts}{ts}}}");
                                sb.AppendLine($"{ts}{ts}{ts}iSizeOfArray{pk.KoppelingNaam}--;");
                                sb.AppendLine($"{ts}{ts}}}");
                                sb.AppendLine($"{ts}}}");
                                sb.AppendLine($"{ts}else");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}iaTime{pk.KoppelingNaam}[0] = iaStatus{pk.KoppelingNaam}[0] = 0;");
                                sb.AppendLine($"{ts}}}");
                                sb.AppendLine($"");
                                sb.AppendLine($"{ts}/* starttijd noteren bij start hulpelement alleen wanneer het peloton het */");
                                sb.AppendLine($"{ts}/* TXD-moment of maximumgroen nog kan halen, anders krijgt starttijd status 0 */");
                                sb.AppendLine($"");
                                if (pk.IsIntern)
                                {
                                    sb.AppendLine($"{ts}if (SH[{_hpf}{_hpku}{pk.GerelateerdePelotonKoppeling}] && (iSizeOfArray{pk.KoppelingNaam} < (MAX_VK_ARRAY - 2)))");
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}if (SH[{_hpf}{pk.PTPKruising}{_hiks}{isg:00}] && (iSizeOfArray{pk.KoppelingNaam} < (MAX_VK_ARRAY - 2)))");
                                }
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}iaTime{pk.KoppelingNaam}[iSizeOfArray{pk.KoppelingNaam}] = (CIF_KLOK[CIF_MINUUT] * 60 + CIF_KLOK[CIF_SECONDE] + PRM[{_prmpf}{_prmpelverschuif}{pk.KoppelingNaam}]) % 3600;");
                                sb.AppendLine($"");
                                sb.AppendLine($"{ts}{ts}iaStatus{pk.KoppelingNaam}[iSizeOfArray{pk.KoppelingNaam}] = 1;");
                                sb.AppendLine($"{ts}{ts}iSizeOfArray{pk.KoppelingNaam}++;");
                                sb.AppendLine($"{ts}}}");
                                sb.AppendLine($"");
                                sb.AppendLine($"{ts}/* eindtijd noteren bij eind hulpelement alleen als er nog een starttijd is */");
                                sb.AppendLine($"{ts}/* en het verschil meer is dan x, anders krijgt starttijd status 0 */");
                                if (pk.IsIntern)
                                {
                                    sb.AppendLine($"{ts}if (EH[{_hpf}{_hpku}{pk.GerelateerdePelotonKoppeling}] && iSizeOfArray{pk.KoppelingNaam} < (MAX_VK_ARRAY - 1))");
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}if (EH[{_hpf}{pk.PTPKruising}{_hiks}{isg:00}] && iSizeOfArray{pk.KoppelingNaam} < (MAX_VK_ARRAY - 1))");
                                }
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}iaTime{pk.KoppelingNaam}[iSizeOfArray{pk.KoppelingNaam}] = (CIF_KLOK[CIF_MINUUT] * 60 + CIF_KLOK[CIF_SECONDE] + PRM[{_prmpf}{_prmpelverschuif}{pk.KoppelingNaam}]) % 3600;");
                                sb.AppendLine($"");
                                sb.AppendLine($"{ts}{ts}if (iSizeOfArray{pk.KoppelingNaam} > 0)");
                                sb.AppendLine($"{ts}{ts}{{");
                                sb.AppendLine($"{ts}{ts}{ts}if (abs(iaTime{pk.KoppelingNaam}[iSizeOfArray{pk.KoppelingNaam}] - iaTime{pk.KoppelingNaam}[iSizeOfArray{pk.KoppelingNaam} - 1]) < PRM[{_prmpf}{_prmpelgrens}{pk.KoppelingNaam}])");
                                sb.AppendLine($"{ts}{ts}{ts}{{");
                                sb.AppendLine($"{ts}{ts}{ts}{ts}iaStatus{pk.KoppelingNaam}[iSizeOfArray{pk.KoppelingNaam} - 1] = 0;");
                                sb.AppendLine($"{ts}{ts}{ts}}}");
                                sb.AppendLine($"{ts}{ts}}}");
                                sb.AppendLine($"");
                                sb.AppendLine($"{ts}{ts}iaStatus{pk.KoppelingNaam}[iSizeOfArray{pk.KoppelingNaam}] = 0;");
                                sb.AppendLine($"{ts}{ts}iSizeOfArray{pk.KoppelingNaam}++;");
                                sb.AppendLine($"{ts}}}");

                            }
                            f = true;
                        }
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Type == PelotonKoppelingTypeEnum.RHDHV && x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                        {
                            if (pk.IsIntern)
                            {
                                sb.AppendLine($"{ts}/* Opzetten hulpelement tbv interne peloton koppeling op {_fcpf}{pk.GekoppeldeSignaalGroep} */");
                            }
                            else
                            {
                                sb.AppendLine($"{ts}/* Opzetten hulpelement tbv peloton koppeling {pk.KoppelingNaam} op {_fcpf}{pk.GekoppeldeSignaalGroep} */");
                            }
                            sb.AppendLine($"{ts}IH[{_hpf}{_hpelin}{pk.KoppelingNaam}] = T_max[{_tpf}{_tpelnl}{pk.KoppelingNaam}] && T[{_tpf}{_tpelnl}{pk.KoppelingNaam}];");
                            sb.AppendLine();
                        }
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Type == PelotonKoppelingTypeEnum.RHDHV && x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                        {
                            if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Nooit)
                            {
                                sb.AppendLine();
                                sb.AppendLine($"{ts}/* Bewaken eenmalig opzetten RW {_fcpf}{pk.GekoppeldeSignaalGroep} */");
                                sb.AppendLine($"{ts}if (!(RW[{_fcpf}{pk.GekoppeldeSignaalGroep}] & BIT14) && (iOldRW{pk.KoppelingNaam} & BIT14)) bSingleRW{pk.KoppelingNaam} = FALSE;");
                                sb.AppendLine($"{ts}if (EG[{_fcpf}{pk.GekoppeldeSignaalGroep}]) bSingleRW{pk.KoppelingNaam} = TRUE;");
                                sb.AppendLine($"{ts}iOldRW{pk.KoppelingNaam} = RW[{_fcpf}{pk.GekoppeldeSignaalGroep}];");
                            }
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCAanvragen:
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                    {
                        var first = true;
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                        {
                            if (pk.Type == PelotonKoppelingTypeEnum.RHDHV && pk.ToepassenAanvraag != NooitAltijdAanUitEnum.Nooit)
                            {
                                if (!first) sb.AppendLine();
                                first = false;
                                sb.AppendLine($"{ts}/* Inkomende peloton koppeling {pk.KoppelingNaam} naar fase {pk.GekoppeldeSignaalGroep} */");
                                sb.Append($"{ts}if (SH[{_hpf}{_hpelin}{pk.KoppelingNaam}]");
                                if (pk.ToepassenAanvraag != NooitAltijdAanUitEnum.Altijd)
                                {
                                    sb.Append($" && SCH[{_schpf}{_schpela}{pk.KoppelingNaam}]");
                                }
                                sb.AppendLine($") A[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT12;");
                            }
                        }
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                    {
                        ff = false;
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                        {
                            if (ff) sb.AppendLine();
                            if (pk.Type == PelotonKoppelingTypeEnum.DenHaag /*&& pk.Detectoren.Any()*/)
                            {
                                if (pk.IsIntern)
                                {
                                    sb.AppendLine($"{ts}/* Inkomende peloton koppeling (intern) naar fase {pk.GekoppeldeSignaalGroep} */");
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}/* Inkomende peloton koppeling {pk.KoppelingNaam} naar fase {pk.GekoppeldeSignaalGroep} */");
                                }
                                if (pk.ToepassenAanvraag != NooitAltijdAanUitEnum.Nooit)
                                {
                                    sb.AppendLine($"{ts}/* timer resetten om aanvraag te zetten */");
                                    sb.AppendLine($"{ts}RT[{_tpf}{_tpela}{pk.KoppelingNaam}] = IH[{_hpf}{_hpelin}{pk.KoppelingNaam}] && !T[{_tpf}{_tpela}{pk.KoppelingNaam}];");
                                }
                                if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Nooit)
                                {
                                    sb.AppendLine($"{ts}/* timer resetten maximale tijd toepassen RW vanaf SG */");
                                    sb.AppendLine($"{ts}RT[{_tpf}{_tpelrwmax}{pk.KoppelingNaam}] = SG[{_fcpf}{pk.GekoppeldeSignaalGroep}];");
                                    sb.AppendLine($"{ts}/* timer resetten om gebied open te houden */");
                                    sb.AppendLine($"{ts}RT[{_tpf}{_tpelstartrw}{pk.KoppelingNaam}] = IH[{_hpf}{_hpelin}{pk.KoppelingNaam}] && !T[{_tpf}{_tpelstartrw}{pk.KoppelingNaam}];");
                                    sb.AppendLine();
                                }
                                if (pk.ToepassenAanvraag != NooitAltijdAanUitEnum.Nooit)
                                {
                                    sb.AppendLine($"{ts}/* zet aanvraag als timer is afgelopen */");
                                    sb.Append($"{ts}if (ET[{_tpf}{_tpela}{pk.KoppelingNaam}]");
                                    if (pk.ToepassenAanvraag != NooitAltijdAanUitEnum.Altijd)
                                    {
                                        sb.Append($" && SCH[{_schpf}{_schpela}{pk.KoppelingNaam}]");
                                    }
                                    sb.AppendLine($") A[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT12;");
                                    sb.AppendLine();
                                }
                                if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Nooit)
                                {
                                    sb.AppendLine($"{ts}/* start vasthoud timer bij einde timer en als de timer nog niet loopt */");
                                    sb.AppendLine($"{ts}RT[{_tpf}{_tpelrw}{pk.KoppelingNaam}] = ET[{_tpf}{_tpelstartrw}{pk.KoppelingNaam}] && !T[{_tpf}{_tpelrw}{pk.KoppelingNaam}];");
                                    sb.AppendLine();
                                }
                                sb.AppendLine($"{ts}MK[{_fcpf}{pk.GekoppeldeSignaalGroep}] &= ~BIT12;");
                                sb.AppendLine();
                                if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Nooit && pk.ToepassenMeetkriterium != NooitAltijdAanUitEnum.Nooit)
                                {
                                    sb.AppendLine($"{ts}/* zet meetkriterium als de vasthoudperiode loopt */");
                                    sb.Append($"{ts}if (T[{_tpf}{_tpelrw}{pk.KoppelingNaam}]");
                                    if (pk.ToepassenMeetkriterium != NooitAltijdAanUitEnum.Altijd)
                                    {
                                        sb.Append($" && SCH[{_schpf}{_schpelmk}{pk.KoppelingNaam}]");
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
                                    sb.Append($"{ts}if (T[{_tpf}{_tpelrw}{pk.KoppelingNaam}]");
                                    if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Altijd)
                                    {
                                        sb.Append($" && SCH[{_schpf}{_schpelrw}{pk.KoppelingNaam}]");
                                    }
                                    sb.AppendLine($" && T[{_tpf}{_tpelrwmax}{pk.KoppelingNaam}] && !IH[{_hpf}{_hpeltegenh}{pk.KoppelingNaam}])");
                                    sb.AppendLine($"{ts}{{");
                                    sb.AppendLine($"{ts}{ts}RW[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT12;");
                                    sb.AppendLine($"{ts}{ts}PP[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT12;");
                                    sb.AppendLine($"{ts}{ts}PAR[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT12;");
                                    sb.AppendLine($"{ts}}}");
                                    sb.AppendLine($"{ts}else");
                                    sb.AppendLine($"{ts}{{");
                                    sb.AppendLine($"{ts}{ts}PP[{_fcpf}{pk.GekoppeldeSignaalGroep}] &= ~BIT12;");
                                    sb.AppendLine($"{ts}}}");
                                    sb.Append($"{ts}if (!(T[{_tpf}{_tpelrw}{pk.KoppelingNaam}]");
                                    if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Altijd)
                                    {
                                        sb.Append($" && SCH[{_schpf}{_schpelrw}{pk.KoppelingNaam}]");
                                    }
                                    sb.AppendLine($"))");
                                    sb.AppendLine($"{ts}{{");
                                    sb.AppendLine($"{ts}{ts}RW[{_fcpf}{pk.GekoppeldeSignaalGroep}] &= ~BIT12;");
                                    sb.AppendLine($"{ts}}}");
                                }
                            }
                            else if (pk.Type == PelotonKoppelingTypeEnum.RHDHV)
                            {
                                sb.AppendLine($"{ts}/* Extra verlengen tbv vrije koppeling */");
                                if (pk.ToepassenMeetkriterium != NooitAltijdAanUitEnum.Nooit)
                                {
                                    if (pk.ToepassenMeetkriterium == NooitAltijdAanUitEnum.Altijd)
                                    {
                                        sb.AppendLine($"{ts}meetkriterium_exp((count){_fcpf}{pk.GekoppeldeSignaalGroep}, ({c.GetBoolV()})(!IH[{_hpf}{_hpeltegenh}{pk.KoppelingNaam}] && T_max[{_tpf}{_tpelnl}{pk.KoppelingNaam}] > 0 && T[{_tpf}{_tpelnl}{pk.KoppelingNaam}]));");
                                    }
                                    else
                                    {
                                        sb.AppendLine($"{ts}meetkriterium_exp((count){_fcpf}{pk.GekoppeldeSignaalGroep}, ({c.GetBoolV()})(!IH[{_hpf}{_hpeltegenh}{pk.KoppelingNaam}] && SCH[{_schpf}{_schpelmk}{pk.KoppelingNaam}] && T_max[{_tpf}{_tpelnl}{pk.KoppelingNaam}] > 0 && T[{_tpf}{_tpelnl}{pk.KoppelingNaam}]));");
                                    }
                                }
                            }
                            ff = true;
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCWachtgroen:
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Type == PelotonKoppelingTypeEnum.RHDHV && x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                    {
                        sb.AppendLine($"{ts}/* RW afzetten */");
                        List<string> kfc = new();
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Type == PelotonKoppelingTypeEnum.RHDHV && x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                        {
                            if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Nooit)
                            {
                                kfc.Add(pk.GekoppeldeSignaalGroep);
                            }
                        }
                        foreach (var rwfc in kfc.Distinct())
                        {
                            sb.AppendLine($"{ts}RW[{_fcpf}{rwfc}] &= ~BIT14;");
                        }
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Type == PelotonKoppelingTypeEnum.RHDHV && x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                        {
                            if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Nooit)
                            {
                                sb.AppendLine();
                                sb.AppendLine($"{ts}/* RW opzetten {_fcpf}{pk.GekoppeldeSignaalGroep} tbv koppeling {pk.KoppelingNaam} */");
                                sb.Append($"{ts}if (");
                                if (pk.ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Altijd)
                                {
                                    sb.Append($"SCH[{_schpf}{_schpelrw}{pk.KoppelingNaam}] && ");
                                }
                                sb.AppendLine($"bSingleRW{pk.KoppelingNaam} && " +
                                    $"IH[{_hpf}{_hpelin}{pk.KoppelingNaam}] && " + 
                                    $"!IH[{_hpf}{_hpeltegenh}{pk.KoppelingNaam}] && " +
                                    $"!fkaa({_fcpf}{pk.GekoppeldeSignaalGroep}) && " +
                                    $"!Z[{_fcpf}{pk.GekoppeldeSignaalGroep}] && " +
                                    $"CG[{_fcpf}{pk.GekoppeldeSignaalGroep}] <= CG_VG" +
                                    $"{(!c.HalfstarData.IsHalfstar ? "" : $" && (!IH[{_hpf}{_hplact}] || TOTXB_PL[{_fcpf}{pk.GekoppeldeSignaalGroep}] == 0 && TOTXD_PL[{_fcpf}{pk.GekoppeldeSignaalGroep}] > 0)")}" +
                                    $") RW[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT14;");
                            }
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPostApplication:
                    ff = false;
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                    {
                        sb.AppendLine("/* Verklikken inkomende pelotons */");
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend /*&& (x.Detectoren.Any() || x.Type == PelotonKoppelingTypeEnum.RHDHV)*/))
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uspelin}{pk.KoppelingNaam}] = IH[{_hpf}{_hpelin}{pk.KoppelingNaam}];");
                        }
                        ff = true;
                    }
                    return sb.ToString();
            }

            return sb.ToString();
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");
            _huks = CCOLGeneratorSettingsProvider.Default.GetElementName("huks");
            _hiks = CCOLGeneratorSettingsProvider.Default.GetElementName("hiks");

            return base.SetSettings(settings);
        }
    }
}
