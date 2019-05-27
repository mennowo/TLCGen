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
                    foreach(var m in c.ModuleMolen.Modules)
                    {
                        if (m.Fasen.Any(x => x.FaseCyclus == fc.Naam))
                        {
                            var ml = c.ModuleMolen.Modules.IndexOf(m);
                            def += 1 << ml;
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
                    // TODO
                    break;
            }

            return sb.ToString();
        }
    }
}
