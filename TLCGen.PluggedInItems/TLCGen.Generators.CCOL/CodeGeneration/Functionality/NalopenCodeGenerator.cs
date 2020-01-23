using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Integrity;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class NalopenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _hnla;
        private CCOLGeneratorCodeStringSettingModel _tnlfg;
        private CCOLGeneratorCodeStringSettingModel _tnlfgd;
        private CCOLGeneratorCodeStringSettingModel _tnlsg;
        private CCOLGeneratorCodeStringSettingModel _tnlsgd;
        private CCOLGeneratorCodeStringSettingModel _tnlcv;
        private CCOLGeneratorCodeStringSettingModel _tnlcvd;
        private CCOLGeneratorCodeStringSettingModel _tnleg;
        private CCOLGeneratorCodeStringSettingModel _tnlegd;
        private CCOLGeneratorCodeStringSettingModel _prmxnl;
#pragma warning restore 0649
	    private string _homschtegenh;

        #endregion // Fields

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var nl in c.InterSignaalGroep.Nalopen)
            {
                foreach (var nlt in nl.Tijden)
                {
                    CCOLGeneratorCodeStringSettingModel _tnl = null;
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
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tnl}{nl.FaseVan}{nl.FaseNaar}",
                            nlt.Waarde,
                            CCOLElementTimeTypeEnum.TE_type, 
                            _tnl, nl.FaseVan, nl.FaseNaar));
                }
                if (nl.DetectieAfhankelijk)
                {
                    foreach (var nld in nl.Detectoren)
                    {
                        var elem = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hnla}{nld.Detector}", _hnla, nld.Detector, nl.FaseVan, nl.FaseNaar);
                        if (_myElements.Count == 0 || _myElements.All(x => x.Naam != elem.Naam))
                        {
                            _myElements.Add(elem);
                        }
                    }
                }
                if(nl.MaximaleVoorstart.HasValue)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmxnl}{nl.FaseVan}{nl.FaseNaar}",
                            nl.MaximaleVoorstart.Value,
                            CCOLElementTimeTypeEnum.TE_type, 
                            _prmxnl, nl.FaseVan, nl.FaseNaar));
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override bool HasFunctionLocalVariables() => true;

        public override IEnumerable<Tuple<string, string, string>> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCMaxgroen:
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                    if(c.InterSignaalGroep?.Nalopen?.Count > 0)
                        return new List<Tuple<string, string, string>> { new Tuple<string, string, string>("int", "fc", "") };
                    return base.GetFunctionLocalVariables(c, type);
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCInitApplication:
                    return 30;
                case CCOLCodeTypeEnum.RegCPreApplication:
                    return 30;
                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    return 20;
                case CCOLCodeTypeEnum.RegCMaxgroenNaAdd:
                    return 10;
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    return 20;
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                    return 10;
                case CCOLCodeTypeEnum.RegCAlternatieven:
                    return 20;
                case CCOLCodeTypeEnum.PrioCPrioriteitsNiveau:
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
				case CCOLCodeTypeEnum.RegCInitApplication:
                    if (c.InterSignaalGroep?.Nalopen?.Count > 0)
                    {
                        sb.AppendLine($"{ts}/* Nalopen */");
                        sb.AppendLine($"{ts}/* ------- */");
                        sb.AppendLine($"{ts}gk_InitGK();");
                        sb.AppendLine($"{ts}gk_InitNL();");
                    }
                    // TODO: eerst controleren
                    //if (c.InterSignaalGroep.Nalopen.Any(x => x.Type == NaloopTypeEnum.EindeGroen && x.InrijdenTijdensGroen))
                    //{
                    //    sb.AppendLine($"{ts}/* Corrigeren FK<>GKL tbv nalopen met inrijden tijdens groen */");
                    //    sb.AppendLine($"{ts}/* Dit gebeurt hier i.p.v. in de tab.c om waarschuwingen door CCOL te voorkomen */");
                    //    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.EindeGroen))
                    //    {
                    //        foreach(var kfc in c.InterSignaalGroep.Conflicten.Where(x => x.FaseVan == nl.FaseVan))
                    //        {
                    //            sb.AppendLine($"{ts}TO_max[{_fcpf}{nl.FaseVan}][{_fcpf}{kfc.FaseNaar}] = GKL;");
                    //        }
                    //    }
                    //}
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCPreApplication:
                    if (c.InterSignaalGroep?.Nalopen?.Count > 0)
                    {
                        sb.AppendLine($"{ts}/* Nalopen */");
                        sb.AppendLine($"{ts}/* ------- */");
                        sb.AppendLine($"{ts}gk_ResetGK();");
                        sb.AppendLine($"{ts}gk_ResetNL();");
                    }
                    // TODO: should only generate if any nalopen are there?
                    if (c.HalfstarData.IsHalfstar && _myElements.Any(x => x.Type == CCOLElementTypeEnum.Timer))
					{
                        sb.AppendLine();
						sb.AppendLine($"{ts}IH[{_hpf}{_homschtegenh}] |=");
						var k = 0;
						foreach (var t in _myElements.Where(x => x.Type == CCOLElementTypeEnum.Timer))
						{
							if (k != 0)
							{
								sb.AppendLine(" ||");
							}
							sb.Append($"{ts}{ts}T[{_tpf}{t.Naam}]");
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
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCMaxgroen:
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                    if (c.InterSignaalGroep?.Nalopen?.Count > 0)
                    {
                        sb.AppendLine($"{ts}/* Nalopen */");
                        sb.AppendLine($"{ts}/* ------- */");
                        sb.AppendLine();
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
#warning This only works for pedestrians
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
                                        sb.AppendLine($"{ts}NaloopFG({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlfg}{vn});");
                                        sb.AppendLine($"{ts}NaloopEG({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnleg}{vn});");
                                    }
                                    if (nl.DetectieAfhankelijk && nl.Detectoren?.Count > 0)
                                    {
                                        sb.Append($"{ts}NaloopFGDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlfgd}{vn}");
                                        foreach (var d in nl.Detectoren)
                                        {
                                            sb.Append($", {_dpf}{d.Detector}");
                                        }
                                        sb.AppendLine(", END);");
                                        sb.Append($"{ts}NaloopEGDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlegd}{vn}");
                                        foreach (var d in nl.Detectoren)
                                        {
                                            sb.Append($", {_dpf}{d.Detector}");
                                        }
                                        sb.AppendLine(", END);");
                                    }
                                    break;

                                case NaloopTypeEnum.CyclischVerlengGroen:
                                    if (nl.VasteNaloop)
                                    {
										sb.AppendLine($"{ts}NaloopFG({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlfg}{vn});");
                                        sb.AppendLine($"{ts}NaloopCV({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlcv}{vn});");
                                    }
                                    if (nl.DetectieAfhankelijk && nl.Detectoren?.Count > 0)
                                    {
                                        sb.Append($"{ts}NaloopFGDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlfgd}{vn}");
                                        foreach (var d in nl.Detectoren)
                                        {
                                            sb.Append($", {_dpf}{d.Detector}");
                                        }
                                        sb.AppendLine(", END);");
                                        sb.Append($"{ts}NaloopCVDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlcvd}{vn}");
                                        foreach (var d in nl.Detectoren)
                                        {
                                            sb.Append($", {_dpf}{d.Detector}");
                                        }
                                        sb.AppendLine(", END);");
                                    }
                                    break;
                            }
                        }
                        sb.AppendLine();
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCMaxgroenNaAdd:
                    if (c.InterSignaalGroep.Nalopen.Count > 0)
                        sb.AppendLine($"{ts}gk_ControlGK();");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCAlternatieven:
                    if (c.InterSignaalGroep.Nalopen.Count > 0)
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
                                sb.Append($"{ts}set_MRLW({_fcpf}{nl.FaseNaar}, {_fcpf}{nl.FaseVan}, ({c.GetBoolV()}) (SG[{_fcpf}{nl.FaseVan}] && A[{_fcpf}{nl.FaseNaar}] && (");
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
                                sb.AppendLine($"{ts}set_MRLW({_fcpf}{nl.FaseNaar}, {_fcpf}{nl.FaseVan}, ({c.GetBoolV()}) (SG[{_fcpf}{nl.FaseVan}] && A[{_fcpf}{nl.FaseNaar}]));");
							}
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCPrioriteitsNiveau:
                    //if(!c.InterSignaalGroep.Nalopen.Any()) return "";
                    //sb.AppendLine($"{ts}/* Tegenhouden OV prio met conflict met nalooprichting indien die nog moet komen */");
                    //foreach (var nl in c.InterSignaalGroep.Nalopen)
                    //{
                    //    foreach (var ov in c.OVData.OVIngrepen.Where(x => TLCGenControllerChecker.IsFasenConflicting(c, nl.FaseNaar, x.FaseCyclus)))
                    //    {
                    //        sb.AppendLine($"{ts}iXPrio[ovFC{ov.FaseCyclus}] |= G[{_fcpf}{nl.FaseVan}] && CV[{_fcpf}{nl.FaseVan}] && !G[{_fcpf}{nl.FaseNaar}] &&;");
                    //    }
                    //}
                    //sb.AppendLine();
                    return sb.ToString();

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