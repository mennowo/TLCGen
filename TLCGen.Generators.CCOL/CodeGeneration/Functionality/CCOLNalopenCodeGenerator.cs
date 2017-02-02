using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLNalopenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

        private List<CCOLElement> _MyElements;
        private string _hnla;
        private string _tnlfg;
        private string _tnlfgd;
        private string _tnlsg;
        private string _tnlsgd;
        private string _tnlcv;
        private string _tnlcvd;
        private string _tnleg;
        private string _tnlegd;
        private string _prmnldgl;

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

            foreach (var nl in c.InterSignaalGroep.Nalopen)
            {
                foreach (var nlt in nl.Tijden)
                {
                    string _tnl = "";
                    switch (nlt.Type)
                    {
                        case NaloopTijdTypeEnum.StartGroen: _tnl = _tnlsg; break;
                        case NaloopTijdTypeEnum.StartGroenDetectie: _tnl = _tnlsgd; break;
                        case NaloopTijdTypeEnum.VastGroen: _tnl = _tnlfg; break;
                        case NaloopTijdTypeEnum.VastGroenDetectie: _tnl = _tnlfgd; break;
                        case NaloopTijdTypeEnum.EindeGroen: _tnl = _tnleg; break;
                        case NaloopTijdTypeEnum.EindeGroenDetectie: _tnl = _tnlegd; break;
                        case NaloopTijdTypeEnum.EindeVerlengGroen: _tnl = _tnlcv; break;
                        case NaloopTijdTypeEnum.EindeVerlengGroenDetectie: _tnl = _tnlcvd; break;
                    }
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_tnl}{nl.FaseVan}{nl.FaseNaar}",
                            nlt.Waarde,
                            CCOLElementTimeTypeEnum.TE_type,
                            CCOLElementTypeEnum.Timer));
                }
                if (nl.DetectieAfhankelijk)
                {
                    foreach (var nld in nl.Detectoren)
                    {
                        _MyElements.Add(
                            new CCOLElement(
                                $"{_hnla}{nld.Detector}",
                                CCOLElementTypeEnum.HulpElement));
                    }
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_prmnldgl}{nl.FaseVan}{nl.FaseNaar}",
                            0,
                            CCOLElementTimeTypeEnum.TE_type,
                            CCOLElementTypeEnum.Parameter));
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

        public override bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Maxgroen:
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
                case CCOLRegCCodeTypeEnum.Maxgroen:


                    if (c.InterSignaalGroep?.Nalopen?.Count > 0)
                    {
                        sb.AppendLine($"{ts}/* Nalopen */");
                        sb.AppendLine($"{ts}/* ------- */");
                        sb.AppendLine($"{ts}for (fc=0; fc<FCMAX; fc++)");
                        sb.AppendLine($"{ts}{{   RW[fc]&= ~BIT2;");
                        sb.AppendLine($"{ts}{ts}YV[fc]&= ~BIT2;");
                        sb.AppendLine($"{ts}{ts}YM[fc]&= ~BIT2;");
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine();
                        foreach (var nl in c.InterSignaalGroep.Nalopen)
                        {
                            string vn = nl.FaseVan + nl.FaseNaar;
                            switch (nl.Type)
                            {
                                case NaloopTypeEnum.StartGroen:


                                    if (nl.DetectieAfhankelijk && nl.Detectoren?.Count > 0)
                                    {
                                        bool bothvtg = true;
                                        foreach (var fc in c.Fasen)
                                        {
                                            if (fc.Naam == nl.FaseVan && fc.Type != FaseTypeEnum.Voetganger ||
                                               fc.Naam == nl.FaseNaar && fc.Type != FaseTypeEnum.Voetganger)
                                            {
                                                bothvtg = false;
                                                break;
                                            }
                                        }
#warning Only first detector is used here. Does this need to change?
                                        if (bothvtg)
                                        {
                                            sb.AppendLine($"{ts}NaloopVtg({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_dpf}{nl.Detectoren[0].Detector}, {_hpf}{_hnla}{nl.Detectoren[0].Detector}, {_tpf}{_tnlsg}{vn});");
                                        }
                                        else
                                        {
#warning Only first detector is used here. Does this need to change?
                                            sb.AppendLine($"{ts}NaloopDet({_fcpf}{nl.FaseVan},{_fcpf}{nl.FaseNaar}, {_dpf}{nl.Detectoren[0].Detector}, {_tpf}nld2224);");
                                        }
                                    }
                                    else
                                    {
                                        sb.AppendLine($"{ts}NaloopSG({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlsg}{vn});");
                                    }
                                    break;

                                case NaloopTypeEnum.EindeGroen:
#warning This function does not exist
                                    sb.Append($"{ts}Naloop_V1({_fcpf}{nl.FaseVan},{_fcpf}{nl.FaseNaar}, ");
#warning Maximum of 3 detectors: enforce in GUI? Also below...
                                    for (int i = 0; i < 3; ++i)
                                    {
                                        if (i < nl.Detectoren.Count)
                                        {
                                            sb.Append($"{_dpf}{nl.Detectoren[i].Detector}, ");
                                        }
                                        else
                                        {
                                            sb.Append("NG, ");
                                        }
                                    }
                                    sb.AppendLine($"{_prmpf}{_prmnldgl}{vn}, {_tpf}{_tnlfg}{vn}, {_tpf}{_tnlfgd}{vn}, {_tpf}{_tnleg}{vn}, {_tpf}{_tnlegd}{vn});");
                                    break;

                                case NaloopTypeEnum.CyclischVerlengGroen:
#warning This function does not exist
                                    sb.Append($"{ts}NaloopCV_V1({_fcpf}{nl.FaseVan},{_fcpf}{nl.FaseNaar}, ");
                                    for (int i = 0; i < 3; ++i)
                                    {
                                        if (i < nl.Detectoren.Count)
                                        {
                                            sb.Append($"{_dpf}{nl.Detectoren[i].Detector}, ");
                                        }
                                        else
                                        {
                                            sb.Append("NG, ");
                                        }
                                    }
                                    sb.AppendLine($"{_prmpf}{_prmnldgl}{vn}, {_tpf}{_tnlfg}{vn}, {_tpf}{_tnlfgd}{vn}, {_tpf}{_tnlcv}{vn}, {_tpf}{_tnlcvd}{vn});");
                                    break;
                            }
                        }
                        sb.AppendLine();
                    }
                    return sb.ToString();
#warning Need to also add code to RealisatieAfhandeling ??? ie. set_MRLW, etc
#warning Need to also change code for RW in Wachtgroen()? ie !fka() for naloop phase...
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
                if (s.Default == "nlsg") _tnlsg = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "nlsgd") _tnlsgd = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "nlfg") _tnlfg = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "nlfgd") _tnlfgd = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "nlcv") _tnlcv = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "nlcvd") _tnlcvd = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "nleg") _tnleg = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "nlegd") _tnlegd = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "nla") _hnla = s.Setting == null ? s.Default : s.Setting;
                if (s.Default == "nldgl") _prmnldgl = s.Setting == null ? s.Default : s.Setting;
            }
            
            return base.SetSettings(settings);
        }

        #region Constructor
        #endregion // Constructor
    }
}