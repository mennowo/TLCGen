using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
	public class HalfstarCodeGenerator : CCOLCodePieceGeneratorBase
	{
		private List<CCOLElement> _myElements;

#pragma warning disable 0649
		private string _mperiod;
		private string _cvc;
		private string _schmv;
		private string _schwg;
		private string _tnlsg;
		private string _tnlsgd;
		private string _tnlcv;
		private string _tnlcvd;
		private string _tnleg;
		private string _tnlegd;
#pragma warning restore 0649

#pragma warning disable 0649
		private string _usmlact;
		private string _usplact;
		private string _uskpact;
		private string _usmlpl;
		private string _ustxtimer;
		private string _usmaster;
		private string _usslave;
		private string _usklok;
		private string _ushand;
		private string _usinleven;
		private string _usinsyncok;
		private string _usintxsok;
		private string _usuitkpuls;
		private string _usuitpervar;
		private string _usuitperarh;
		private string _usuitpl;

		private string _mklok;
		private string _mhand;
		private string _hkpact;
		private string _hplact;
		private string _hmlact;
		private string _schvar;
		private string _scharh;
		private string _schvarper;
		private string _hvarper;
		private string _scharhper;
		private string _harhper;
		private string _schvarstreng;
		private string _schvaml;
		private string _hplhulpdienst;
		private string _schovpriople;
		private string _prmplxper;
		private string _tin;
		private string _hxpl;
		private string _schinst;
		private string _homschtegenh;
		private string _prmrstotxa;
#pragma warning restore 0649

		public override void CollectCCOLElements(ControllerModel c)
		{
			_myElements = new List<CCOLElement>();

			if (c.HalfstarData.IsHalfstar)
			{
				var hsd = c.HalfstarData;
				_myElements.Add(new CCOLElement($"{_hplact}", CCOLElementTypeEnum.HulpElement));
				_myElements.Add(new CCOLElement($"{_hkpact}", CCOLElementTypeEnum.HulpElement));
				_myElements.Add(new CCOLElement($"{_hmlact}", CCOLElementTypeEnum.HulpElement));
				_myElements.Add(new CCOLElement($"{_hvarper}", CCOLElementTypeEnum.HulpElement));
				_myElements.Add(new CCOLElement($"{_harhper}", CCOLElementTypeEnum.HulpElement));
				_myElements.Add(new CCOLElement($"{_hplhulpdienst}", CCOLElementTypeEnum.HulpElement));
				_myElements.Add(new CCOLElement($"{_hxpl}", CCOLElementTypeEnum.HulpElement));
				_myElements.Add(new CCOLElement($"{_homschtegenh}", CCOLElementTypeEnum.HulpElement));
				_myElements.Add(new CCOLElement($"{_prmrstotxa}", 50, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
				_myElements.Add(new CCOLElement($"{_schinst}", 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
				
				_myElements.Add(new CCOLElement($"{_mklok}", CCOLElementTypeEnum.GeheugenElement));
				_myElements.Add(new CCOLElement($"{_mhand}", CCOLElementTypeEnum.GeheugenElement));
				
				_myElements.Add(new CCOLElement($"{_schvaml}", hsd.TypeVARegelen == HalfstarVARegelenTypeEnum.ML ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
				_myElements.Add(new CCOLElement($"{_schvar}", hsd.VARegelen ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
				_myElements.Add(new CCOLElement($"{_scharh}", hsd.AlternatievenVoorHoofdrichtingen ? 1 : 0 , CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
				if (hsd.Type != HalfstarTypeEnum.Slave)
				{
					_myElements.Add(new CCOLElement($"{_schvarstreng}", 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
				}

				var iplx = 0;
				for (var index = 0; index < hsd.SignaalPlannen.Count; index++)
				{
					var pl = hsd.SignaalPlannen[index];
					if (hsd.DefaultPeriodeSignaalplan == pl.Naam)
					{
						iplx = index;
						break;
					}
				}
				_myElements.Add(new CCOLElement($"{_prmplxper}def", iplx, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
				var iper = 1;
				foreach (var per in hsd.HalfstarPeriodenData)
				{
					for (var index = 0; index < hsd.SignaalPlannen.Count; index++)
					{
						var pl = hsd.SignaalPlannen[index];
						if (per.Signaalplan == pl.Naam)
						{
							iplx = index;
							break;
						}
					}
					_myElements.Add(new CCOLElement($"{_prmplxper}{iper}", iplx, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
					++iper;
				}

				_myElements.Add(new CCOLElement($"{_schvarper}def", hsd.DefaultPeriodeVARegelen ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
				iper = 1;
				foreach (var per in hsd.HalfstarPeriodenData)
				{
					_myElements.Add(new CCOLElement($"{_schvarper}{iper}", per.VARegelen ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
					++iper;
				}

				_myElements.Add(new CCOLElement($"{_scharhper}def", hsd.DefaultPeriodeAlternatievenVoorHoofdrichtingen ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
				iper = 1;
				foreach (var per in hsd.HalfstarPeriodenData)
				{
					_myElements.Add(new CCOLElement($"{_scharhper}{iper}", per.AlternatievenVoorHoofdrichtingen ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
					++iper;
				}

				if (c.OVData.OVIngreepType != OVIngreepTypeEnum.Geen)
				{
					_myElements.Add(new CCOLElement($"{_schovpriople}", hsd.OVPrioriteitPL ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
				}
			}
			
		}

		public override bool HasCCOLElements()
		{
			return true;
		}

		public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
		{
			return _myElements.Where(x => x.Type == type);
		}

		public override int HasCode(CCOLCodeTypeEnum type)
		{
			switch (type)
			{
				case CCOLCodeTypeEnum.RegCPreApplication:
					return 20;
				case CCOLCodeTypeEnum.HstCPreApplication:
					return 10;
				case CCOLCodeTypeEnum.HstCKlokPerioden:
					return 10;
				case CCOLCodeTypeEnum.HstCAanvragen:
					return 10;
				case CCOLCodeTypeEnum.HstCVerlenggroen:
					return 10;
				case CCOLCodeTypeEnum.HstCMaxgroen:
					return 10;
				case CCOLCodeTypeEnum.HstCWachtgroen:
					return 10;
				case CCOLCodeTypeEnum.HstCMeetkriterium:
					return 10;
				case CCOLCodeTypeEnum.HstCDetectieStoring:
					return 10;
				case CCOLCodeTypeEnum.HstCMeeverlengen:
					return 10;
				case CCOLCodeTypeEnum.HstCSynchronisaties:
					return 10;
				case CCOLCodeTypeEnum.HstCAlternatief:
					return 10;
				case CCOLCodeTypeEnum.HstCRealisatieAfhandeling:
					return 10;
				case CCOLCodeTypeEnum.HstCPostApplication:
					return 10;
				case CCOLCodeTypeEnum.HstCPreSystemApplication:
					return 10;
				case CCOLCodeTypeEnum.HstCPostSystemApplication:
					return 10;
				case CCOLCodeTypeEnum.HstCPostDumpApplication:
					return 10;
				default:
					return 0;
			}
		}

		public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
		{
			StringBuilder sb = new StringBuilder();

			switch (type)
			{
				case CCOLCodeTypeEnum.RegCPreApplication:
					sb.AppendLine($"{ts}/* bepalen of regeling mag omschakelen */");
					sb.AppendLine($"{ts}IH[{_hpf}{_homschtegenh}] = FALSE;");
					return sb.ToString();
				case CCOLCodeTypeEnum.HstCPreApplication:
					sb.AppendLine($"{ts}/* na omschakeling van PL -> VA, modules opnieuw initialiseren */");
					sb.AppendLine($"{ts}if (SH[{_hpf}{_hplact}] || EH[{_hpf}{_hplact}])");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}init_modules(ML_MAX, PRML, YML, &ML, &SML);");
					if (c.InterSignaalGroep.Gelijkstarten.Any() ||
					    c.InterSignaalGroep.Voorstarten.Any())
					{
						sb.AppendLine($"{ts}{ts}reset_realisation_timers();");
					}
					sb.AppendLine($"{ts}{ts}sync_pg();");
					sb.AppendLine($"{ts}{ts}reset_fc_halfstar();");
					sb.AppendLine($"{ts}}}");
					sb.AppendLine();
					sb.AppendLine($"{ts}/* bijhouden verlenggroentijden t.b.v. calculaties diverse functies */");
					sb.AppendLine($"{ts}tvga_timer_halfstar();");
					sb.AppendLine();
					if (c.OVData.OVIngreepType != OVIngreepTypeEnum.Geen)
					{
						sb.AppendLine($"{ts}/* tbv ov_ple */");
						sb.AppendLine($"{ts}if (SCH[sch{_schovpriople}])");
						sb.AppendLine($"{ts}{{");
						sb.AppendLine($"{ts}{ts}/* Instellen OV parameters */");
						sb.AppendLine($"{ts}{ts}if (CIF_PARM1WIJZPB != CIF_GEEN_PARMWIJZ ||");
						sb.AppendLine($"{ts}{ts}{ts}{ts}CIF_PARM1WIJZAP != CIF_GEEN_PARMWIJZ)");
						sb.AppendLine($"{ts}{ts}{{");
						sb.AppendLine($"{ts}{ts}{ts}OV_ple_settings();");
						sb.AppendLine($"{ts}{ts}}}");
						sb.AppendLine($"{ts}{ts}{ts}");
						sb.AppendLine($"{ts}{ts}BijhoudenWachtTijd();");
						sb.AppendLine($"{ts}{ts}BijhoudenMinimumGroenTijden();");
						sb.AppendLine($"{ts}}}");
						sb.AppendLine();
					}
					return sb.ToString();
				case CCOLCodeTypeEnum.HstCKlokPerioden:
					sb.AppendLine($"{ts}bool omschakelmag = FALSE;");
					sb.AppendLine();
					sb.AppendLine($"{ts}/* BepaalKoppeling */");
					sb.AppendLine($"{ts}/* --------------- */");
					sb.AppendLine($"{ts}MM[{_mpf}{_mklok}] = MM[{_mpf}{_mhand}] = FALSE;");
					sb.AppendLine($"{ts}IH[{_hpf}{_hkpact}] = TRUE;");
					sb.AppendLine($"{ts}IH[{_hpf}{_hplact}] = TRUE;");
					sb.AppendLine($"{ts}IH[{_hpf}{_hmlact}] = FALSE;");
					sb.AppendLine($"{ts}APL = NG;");
					sb.AppendLine();
					sb.AppendLine($"{ts}if (SCH[{_schpf}{_schvar}] || SCH[{_schpf}{_schvarstreng}])");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}/* halfstar/va afhankelijk van schakelaar */");
					sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hkpact}] = FALSE;");
					sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mhand}]  = TRUE;");
					sb.AppendLine($"{ts}}} ");
					sb.AppendLine($"{ts}else");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mklok}] = TRUE;");
					sb.AppendLine($"{ts}}}");
					sb.AppendLine();
					sb.AppendLine($"{ts}switch (MM[{_mpf}{_mperiod}])");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}case 0: /* default */");
					sb.AppendLine($"{ts}{ts}{{");
					sb.AppendLine($"{ts}{ts}{ts}APL = PRM[{_prmpf}{_prmplxper}def] - 1;");
					sb.AppendLine($"{ts}{ts}{ts}break;");
					sb.AppendLine($"{ts}{ts}}}");
					var iper = 1;
					foreach (var per in c.HalfstarData.HalfstarPeriodenData)
					{
						sb.AppendLine($"{ts}{ts}case {iper}: /* default */");
						sb.AppendLine($"{ts}{ts}{{");
						sb.AppendLine($"{ts}{ts}{ts}APL = PRM[{_prmpf}{_prmplxper}{iper}] - 1;");
						sb.AppendLine($"{ts}{ts}{ts}break;");
						sb.AppendLine($"{ts}{ts}}}");
						++iper;
					}
					sb.AppendLine($"{ts}{ts}default:");
					sb.AppendLine($"{ts}{ts}{{");
					sb.AppendLine($"{ts}{ts}{ts}APL = PRM[{_prmpf}{_prmplxper}def] - 1;");
					sb.AppendLine($"{ts}{ts}{ts}break;");
					sb.AppendLine($"{ts}{ts}}}");
					sb.AppendLine($"{ts}}}");
					sb.AppendLine();
					sb.AppendLine($"{ts}/* Klokbepaling voor VA-bedrijf */");
					sb.AppendLine($"{ts}if (IH[{_hpf}{_homschtegenh}])");
					sb.AppendLine($"{ts}{{");
					sb.Append($"{ts}{ts}if ((SCH[{_schpf}{_schvarper}def] && (MM[{_mpf}{_mperiod}] == 0)");
					iper = 1;
					foreach (var per in c.HalfstarData.HalfstarPeriodenData)
					{
						sb.AppendLine(") ||");
						sb.Append($"{ts}{ts}    (SCH[{_schpf}{_schvarper}{iper}] && (MM[{_mpf}{_mperiod}] == {iper})");
						++iper;
					}
					sb.AppendLine("))");
					sb.AppendLine($"{ts}{ts}{{");
					sb.AppendLine($"{ts}{ts}{ts}IH[{_hpf}{_hvarper}] = TRUE;");
					sb.AppendLine($"{ts}{ts}}}");
					sb.AppendLine($"{ts}{ts}else");
					sb.AppendLine($"{ts}{ts}{{");
					sb.AppendLine($"{ts}{ts}{ts}IH[{_hpf}{_hvarper}] = FALSE;");
					sb.AppendLine($"{ts}{ts}}}");
					sb.AppendLine($"{ts}}}");
					sb.AppendLine();

					sb.AppendLine($"{ts}/* Klokbepaling voor alternatieve realisaties voor de hoofdrichtingen */");
					sb.Append($"{ts}if ((SCH[{_schpf}{_scharhper}def] && (MM[{_mpf}{_mperiod}] == 0)");
					iper = 1;
					foreach (var per in c.HalfstarData.HalfstarPeriodenData)
					{
						sb.AppendLine(") ||");
						sb.Append($"{ts}    (SCH[{_schpf}{_scharhper}{iper}] && (MM[{_mpf}{_mperiod}] == {iper})");
						++iper;
					}
					sb.AppendLine("))");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}IH[{_hpf}{_harhper}] = TRUE;");
					sb.AppendLine($"{ts}}}");
					sb.AppendLine($"{ts}else");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}IH[{_hpf}{_harhper}] = FALSE;");
					sb.AppendLine($"{ts}}}");

					sb.AppendLine($"{ts}/* Toestaan alternatief hoofdrichtingen ook mogelijk met schakelaar */");
					sb.AppendLine($"{ts}if (SCH[{_schpf}{_scharh}])");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}IH[{_hpf}{_harhper}] = TRUE;");
					sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mhand}]   = TRUE;");
					sb.AppendLine($"{ts}}}");

					sb.AppendLine($"{ts}/* Koppelen actief */");
					sb.AppendLine($"{ts}if (H[{_hpf}{_hvarper}] || SCH[{_schpf}{_schvar}] || SCH[{_schpf}{_schvarstreng}])");
					sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hkpact}] = FALSE;");
					sb.AppendLine($"{ts}else");
					sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hkpact}] = TRUE;");

					sb.AppendLine($"{ts}/* Indien VA-bedrijf, dan met schakelaar te bepalen of dit in ML-bedrijf of in versneld PL-bedrijf gebeurt */");
					sb.AppendLine($"{ts}if (IH[{_hpf}{_hkpact}] == FALSE)");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}if (SCH[{_schpf}{_schvaml}] || (APL == NG))");
					sb.AppendLine($"{ts}{ts}{{");
					sb.AppendLine($"{ts}{ts}{ts}IH[{_hpf}{_hmlact}] = TRUE;");
					sb.AppendLine($"{ts}{ts}{ts}IH[{_hpf}{_hplact}] = FALSE;");
					sb.AppendLine($"{ts}{ts}}}");
					sb.AppendLine($"{ts}{ts}else");
					sb.AppendLine($"{ts}{ts}{{");
					sb.AppendLine($"{ts}{ts}{ts}IH[{_hpf}{_hplact}] = TRUE;");
					sb.AppendLine($"{ts}{ts}{ts}IH[{_hpf}{_hmlact}] = FALSE;");
					sb.AppendLine($"{ts}{ts}}}");
					sb.AppendLine($"{ts}}}");
					sb.AppendLine($"{ts}else");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hplact}] = TRUE;");
					sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hmlact}] = FALSE;");
					sb.AppendLine($"{ts}}}");
					sb.AppendLine();

					if (c.OVData.HDIngrepen.Any())
					{
						sb.AppendLine($"{ts}/* Bij hulpdienstingreep, lokaal VA regelen */");
						sb.AppendLine($"{ts}if (IH[{_hpf}hulpdienst])");
						sb.AppendLine($"{ts}{{");
						sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hmlact}] = TRUE;");
						sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hplact}] = FALSE;");
						sb.AppendLine($"{ts}}}");
					}

					return sb.ToString();

				case CCOLCodeTypeEnum.HstCAanvragen:
					sb.AppendLine($"{ts}/* tijdens ple, wachtstandaanvraag uit */");
					sb.AppendLine($"{ts}if (IH[{_hpf}{_hkpact}])");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}for (fc = 0; fc < FCMAX; ++fc)");
					sb.AppendLine($"{ts}{ts}{ts}A[fc] &= ~BIT2;");
					sb.AppendLine($"{ts}}}");
					return sb.ToString();

				case CCOLCodeTypeEnum.HstCMaxgroen:
					sb.AppendLine($"{ts}if (IH[{_hpf}{_hplact}]) /* Code alleen bij PL-bedrijf */");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}for (fc = 0; fc < FCMAX; ++fc)");
					sb.AppendLine($"{ts}{ts}{{");
					sb.AppendLine($"{ts}{ts}{ts}/* afzetten functies en BITJES van ML-bedrijf */");
					sb.AppendLine($"{ts}{ts}{ts}TVG_max[fc] = 0;");
					sb.AppendLine($"{ts}{ts}{ts}YV[fc] &= ~(BIT2 | BIT4);");
					sb.AppendLine($"{ts}{ts}{ts}FM[fc] &= ~BIT2;");
					sb.AppendLine($"{ts}{ts}{ts}RW[fc] &= ~BIT2;");
					sb.AppendLine($"{ts}{ts}{ts}/* opzetten verlengfunctie (Vasthouden verlenggroen) bij PL-bedrijf */");
					sb.AppendLine($"{ts}{ts}{ts}YV[fc] |= MK[fc] && (YV_PL[fc] && PR[fc] || AR[fc] && yv_ar_max_pl(fc, 0)) ? BIT4 : 0;");
					sb.AppendLine($"{ts}{ts}}}");
					sb.AppendLine($"{ts}}}");
					return sb.ToString();

				case CCOLCodeTypeEnum.HstCWachtgroen:
					sb.AppendLine($"{ts}if (IH[h{_hplact}]) /* Code alleen bij PL-bedrijf */");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}/* Retour wachtgroen bij wachtgroen richtingen, let op: inclusief aanvraag! */");
					foreach (var fc in c.Fasen)
					{
						sb.AppendLine($"{ts}{ts}wachtstand_halfstar({_fcpf}{fc.Naam}, (bool)(SCH[{_schpf}{_schwg}{fc.Naam}]), (bool)(SCH[{_schpf}{_schwg}{fc.Naam}]));");
					}
					sb.AppendLine($"{ts}}}");
					return sb.ToString();

				case CCOLCodeTypeEnum.HstCMeetkriterium:
					sb.AppendLine($"if(IH[{_hpf}{_hplact}])");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}for (fc = 0; fc < FCMAX; ++fc)");
					sb.AppendLine($"{ts}{ts}{{");
					sb.AppendLine($"{ts}{ts}{ts}{ts}/* afzetten BITJES van ML-bedrijf */");
					sb.AppendLine($"{ts}{ts}{ts}{ts} Z[fc] &= ~BIT6;");
					sb.AppendLine($"{ts}{ts}{ts}{ts}FM[fc] &= ~BIT6;");
					sb.AppendLine($"{ts}{ts}{ts}{ts}RW[fc] &= ~BIT6;");
					sb.AppendLine($"{ts}{ts}{ts}{ts}RR[fc] &= ~BIT6;");
					sb.AppendLine($"{ts}{ts}{ts}{ts}YV[fc] &= ~BIT6;");
					sb.AppendLine($"{ts}{ts}{ts}{ts}MK[fc] &= ~BIT6;");
					sb.AppendLine($"{ts}{ts}{ts}{ts}PP[fc] &= ~BIT6;");
					sb.AppendLine($"{ts}{ts}}}");
					sb.AppendLine();
					var tsov = c.OVData.OVIngreepType == OVIngreepTypeEnum.Geen ? $"{ts}{ts}" : $"{ts}{ts}{ts}";
					if (c.OVData.OVIngreepType != OVIngreepTypeEnum.Geen)
					{
						sb.AppendLine($"{ts}{ts}if (!SCH[{_schpf}{_schovpriople}])");
						sb.AppendLine($"{ts}{ts}{{");
						sb.AppendLine($"{tsov}/* OV meetkriterium bij PL bedrijf */");
						foreach (var ov in c.OVData.OVIngrepen)
						{
							sb.AppendLine($"{tsov}rhdhv_yv_ov_pl({_fcpf}{ov.FaseCyclus}, BIT7, C[{_ctpf}{_cvc}{ov.FaseCyclus}]);");
						}
						sb.AppendLine($"{ts}{ts}}}");
					}
					sb.AppendLine($"{ts}}}");

					return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCDetectieStoring:
					return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCMeeverlengen:
					sb.AppendLine($"{ts}/* Resetten YM bit voor PL regelen */");
					sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
					sb.AppendLine($"{ts}{ts}YM[fc] &= ~RHDHV_YM_PL;");
					sb.AppendLine();
					foreach (var fc in c.Fasen)
					{
						sb.AppendLine($"{ts}set_ym_pl_halfstar({_fcpf}{fc.Naam}, (bool)(SCH[{_schpf}{_schmv}{fc.Naam}]));");
					}

					return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCSynchronisaties:
					//sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
					//sb.AppendLine($"{ts}{ts}YV[fc] &= ~RHDHV_YV_KOP;");
					//sb.AppendLine();
					//sb.AppendLine($"{ts}if (IH[{_hpf}{_hplact}])");
					//sb.AppendLine($"{ts}{{");
					//sb.AppendLine($"{ts}{ts}for (fc = 0; fc < FCMAX; ++fc)");
					//sb.AppendLine($"{ts}{ts}{{");
					//sb.AppendLine($"{ts}{ts}{ts}RR[fc]&= ~(BIT1 | BIT2 | BIT3 | RHDHV_RR_KOP | RHDHV_RR_VS);");
					//sb.AppendLine($"{ts}{ts}{ts}RW[fc]&= ~(BIT3 | RHDHV_RW_KOP);");
					//sb.AppendLine($"{ts}{ts}{ts}YV[fc]&= ~(BIT1 | RHDHV_YV_KOP);");
					//sb.AppendLine($"{ts}{ts}{ts}YM[fc]&= ~(BIT3 | RHDHV_YM_KOP);");
					//sb.AppendLine($"{ts}{ts}{ts} X[fc]&= ~(BIT1 | BIT2 |BIT3 | RHDHV_X_GELIJK | RHDHV_X_VOOR | RHDHV_X_DEELC);");
					//sb.AppendLine($"{ts}{ts}{ts}KR[fc]&= ~(BIT0 | BIT1 |BIT2 | BIT3 |BIT4 |BIT5 |BIT6 | BIT7);");
					//sb.AppendLine($"{ts}{ts}}}");
					//sb.AppendLine();
					//
					//foreach (var nl in c.InterSignaalGroep.Nalopen)
					//{
					//	if (nl.Type == NaloopTypeEnum.EindeGroen ||
					//	    nl.Type == NaloopTypeEnum.CyclischVerlengGroen)
					//	{
					//		if (nl.VasteNaloop)
					//		{
					//			var t = nl.Type == NaloopTypeEnum.EindeGroen ? _tnleg : _tnlcv;
					//			sb.AppendLine($"hardekoppeling_halfstar(TRUE, {_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, NG, NG, {_tpf}{t}{nl.FaseVan}{nl.FaseNaar});");
					//		}
					//		else if (nl.DetectieAfhankelijk)
					//		{
					//			var t = nl.Type == NaloopTypeEnum.EindeGroen ? _tnlegd : _tnlcvd;
					//			sb.AppendLine($"hardekoppeling_halfstar(TRUE, {_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, NG, NG, {_tpf}{t}{nl.FaseVan}{nl.FaseNaar});");
					//		}
					//	}
					//}
					//
					//sb.AppendLine();
					return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCAlternatief:
					sb.AppendLine($"{ts}for (fc=0; fc<FCMAX; fc++)");
					sb.AppendLine($"{ts}{ts}RR[fc] &= ~RHDHV_RR_ALTCOR;");
					sb.AppendLine();
					sb.AppendLine($"{ts}/* PAR wordt geregeld in reg.c */");
					sb.AppendLine();
					
					sb.AppendLine($"{ts}if (IH[{_hpf}{_hplact}]) /* Code alleen bij PL-bedrijf */");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}/* hoofdrichtingen alleen tijdens periode alternatieve realisaties en koppeling uit */");
					sb.AppendLine($"{ts}{ts}if (!H[{_hpf}{_harhper}] && H[{_hpf}kpact])");
					sb.AppendLine($"{ts}{ts}{{");
					foreach (var hfc in c.HalfstarData.Hoofdrichtingen)
					{
						sb.AppendLine($"{ts}{ts}{ts}PAR[{_fcpf}{hfc.FaseCyclus}] = FALSE;");
					}
					sb.AppendLine($"{ts}{ts}}}");
					sb.AppendLine($"{ts}}}");
					sb.AppendLine($"{ts}");
					sb.AppendLine($"{ts}if (IH[{_hpf}{_hplact}])");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}/* retour rood wanneer richting AR heeft maar geen PAR meer */");
					sb.AppendLine($"{ts}{ts}/* -------------------------------------------------------- */");
					sb.AppendLine($"{ts}{ts}rhdhv_reset_altreal();");
					sb.AppendLine($"{ts}{ts}");
					sb.AppendLine($"{ts}{ts}signaalplan_alternatief();");
					sb.AppendLine($"{ts}}}");

					return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCRealisatieAfhandeling:
					sb.AppendLine($"{ts}if (IH[{_hpf}{_hplact}]) /* Code alleen bij PL-bedrijf */");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}for (fc = 0; fc < FCMAX; ++fc)");
					sb.AppendLine($"{ts}{ts}{{");
					sb.AppendLine($"{ts}{ts}{ts}PP[fc] &= ~BIT4;");
					sb.AppendLine($"{ts}{ts}{ts}YM[fc] &= ~BIT5;");
					sb.AppendLine($"{ts}{ts}{ts}RR[fc] &= ~RHDHV_RR_VS;");
					sb.AppendLine($"{ts}{ts}{ts}RS[fc] &= ~RHDHV_RS_PLE;");
					sb.AppendLine($"{ts}{ts}{ts}PP[fc] |= GL[fc] ? BIT4 : 0; /* i.v.m. overslag door conflicten */");
					sb.AppendLine($"{ts}{ts}}}");
					sb.AppendLine($"{ts}{ts}");
					sb.AppendLine($"{ts}{ts}for (fc = 0; fc < FCMAX; ++fc)");
					sb.AppendLine($"{ts}{ts}{{");
					sb.AppendLine($"{ts}{ts}{ts}/* PP opzetten en cyclische aanvraag op TXB moment bij PP ");
					sb.AppendLine($"{ts}{ts}{ts}{ts} Iedere richting met een C moment is onderdeel van een coordinatie en");
					sb.AppendLine($"{ts}{ts}{ts}{ts} dient iedere cyclus op zijn B moment groen te worden */");
					sb.AppendLine($"{ts}{ts}{ts}if (TXC_PL[fc] > 0)");
					sb.AppendLine($"{ts}{ts}{ts}{{");
					sb.AppendLine($"{ts}{ts}{ts}{ts}rhdhv_set_pp(fc, IH[{_hpf}{_hkpact}], BIT4);");
					sb.AppendLine($"{ts}{ts}{ts}}}");
					sb.AppendLine($"");
					sb.AppendLine($"{ts}{ts}{ts}/* Voorstartgroen tijdens voorstart t.o.v. sg-plan, alleen als gekoppeld wordt geregeld */");
					sb.AppendLine($"{ts}{ts}{ts}rhdhv_vs_ple(fc, {_prmpf}{_prmrstotxa}, IH[{_hpf}{_hkpact}]);");
					sb.AppendLine($"");
					sb.AppendLine($"{ts}{ts}{ts}/* opzetten van YS en YW tijdens halfstar bedrijf */");
					sb.AppendLine($"{ts}{ts}{ts}/* resetten */");
					sb.AppendLine($"{ts}{ts}{ts}RW[fc] &= ~RHDHV_RW_WG;");
					sb.AppendLine($"{ts}{ts}{ts}YW[fc] &= ~RHDHV_YW_PL;");
					sb.AppendLine($"{ts}{ts}{ts}/* vasthouden wachtgroen functie bij PL-bedrijf */");
					sb.AppendLine($"{ts}{ts}{ts}RW[fc] |= YW_PL[fc] && rhdhv_tussen_txb_en_txc(fc) && (TXC[PL][fc] > 0) ? RHDHV_RW_WG : 0; /* TXC-afhandeling */");
					sb.AppendLine($"{ts}{ts}{ts}YW[fc] |= YW_PL[fc] && rhdhv_tussen_txb_en_txc(fc) && (TXC[PL][fc] > 0) ? RHDHV_YW_PL : 0; /* TXC-afhandeling */");
					sb.AppendLine($"{ts}{ts}}}");
					sb.AppendLine($"");
					sb.AppendLine($"{ts}{ts}/* primaire realisaties signaalplansturing */");
					sb.AppendLine($"{ts}{ts}/* --------------------------------------- */");
					if (c.OVData.OVIngreepType == OVIngreepTypeEnum.Geen)
					{
						sb.AppendLine($"{ts}{ts}signaalplan_primair();");
					}
					else
					{
						sb.AppendLine($"{ts}{ts}if (SCH[{_schpf}{_schovpriople}])");
						sb.AppendLine($"{ts}{ts}{{");
						sb.AppendLine($"{ts}{ts}{ts}signaalplan_primair_ov_ple();");
						sb.AppendLine($"{ts}{ts}}}");
						sb.AppendLine($"{ts}{ts}else");
						sb.AppendLine($"{ts}{ts}{{");
						sb.AppendLine($"{ts}{ts}{ts}signaalplan_primair();");
						sb.AppendLine($"{ts}{ts}}}");
					}
					sb.AppendLine();
					sb.AppendLine($"{ts}{ts}/* afsluiten primaire aanvraaggebieden */");
					sb.AppendLine($"{ts}{ts}/* ----------------------------------- */");
					if (c.OVData.OVIngreepType == OVIngreepTypeEnum.Geen)
					{
						sb.AppendLine($"{ts}{ts}set_pg_primair_fc();");
					}
					else
					{
						sb.AppendLine($"{ts}{ts}if (SCH[{_schpf}{_schovpriople}])");
						sb.AppendLine($"{ts}{ts}{{");
						sb.AppendLine($"{ts}{ts}{ts}set_pg_primair_fc_ov_ple();");
						sb.AppendLine($"{ts}{ts}}}");
						sb.AppendLine($"{ts}{ts}else");
						sb.AppendLine($"{ts}{ts}{{");
						sb.AppendLine($"{ts}{ts}{ts}set_pg_primair_fc();");
						sb.AppendLine($"{ts}{ts}}}");
					}
					sb.AppendLine();
					sb.AppendLine($"{ts}{ts}/* reset PG bij planwisseling */");
					sb.AppendLine($"{ts}{ts}/* -------------------------- */");
					sb.AppendLine($"{ts}{ts}/* anders kan PG op blijven staan, waardoor richting eenmaal wordt overgeslagen en de regeling kan vastlopen */");
					sb.AppendLine($"{ts}{ts}if (SH[{_hpf}{_hmlact}] || SH[{_hpf}{_hplact}] || SPL)");
					sb.AppendLine($"{ts}{ts}{{");
					sb.AppendLine($"{ts}{ts}{ts}for (fc = 0; fc < FCMAX; ++fc)");
					sb.AppendLine($"{ts}{ts}{ts}{{");
					sb.AppendLine($"{ts}{ts}{ts}{ts}PG[fc] = FALSE;");
					sb.AppendLine($"{ts}{ts}{ts}}}");
					sb.AppendLine($"{ts}{ts}}}");
					sb.AppendLine($"{ts}}}");
					return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCPostApplication:
					sb.AppendLine($"{ts}/* herstart fasebewakingstimers bij wisseling tussen ML/PL en SPL */");
					sb.AppendLine($"{ts}/* -------------------------------------------------------------- */");
					sb.AppendLine($"{ts}RTFB &= ~RHDHV_RTFB_PLVA;");
					sb.AppendLine($"{ts}RTFB |= (SH[{_hpf}{_hplact}] || SH[{_hpf}{_hmlact}] || (SPL && IH[{_hpf}{_hplact}])) ? RHDHV_RTFB_PLVA: 0;");
					return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCPreSystemApplication:
					sb.AppendLine($"{ts}/* copieer signaalplantijden - na wijziging */");
					sb.AppendLine($"{ts}/* ---------------------------------------- */");
					sb.AppendLine($"{ts}if (SCH[{_schpf}{_schinst}] || RHDHV_COPY_2_TIG)");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}copy_signalplan(PL);");
					sb.AppendLine($"{ts}{ts}create_tig();        /* creëer nieuwe TIG-tabel na wijzigingen geel-, ontruimingstijden */");
					sb.AppendLine($"{ts}{ts}correction_tig();    /* correcties TIG-tabel a.g.v. koppelingen e.d. */");
					sb.AppendLine($"{ts}{ts}check_signalplans(); /* check signalplans */");
					sb.AppendLine($"{ts}{ts}SCH[{_schpf}{_schinst}] = 0;");
					sb.AppendLine($"{ts}{ts}RHDHV_COPY_2_TIG = FALSE;");
					sb.AppendLine($"{ts}{ts}CIF_PARM1WIJZAP = (s_int16) (&SCH[{_schpf}{_schinst}] - CIF_PARM1);");
					sb.AppendLine($"{ts}}}");
					sb.AppendLine($"{ts}RTX = FALSE;");
					sb.AppendLine($"{ts}");
					sb.AppendLine($"{ts}if (IH[{_hpf}{_hplact}]) /* Code alleen bij PL-bedrijf */");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}FTX = HTX = FALSE;  /* reset instructievariabelen van TX */");
					sb.AppendLine($"{ts}{ts}");
					sb.AppendLine($"{ts}{ts}if (!IH[{_hpf}{_hkpact}] && !IH[{_hpf}{_hmlact}])");
					sb.AppendLine($"{ts}{ts}{{");
					sb.AppendLine($"{ts}{ts}{ts}/* ongekoppelde voertuigafhankelijke signaalplansturing */");
					sb.AppendLine($"{ts}{ts}{ts}/* ---------------------------------------------------- */");
					sb.AppendLine($"{ts}{ts}{ts}for (fc = 0; fc < FC_MAX; ++fc)  ");
					sb.AppendLine($"{ts}{ts}{ts}{ts}YW_PL[fc] = FALSE;");
					sb.AppendLine($"{ts}{ts}{ts}");
					sb.AppendLine($"{ts}{ts}{ts}FTX = !H[{_hpf}{_homschtegenh}] &&");
					sb.AppendLine($"{ts}{ts}{ts}      versnel_tx(TRUE); /* voertuigafhankelijk */");
					sb.AppendLine($"{ts}{ts}}}");
					sb.AppendLine($"{ts}}} /* Einde code PL-bedrijf */");

					return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCPostSystemApplication:
					return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCPostDumpApplication:
					return sb.ToString();

				default:
					return null;
			}
		}
			
		public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
		{
			_mperiod = CCOLGeneratorSettingsProvider.Default.GetElementName("mperiod");
			_cvc = CCOLGeneratorSettingsProvider.Default.GetElementName("cvc");
			_schmv = CCOLGeneratorSettingsProvider.Default.GetElementName("schmv");
			_schwg = CCOLGeneratorSettingsProvider.Default.GetElementName("schwg");
			_tnlsg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsg");
			_tnlsgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsgd");
			_tnlcv = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcv");
			_tnlcvd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcvd");
			_tnleg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnleg");
			_tnlegd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlegd");

			return base.SetSettings(settings);
		}
	}
}
