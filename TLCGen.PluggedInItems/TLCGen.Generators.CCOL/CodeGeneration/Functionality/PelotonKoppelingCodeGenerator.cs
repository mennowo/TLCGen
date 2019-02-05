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
        private CCOLGeneratorCodeStringSettingModel _tpelstartrw;
        private CCOLGeneratorCodeStringSettingModel _schpelrw;
        private CCOLGeneratorCodeStringSettingModel _schpelmk;
        private CCOLGeneratorCodeStringSettingModel _tpela;
        private CCOLGeneratorCodeStringSettingModel _schpela;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            _myBitmapOutputs = new List<CCOLIOElement>();

            foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Uitgaand))
            {
                CCOLElementCollector.AddKoppelSignaal($"{pk.KruisingNaam}g{pk.GekoppeldeSignaalGroep}", CCOLKoppelSignaalRichtingEnum.Uit);
                foreach(var d in pk.Detectoren)
                {
                    CCOLElementCollector.AddKoppelSignaal($"{pk.KruisingNaam}d{d.DetectorNaam}", CCOLKoppelSignaalRichtingEnum.Uit);
                }
            }
            foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
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
                _myBitmapOutputs.Add(new CCOLIOElement(pk.InkomendVerklikking, $"{_uspf}{_uspelin}{pk.GekoppeldeSignaalGroep}"));

                CCOLElementCollector.AddKoppelSignaal($"{pk.KruisingNaam}g{pk.GekoppeldeSignaalGroep}", CCOLKoppelSignaalRichtingEnum.In);
                foreach (var d in pk.Detectoren)
                {
                    CCOLElementCollector.AddKoppelSignaal($"{pk.KruisingNaam}d{d.DetectorNaam}", CCOLKoppelSignaalRichtingEnum.In);
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override bool HasCCOLBitmapOutputs() => true;

        public override bool HasCodeForController(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
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
                case CCOLCodeTypeEnum.RegCPreApplication:
                    return 50;
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    return 30;
            }
            return 0;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPreApplication:
                    var f = false;
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Uitgaand))
                    {
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Uitgaand))
                        {
                            if (f) sb.AppendLine();
                            sb.AppendLine($"{ts}/* Uitgaande peloton koppeling naar {pk.KruisingNaam} */");
                            var ipl = CCOLElementCollector.GetKoppelSignaalCount($"{pk.KruisingNaam}g{pk.GekoppeldeSignaalGroep}", CCOLKoppelSignaalRichtingEnum.Uit);
                            sb.AppendLine($"{ts}IH[{_hpf}{pk.PTPKruising}{_huks}{ipl:00}] = SG[{_fcpf}{pk.GekoppeldeSignaalGroep}] || FG[{_fcpf}{pk.GekoppeldeSignaalGroep}];");
                            foreach (var d in pk.Detectoren)
                            {
                                ipl = CCOLElementCollector.GetKoppelSignaalCount($"{pk.KruisingNaam}d{d.DetectorNaam}", CCOLKoppelSignaalRichtingEnum.Uit);
                                sb.AppendLine($"{ts}IH[{_hpf}{pk.PTPKruising}{_huks}{ipl:00}] = G[{_fcpf}{pk.GekoppeldeSignaalGroep}] && ED[{_dpf}{d.DetectorNaam}];");
                            }
                            f = true;
                        }
                    }
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                    {
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                        {
                            if (f) sb.AppendLine();
                            sb.AppendLine($"{ts}/* Inkomende peloton koppeling van {pk.KruisingNaam} */");
                            var isg = CCOLElementCollector.GetKoppelSignaalCount($"{pk.KruisingNaam}g{pk.GekoppeldeSignaalGroep}", CCOLKoppelSignaalRichtingEnum.In);
                            sb.Append($"{ts}IH[{_hpf}{_hpelin}{pk.GekoppeldeSignaalGroep}] = proc_pel_in_V1({_hpf}{pk.PTPKruising}{_huks}{isg:00}, {_tpf}{_tpelmeet}{pk.GekoppeldeSignaalGroep}, {_tpf}{_tpelmaxhiaat}{pk.GekoppeldeSignaalGroep}, {_prmpf}{_prmpelgrens}{pk.GekoppeldeSignaalGroep}, {_mpf}{_mpelvtg}{pk.GekoppeldeSignaalGroep}, {_mpf}{_mpelin}{pk.GekoppeldeSignaalGroep}, ");
                            foreach (var d in pk.Detectoren)
                            {
                                var id = CCOLElementCollector.GetKoppelSignaalCount($"{pk.KruisingNaam}d{d.DetectorNaam}", CCOLKoppelSignaalRichtingEnum.In);
                                sb.Append($"{ts}{_hpf}{pk.PTPKruising}{_hiks}{id:00}, ");
                            }
                            sb.AppendLine("END);");
                            f = true;
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    if (c.PelotonKoppelingenData.PelotonKoppelingen.Any(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                    {
                        var ff = false;
                        foreach (var pk in c.PelotonKoppelingenData.PelotonKoppelingen.Where(x => x.Richting == PelotonKoppelingRichtingEnum.Inkomend))
                        {
                            if (ff) sb.AppendLine();
                            sb.AppendLine($"{ts}/* Inkomende peloton koppeling van {pk.KruisingNaam} */");
                            sb.AppendLine($"{ts}/* timer resetten om aanvraag te zetten */");
                            sb.AppendLine($"{ts}RT[{_tpf}{_tpela}{pk.GekoppeldeSignaalGroep}] = IH[{_hpf}{_hpelin}{pk.GekoppeldeSignaalGroep}] && !T[{_tpf}{_tpela}{pk.GekoppeldeSignaalGroep}];");
                            sb.AppendLine($"{ts}/* timer resetten om gebied open te houden */");
                            sb.AppendLine($"{ts}RT[{_tpf}{_tpelstartrw}{pk.GekoppeldeSignaalGroep}] = IH[{_hpf}{_hpelin}{pk.GekoppeldeSignaalGroep}] && !T[{_tpf}{_tpelstartrw}{pk.GekoppeldeSignaalGroep}];");
                            sb.AppendLine();
                            sb.AppendLine($"{ts}/* zet aanvraag als timer is afgelopen */");
                            sb.AppendLine($"{ts}if (ET[{_tpf}{_tpela}{pk.GekoppeldeSignaalGroep}] && SCH[{_schpf}{_schpela}{pk.GekoppeldeSignaalGroep}]) A[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT12;");
                            sb.AppendLine();
                            sb.AppendLine($"{ts}/* start vasthoud timer bij einde timer en als de timer nog niet loopt */");
                            sb.AppendLine($"{ts}RT[{_tpf}{_tpelrw}{pk.GekoppeldeSignaalGroep}] = ET[{_tpf}{_tpelstartrw}{pk.GekoppeldeSignaalGroep}] && !T[{_tpf}{_tpelrw}{pk.GekoppeldeSignaalGroep}];");
                            sb.AppendLine();
                            sb.AppendLine($"{ts}MK[{_fcpf}{pk.GekoppeldeSignaalGroep}] &= ~BIT12;");
                            sb.AppendLine();
                            sb.AppendLine($"{ts}/* zet meetkriterium als de vasthoudperiode loopt */");
                            sb.AppendLine($"{ts}if (T[{_tpf}{_tpelrw}{pk.GekoppeldeSignaalGroep}] && SCH[{_schpf}{_schpelmk}{pk.GekoppeldeSignaalGroep}])");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}MK[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT2 | BIT12;");
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine();
                            sb.AppendLine($"{ts}/* houd groen vast als de vasthoudperiode loopt,");
                            sb.AppendLine($"{ts}   de maximale wachttijd nog niet bereikt is,");
                            sb.AppendLine($"{ts}   tenzij de timer al loopt (besluit wordt niet teruggenomen) */");
                            sb.AppendLine($"{ts}if (T[{_tpf}{_tpelrw}{pk.GekoppeldeSignaalGroep}] && SCH[{_schpf}{_schpelrw}{pk.GekoppeldeSignaalGroep}] /*&& !CIF_GUS[usmaxwto]*/)");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}RW[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT12;");
                            sb.AppendLine($"{ts}{ts}PP[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT12;");
                            sb.AppendLine($"{ts}{ts}PAR[{_fcpf}{pk.GekoppeldeSignaalGroep}] |= BIT12;");
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine($"{ts}else");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}PP[{_fcpf}{pk.GekoppeldeSignaalGroep}] &= ~BIT12;");
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine($"{ts}if (!(T[{_tpf}{_tpelrw}{pk.GekoppeldeSignaalGroep}] && SCH[{_schpf}{_schpelrw}{pk.GekoppeldeSignaalGroep}]))");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}RW[{_fcpf}{pk.GekoppeldeSignaalGroep}] &= ~BIT12;");
                            sb.AppendLine($"{ts}}}");
                            ff = true;
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
