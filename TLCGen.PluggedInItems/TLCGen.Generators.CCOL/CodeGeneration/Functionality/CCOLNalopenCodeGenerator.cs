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
#pragma warning disable 0649
        private string _hnla;
        private string _tnlfg;
        private string _tnlfgd;
        private string _tnlsg;
        private string _tnlsgd;
        private string _tnlcv;
        private string _tnlcvd;
        private string _tnleg;
        private string _tnlegd;
        private string _prmxnl;
#pragma warning restore 0649

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
                        var elem = new CCOLElement($"{_hnla}{nld.Detector}", CCOLElementTypeEnum.HulpElement);
                        if (_MyElements.Count == 0 || !_MyElements.Where(x => x.Naam == elem.Naam).Any())
                        {
                            _MyElements.Add(elem);
                        }
                    }
                }
                if(nl.MaximaleVoorstart.HasValue)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_prmxnl}{nl.FaseVan}{nl.FaseNaar}",
                            nl.MaximaleVoorstart.Value,
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
                case CCOLRegCCodeTypeEnum.Synchronisaties:
                case CCOLRegCCodeTypeEnum.Maxgroen:
                case CCOLRegCCodeTypeEnum.Verlenggroen:
                case CCOLRegCCodeTypeEnum.RealisatieAfhandelingNaModules:
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
                case CCOLRegCCodeTypeEnum.Synchronisaties:
                    if (c.InterSignaalGroep?.Nalopen?.Count > 0)
                    {
                        if (c.InterSignaalGroep.Nalopen.Where(x => x.MaximaleVoorstart.HasValue).Any())
                        {
                            var nls = c.InterSignaalGroep.Nalopen.Where(x => x.MaximaleVoorstart.HasValue);
                            sb.AppendLine($"{ts}/* Tegenhouden voedende fietsers tot tijd t voor naloop mag komen */");
                            sb.AppendLine($"{ts}/* afzetten X */");
                            foreach (var nl in nls)
                            {
                                sb.AppendLine($"{ts}X[{_fcpf}{nl.FaseVan}] &= ~{_BITxnl};");
                            }
                            sb.AppendLine();
                            sb.AppendLine($"{ts}/* Vasthouden voedende fietsrichtingen tot in 1 keer kan worden overgefietst */");
                            sb.AppendLine($"{ts}/* Betekenis {_prmpf}x##: tijd dat fase ## eerder mag komen dan SG nalooprichting */");
                            foreach (var nl in nls)
                            {
                                sb.AppendLine($"{ts}X[{_fcpf}{nl.FaseVan}] |= x_aanvoer({_fcpf}{nl.FaseNaar}, PRM[{_prmpf}{_prmxnl}{nl.FaseVan}{nl.FaseNaar}]) ? {_BITxnl} : 0;");
                            }
                            sb.AppendLine();
                        }
                    }
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.Maxgroen:
                case CCOLRegCCodeTypeEnum.Verlenggroen:

                    if (c.InterSignaalGroep?.Nalopen?.Count > 0)
                    {
                        sb.AppendLine($"{ts}/* Nalopen */");
                        sb.AppendLine($"{ts}/* ------- */");
                        sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}RW[fc] &= ~BIT2;");
                        sb.AppendLine($"{ts}{ts}YV[fc] &= ~BIT2;");
                        sb.AppendLine($"{ts}{ts}YM[fc] &= ~BIT2;");
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine();
                        foreach (var nl in c.InterSignaalGroep.Nalopen)
                        {
                            string vn = nl.FaseVan + nl.FaseNaar;
                            switch (nl.Type)
                            {
                                case NaloopTypeEnum.StartGroen:
                                    if(nl.VasteNaloop)
                                    {
                                        sb.AppendLine($"{ts}NaloopVtg({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlsg}{vn});");
                                    }
                                    if (nl.DetectieAfhankelijk && nl.Detectoren?.Count > 0)
                                    {
                                        sb.AppendLine($"{ts}NaloopVtgDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_dpf}{nl.Detectoren[0].Detector}, {_hpf}{_hnla}{nl.Detectoren[0].Detector}, {_tpf}{_tnlsgd}{vn});");
                                    }
                                    break;

                                case NaloopTypeEnum.EindeGroen:
                                    if(nl.VasteNaloop)
                                    {
                                        sb.Append($"{ts}NaloopFG({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnleg}{vn}");
                                        sb.Append($"{ts}NaloopEG({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnleg}{vn}");
                                    }
                                    if (nl.DetectieAfhankelijk && nl.Detectoren?.Count > 0)
                                    {
                                        foreach (var d in nl.Detectoren)
                                        {
                                            sb.Append($"{ts}NaloopFGDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_dpf}{d.Detector}, {_tpf}{_tnleg}{vn}");
                                            sb.Append($"{ts}NaloopEGDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_dpf}{d.Detector}, {_tpf}{_tnleg}{vn}");
                                        }
                                    }
                                    break;

                                case NaloopTypeEnum.CyclischVerlengGroen:
                                    if (nl.VasteNaloop)
                                    {
                                        sb.Append($"{ts}NaloopFG({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnleg}{vn}");
                                        sb.Append($"{ts}NaloopCV({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnleg}{vn}");
                                    }
                                    if (nl.DetectieAfhankelijk && nl.Detectoren?.Count > 0)
                                    {
                                        foreach (var d in nl.Detectoren)
                                        {
                                            sb.Append($"{ts}NaloopCVDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_dpf}{d.Detector}, {_tpf}{_tnleg}{vn}");
                                            sb.Append($"{ts}NaloopCVDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_dpf}{d.Detector}, {_tpf}{_tnleg}{vn}");
                                        }
                                    }
                                    break;
                            }
                        }
                        sb.AppendLine();
                    }
                    return sb.ToString();
                case CCOLRegCCodeTypeEnum.RealisatieAfhandelingNaModules:
                    if(c.InterSignaalGroep.Nalopen.Count > 0)
                    {
                        sb.AppendLine($"{ts}/* set meerealisatie voor richtingen met nalopen */");
                        sb.AppendLine($"{ts}/* --------------------------------------------- */");
                        foreach (var nl in c.InterSignaalGroep.Nalopen)
                        {
                            sb.AppendLine($"{ts}set_MRLW({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, (bool) ((RA[{_fcpf}{nl.FaseNaar}] || SG[{_fcpf}{nl.FaseNaar}]) && (PR[{_fcpf}{nl.FaseNaar}] || AR[{_fcpf}{nl.FaseNaar}]) && R[{_fcpf}{nl.FaseVan}] && !TRG[{_fcpf}{nl.FaseVan}] && A[{_fcpf}{nl.FaseVan}] && !kcv({_fcpf}{nl.FaseVan})));");
                            bool sym = false;
                            foreach (var _nl in c.InterSignaalGroep.Nalopen)
                            {
                                if(_nl.FaseVan == nl.FaseNaar && _nl.FaseNaar == nl.FaseVan)
                                {
                                    sym = true;
                                }
                            }
                            if(!sym)
                            {
                                sb.AppendLine($"{ts}set_MRLW({_fcpf}{nl.FaseNaar}, {_fcpf}{nl.FaseVan}, (bool) ((RA[{_fcpf}{nl.FaseVan}] || SG[{_fcpf}{nl.FaseVan}]) && (PR[{_fcpf}{nl.FaseVan}] || AR[{_fcpf}{nl.FaseVan}]) && R[{_fcpf}{nl.FaseNaar}] && !TRG[{_fcpf}{nl.FaseNaar}] && A[{_fcpf}{nl.FaseNaar}] && !kcv({_fcpf}{nl.FaseNaar})));");
                            }
                        }
                        sb.AppendLine();
                    }
                    return sb.ToString();
#warning Need to also change code for RW in Wachtgroen()? ie !fka() for naloop phase...
                default:
                    return null;
            }
        }
    }
}