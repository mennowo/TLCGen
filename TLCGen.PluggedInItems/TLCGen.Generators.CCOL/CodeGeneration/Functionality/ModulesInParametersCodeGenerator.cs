using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class ModulesInParametersCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schmlprm;
        private CCOLGeneratorCodeStringSettingModel _prmprml;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            _myBitmapInputs = new List<CCOLIOElement>();

            if (c.Data.ModulenInParameters)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_schmlprm}",
                        0,
                        CCOLElementTimeTypeEnum.SCH_type,
                        _schmlprm));
                foreach (var fc in c.Fasen)
                {
                    var def = 0;
                    if (c.Data.MultiModuleReeksen)
                    {
                        var reeks = c.MultiModuleMolens.Where(x => x.Modules.Any(x2 => x2.Fasen.Any(x3 => x3.FaseCyclus == fc.Naam))).ToList();
                        if (reeks.Count == 1)
                        {
                            foreach (var m in reeks[0].Modules)
                            {
                                if (m.Fasen.Any(x => x.FaseCyclus == fc.Naam))
                                {
                                    var ml = reeks[0].Modules.IndexOf(m);
                                    def += 1 << ml;
                                }
                            }
                        }
                        else if (reeks.Count > 1)
                        {
                            Dependencies.Providers.TLCGenDialogProvider.Default.ShowMessageBox(
                                $"LET OP! Fase {fc.Naam} is toegedeeld aan zowel module reeks {reeks[0].Reeks} als module reeks {reeks[1].Reeks}. " +
                                $"De module parameter voor deze fase wordt ingesteld op BIT15 (niet toegedeeld).", "Fout in modulestructuur", System.Windows.MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        foreach (var m in c.ModuleMolen.Modules)
                        {
                            if (m.Fasen.Any(x => x.FaseCyclus == fc.Naam))
                            {
                                var ml = c.ModuleMolen.Modules.IndexOf(m);
                                def += 1 << ml;
                            }
                        }
                    }
                    if (def == 0) def = 0x8000;
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmprml}{fc.Naam}",
                            def,
                            CCOLElementTimeTypeEnum.None,
                            _prmprml, fc.Naam));
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override bool HasCCOLBitmapInputs() => true;

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCInitApplication:
                    return 90;
            }
            return 0;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            if (!c.Data.ModulenInParameters) return null;

            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCInitApplication:
                    sb.AppendLine($"{ts}/* Toepassen parametreerbare blokindeling:");
                    sb.AppendLine($"{ts}   - obv schakelaar");
                    sb.AppendLine($"{ts}   - alleen indien de actuele instellingen door een check heen komen");
                    sb.AppendLine($"{ts}     waarbij middels een functie de blokkenstructuur wordt gecheckt");
                    sb.AppendLine($"{ts}     (op toedeling en conflicten) */");
                    if (c.Data.MultiModuleReeksen)
                    {
                        sb.AppendLine($"{ts}if (SCH[{_schpf}{_schmlprm}])");
                        sb.AppendLine($"{ts}{{");
                        foreach(var r in c.MultiModuleMolens)
                        {
                            var args = "";
                            var rfc1 = c.Fasen.FirstOrDefault(x => r.Modules.SelectMany(x2 => x2.Fasen).Any(x3 => x3.FaseCyclus == x.Naam));
                            var rfc2 = c.Fasen.LastOrDefault(x => r.Modules.SelectMany(x2 => x2.Fasen).Any(x3 => x3.FaseCyclus == x.Naam));
                            if (rfc1 == null || rfc2 == null)
                            {
                                continue;
                            }
                            else
                            {
                                var id2 = c.Fasen.IndexOf(rfc2);
                                ++id2;
                                args = $"{_fcpf}{rfc1.Naam}, {(id2 == c.Fasen.Count ? "FCMAX" : $"{_fcpf}{c.Fasen[id2].Naam}")}";
                            }
                            sb.AppendLine($"{ts}{ts}ModuleStructuurPRM({_prmpf}{_prmprml}{rfc1.Naam}, {args}, {r.Reeks}_MAX, PR{r.Reeks}, Y{r.Reeks}, &{r.Reeks}, &S{r.Reeks});");
                        }

                        sb.AppendLine($"{ts}}}");
                    }
                    else
                    {
                        sb.AppendLine($"{ts}if (SCH[{_schpf}{_schmlprm}])");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}ModuleStructuurPRM({_prmpf}{_prmprml}{c.Fasen.First().Naam}, 0, FCMAX, ML_MAX, PRML, YML, &ML, &SML);");
                        sb.AppendLine($"{ts}}}");
                    }
                    break;
            }

            return sb.ToString();
        }
    }
}
