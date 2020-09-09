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
        private CCOLGeneratorCodeStringSettingModel _isstar;
        private CCOLGeneratorCodeStringSettingModel _usstar;
        private CCOLGeneratorCodeStringSettingModel _mstarprog;
        private CCOLGeneratorCodeStringSettingModel _mstarprogwens;
        private CCOLGeneratorCodeStringSettingModel _mstarprogwissel;
        private CCOLGeneratorCodeStringSettingModel _usstarprogwissel;
        private CCOLGeneratorCodeStringSettingModel _prmstarcyclustijd;
        private CCOLGeneratorCodeStringSettingModel _hblokvolgrichting;
#pragma warning restore 0649
        private string _mperiodstar;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            _myBitmapInputs = new List<CCOLIOElement>();
            _myBitmapOutputs = new List<CCOLIOElement>();

            if (!c.StarData.ToepassenStar || !c.StarData.Programmas.Any()) return;

            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schstar}", 0, CCOLElementTimeTypeEnum.SCH_type, _schstar));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mstarprog}", _mstarprog));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mstarprogwens}", _mstarprogwens));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mstarprogwissel}", _mstarprogwissel));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hblokvolgrichting}", _hblokvolgrichting));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usstarprogwissel}", _usstarprogwissel));
            _myBitmapOutputs.Add(new CCOLIOElement(c.StarData.ProgrammaWisselBitmapInfo, $"{_uspf}{_usstarprogwissel}"));

            if (c.StarData.ProgrammaSturingViaParameter)
            {
                var dp = c.StarData.Programmas.FirstOrDefault(x => x.Naam == c.StarData.DefaultProgramma);
                var iDp = dp == null ? -1 : c.StarData.Programmas.IndexOf(dp);
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmstarprogdef}", iDp + 1, CCOLElementTimeTypeEnum.SCH_type, _prmstarprogdef));
            }

            if (c.StarData.IngangAlsVoorwaarde)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_isstar}", _isstar));
                _myBitmapInputs.Add(new CCOLIOElement(c.StarData.StarRegelenIngang, $"{_ispf}{_isstar}"));
            }

            foreach (var pr in c.StarData.Programmas)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usstar}{pr.Naam}", _usstar, pr.Naam));
                _myBitmapOutputs.Add(new CCOLIOElement(pr, $"{_uspf}{_usstar}{pr.Naam}"));
            }

            if (c.StarData.ProgrammaTijdenInParameters)
            {
                foreach (var pr in c.StarData.Programmas)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmstarcyclustijd}", pr.Cyclustijd, CCOLElementTimeTypeEnum.TS_type, _prmstarcyclustijd, pr.Naam));
                    foreach (var f in pr.Fasen)
                    {
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmstarstart}1{pr.Naam}{f.FaseCyclus}", f.Start1, CCOLElementTimeTypeEnum.None, _prmstarstart, "1", f.FaseCyclus, pr.Naam));
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmstareind}1{pr.Naam}{f.FaseCyclus}", f.Eind1, CCOLElementTimeTypeEnum.None, _prmstareind, "1", f.FaseCyclus, pr.Naam));
                        if (f.Start2.HasValue && f.Eind2.HasValue)
                        {
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmstarstart}2{pr.Naam}{f.FaseCyclus}", f.Start2.Value, CCOLElementTimeTypeEnum.None, _prmstarstart, "2", f.FaseCyclus, pr.Naam));
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmstareind}2{pr.Naam}{f.FaseCyclus}", f.Eind2.Value, CCOLElementTimeTypeEnum.None, _prmstareind, "2", f.FaseCyclus, pr.Naam));
                        }
                    }
                }
            }

            if (c.StarData.ProgrammaSturingViaKlok)
            {
                foreach (var p in c.StarData.PeriodenData)
                {
                    var dp = c.StarData.Programmas.FirstOrDefault(x => x.Naam == p.StarProgramma);
                    var iDp = dp == null ? -1 : c.StarData.Programmas.IndexOf(dp);
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmstarprog}{p.Periode}", iDp + 1, CCOLElementTimeTypeEnum.None, _prmstarprog, p.Periode));
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override bool HasCCOLBitmapInputs() => true;

        public override bool HasCCOLBitmapOutputs() => true;

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.TabCIncludes => 100,
                CCOLCodeTypeEnum.RegCIncludes => 100,
                CCOLCodeTypeEnum.RegCTop => 100,
                CCOLCodeTypeEnum.RegCPreApplication => 100,
                CCOLCodeTypeEnum.RegCKlokPerioden => 20,
                CCOLCodeTypeEnum.RegCVerlenggroen => 60,
                CCOLCodeTypeEnum.RegCMaxgroen => 60,
                CCOLCodeTypeEnum.RegCPostApplication => 50,
                _ => 0
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            if (!c.StarData.ToepassenStar) return null;

            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.TabCIncludes:
                    sb.AppendLine($"{ts}#include \"starvar.h\" /* Variabelen t.b.v. star regelen */");
                    break;
                case CCOLCodeTypeEnum.RegCIncludes:
                    sb.AppendLine($"{ts}#include \"starfunc.c\" /* Functies t.b.v. star regelen */");
                    sb.AppendLine($"{ts}#include \"starvar.c\" /* Variabelen t.b.v. star regelen */");
                    break;
                case CCOLCodeTypeEnum.RegCTop:
                    sb.AppendLine($"{ts}extern mulv star_cyclustimer;");
                    break;
                case CCOLCodeTypeEnum.RegCPreApplication:
                    if (c.Data.MultiModuleReeksen)
                    {
                        sb.AppendLine($"{ts}if (SMLA && MLA == ML1)");
                    }
                    else
                    {
                        sb.AppendLine($"{ts}if (SML && ML == ML1)");
                    }
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}star_cyclustimer = 0;");
                    sb.AppendLine($"{ts}}}");
                    break;

                case CCOLCodeTypeEnum.RegCKlokPerioden:

                    
                    sb.AppendLine($"{ts}/* Bepalen actief star programma wens */");
                    sb.AppendLine($"{ts}MM[{_mpf}{_mstarprogwens}] = 0;");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schstar}]{(c.StarData.IngangAlsVoorwaarde ? $" || IS[{_ispf}{_isstar}]" : "")})");
                    sb.AppendLine($"{ts}{{");
                    if (c.StarData.ProgrammaSturingViaKlok)
                    {
                        sb.AppendLine($"{ts}{ts}/* Actief star programma o.b.v. kloksturing */");
                        sb.AppendLine($"{ts}{ts}switch (MM[{_mpf}{_mperiodstar}])");
                        sb.AppendLine($"{ts}{ts}{{");
                        int iPeriode = 1;
                        foreach (var periode in c.StarData.PeriodenData)
                        {
                            sb.AppendLine($"{ts}{ts}{ts}case {iPeriode++}:");
                            sb.AppendLine($"{ts}{ts}{ts}{ts}MM[{_mpf}{_mstarprogwens}] = PRM[{_prmpf}{_prmstarprog}{periode.Periode}];");
                            sb.AppendLine($"{ts}{ts}{ts}{ts}break;");
                        }
                        sb.AppendLine($"{ts}{ts}{ts}default:");
                        sb.AppendLine($"{ts}{ts}{ts}{ts}break;");
                        sb.AppendLine($"{ts}{ts}}}");
                    }
                    if (c.StarData.ProgrammaSturingViaParameter)
                    {
                        if (c.StarData.ProgrammaSturingViaKlok) sb.AppendLine();
                        sb.AppendLine($"{ts}{ts}/* Actief star programma o.b.v. parameter */");
                        sb.AppendLine($"{ts}{ts}if (PRM[{_prmpf}{_prmstarprogdef}] != 0) MM[{_mpf}{_mstarprogwens}] = PRM[{_prmpf}{_prmstarprogdef}];");
                    }
                    sb.AppendLine($"{ts}}}");

                    break;

                case CCOLCodeTypeEnum.RegCPostApplication:
                    sb.AppendLine($"{ts}/* star programmawisseling */");
                    sb.AppendLine($"{ts}star_bepaal_omschakelen({_mpf}{_mstarprogwens}, {_mpf}{_mstarprog}, {_mpf}{_mstarprogwissel}, {_hpf}{_hblokvolgrichting});");
                    sb.AppendLine($"{ts}star_programma = MM[{_mpf}{_mstarprog}];");
                    var iPr = 1;
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* verklikken actief star programma en wisseling*/");
                    foreach (var pr in c.StarData.Programmas)
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usstar}{pr.Naam}] = MM[{_mpf}{_mstarprog}] == {iPr++};");
                    }
                    sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usstarprogwissel}] = MM[{_mpf}{_mstarprogwissel}] != 0 || MM[{_mpf}{_mstarprog}] != 0 && MM[{_mpf}{_mstarprogwens}] != MM[{_mpf}{_mstarprog}];");

                    break;
            }

            return sb.ToString();
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _mperiodstar = CCOLGeneratorSettingsProvider.Default.GetElementName("mperiodstar");

            return base.SetSettings(settings);
        }
    }
}
