using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class StarCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _prmstarstart;
        private CCOLGeneratorCodeStringSettingModel _prmstareind;
        private CCOLGeneratorCodeStringSettingModel _prmstarprog;
        private CCOLGeneratorCodeStringSettingModel _prmstarprogdef;
        private CCOLGeneratorCodeStringSettingModel _schstar;
        private CCOLGeneratorCodeStringSettingModel _prmstarcyclustijd;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            if (!c.StarData.ToepassenStar || !c.StarData.Programmas.Any()) return;

            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schstar}", 0, CCOLElementTimeTypeEnum.SCH_type, _schstar));

            if (c.StarData.ProgrammaSturingViaParameter)
            {
                var dp = c.StarData.Programmas.FirstOrDefault(x => x.Naam == c.StarData.DefaultProgramma);
                var iDp = dp == null ? 0 : c.StarData.Programmas.IndexOf(dp);
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmstarprogdef}", iDp, CCOLElementTimeTypeEnum.SCH_type, _prmstarprogdef));
            }

            if (c.StarData.ProgrammaTijdenInParameters)
            {
                foreach (var pr in c.StarData.Programmas)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmstarcyclustijd}", pr.Cyclustijd, CCOLElementTimeTypeEnum.TS_type, _prmstarcyclustijd, pr.Naam));
                    foreach (var f in pr.Fasen)
                    {
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmstarstart}", f.Start1, CCOLElementTimeTypeEnum.None, _prmstarstart, "1", f.FaseCyclus, pr.Naam));
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmstareind}", f.Eind1, CCOLElementTimeTypeEnum.None, _prmstareind, "1", f.FaseCyclus, pr.Naam));
                        if (f.Start2.HasValue && f.Eind2.HasValue)
                        {
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmstarstart}", f.Start2.Value, CCOLElementTimeTypeEnum.None, _prmstarstart, "2", f.FaseCyclus, pr.Naam));
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmstareind}", f.Eind2.Value, CCOLElementTimeTypeEnum.None, _prmstareind, "2", f.FaseCyclus, pr.Naam));
                        }
                    }
                }
            }

            if (c.StarData.ProgrammaSturingViaKlok)
            {
                foreach (var p in c.StarData.PeriodenData)
                {
                    var dp = c.StarData.Programmas.FirstOrDefault(x => x.Naam == p.StarProgramma);
                    var iDp = dp == null ? 0 : c.StarData.Programmas.IndexOf(dp);
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmstarprog}", iDp, CCOLElementTimeTypeEnum.None, _prmstarprog, p.Periode));
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override bool HasFunctionLocalVariables() => true;

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPreApplication:
                    return 5000;
                case CCOLCodeTypeEnum.RegCKlokPerioden:
                    return 5000;
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    return 5000;
                case CCOLCodeTypeEnum.RegCPostApplication:
                    return 5000;
            }
            return 0;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var fcs = c.Fasen.Where(x => x.SeniorenIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Nooit &&
                                         x.Detectoren.Any(x2 => x2.Type == Models.Enumerations.DetectorTypeEnum.KnopBinnen || x2.Type == Models.Enumerations.DetectorTypeEnum.KnopBuiten)).ToList();
            if (!fcs.Any()) return null;

            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPreApplication:
                    sb.AppendLine($"{ts}/*********************************/");
                    sb.AppendLine($"{ts}/* reset systeem cyclustimer{ts}{ts}*/");
                    sb.AppendLine($"{ts}/*********************************/");
                    sb.AppendLine($"{ts}if (TS && !(MLA == ML1 && yml_wml(PRMLA, MLA_MAX)/* && IH[h_a_wachtstand]*/))");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}DICG_TX_timer++;");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}if (SMLA && (MLA==ML1))");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}DICG_TX_timer=0;");
                    sb.AppendLine($"{ts}}}");

                    break;
                case CCOLCodeTypeEnum.RegCPostApplication:
                    sb.AppendLine($"{ts}/*********************************************/");
                    sb.AppendLine($"{ts}/* stuur alles rood tbv programmawisseling   */");
                    sb.AppendLine($"{ts}/*********************************************/");
                    sb.AppendLine($"{ts}if (MM[mprg_wissel])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}int fc;");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}IH[hblok_volgrichting] = FALSE;");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}/***************************************/");
                    sb.AppendLine($"{ts}{ts}/* stuur alle signaalgroepen naar rood */");
                    sb.AppendLine($"{ts}{ts}/***************************************/");
                    sb.AppendLine($"{ts}{ts}for (fc = fc_begin; fc < fc_end; fc++)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}RR[fc_begin + fc]  = (RW[fc]&BIT2 || YV[fc]&BIT2) ? FALSE : TRUE;  /* (DR)  */");
                    sb.AppendLine($"{ts}{ts}{ts}Z[fc_begin  + fc]  = (RW[fc]&BIT2 || YV[fc]&BIT2) ? FALSE : TRUE;  /* (FR)  */");
                    sb.AppendLine($"");
                    sb.AppendLine($"{ts}{ts}{ts}IH[hblok_volgrichting] |= (RW[fc]&BIT2 || YV[fc]&BIT2);");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine();

                    break;
            }

            return sb.ToString();
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            return base.SetSettings(settings);
        }
    }
}
