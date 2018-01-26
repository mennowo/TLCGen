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
    public class NalopenCodeGenerator : CCOLCodePieceGeneratorBase
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
	    private string _homschtegenh;

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
                        if (_MyElements.Count == 0 || _MyElements.All(x => x.Naam != elem.Naam))
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

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPreApplication:
					return 30;
                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    return 20;
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    return 10;
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                    return 10;
                case CCOLCodeTypeEnum.RegCRealisatieAfhandelingNaModules:
                    return 20;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();

            switch (type)
            {
				case CCOLCodeTypeEnum.RegCPreApplication:
					if (c.HalfstarData.IsHalfstar && _MyElements.Any(x => x.Type == CCOLElementTypeEnum.Timer))
					{
						sb.AppendLine($"{ts}IH[{_hpf}{_homschtegenh}] |=");
						var k = 0;
						foreach (var t in _MyElements.Where(x => x.Type == CCOLElementTypeEnum.Timer))
						{
							if (k != 0)
							{
								sb.AppendLine(" &&");
							}
							sb.Append($"{ts}{ts}!T[{_tpf}{t.Naam}]");
							++k;
						}
						sb.AppendLine(";");
					}
					return sb.ToString();
                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    if (c.InterSignaalGroep?.Nalopen?.Count > 0)
                    {
                        if (c.InterSignaalGroep.Nalopen.Any(x => x.MaximaleVoorstart.HasValue))
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

                case CCOLCodeTypeEnum.RegCMaxgroen:
                case CCOLCodeTypeEnum.RegCVerlenggroen:

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
                                        sb.AppendLine($"{ts}NaloopFG({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnleg}{vn});");
                                        sb.AppendLine($"{ts}NaloopEG({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnleg}{vn});");
                                    }
                                    if (nl.DetectieAfhankelijk && nl.Detectoren?.Count > 0)
                                    {
                                        foreach (var d in nl.Detectoren)
                                        {
                                            sb.AppendLine($"{ts}NaloopFGDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_dpf}{d.Detector}, {_tpf}{_tnleg}{vn});");
                                            sb.AppendLine($"{ts}NaloopEGDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_dpf}{d.Detector}, {_tpf}{_tnleg}{vn});");
                                        }
                                    }
                                    break;

                                case NaloopTypeEnum.CyclischVerlengGroen:
                                    if (nl.VasteNaloop)
                                    {
                                        sb.AppendLine($"{ts}NaloopFG({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnleg}{vn});");
                                        sb.AppendLine($"{ts}NaloopCV({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnleg}{vn});");
                                    }
                                    if (nl.DetectieAfhankelijk && nl.Detectoren?.Count > 0)
                                    {
                                        foreach (var d in nl.Detectoren)
                                        {
                                            sb.AppendLine($"{ts}NaloopCVDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_dpf}{d.Detector}, {_tpf}{_tnleg}{vn});");
                                            sb.AppendLine($"{ts}NaloopCVDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_dpf}{d.Detector}, {_tpf}{_tnleg}{vn});");
                                        }
                                    }
                                    break;
                            }
                        }
                        sb.AppendLine();
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCRealisatieAfhandelingNaModules:
                    if(c.InterSignaalGroep.Nalopen.Count > 0)
                    {
                        sb.AppendLine($"{ts}/* set meerealisatie voor richtingen met nalopen */");
                        sb.AppendLine($"{ts}/* --------------------------------------------- */");
                        foreach (var nl in c.InterSignaalGroep.Nalopen)
                        {
                            var sgv = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseVan);
                            var sgn = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseNaar);
                            if (nl.DetectieAfhankelijk && nl.Detectoren?.Count > 0 && 
                                sgv != null && sgv.Type == FaseTypeEnum.Voetganger &&
                                sgn != null && sgn.Type == FaseTypeEnum.Voetganger)
                            {
                                sb.Append($"{ts}set_MRLW({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, (bool) (SG[{_fcpf}{nl.FaseNaar}] && (");
                                int i = 0;
                                foreach (var d in nl.Detectoren)
                                {
                                    if (i < 0) sb.Append(" || ");
                                    ++i;
                                    sb.Append($"IH[{_hpf}{_hnla}{d.Detector}]");
                                }
                                sb.AppendLine(")));");
                            }
                            else
                            {
                                sb.AppendLine($"{ts}set_MRLW({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, (bool) (SG[{_fcpf}{nl.FaseVan}]));");
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
		
	    public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
	    {
		    _homschtegenh = CCOLGeneratorSettingsProvider.Default.GetElementName("homschtegenh");
		    
		    return base.SetSettings(settings);
	    }
    }
}