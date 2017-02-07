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
    public class CCOLRateltikkersCodeGenerator : CCOLCodePieceGeneratorBase
    {

        #region Fields

        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapOutputs;
        private string _usrt;
        private string _hrt;
        private string _hperiod;
        private string _tnlrt;
        private string _prmperrt;
        private string _prmperrta;
        private string _prmperrtdim;
    
        #endregion // Fields

        #region Properties


        #endregion // Properties

        #region Commands
        #endregion // Commands

        #region Command Functionality
        #endregion // Command Functionality

        #region Private Methods
        #endregion // Private Methods

        #region Public Methods
        #endregion // Public Methods

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

        public override bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.SystemApplication:
                    return true;
                default:
                    return false;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.SystemApplication:
                    sb.AppendLine($"{ts}/* uitsturing aanvraag rateltikkers */");
                    foreach (var rt in c.Signalen.Rateltikkers)
                    {
                        string rtd1 = "NG";
                        string rtd2 = "NG";
                        string nlfcd = "NG";

                        if (rt.Detectoren.Count > 0) rtd1 = rt.Detectoren[0].Detector;
                        if (rt.Detectoren.Count > 1) rtd2 = rt.Detectoren[1].Detector;
                        if (rt.Detectoren.Count > 2) nlfcd = rt.Detectoren[2].Detector;

                        switch (rt.Type)
                        {
                            case RateltikkerTypeEnum.Accross:
                                sb.AppendLine($"{ts}GUS[{_uspf}{_usrt}{rt.FaseCyclus}] = Rateltikkers_Accross({_fcpf}{rt.FaseCyclus}, {_hpf}{_hrt}{rt.FaseCyclus}, {_hpf}{_hperiod}{_prmperrta}, {_hpf}{_hperiod}{_prmperrt}, {_dpf}{rtd1}, {_dpf}{rtd2}, {_dpf}{nlfcd});");
                                break;
                            case RateltikkerTypeEnum.Hoeflake:
                                sb.AppendLine($"{ts}GUS[{_uspf}{_usrt}{rt.FaseCyclus}] = Rateltikkers({_fcpf}{rt.FaseCyclus}, {_hpf}{_hrt}{rt.FaseCyclus}, {_hpf}{_hperiod}{_prmperrta}, {_hpf}{_hperiod}{_prmperrt}, {_tpf}{_tnlrt}{rt.FaseCyclus}, {_dpf}{rtd1}, {_dpf}{rtd2}, {_dpf}{nlfcd});");
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
