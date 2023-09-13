using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class GelijkstartenVoorstartenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _tgsot;
        private CCOLGeneratorCodeStringSettingModel _schgs;
        private CCOLGeneratorCodeStringSettingModel _tvs;
        private CCOLGeneratorCodeStringSettingModel _tvsot;
#pragma warning restore 0649

        #endregion // Fields

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
        }

        public override bool HasCCOLElements() => true;
        
        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    return base.GetFunctionLocalVariables(c, type);
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCAlternatieven => [30],
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            // return if no sync
            if (c.InterSignaalGroep?.Gelijkstarten?.Count == 0 && c.InterSignaalGroep?.Voorstarten?.Count == 0)
                return null;

            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAlternatieven:
					sb.AppendLine($"{ts}/* set meerealisatie voor gelijk- of voorstartende richtingen */");
                    sb.AppendLine($"{ts}/* ---------------------------------------------------------- */");
                    foreach(var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}set_MRLW({_fcpf}{vs.FaseVan}, {_fcpf}{vs.FaseNaar}, ({c.GetBoolV()}) (RA[{_fcpf}{vs.FaseNaar}] && (PR[{_fcpf}{vs.FaseNaar}] || AR[{_fcpf}{vs.FaseNaar}] || BR[{_fcpf}{vs.FaseNaar}] || (AA[{_fcpf}{vs.FaseNaar}] & BIT11)) && A[{_fcpf}{vs.FaseVan}] && R[{_fcpf}{vs.FaseVan}] && !TRG[{_fcpf}{vs.FaseVan}] && !kcv({_fcpf}{vs.FaseVan})));");
                    }
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        if (gs.Schakelbaar == AltijdAanUitEnum.Altijd)
                        {
                            sb.AppendLine($"{ts}set_MRLW({_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar}, ({c.GetBoolV()}) ((RA[{_fcpf}{gs.FaseNaar}] || SG[{_fcpf}{gs.FaseNaar}]) && (PR[{_fcpf}{gs.FaseNaar}] || AR[{_fcpf}{gs.FaseNaar}] || (AA[{_fcpf}{gs.FaseNaar}] & BIT11)) && A[{_fcpf}{gs.FaseVan}] && R[{_fcpf}{gs.FaseVan}] && !TRG[{_fcpf}{gs.FaseVan}] && !kcv({_fcpf}{gs.FaseVan})));");
                            sb.AppendLine($"{ts}set_MRLW({_fcpf}{gs.FaseNaar}, {_fcpf}{gs.FaseVan}, ({c.GetBoolV()}) ((RA[{_fcpf}{gs.FaseVan}] || SG[{_fcpf}{gs.FaseVan}]) && (PR[{_fcpf}{gs.FaseVan}] || AR[{_fcpf}{gs.FaseVan}] || (AA[{_fcpf}{gs.FaseNaar}] & BIT11)) && A[{_fcpf}{gs.FaseNaar}] && R[{_fcpf}{gs.FaseNaar}] && !TRG[{_fcpf}{gs.FaseNaar}] && !kcv({_fcpf}{gs.FaseNaar})));");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}if (SCH[{_schpf}{_schgs}{gs.FaseVan}{gs.FaseNaar}]) set_MRLW({_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar}, ({c.GetBoolV()}) ((RA[{_fcpf}{gs.FaseNaar}] || SG[{_fcpf}{gs.FaseNaar}]) && (PR[{_fcpf}{gs.FaseNaar}] || AR[{_fcpf}{gs.FaseNaar}] || (AA[{_fcpf}{gs.FaseNaar}] & BIT11)) && A[{_fcpf}{gs.FaseVan}] && R[{_fcpf}{gs.FaseVan}] && !TRG[{_fcpf}{gs.FaseVan}] && !kcv({_fcpf}{gs.FaseVan})));");
                            sb.AppendLine($"{ts}if (SCH[{_schpf}{_schgs}{gs.FaseVan}{gs.FaseNaar}]) set_MRLW({_fcpf}{gs.FaseNaar}, {_fcpf}{gs.FaseVan}, ({c.GetBoolV()}) ((RA[{_fcpf}{gs.FaseVan}] || SG[{_fcpf}{gs.FaseVan}]) && (PR[{_fcpf}{gs.FaseVan}] || AR[{_fcpf}{gs.FaseVan}] || (AA[{_fcpf}{gs.FaseNaar}] & BIT11)) && A[{_fcpf}{gs.FaseNaar}] && R[{_fcpf}{gs.FaseNaar}] && !TRG[{_fcpf}{gs.FaseNaar}] && !kcv({_fcpf}{gs.FaseNaar})));");
                        }
                    }
                    return sb.ToString();

				default:
                    return null;
            }
        }

        public override bool HasSettings() => true;

        #region Constructor
        #endregion // Constructor
    }
}