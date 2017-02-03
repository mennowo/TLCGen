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

            foreach (var fc in c.Fasen)
            {
                if (fc.RatelTikkerType != RateltikkerTypeEnum.Geen)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_usrt}{fc.Naam}",
                            CCOLElementTypeEnum.Uitgang));
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_hrt}{fc.Naam}",
                            CCOLElementTypeEnum.HulpElement));

                    if(fc.RatelTikkerType == RateltikkerTypeEnum.Hoeflake)
                    {
                        _MyElements.Add(
                        new CCOLElement(
                            $"{_tnlrt}{fc.Naam}",
                            fc.RatelTikkerNaloopTijd,
                            CCOLElementTimeTypeEnum.TE_type,
                            CCOLElementTypeEnum.Timer));
                    }

                    _MyBitmapOutputs.Add(new CCOLIOElement(fc.RatelTikkerBitmapData as IOElementModel, $"{_uspf}{_usrt}{fc.Naam}"));
                }
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
                    foreach(var fc in c.Fasen)
                    {
                        if(fc.RatelTikkerType != RateltikkerTypeEnum.Geen)
                        {
                            string rtd1 = "NG";
                            string rtd2 = "NG";
                            string nlfcd = "NG";
                            // Find buttons
                            foreach(var dm in fc.Detectoren)
                            {
                                if(dm.Type == DetectorTypeEnum.KnopBuiten)
                                {
                                    rtd1 = dm.Naam;
                                    break;
                                }
                            }
                            foreach (var dm in fc.Detectoren)
                            {
                                if (dm.Type == DetectorTypeEnum.KnopBinnen)
                                {
                                    rtd2 = dm.Naam;
                                    break;
                                }
                            }
                            // Find possible outside button of follow-up signal group
                            foreach (var nl in c.InterSignaalGroep.Nalopen)
                            {
                                if(nl.FaseNaar == fc.Naam && nl.DetectieAfhankelijk)
                                {
                                    FaseCyclusModel fcnl = null;
                                    foreach(var fc2 in c.Fasen)
                                    {
                                        if(nl.FaseVan == fc2.Naam)
                                        {
                                            fcnl = fc2;
                                            break;
                                        }
                                    }
                                    if(fcnl == null)
                                    {
                                        break;
                                    }
                                    foreach(var nld in nl.Detectoren)
                                    {
                                        foreach(DetectorModel dm in fcnl.Detectoren)
                                        {
                                            if(dm.Naam == nld.Detector && dm.Type == DetectorTypeEnum.KnopBuiten)
                                            {
                                                nlfcd = _dpf + dm.Naam;
                                            }
                                        }
                                    }
                                }
                            }
                            switch(fc.RatelTikkerType)
                            {
                                case RateltikkerTypeEnum.Accross:
                                    sb.AppendLine($"{ts}GUS[{_uspf}{_usrt}{fc.Naam}] = Rateltikkers_Accross({_fcpf}{fc.Naam}, {_hpf}{_hrt}{fc.Naam}, {_hpf}{_hperiod}{_prmperrta}, {_hpf}{_hperiod}{_prmperrt}, {_dpf}{rtd1}, {_dpf}{rtd2}, {nlfcd});");
                                    break;
                                case RateltikkerTypeEnum.Hoeflake:
                                    sb.AppendLine($"{ts}GUS[{_uspf}{_usrt}{fc.Naam}] = Rateltikkers({_fcpf}{fc.Naam}, {_hpf}{_hrt}{fc.Naam}, {_hpf}{_hperiod}{_prmperrta}, {_hpf}{_hperiod}{_prmperrt}, {_tpf}{_tnlrt}{fc.Naam}, {_dpf}{rtd1}, {_dpf}{rtd2}, {nlfcd});");
                                    break;
                            }
                        }
                    }
                    sb.AppendLine();
                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override bool HasSettings()
        {
            return true;
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            if (settings == null || settings.Settings == null)
            {
                return false;
            }

            foreach (var s in settings.Settings)
            {
                if (s.Default == "rt")
                {
                    switch (s.Type)
                    {
                        case CCOLGeneratorSettingTypeEnum.HulpElement: _hrt = s.Setting == null ? s.Default : s.Setting; break;
                        case CCOLGeneratorSettingTypeEnum.Uitgang: _usrt = s.Setting == null ? s.Default : s.Setting; break;
                    }
                }
                if (s.Default == "nlrt") _tnlrt = s.Setting == null ? s.Default : s.Setting;
            }

            _hperiod = CCOLGeneratorSettingsProvider.Default.GetElementName("hperiod");
            _prmperrt = CCOLGeneratorSettingsProvider.Default.GetElementName("prmperrt");
            _prmperrta = CCOLGeneratorSettingsProvider.Default.GetElementName("prmperrta");
            _prmperrtdim = CCOLGeneratorSettingsProvider.Default.GetElementName("prmperrtdim");

            return base.SetSettings(settings);
        }

        #region Constructor
        #endregion // Constructor
    }
}
