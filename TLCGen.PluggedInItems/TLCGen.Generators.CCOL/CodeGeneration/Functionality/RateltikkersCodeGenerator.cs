using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{

    [CCOLCodePieceGenerator]
    public class RateltikkersCodeGenerator : CCOLCodePieceGeneratorBase
    {

        #region Fields

        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapOutputs;
#pragma warning disable 0649
        private string _usrt;
        private string _hrt;
        private string _hperiod;
        private string _tnlrt;
        private string _prmperrt;
        private string _prmperrta;
        private string _prmperrtdim;
#pragma warning restore 0649

        #endregion // Fields
        
        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
            _MyBitmapOutputs = new List<CCOLIOElement>();

            foreach (var rt in c.Signalen.Rateltikkers)
            {
                _MyElements.Add(
                        new CCOLElement(
                            $"{_usrt}{rt.FaseCyclus}",
                            CCOLElementTypeEnum.Uitgang));
                _MyElements.Add(
                    new CCOLElement(
                        $"{_hrt}{rt.FaseCyclus}",
                        CCOLElementTypeEnum.HulpElement));

                if (rt.Type == RateltikkerTypeEnum.Hoeflake)
                {
                    _MyElements.Add(
                    new CCOLElement(
                        $"{_tnlrt}{rt.FaseCyclus}",
                        rt.NaloopTijd,
                        CCOLElementTimeTypeEnum.TE_type,
                        CCOLElementTypeEnum.Timer));
                }

                _MyBitmapOutputs.Add(new CCOLIOElement(rt.BitmapData as IOElementModel, $"{_uspf}{_usrt}{rt.FaseCyclus}"));
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _MyElements.Where(x => x.Type == type);
        }

        public override bool HasCCOLBitmapOutputs()
        {
            return true;
        }

        public override IEnumerable<CCOLIOElement> GetCCOLBitmapOutputs()
        {
            return _MyBitmapOutputs;
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCSystemApplication:
                    return 60;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCSystemApplication:
                    if(c.Signalen.Rateltikkers.Count == 0)
                    {
                        return "";
                    }
                    sb.AppendLine($"{ts}/* uitsturing aanvraag rateltikkers */");
                    foreach (var rt in c.Signalen.Rateltikkers)
                    {
                        switch (rt.Type)
                        {
                            case RateltikkerTypeEnum.Accross:
                                sb.Append($"{ts}GUS[{_uspf}{_usrt}{rt.FaseCyclus}] = Rateltikkers_Accross({_fcpf}{rt.FaseCyclus}, {_hpf}{_hrt}{rt.FaseCyclus}, {_hpf}{_hperiod}{_prmperrta}, {_hpf}{_hperiod}{_prmperrt}, ");
                                foreach (var d in rt.Detectoren)
                                {
                                    sb.Append($"{_dpf}{d.Detector}, ");
                                }
                                sb.AppendLine("END);");
                                break;
                            case RateltikkerTypeEnum.Hoeflake:
                                sb.Append($"{ts}GUS[{_uspf}{_usrt}{rt.FaseCyclus}] = Rateltikkers({_fcpf}{rt.FaseCyclus}, {_hpf}{_hrt}{rt.FaseCyclus}, {_hpf}{_hperiod}{_prmperrta}, {_hpf}{_hperiod}{_prmperrt}, {_tpf}{_tnlrt}{rt.FaseCyclus}, ");
                                foreach (var d in rt.Detectoren)
                                {
                                    sb.Append($"{_dpf}{d.Detector}, ");
                                }
                                sb.AppendLine("END);");
                                break;
                        }
                    }
                    sb.AppendLine();
                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _hperiod = CCOLGeneratorSettingsProvider.Default.GetElementName("hperiod");
            _prmperrt = CCOLGeneratorSettingsProvider.Default.GetElementName("prmperrt");
            _prmperrta = CCOLGeneratorSettingsProvider.Default.GetElementName("prmperrta");
            _prmperrtdim = CCOLGeneratorSettingsProvider.Default.GetElementName("prmperrtdim");

            return base.SetSettings(settings);
        }
    }
}
