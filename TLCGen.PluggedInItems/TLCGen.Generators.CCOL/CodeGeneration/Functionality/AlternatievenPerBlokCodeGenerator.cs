using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class AlternatievenPerBlokCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _prmaltb;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            if (c.AlternatievenPerBlokData.ToepassenAlternatievenPerBlok)
            {
                foreach (var fc in c.Fasen)
                {
                    var plfc = c.AlternatievenPerBlokData.AlternatievenPerBlok.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                    if (plfc != null)
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_prmaltb}{fc.Naam}",
                                plfc.BitWiseBlokAlternatief,
                                CCOLElementTimeTypeEnum.None,
                                _prmaltb, fc.Naam));
                    }
                }
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAlternatieven:
                    return 101;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAlternatieven:
                    if (!c.AlternatievenPerBlokData.ToepassenAlternatievenPerBlok)
                    {
                        return "";
                    }
                    sb.AppendLine($"{ts}/* BLOKGEBONDEN ALTERNATIEF */");
                    sb.AppendLine($"{ts}/* ======================== */");
                    sb.AppendLine($"{ts} * Voor instellingen de volgende waarden voor het blok waarin het alternatief mag plaatsvinden optellen:");
                    sb.AppendLine($"{ts} * 1  alternatief mogelijk in blok 1");
                    sb.AppendLine($"{ts} * 2  alternatief mogelijk in blok 2");
                    sb.AppendLine($"{ts} * 4  alternatief mogelijk in blok 3");
                    sb.AppendLine($"{ts} * ...  etc  ... t/m 256 voor blok 9");
                    sb.AppendLine($"{ts} */");
                    foreach (var fc in c.Fasen)
                    {
                        sb.AppendLine($"{ts}if(!(PRM[{_prmpf}{_prmaltb}{fc.Naam}] & (1 << ML))) PAR[{_fcpf}{fc.Naam}] = FALSE;");
                    }
                    return sb.ToString();
                default:
                    return null;
            }
        }
    }
}
