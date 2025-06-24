using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
	public class HalfstarCodeGenerator : CCOLCodePieceGeneratorBase
	{
		private string _mperiod;
		private string _cvc;
		private string _schmv;
		private string _schwg;
		private string _schca;
        private string _tnlfg;
        private string _tnlfgd;
        private string _tnlsg;
		private string _tnlsgd;
		private string _tnlcv;
		private string _tnlcvd;
		private string _tnleg;
		private string _tnlegd;
		private string _huks;
		private string _hiks;
        private string _prmxnl;
        private string _hnla;
        private string _hprioin;
        private string _hpriouit;
        private string _treallr;
        private string _tinl;
        private string _trealil;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _usmlact;
		private CCOLGeneratorCodeStringSettingModel _usplact;
		private CCOLGeneratorCodeStringSettingModel _uskpact;
		private CCOLGeneratorCodeStringSettingModel _usmlpl;
		private CCOLGeneratorCodeStringSettingModel _ustxtimer;
		private CCOLGeneratorCodeStringSettingModel _usmaster;
		private CCOLGeneratorCodeStringSettingModel _usslave;
		private CCOLGeneratorCodeStringSettingModel _usklok;
		private CCOLGeneratorCodeStringSettingModel _ushand;
		private CCOLGeneratorCodeStringSettingModel _usleven;
		private CCOLGeneratorCodeStringSettingModel _ussyncok;
		private CCOLGeneratorCodeStringSettingModel _ustxsok;
		private CCOLGeneratorCodeStringSettingModel _uskpuls;
		private CCOLGeneratorCodeStringSettingModel _uspervar;
		private CCOLGeneratorCodeStringSettingModel _usperarh;

		private CCOLGeneratorCodeStringSettingModel _mklok;
		private CCOLGeneratorCodeStringSettingModel _mhand;
		private CCOLGeneratorCodeStringSettingModel _mmaster;
		private CCOLGeneratorCodeStringSettingModel _mslave;
		private CCOLGeneratorCodeStringSettingModel _mleven;
		private CCOLGeneratorCodeStringSettingModel _hkpact;
		private CCOLGeneratorCodeStringSettingModel _hplact;
		private CCOLGeneratorCodeStringSettingModel _hmlact;
		private CCOLGeneratorCodeStringSettingModel _schvar;
		private CCOLGeneratorCodeStringSettingModel _scharh;
		private CCOLGeneratorCodeStringSettingModel _schpervar;
		private CCOLGeneratorCodeStringSettingModel _schslavebep;
		private CCOLGeneratorCodeStringSettingModel _hpervar;
		private CCOLGeneratorCodeStringSettingModel _schperarh;
		private CCOLGeneratorCodeStringSettingModel _hperarh;
		private CCOLGeneratorCodeStringSettingModel _schvarstreng;
		private CCOLGeneratorCodeStringSettingModel _schvaml;
		private CCOLGeneratorCodeStringSettingModel _hplhd;
		private CCOLGeneratorCodeStringSettingModel _schovpriople;
		private CCOLGeneratorCodeStringSettingModel _prmplxper;
		private CCOLGeneratorCodeStringSettingModel _prmtx;
		private CCOLGeneratorCodeStringSettingModel _hxpl;
		private CCOLGeneratorCodeStringSettingModel _schinst;
		private CCOLGeneratorCodeStringSettingModel _schinstprm;
        private CCOLGeneratorCodeStringSettingModel _homschtegenh;
		private CCOLGeneratorCodeStringSettingModel _prmrstotxa;
		private CCOLGeneratorCodeStringSettingModel _tleven;
		private CCOLGeneratorCodeStringSettingModel _hleven;
		private CCOLGeneratorCodeStringSettingModel _prmvolgmasterpl;
		private CCOLGeneratorCodeStringSettingModel _toffset;
		private CCOLGeneratorCodeStringSettingModel _txmarge;
		private CCOLGeneratorCodeStringSettingModel _uspl;

        private CCOLGeneratorCodeStringSettingModel _prmaltphst;
        private CCOLGeneratorCodeStringSettingModel _schaltghst;

        private CCOLGeneratorCodeStringSettingModel _cvchst;
        private CCOLGeneratorCodeStringSettingModel _prmpriohst;

        private CCOLGeneratorCodeStringSettingModel _schtegenov;
        private CCOLGeneratorCodeStringSettingModel _schafkwgov;
        private CCOLGeneratorCodeStringSettingModel _schafkvgov;

        private CCOLGeneratorCodeStringSettingModel _prmnatxdhst;

#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
		{
			_myElements = new List<CCOLElement>();

			if (c.HalfstarData.IsHalfstar)
			{
				var hsd = c.HalfstarData;

                var gelijkstarttuples = CCOLCodeHelper.GetFasenWithGelijkStarts(c);
                if (c.ModuleMolen.LangstWachtendeAlternatief)
                {
                    foreach (var fc in hsd.FaseCyclusInstellingen)
                    {
                        var hasgs = gelijkstarttuples.FirstOrDefault(x => x.Item1 == fc.FaseCyclus && x.Item2.Count > 1);
                        if (hasgs != null)
                        {
                            var namealtphst = _prmaltphst + string.Join(string.Empty, hasgs.Item2);
                            if (_myElements.All(i => i.Naam != namealtphst))
                            {
                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{namealtphst}", fc.AlternatieveRuimte, CCOLElementTimeTypeEnum.TE_type, _prmaltphst, "fasen", string.Join(", ", hasgs.Item2)));
                            }
                        }
                        else
                        {
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmaltphst}{fc.FaseCyclus}", fc.AlternatieveRuimte, CCOLElementTimeTypeEnum.TE_type, _prmaltphst, "fase", fc.FaseCyclus));
                        }
                    }
                    foreach (var fc in hsd.FaseCyclusInstellingen)
                    {
                        var hasgs = gelijkstarttuples.FirstOrDefault(x => x.Item1 == fc.FaseCyclus && x.Item2.Count > 1);
                        if (hasgs != null)
                        {
                            var namealtghst = _schaltghst + string.Join(string.Empty, hasgs.Item2);
                            if (!_myElements.Any(i => i.Naam == namealtghst && i.Type == CCOLElementTypeEnum.Schakelaar))
                                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{namealtghst}", fc.AlternatiefToestaan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schaltghst, "fasen", string.Join(", ", hasgs.Item2)));
                        }
                        else
                        {
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schaltghst}{fc.FaseCyclus}", fc.AlternatiefToestaan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schaltghst, "fase", fc.FaseCyclus));
                        }
                    }
                }

                foreach(var hr in c.HalfstarData.Hoofdrichtingen)
                {
				    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schtegenov}{hr.FaseCyclus}", hr.Tegenhouden ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schtegenov, hr.FaseCyclus));
				    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schafkwgov}{hr.FaseCyclus}", hr.AfkappenWG ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schafkwgov, hr.FaseCyclus));
				    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schafkvgov}{hr.FaseCyclus}", hr.AfkappenVG ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schafkvgov, hr.FaseCyclus));
                }

                if (c.PrioData.PrioIngrepen.Any())
                {
                    foreach (var prio in c.PrioData.PrioIngrepen) _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_cvchst}{CCOLCodeHelper.GetPriorityName(c, prio)}", 999, CCOLElementTimeTypeEnum.CT_type, _cvchst, prio.FaseCyclus, prio.Type.GetDescription()));
                    foreach (var prio in c.PrioData.PrioIngrepen) _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriohst}{CCOLCodeHelper.GetPriorityName(c, prio)}", prio.HalfstarIngreepData.Prioriteit, CCOLElementTimeTypeEnum.None, _prmpriohst, prio.FaseCyclus, prio.Type.GetDescription()));
                    foreach (var prio in c.PrioData.PrioIngrepen) _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmnatxdhst}{CCOLCodeHelper.GetPriorityName(c, prio)}", prio.HalfstarIngreepData.GroenNaTXDTijd, CCOLElementTimeTypeEnum.TE_type, _prmnatxdhst, prio.FaseCyclus, prio.Type.GetDescription()));
                }
                if (c.PrioData.HDIngrepen.Any())
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hplhd}", _hplhd));
                }

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usplact}", _usplact, hsd.PlActUitgang));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uskpact}", _uskpact, hsd.KpActUitgang));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usmlact}", _usmlact, hsd.MlActUitgang));
                var elem = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usmlpl}", _usmlpl, hsd.MlPlUitgang);
                elem.IOMultivalent = true;
                _myElements.Add(elem);
                elem = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_ustxtimer}", _ustxtimer, hsd.TxTimerUitgang);
                elem.IOMultivalent = true;
                _myElements.Add(elem);
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usklok}", _usklok, hsd.KlokUitgang));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_ushand}", _ushand, hsd.HandUitgang));
				
                if (c.HalfstarData.PlantijdenInParameters)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schinstprm}", 0, CCOLElementTimeTypeEnum.SCH_type, _schinstprm));
                }

                foreach (var pl in c.HalfstarData.SignaalPlannen)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(pl.ToString(_uspl), _uspl, pl, pl.Naam));

                    foreach(var fcpl in pl.Fasen)
                    {
                        if (fcpl.B2.HasValue && fcpl.D2.HasValue || c.HalfstarData.PlantijdenInParameters)
                        {
                            var times = new []{ fcpl.A1, fcpl.B1, fcpl.C1, fcpl.D1, fcpl.E1, fcpl.A2, fcpl.B2, fcpl.C2, fcpl.D2, fcpl.E2 };
                            var moments = new [] { "A", "B", "C", "D", "E", "A", "B", "C", "D", "E" };
                            var realisation = 1;
                            for (var i = 0; i < 10; ++i)
                            {
                                if (i == 5) realisation = 2;
                                _myElements.Add(new CCOLElement(
                                    $"{_prmtx}{moments[i]}{realisation}{pl.Naam}_{fcpl.FaseCyclus}",
                                    PrioCodeGeneratorHelper.CAT_Signaalplan,
                                    PrioCodeGeneratorHelper.SUBCAT_Plantijden,
                                    times[i] ?? 0,
                                    CCOLElementTimeTypeEnum.None,
                                    CCOLElementTypeEnum.Parameter,
                                    CCOLGeneratorSettingsProvider.Default.GetElementDescription(_prmtx.Description, CCOLElementTypeEnum.Parameter, realisation == 1 ? "Eerste" : "Tweede", pl.Naam, fcpl.FaseCyclus, moments[i]))
                                {
	                                Categorie = PrioCodeGeneratorHelper.CAT_Signaalplan,
	                                SubCategorie = PrioCodeGeneratorHelper.SUBCAT_Plantijden
                                });
                            }
                        }
                    }
                }
				
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hplact}", _hplact));
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hkpact}", _hkpact));
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hmlact}", _hmlact));
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpervar}", _hpervar));
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hperarh}", _hperarh));
				
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_homschtegenh}", _homschtegenh));
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrstotxa}", 50, CCOLElementTimeTypeEnum.TE_type, _prmrstotxa));
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schinst}", 0, CCOLElementTimeTypeEnum.SCH_type, _schinst));
				
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tleven}", 10, CCOLElementTimeTypeEnum.TE_type, _tleven));
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mleven}", _mleven));
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hleven}", _hleven));
				
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mklok}", _mklok));
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mhand}", _mhand));
				if (c.HalfstarData.Type != HalfstarTypeEnum.Master)
				{
					_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usmaster}", _usmaster, hsd.MasterUitgang));

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usslave}", _usslave, hsd.SlaveUitgang));

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mmaster}", _mmaster));
					_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mslave}", _mslave));
					
					_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schslavebep}", 0, CCOLElementTimeTypeEnum.SCH_type, _schslavebep));
					_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmvolgmasterpl}", 65535, CCOLElementTimeTypeEnum.None, _prmvolgmasterpl));
					_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_toffset}", 0, CCOLElementTimeTypeEnum.TS_type, _toffset));
					_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_txmarge}", 2, CCOLElementTimeTypeEnum.TS_type, _txmarge));
				}
				
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schvaml}", hsd.TypeVARegelen == HalfstarVARegelenTypeEnum.ML ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schvaml));
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schvar}", hsd.VARegelen ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schvar));
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_scharh}", hsd.AlternatievenVoorHoofdrichtingen ? 1 : 0 , CCOLElementTimeTypeEnum.SCH_type, _scharh));
				if (hsd.Type != HalfstarTypeEnum.Slave)
				{
					_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schvarstreng}", 0, CCOLElementTimeTypeEnum.SCH_type, _schvarstreng));
				}

				var iplx = 0;
				for (var index = 0; index < hsd.SignaalPlannen.Count; index++)
				{
					var pl = hsd.SignaalPlannen[index];
					if (hsd.DefaultPeriodeSignaalplan == pl.Naam)
					{
						iplx = index + 1;
						break;
					}
				}
				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmplxper}def", iplx, CCOLElementTimeTypeEnum.None, _prmplxper, "default"));
				var iper = 1;
				foreach (var per in hsd.HalfstarPeriodenData)
				{
					for (var index = 0; index < hsd.SignaalPlannen.Count; index++)
					{
						var pl = hsd.SignaalPlannen[index];
						if (per.Signaalplan == pl.Naam)
						{
							iplx = index + 1;
							break;
						}
					}
					_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmplxper}{(c.PeriodenData.GebruikPeriodenNamen ? per.Periode : iper.ToString())}", iplx, CCOLElementTimeTypeEnum.None, _prmplxper, per.Periode));
					++iper;
				}

				foreach (var k in hsd.GekoppeldeKruisingen)
				{
					_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tleven}{k.KruisingNaam}", 30, CCOLElementTimeTypeEnum.TE_type, _tleven, k.KruisingNaam));
					_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mleven}{k.KruisingNaam}", _mleven, k.KruisingNaam));
					if(k.Type == HalfstarGekoppeldTypeEnum.Master)
					{
					
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"in{k.KruisingNaam}{_usleven}", _usleven, k.InLeven, k.KruisingNaam, "in"));
						_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"in{k.KruisingNaam}{_uskpuls}", _uskpuls, k.InKoppelpuls, k.KruisingNaam, "in"));
						_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"in{k.KruisingNaam}{_uspervar}", _uspervar, k.InPeriodeVARegelen, k.KruisingNaam, "in"));
						_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"in{k.KruisingNaam}{_usperarh}", _usperarh, k.InPeriodenAlternatievenHoofdrichtingen, k.KruisingNaam, "in"));
                        foreach (var pl in hsd.SignaalPlannen)
                        {
                            var plin = k.PlanIngangen.FirstOrDefault(x => x.Plan == pl.Naam);
                        	    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"in{k.KruisingNaam}{pl.Naam}", _uspl, plin, pl.Naam, k.KruisingNaam, "in"));
						}
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"uit{k.KruisingNaam}{_usleven}", _usleven, k.UitLeven, k.KruisingNaam, "uit"));
						_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"uit{k.KruisingNaam}{_ussyncok}", _ussyncok, k.UitSynchronisatieOk, k.KruisingNaam, "uit"));
						_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"uit{k.KruisingNaam}{_ustxsok}", _ustxsok, k.UitTxsOk, k.KruisingNaam, "uit"));
					}
					if (k.Type == HalfstarGekoppeldTypeEnum.Slave)
					{
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"uit{k.KruisingNaam}{_usleven}", _usleven, k.UitLeven, k.KruisingNaam, "uit"));
						_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"uit{k.KruisingNaam}{_uskpuls}", _uskpuls, k.UitKoppelpuls, k.KruisingNaam, "uit"));
						_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"uit{k.KruisingNaam}{_uspervar}", _uspervar, k.UitPeriodeVARegelen, k.KruisingNaam, "uit"));
						_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"uit{k.KruisingNaam}{_usperarh}", _usperarh, k.UitPeriodenAlternatievenHoofdrichtingen, k.KruisingNaam, "uit"));
						foreach (var pl in hsd.SignaalPlannen)
						{
                            var pluit = k.PlanUitgangen.FirstOrDefault(x => x.Plan == pl.Naam);
                        	_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"uit{k.KruisingNaam}{pl.Naam}", _uspl, pluit, pl.Naam, k.KruisingNaam, "uit"));
						}
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"in{k.KruisingNaam}{_usleven}", _usleven, k.InLeven, k.KruisingNaam, "in"));
						_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"in{k.KruisingNaam}{_ussyncok}", _ussyncok, k.InSynchronisatieOk, k.KruisingNaam, "in"));
						_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"in{k.KruisingNaam}{_ustxsok}", _ustxsok, k.InTxsOk, k.KruisingNaam, "in"));
					}
                    var signals = ((IHaveKoppelSignalen)k).UpdateKoppelSignalen();
                    foreach (var s in signals)
                    {
                        CCOLElementCollector.AddKoppelSignaal(k.PTPKruising, s.Count, s.Name, s.Richting);
                    }
                }

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpervar}def", hsd.DefaultPeriodeVARegelen ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schpervar, "default"));
				iper = 1;
				foreach (var per in hsd.HalfstarPeriodenData)
				{
					_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpervar}{(c.PeriodenData.GebruikPeriodenNamen ? per.Periode : iper.ToString())}", 
                        per.VARegelen ? 1 : 0, 
                        CCOLElementTimeTypeEnum.SCH_type, 
                        _schpervar, 
                        per.Periode));
					++iper;
				}

				_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schperarh}def", hsd.DefaultPeriodeAlternatievenVoorHoofdrichtingen ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schperarh, "default"));
				iper = 1;
				foreach (var per in hsd.HalfstarPeriodenData)
				{
					_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schperarh}{(c.PeriodenData.GebruikPeriodenNamen ? per.Periode : iper.ToString())}", per.AlternatievenVoorHoofdrichtingen ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schperarh, per.Periode));
					++iper;
				}

				if (c.PrioData.PrioIngreepType != PrioIngreepTypeEnum.Geen)
				{
					_myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schovpriople}", hsd.OVPrioriteitPL ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schovpriople));
				}

                if (c.InterSignaalGroep.Gelijkstarten.Any())
                {
                    var added = new List<string>();
                    foreach (var gs in gelijkstarttuples)
                    {
                        var hxpl = _hxpl + string.Join(string.Empty, gs.Item2);
                        if (!added.Contains(hxpl))
                        {
                            added.Add(hxpl);
					        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(hxpl, _hxpl, string.Join(" ", gs.Item2)));
                        }
                    }
                }
			}
			
		}

		public override bool HasCCOLElements() => true;
		
        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            if (!c.HalfstarData.IsHalfstar) return base.GetFunctionLocalVariables(c, type);
            return type switch
            {
                CCOLCodeTypeEnum.RegCPreApplication => new List<CCOLLocalVariable>{new("int", "fc")},
                CCOLCodeTypeEnum.HstCAanvragen => new List<CCOLLocalVariable>{new("int", "fc")},
                CCOLCodeTypeEnum.HstCVerlenggroen => new List<CCOLLocalVariable>{new("int", "fc")},
                CCOLCodeTypeEnum.HstCMaxgroen => new List<CCOLLocalVariable>{new("int", "fc")},
                CCOLCodeTypeEnum.HstCMeetkriterium => new List<CCOLLocalVariable>{new("int", "fc")},
                CCOLCodeTypeEnum.HstCMeeverlengen => new List<CCOLLocalVariable>{new("int", "fc")},
                CCOLCodeTypeEnum.HstCSynchronisaties => new List<CCOLLocalVariable>{new("int", "fc")},
                CCOLCodeTypeEnum.HstCRealisatieAfhandeling => new List<CCOLLocalVariable>{new("int", "fc")},
                CCOLCodeTypeEnum.HstCPreSystemApplication => new List<CCOLLocalVariable>{new("int", "fc")},
                CCOLCodeTypeEnum.PrioCPrioriteitsOpties => new List<CCOLLocalVariable>{new("int", "fc")},
                CCOLCodeTypeEnum.PrioCPostAfhandelingPrio => new List<CCOLLocalVariable>{new("int", "fc")},
                CCOLCodeTypeEnum.HstCAlternatief => new List<CCOLLocalVariable>{new("int", "fc")},
                CCOLCodeTypeEnum.HstCKlokPerioden => c.HalfstarData.Type != HalfstarTypeEnum.Master
				? new List<CCOLLocalVariable>
                  {
                      new("char", "volgMaster", "TRUE")
                  }
				: base.GetFunctionLocalVariables(c, type),
                _ => base.GetFunctionLocalVariables(c, type)
            };
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCPreApplication => new []{31},
                CCOLCodeTypeEnum.HstCPreApplication => new []{10},
                CCOLCodeTypeEnum.HstCKlokPerioden => new []{10},
                CCOLCodeTypeEnum.HstCAanvragen => new []{10},
                CCOLCodeTypeEnum.HstCVerlenggroen => new []{10},
                CCOLCodeTypeEnum.HstCMaxgroen => new []{10},
                CCOLCodeTypeEnum.HstCWachtgroen => new []{10},
                CCOLCodeTypeEnum.HstCMeetkriterium => new []{10},
                CCOLCodeTypeEnum.HstCMeeverlengen => new []{10},
                CCOLCodeTypeEnum.HstCSynchronisaties => new []{10},
                CCOLCodeTypeEnum.HstCAlternatief => new []{20},
                CCOLCodeTypeEnum.HstCRealisatieAfhandeling => new []{10},
                CCOLCodeTypeEnum.HstCPostApplication => new []{10},
                CCOLCodeTypeEnum.HstCPreSystemApplication => new []{10},
                CCOLCodeTypeEnum.HstCPostSystemApplication => new []{10},
                CCOLCodeTypeEnum.HstCPostDumpApplication => new []{10},
                CCOLCodeTypeEnum.HstCPrioHalfstarSettings => new []{10},
                CCOLCodeTypeEnum.PrioCInitPrio => new []{10},
                CCOLCodeTypeEnum.PrioCInstellingen => new []{10},
                CCOLCodeTypeEnum.PrioCPrioriteitsOpties => new []{20},
                CCOLCodeTypeEnum.PrioCOnderMaximum => new []{10},
                CCOLCodeTypeEnum.PrioCAfkapGroen => new []{10},
                CCOLCodeTypeEnum.PrioCStartGroenMomenten => new []{10},
                CCOLCodeTypeEnum.PrioCTegenhoudenConflicten => new []{20},
                CCOLCodeTypeEnum.PrioCAfkappen => new []{10},
                CCOLCodeTypeEnum.PrioCTerugkomGroen => new []{10},
                CCOLCodeTypeEnum.PrioCGroenVasthouden => new []{10},
                CCOLCodeTypeEnum.PrioCMeetkriterium => new []{10},
                _ => null
            };
        }

		public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
		{
			if (!c.HalfstarData.IsHalfstar || !c.HalfstarData.SignaalPlannen.Any())
			{
				return "";
			}

			var sb = new StringBuilder();
			var master = c.HalfstarData.GekoppeldeKruisingen.FirstOrDefault(x => x.IsMaster);

			if (c.HalfstarData.Type != HalfstarTypeEnum.Master && master == null)
			{
				return "";
			}

			switch (type)
			{
                #region reg.c

                case CCOLCodeTypeEnum.RegCPreApplication:
					sb.AppendLine($"{ts}/* Bepalen of regeling mag omschakelen */");
					sb.AppendLine($"{ts}/* Tegenhouden inschakelen naar PL als een naloop nog actief is of als inrijden/inlopen actief is */");
                    sb.AppendLine($"{ts}/* Opzetten IH[homschtegenh] */");
					sb.AppendLine($"{ts}if (!IH[{_hpf}{_hkpact}] && !IH[{_hpf}{_hpervar}] && !SCH[{_schpf}{_schvar}] && !IH[{_hpf}{_hplhd}])");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}IH[{_hpf}{_homschtegenh}] = TRUE;");
                    sb.AppendLine($"{ts}}}");
					sb.AppendLine();
                    sb.AppendLine($"{ts}/* Wenselijk is dat pas wordt omgeschakeld naar PL wanneer nalopen zijn afgemaakt; echter andere (voedende)");
                    sb.AppendLine($"{ts} * richtingen moeten in deze tijd niet groen kunnen worden, anders bestaat het risico dat er permanent");
                    sb.AppendLine($"{ts} * wordt gewacht op nieuwe nalopen.");
                    sb.AppendLine($"{ts} */");
                    sb.AppendLine($"{ts}/* reset */");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}RR[fc] &= ~RR_INSCH_HALFSTAR;");
                    sb.AppendLine($"{ts}{ts}Z[fc] &= ~Z_INSCH_HALFSTAR;");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}/* set voor alle richtingen waar een richting al inloopt of inrijdt */");
                    sb.AppendLine($"{ts}if (IH[homschtegenh]) /* tegenhouden inschakelen naar PL */");
                    sb.AppendLine($"{ts}{{");
                    var tinl = c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.RealFunc ? _trealil : _tinl;
                    foreach (var nl in c.InterSignaalGroep.Nalopen)
					{
						if (nl.Type == NaloopTypeEnum.StartGroen && nl.MaximaleVoorstart.HasValue)
                            sb.AppendLine($"{ts}{ts}if (!(T[{_tpf}{tinl}{nl:vannaar}] || RT[{_tpf}{tinl}{nl:vannaar}])) RR[{_fcpf}{nl:naar}] |= RR_INSCH_HALFSTAR;");
						else
							sb.AppendLine($"{ts}{ts}RR[{_fcpf}{nl:van}] |= RR_INSCH_HALFSTAR;");
					}
                    foreach (var nl in c.InterSignaalGroep.Nalopen)
                    {
                        if (nl.Type == NaloopTypeEnum.StartGroen && nl.MaximaleVoorstart.HasValue)
						{
							var tnl = nl.DetectieAfhankelijk && nl.Detectoren.Count > 0 ? _tnlsgd : _tnlsg;
                            sb.AppendLine($"{ts}{ts}if (!(VS[{_fcpf}{nl:naar}] || FG[{_fcpf}{nl:naar}] || T[{_tpf}{tnl}{nl:vannaar}])) Z[{_fcpf}{nl:naar}] |= Z_INSCH_HALFSTAR;");
						}
                        else
                            sb.AppendLine($"{ts}{ts}if (!(VS[{_fcpf}{nl:van}] || FG[{_fcpf}{nl:van}])) Z[{_fcpf}{nl:van}] |= Z_INSCH_HALFSTAR;");
                    }
                    sb.AppendLine($"{ts}}}");

                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Afzetten IH[homschtegenh] */");
                    sb.AppendLine($"{ts}if (!IH[{_hpf}{_hkpact}] && !IH[{_hpf}{_hpervar}] && !SCH[{_schpf}{_schvar}] && !IH[{_hpf}{_hplhd}])");
                    sb.AppendLine($"{ts}{{");
					if (c.InterSignaalGroep.Nalopen.Count > 0)
					{
						sb.Append($"{ts}{ts}if (");
						var k = 0;
						foreach (var nl in c.InterSignaalGroep.Nalopen)
						{
							if (k != 0)
							{
								sb.AppendLine(" &&");
								sb.Append($"{ts}{ts}    ");
							}
							var tnlf = nl.Type switch
							{
								NaloopTypeEnum.StartGroen => null,
								NaloopTypeEnum.EindeGroen => _tnlfg,
								NaloopTypeEnum.CyclischVerlengGroen => _tnlfg,
								_ => throw new NotImplementedException(),
							};
							var tnlfd = nl.Type switch
							{
								NaloopTypeEnum.StartGroen => null,
								NaloopTypeEnum.EindeGroen => _tnlfgd,
								NaloopTypeEnum.CyclischVerlengGroen => _tnlfgd,
								_ => throw new NotImplementedException(),
							};
							var tnl = nl.Type switch
							{
								NaloopTypeEnum.StartGroen => _tnlsg,
								NaloopTypeEnum.EindeGroen => _tnleg,
								NaloopTypeEnum.CyclischVerlengGroen => _tnlcv,
								_ => throw new NotImplementedException(),
							};
							var tnld = nl.Type switch
							{
								NaloopTypeEnum.StartGroen => _tnlsgd,
								NaloopTypeEnum.EindeGroen => _tnlegd,
								NaloopTypeEnum.CyclischVerlengGroen => _tnlcvd,
								_ => throw new NotImplementedException(),
							};
							if (!nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.VastGroen)) 
							{
								tnlf = null;
								tnlfd = null;
							}
							if (!nl.DetectieAfhankelijk || nl.Detectoren.Count == 0)
							{ 
								tnld = null;
								tnlfd = null;
							}
							if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.VastGroen))
							{
								sb.Append($"!T[{_tpf}{tnlf}{nl:vannaar}] && !RT[{_tpf}{tnlf}{nl:vannaar}] && ");
							}
							if (nl.VasteNaloop)
							{ 
								sb.Append($"!T[{_tpf}{tnl}{nl:vannaar}] && !RT[{_tpf}{tnl}{nl:vannaar}]");
							}
							if (nl.VasteNaloop && nl.DetectieAfhankelijk && nl.Detectoren.Count > 0)
							{
								sb.Append(" && ");
							}
							if (nl.DetectieAfhankelijk && nl.Detectoren.Count > 0)
							{
								sb.Append($"!T[{_tpf}{tnld}{nl:vannaar}] && !RT[{_tpf}{tnld}{nl:vannaar}]");
								if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.VastGroen))
								{
									sb.Append($" && !T[{_tpf}{tnlfd}{nl:vannaar}] && !RT[{_tpf}{tnlfd}{nl:vannaar}]");
								}
							}
							++k;
						}

                        var maxVs = false;
						foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.MaximaleVoorstart.HasValue))
                        {
                            maxVs = true;
							if (k != 0)
							{
								sb.AppendLine(" &&");
								sb.Append($"{ts}{ts}    ");
							}

							var sgv = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseVan);
							var sgn = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseNaar);
							if (nl.DetectieAfhankelijk && nl.Detectoren?.Count > 0 &&
								sgv is { Type: FaseTypeEnum.Voetganger } && sgn is { Type: FaseTypeEnum.Voetganger })
							{

								if (nl.MaximaleVoorstart.HasValue)
								{
									sb.Append($"!T[{_tpf}{tinl}{nl.FaseVan}{nl.FaseNaar}] && !RT[{_tpf}{tinl}{nl.FaseVan}{nl.FaseNaar}] ");
								}
							}
							else
							{
								if (nl.MaximaleVoorstart.HasValue)
								{
									var tt = sgv is { Type: FaseTypeEnum.Voetganger } && sgn is { Type: FaseTypeEnum.Voetganger }
										? tinl
										: _treallr;
									sb.Append($"!T[{_tpf}{tt}{nl.FaseNaar}{nl.FaseVan}] && !RT[{_tpf}{tt}{nl.FaseNaar}{nl.FaseVan}]");
								}
							}
							++k;
						}

                        if (maxVs) sb.AppendLine($"{ts}{ts}");
                        sb.AppendLine($")");
						sb.AppendLine($"{ts}{ts}{{");
						sb.AppendLine($"{ts}{ts}{ts}IH[{_hpf}{_homschtegenh}] = FALSE;");
						sb.AppendLine($"{ts}{ts}}}");
					}
					else
					{ 
                        sb.AppendLine($"{ts}{ts}IH[{_hpf}{_homschtegenh}] = FALSE;");
					}
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();

                #endregion // reg.c

                #region hst.c
                
                case CCOLCodeTypeEnum.HstCTop:
                    return sb.ToString();

				case CCOLCodeTypeEnum.HstCPreApplication:
					sb.AppendLine($"{ts}/* na omschakeling van PL -> VA, modules opnieuw initialiseren */");
					sb.AppendLine($"{ts}if (SH[{_hpf}{_hplact}] || EH[{_hpf}{_hplact}])");
					sb.AppendLine($"{ts}{{");
                    if (c.Data.MultiModuleReeksen)
                    {
                        foreach (var r in c.MultiModuleMolens.Where(x => x.Modules.Any(x2 => x2.Fasen.Any())))
                        {
                            sb.AppendLine($"{ts}{ts}init_modules({r.Reeks}_MAX, PR{r.Reeks}, Y{r.Reeks}, &{r.Reeks}, &S{r.Reeks});");
                        }
                    }
                    else
                    {
					    sb.AppendLine($"{ts}{ts}init_modules(ML_MAX, PRML, YML, &ML, &SML);");
                    }
					sb.AppendLine($"{ts}{ts}sync_pg();");
					sb.AppendLine($"{ts}{ts}reset_fc_halfstar();");
					sb.AppendLine($"{ts}}}");
					sb.AppendLine();
                    sb.AppendLine($"{ts}if (IH[{_hpf}{_hkpact}])");
					sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}/* bijhouden verlenggroentijden t.b.v. calculaties diverse functies */");
					sb.AppendLine($"{ts}{ts}tvga_timer_halfstar();");
					sb.AppendLine($"{ts}}}");
					//sb.AppendLine();
                    //if (c.OVData.OVIngreepType != OVIngreepTypeEnum.Geen)
					//{
					//	sb.AppendLine($"{ts}/* tbv PRIO_ple */");
					//	sb.AppendLine($"{ts}if (SCH[{_schpf}{_schovpriople}])");
					//	sb.AppendLine($"{ts}{{");
					//	sb.AppendLine($"{ts}{ts}/* Instellen OV parameters */");
					//	sb.AppendLine($"{ts}{ts}if (CIF_PARM1WIJZPB != CIF_GEEN_PARMWIJZ ||");
					//	sb.AppendLine($"{ts}{ts}    CIF_PARM1WIJZAP != CIF_GEEN_PARMWIJZ)");
					//	sb.AppendLine($"{ts}{ts}{{");
					//	sb.AppendLine($"{ts}{ts}{ts}PrioHalfstarSettings();");
					//	sb.AppendLine($"{ts}{ts}}}");
					//	sb.AppendLine($"{ts}{ts}{ts}");
					//	sb.AppendLine($"{ts}{ts}BijhoudenWachtTijd();");
					//	sb.AppendLine($"{ts}{ts}BijhoudenMinimumGroenTijden();");
					//	sb.AppendLine($"{ts}}}");
					//	sb.AppendLine();
					//}
					return sb.ToString();

				case CCOLCodeTypeEnum.HstCKlokPerioden:
					sb.AppendLine($"{ts}/* BepaalKoppeling */");
					sb.AppendLine($"{ts}/* --------------- */");

					#region Reset data

					sb.AppendLine($"{ts}MM[{_mpf}{_mklok}] = FALSE;");
					sb.AppendLine($"{ts}MM[{_mpf}{_mhand}] = FALSE;");
					if (c.HalfstarData.Type != HalfstarTypeEnum.Master)
					{
						sb.AppendLine($"{ts}MM[{_mpf}{_mmaster}] = FALSE;");
						sb.AppendLine($"{ts}MM[{_mpf}{_mslave}] = FALSE;");
                    }
					sb.AppendLine($"{ts}IH[{_hpf}{_hkpact}] = TRUE;");
					sb.AppendLine($"{ts}IH[{_hpf}{_hplact}] = TRUE;");
					sb.AppendLine($"{ts}IH[{_hpf}{_hmlact}] = FALSE;");
					sb.AppendLine($"{ts}APL = NG;");
					sb.AppendLine();

					#endregion // Reset data

					#region Bepalen PL/var/arh door master

					if (c.HalfstarData.Type != HalfstarTypeEnum.Master)
					{
						if (master != null)
						{
							sb.AppendLine($"/* Master bepaalt wat er gaat gebeuren */");
							sb.AppendLine($"{ts}if (MM[{_mpf}{_mleven}{master.KruisingNaam}] && !SCH[{_schpf}{_schslavebep}])");
							sb.AppendLine($"{ts}{{");
							sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mmaster}] = TRUE;");
							sb.AppendLine();
							sb.Append($"{ts}{ts}if      ");
							var i = 0;
							foreach (var pl in c.HalfstarData.SignaalPlannen)
							{
								if (i > 0)
								{
									sb.AppendLine();
									sb.Append($"{ts}{ts}else if ");
								}
                                var ipl = CCOLElementCollector.GetKoppelSignaalCount(master.PTPKruising, $"{master.KruisingNaam}pl{pl.Naam}", KoppelSignaalRichtingEnum.In);
								sb.Append($"(IH[{_hpf}{master.PTPKruising}{_hiks}{ipl:00}]) APL = {pl.Naam};");
								++i;
							}
							sb.AppendLine();
							sb.AppendLine($"{ts}{ts}else APL = PL1;");
							sb.AppendLine();
							sb.AppendLine($"{ts}{ts}if (PRM[{_prmpf}{_prmvolgmasterpl}] > 0)");
							sb.AppendLine($"{ts}{ts}{{");
							i = 1;
							sb.Append($"{ts}{ts}{ts}if (");
							foreach (var pl in c.HalfstarData.SignaalPlannen)
							{
								if (i > 1)
								{
									sb.AppendLine(" ||");
								}
								sb.Append($"{ts}{ts}{ts}    (APL == {pl.Naam}) && !(PRM[{_prmpf}{_prmvolgmasterpl}] & BIT{i})");
								++i;
							}
							sb.AppendLine($")");
							sb.AppendLine($"{ts}{ts}{ts}{{");
							sb.AppendLine($"{ts}{ts}{ts}{ts}volgMaster = FALSE;");
							sb.AppendLine($"{ts}{ts}{ts}}}");
							sb.AppendLine($"{ts}{ts}}}");
							sb.AppendLine($"{ts}{ts}if (volgMaster == FALSE)");
							sb.AppendLine($"{ts}{ts}{{");
							sb.AppendLine($"{ts}{ts}{ts}IH[{_hpf}{_hkpact}] = FALSE;");
							sb.AppendLine($"{ts}{ts}}}");
							sb.AppendLine($"{ts}{ts}else");
							sb.AppendLine($"{ts}{ts}{{");
                            var ipl2 = CCOLElementCollector.GetKoppelSignaalCount(master.PTPKruising, $"{master.KruisingNaam}pervar", KoppelSignaalRichtingEnum.In);
                            sb.AppendLine($"{ts}{ts}{ts}IH[{_hpf}{_hpervar}] =  IH[{_hpf}{master.PTPKruising}{_hiks}{ipl2:00}];");
                            ipl2 = CCOLElementCollector.GetKoppelSignaalCount(master.PTPKruising, $"{master.KruisingNaam}perarh", KoppelSignaalRichtingEnum.In);
							sb.AppendLine($"{ts}{ts}{ts}IH[{_hpf}{_hperarh}] =  IH[{_hpf}{master.PTPKruising}{_hiks}{ipl2:00}];");
							sb.AppendLine($"{ts}{ts}}}");
							sb.AppendLine($"{ts}}}");
						}
					}

					#endregion // Bepalen PL/var/arh door master

					#region Zelf bepalen PL/var/arh
					
					var mts = c.HalfstarData.Type == HalfstarTypeEnum.Master ? ts : ts + ts;

					if (c.HalfstarData.Type != HalfstarTypeEnum.Master)
					{
						sb.AppendLine(
							$"{ts}/* Bij afwezigheid Master bepaalt Slave zelf wat er gaat gebeuren. In dit geval neemt de slave de functie van Master over */");
						sb.AppendLine($"{ts}else");
						sb.AppendLine($"{ts}{{");
						sb.AppendLine($"{mts}MM[{_mpf}{_mslave}] = TRUE;");
					}

					sb.AppendLine($"{mts}switch (MM[{_mpf}{_mperiod}])");
					sb.AppendLine($"{mts}{{");
					sb.AppendLine($"{mts}{ts}case 0: /* default */");
					sb.AppendLine($"{mts}{ts}{{");
					sb.AppendLine($"{mts}{ts}{ts}APL = PRM[{_prmpf}{_prmplxper}def] - 1;");
					sb.AppendLine($"{mts}{ts}{ts}break;");
					sb.AppendLine($"{mts}{ts}}}");
					var iper = 1;
					foreach (var per in c.HalfstarData.HalfstarPeriodenData)
					{
						sb.AppendLine($"{mts}{ts}case {iper}: /* default */");
						sb.AppendLine($"{mts}{ts}{{");
						sb.AppendLine($"{mts}{ts}{ts}APL = PRM[{_prmpf}{_prmplxper}{(c.PeriodenData.GebruikPeriodenNamen ? per.Periode : iper.ToString())}] - 1;");
						sb.AppendLine($"{mts}{ts}{ts}break;");
						sb.AppendLine($"{mts}{ts}}}");
						++iper;
					}
					sb.AppendLine($"{mts}{ts}default:");
					sb.AppendLine($"{mts}{ts}{{");
					sb.AppendLine($"{mts}{ts}{ts}APL = PRM[{_prmpf}{_prmplxper}def] - 1;");
					sb.AppendLine($"{mts}{ts}{ts}break;");
					sb.AppendLine($"{mts}{ts}}}");
					sb.AppendLine($"{mts}}}");

					#region Klok bepaling VA bedrijf

					sb.AppendLine($"{mts}/* Klokbepaling voor VA-bedrijf */");
					sb.Append($"{mts}if ((SCH[{_schpf}{_schpervar}def] && (MM[{_mpf}{_mperiod}] == 0)");
					iper = 1;
					foreach (var per in c.HalfstarData.HalfstarPeriodenData)
					{
						sb.AppendLine(") ||");
						sb.Append($"{mts}{ts}(SCH[{_schpf}{_schpervar}{(c.PeriodenData.GebruikPeriodenNamen ? per.Periode : iper.ToString())}] && (MM[{_mpf}{_mperiod}] == {iper})");
						++iper;
					}
					sb.AppendLine("))");
					sb.AppendLine($"{mts}{{");
					sb.AppendLine($"{mts}{ts}IH[{_hpf}{_hpervar}] = TRUE;");
					sb.AppendLine($"{mts}}}");
					sb.AppendLine($"{mts}else");
					sb.AppendLine($"{mts}{{");
					sb.AppendLine($"{mts}{ts}IH[{_hpf}{_hpervar}] = FALSE;");
					sb.AppendLine($"{mts}}}");
					sb.AppendLine();

					#endregion // Klok bepaling VA bedrijf

					#region Klok bepaling alternatieven hoofdrichtingen

					sb.AppendLine($"{mts}/* Klokbepaling voor alternatieve realisaties voor de hoofdrichtingen */");
					sb.Append($"{mts}if ((SCH[{_schpf}{_schperarh}def] && (MM[{_mpf}{_mperiod}] == 0)");
					iper = 1;
					foreach (var per in c.HalfstarData.HalfstarPeriodenData)
					{
						sb.AppendLine(") ||");
						sb.Append($"{mts}    (SCH[{_schpf}{_schperarh}{(c.PeriodenData.GebruikPeriodenNamen ? per.Periode : iper.ToString())}] && (MM[{_mpf}{_mperiod}] == {iper})");
						++iper;
					}
					sb.AppendLine("))");
					sb.AppendLine($"{mts}{{");
					sb.AppendLine($"{mts}{ts}IH[{_hpf}{_hperarh}] = TRUE;");
					sb.AppendLine($"{mts}}}");
					sb.AppendLine($"{mts}else");
					sb.AppendLine($"{mts}{{");
					sb.AppendLine($"{mts}{ts}IH[{_hpf}{_hperarh}] = FALSE;");
					sb.AppendLine($"{mts}}}");
					
					#endregion // Bepalen alternatieven hoofdrichtingen

					if (c.HalfstarData.Type != HalfstarTypeEnum.Master)
					{
						sb.AppendLine($"{ts}}}");
					}
					sb.AppendLine();

					#endregion // Zelf bepalen PL/var/arh

					#region Bepalen VA bedrijf schakelaar

					sb.AppendLine($"{ts}/* Klokbepaling voor VA-bedrijf */");
					if (c.HalfstarData.Type == HalfstarTypeEnum.Slave)
					{
						sb.AppendLine($"{ts}if (SCH[{_schpf}{_schvar}])");
					}
					else
					{
						sb.AppendLine($"{ts}if (SCH[{_schpf}{_schvar}] || SCH[{_schpf}{_schvarstreng}])");
					}
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}/* Halfstar/va afhankelijk van schakelaar */");
					sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hkpact}] = FALSE;");
					sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mhand}]  = TRUE;");
					if (c.HalfstarData.Type != HalfstarTypeEnum.Master)
					{
						sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mmaster}]  = FALSE;");
						sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mslave}]  = FALSE;");
					}
					sb.AppendLine($"{ts}}} ");
					sb.AppendLine($"{ts}else");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mklok}] = TRUE;");
					sb.AppendLine($"{ts}}}");
					sb.AppendLine();

					#endregion // Bepalen VA bedrijf schakelaar

					#region Bepalen alternatieven hoofdrichtingen schakelaar

					sb.AppendLine($"{ts}/* Toestaan alternatief hoofdrichtingen ook mogelijk met schakelaar */");
					sb.AppendLine($"{ts}if (SCH[{_schpf}{_scharh}])");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hperarh}] = TRUE;");
					sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mhand}]   = TRUE;");
					sb.AppendLine($"{ts}}}");

					#endregion // Bepalen alternatieven hoofdrichtingen schakelaar

					sb.AppendLine($"{ts}/* Koppelen actief */");
					switch (c.HalfstarData.Type)
					{
						case HalfstarTypeEnum.Master:
							sb.AppendLine($"{ts}if (H[{_hpf}{_hpervar}] || SCH[{_schpf}{_schvar}] || SCH[{_schpf}{_schvarstreng}] || IH[{_hpf}{_homschtegenh}])");
							break;
						case HalfstarTypeEnum.FallbackMaster:
							sb.AppendLine($"{ts}if (H[{_hpf}{_hpervar}] || SCH[{_schpf}{_schvar}] || SCH[{_schpf}{_schvarstreng}] || IH[{_hpf}{_homschtegenh}])");
							break;
						case HalfstarTypeEnum.Slave:
							sb.AppendLine($"{ts}if (H[{_hpf}{_hpervar}] || SCH[{_schpf}{_schvar}] || IH[{_hpf}{_homschtegenh}])");
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hkpact}] = FALSE;");
					sb.AppendLine($"{ts}else");
					sb.AppendLine($"{ts}{ts}IH[{_hpf}{_hkpact}] = TRUE;");

					sb.AppendLine($"{ts}/* Indien VA-bedrijf, dan met schakelaar te bepalen of dit in ML-bedrijf of in versneld PL-bedrijf gebeurt */");
					sb.AppendLine($"{ts}if (!IH[{_hpf}{_hkpact}])");
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

					if (c.PrioData.HDIngrepen.Any())
					{
						sb.AppendLine($"{ts}/* Bij hulpdienstingreep, lokaal VA regelen */");
						sb.AppendLine($"{ts}if (IH[{_hpf}{_hplhd}])");
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
                    if(c.HalfstarData.FaseCyclusInstellingen.Any(x => x.AanvraagOpTxB))
                    {
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* Aanvragen op TXB */");
                        foreach(var sg in c.HalfstarData.FaseCyclusInstellingen.Where(x => x.AanvraagOpTxB))
                        {
                            sb.Append($"{ts}if (aanvraag_txb({_fcpf}{sg.FaseCyclus})");
                            if (sg.PrivilegePeriodeOpzetten) sb.Append($" && PP[{_fcpf}{sg.FaseCyclus}]");
                            sb.AppendLine($") A[{_fcpf}{sg.FaseCyclus}] |= TRUE;");
                        }
                    }
					sb.AppendLine($"{ts}}}");
					return sb.ToString();

				case CCOLCodeTypeEnum.HstCVerlenggroen:
				case CCOLCodeTypeEnum.HstCMaxgroen:
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}/* afzetten functies en BITJES van ML-bedrijf */");
					sb.AppendLine($"{ts}{ts}TVG_max[fc] = 0;");
					sb.AppendLine($"{ts}{ts}YV[fc] &= ~(BIT2 | BIT4);");
					sb.AppendLine($"{ts}{ts}FM[fc] &= ~BIT2;");
					sb.AppendLine($"{ts}{ts}RW[fc] &= ~BIT2;");
					sb.AppendLine($"{ts}{ts}/* opzetten verlengfunctie (Vasthouden verlenggroen) bij PL-bedrijf */");
					sb.AppendLine($"{ts}{ts}YV[fc] |= MK[fc] && (YV_PL[fc] && PR[fc] || AR[fc] && yv_ar_max_pl(fc, 0)) ? BIT4 : 0;");
					sb.AppendLine($"{ts}}}");
					return sb.ToString();

				case CCOLCodeTypeEnum.HstCWachtgroen:
					sb.AppendLine($"{ts}/* Retour wachtgroen bij wachtgroen richtingen, let op: inclusief aanvraag! */");
					foreach (var fc in c.Fasen)
					{
						if (fc.Wachtgroen != NooitAltijdAanUitEnum.Nooit)
						{
							if (fc.Wachtgroen == NooitAltijdAanUitEnum.Altijd)
							{
								sb.AppendLine($"{ts}wachtstand_halfstar({_fcpf}{fc.Naam}, IH[{_hpf}{_hplact}], ({c.GetBoolV()})(TRUE), ({c.GetBoolV()})(TRUE));");
							}
							else
							{
								sb.AppendLine($"{ts}wachtstand_halfstar({_fcpf}{fc.Naam}, IH[{_hpf}{_hplact}], ({c.GetBoolV()})(SCH[{_schpf}{_schca}{fc.Naam}]), ({c.GetBoolV()})(SCH[{_schpf}{_schwg}{fc.Naam}]));");								
							}
						}
					}
					return sb.ToString();

				case CCOLCodeTypeEnum.HstCMeetkriterium:
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}/* afzetten BITJES van ML-bedrijf */");
					if (c.PrioData.PrioIngreepType != PrioIngreepTypeEnum.Geen)
					{
						sb.AppendLine($"{ts}{ts}{ts} Z[fc] &= ~PRIO_Z_BIT;");
						sb.AppendLine($"{ts}{ts}{ts}FM[fc] &= ~PRIO_FM_BIT;");
						sb.AppendLine($"{ts}{ts}{ts}RW[fc] &= ~PRIO_RW_BIT;");
						sb.AppendLine($"{ts}{ts}{ts}RR[fc] &= ~PRIO_RR_BIT;");
						sb.AppendLine($"{ts}{ts}{ts}YV[fc] &= ~PRIO_YV_BIT;");
						sb.AppendLine($"{ts}{ts}{ts}MK[fc] &= ~PRIO_MK_BIT;");
						sb.AppendLine($"{ts}{ts}{ts}PP[fc] &= ~PRIO_PP_BIT;");
					}
					else
					{
                        sb.AppendLine($"{ts}{ts}{ts} Z[fc] &= ~BIT6;");
                        sb.AppendLine($"{ts}{ts}{ts}FM[fc] &= ~BIT6;");
                        sb.AppendLine($"{ts}{ts}{ts}RW[fc] &= ~BIT6;");
                        sb.AppendLine($"{ts}{ts}{ts}RR[fc] &= ~BIT6;");
                        sb.AppendLine($"{ts}{ts}{ts}YV[fc] &= ~BIT6;");
                        sb.AppendLine($"{ts}{ts}{ts}MK[fc] &= ~BIT11; /* Hier geen BIT6 wegens conflict met MeetKriteriumRGprm */");
                        sb.AppendLine($"{ts}{ts}{ts}PP[fc] &= ~BIT6;");
                    }
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    if (c.PrioData.PrioIngreepType != PrioIngreepTypeEnum.Geen)
                    {
                        sb.AppendLine($"{ts}if (SCH[{_schpf}{_schovpriople}])");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}/* Prio meetkriterium bij PL bedrijf */");
                        foreach (var prio in c.PrioData.PrioIngrepen)
                        {
                            sb.AppendLine($"{ts}{ts}yv_PRIO_pl_halfstar({_fcpf}{prio.FaseCyclus}, BIT7, C[{_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, prio)}]);");
                        }
                        sb.AppendLine($"{ts}}}");
                    }

					return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCMeeverlengen:
					sb.AppendLine($"{ts}/* Resetten YM bit voor PL regelen */");
					sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
					sb.AppendLine($"{ts}{ts}YM[fc] &= ~YM_HALFSTAR;");
					sb.AppendLine();

                    foreach (var fc in c.Fasen.Where(x => x.Meeverlengen != NooitAltijdAanUitEnum.Nooit))
                    {
                        var set_ym_pl_halfstar = "set_ym_pl_halfstar";
                        var set_ym_pl_halfstar_args = "";
                        if (c.Data.MultiModuleReeksen)
                        {
                            set_ym_pl_halfstar = "set_ym_pl_halfstar_fcfc";
                            var reeks = c.MultiModuleMolens.FirstOrDefault(x => x.Modules.Any(x2 => x2.Fasen.Any(x3 => x3.FaseCyclus == fc.Naam)));
                            if (reeks != null)
                            {
                                var rfc1 = c.Fasen.FirstOrDefault(x => reeks.Modules.SelectMany(x2 => x2.Fasen).Any(x3 => x3.FaseCyclus == x.Naam));
                                var rfc2 = c.Fasen.LastOrDefault(x => reeks.Modules.SelectMany(x2 => x2.Fasen).Any(x3 => x3.FaseCyclus == x.Naam));
                                if (rfc1 == null || rfc2 == null)
                                {
                                    set_ym_pl_halfstar_args = ", 0, FCMAX";
                                }
                                else
                                {
                                    var id2 = c.Fasen.IndexOf(rfc2);
                                    ++id2;
                                    set_ym_pl_halfstar_args = $", {_fcpf}{rfc1.Naam}, {(id2 == c.Fasen.Count ? "FCMAX" : $"{_fcpf}{c.Fasen[id2].Naam}")}";
                                }
                            }
                            else
                            {
                                set_ym_pl_halfstar_args = ", 0, FCMAX";
                            }
                        }

                        switch (fc.Meeverlengen)
                        {
                            case NooitAltijdAanUitEnum.Altijd:
                                sb.AppendLine($"{ts}{set_ym_pl_halfstar}({_fcpf}{fc.Naam}, TRUE{set_ym_pl_halfstar_args});");
                                break;
                            case NooitAltijdAanUitEnum.SchAan:
                            case NooitAltijdAanUitEnum.SchUit:
                                sb.AppendLine($"{ts}{set_ym_pl_halfstar}({_fcpf}{fc.Naam}, ({c.GetBoolV()})(SCH[{_schpf}{_schmv}{fc.Naam}]){set_ym_pl_halfstar_args});");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

					return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCSynchronisaties:
					sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
					sb.AppendLine($"{ts}{ts}YV[fc] &= ~YV_KOP_HALFSTAR;");
					sb.AppendLine();
					sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}RR[fc]&= ~(BIT1 | BIT2 | BIT3 | RR_KOP_HALFSTAR | RR_VS_HALFSTAR);");
					sb.AppendLine($"{ts}{ts}RW[fc]&= ~(BIT3 | RW_KOP_HALFSTAR);");
					sb.AppendLine($"{ts}{ts}YV[fc]&= ~(BIT1 | YV_KOP_HALFSTAR);");
					sb.AppendLine($"{ts}{ts}YM[fc]&= ~(BIT3 | YM_KOP_HALFSTAR);");
					sb.AppendLine($"{ts}{ts} X[fc]&= ~(BIT1 | BIT2 |BIT3 | X_GELIJK_HALFSTAR | X_VOOR_HALFSTAR | X_DEELC_HALFSTAR);");
                    sb.AppendLine($"{ts}}}");
					sb.AppendLine();
					
					foreach (var nl in c.InterSignaalGroep.Nalopen)
					{
						if (nl.Type == NaloopTypeEnum.EindeGroen ||
						    nl.Type == NaloopTypeEnum.CyclischVerlengGroen)
						{
							var t = nl.Type == NaloopTypeEnum.EindeGroen ? _tnleg : _tnlcv;
                            var dt = "NG";
                            if (nl.DetectieAfhankelijk)
                            {
                                dt = nl.Type == NaloopTypeEnum.EindeGroen ? $"{_tpf}{_tnlegd}{nl.FaseVan}{nl.FaseNaar}" : $"{_tpf}{_tnlcvd}{nl.FaseVan}{nl.FaseNaar}";
                            }
                            var vsTijdNl = "NG";
                            if (nl.MaximaleVoorstart.HasValue)
                            {
	                            vsTijdNl = $"T_max[{_tpf}{_treallr}{nl.FaseNaar}{nl.FaseVan}]";
                            }

                            var nlv = "NG";
                            if (nl.VasteNaloop)
                            {
	                            nlv = $"{_tpf}{t}{nl.FaseVan}{nl.FaseNaar}";
                            }
							sb.AppendLine($"{ts}naloopEG_CV_halfstar(TRUE, {_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {vsTijdNl}, {dt}, {nlv});");
						}

                        if(nl.Type == NaloopTypeEnum.StartGroen)
                        {
                            if (nl.DetectieAfhankelijk && nl.Detectoren.Any())
                            {
                                sb.AppendLine($"{ts}naloopSG_halfstar({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_dpf}{nl.Detectoren[0].Detector}, {_hpf}{_hnla}{nl.Detectoren[0].Detector}, {_tpf}{_tnlsgd}{nl.FaseVan}{nl.FaseNaar});");
                            }
                            else
                            {
                                sb.AppendLine($"{ts}naloopSG_halfstar({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, NG, NG, {_tpf}{_tnlsg}{nl.FaseVan}{nl.FaseNaar});");
                            }
                        }
					}
                    var tinl2 = c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.RealFunc ? _trealil : _tinl;
                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.StartGroen && x.MaximaleVoorstart.HasValue))
                    {
                        var sgv = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseVan);
                        var sgn = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseNaar);
						if (sgv is { Type: FaseTypeEnum.Voetganger } && sgn is { Type: FaseTypeEnum.Voetganger })
						{ 
							sb.AppendLine($"{ts}inloopSG_halfstar({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {_tpf}{tinl2}{nl:vannaar});");
						}
					}

                    return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCAlternatief:
                    var gelijkstarttuples2 = CCOLCodeHelper.GetFasenWithGelijkStarts(c);

                    if (c.HasPTorHD())
                    {
	                    sb.AppendLine($"{ts}PrioHalfstarPARCorrectieAlternatievenZonderPrio();");   
	                    sb.AppendLine();   
                    }

                    foreach (var fc in c.ModuleMolen.FasenModuleData)
                    {
                        Tuple<string, List<string>> hasgs = null;
                        foreach (var gs in gelijkstarttuples2)
                        {
                            if (gs.Item1 == fc.FaseCyclus && gs.Item2.Count > 1)
                            {
                                hasgs = gs;
                                break;
                            }
                        }
                        if (hasgs != null)
                        {
                            sb.Append(
                                $"{ts}alternatief_halfstar({_fcpf}{fc.FaseCyclus}, PRM[{_prmpf}{_prmaltphst}");
                            foreach (var ofc in hasgs.Item2)
                            {
                                sb.Append(ofc);
                            }
                            sb.Append($"], SCH[{_schpf}{_schaltghst}");
                            foreach (var ofc in hasgs.Item2)
                            {
                                sb.Append(ofc);
                            }
                            sb.AppendLine("]);");
                        }
                        else
                        {
                            sb.AppendLine(
                                $"{ts}alternatief_halfstar({_fcpf}{fc.FaseCyclus}, PRM[{_prmpf}{_prmaltphst}{fc.FaseCyclus}], SCH[{_schpf}{_schaltghst}{fc.FaseCyclus}]);");
                        }
                    }
                    foreach (var nl in c.InterSignaalGroep.Nalopen)
                    {
                        if (nl.Type == NaloopTypeEnum.EindeGroen ||
                            nl.Type == NaloopTypeEnum.CyclischVerlengGroen)
                        {
                            var t = nl.Type == NaloopTypeEnum.EindeGroen ? $"{_tpf}{_tnleg}{nl.FaseVan}{nl.FaseNaar}" : $"{_tpf}{_tnlcv}{nl.FaseVan}{nl.FaseNaar}";
                            if (nl.DetectieAfhankelijk)
                            {
                                t = nl.Type == NaloopTypeEnum.EindeGroen ? $"{_tpf}{_tnlegd}{nl.FaseVan}{nl.FaseNaar}" : $"{_tpf}{_tnlcvd}{nl.FaseVan}{nl.FaseNaar}";
                            }
                            sb.AppendLine($"{ts}altcor_kop_halfstar({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {t});");
                        }
                        if (nl.Type == NaloopTypeEnum.StartGroen)
                        {
                            if (nl.DetectieAfhankelijk && nl.Detectoren.Any())
                            {
                                sb.AppendLine($"{ts}altcor_naloopSG_halfstar({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, IH[{_hpf}{_hnla}{nl.Detectoren[0].Detector}], {_tpf}{_tnlsgd}{nl.FaseVan}{nl.FaseNaar}, TRUE);");
                            }
                            else
                            {
                                sb.AppendLine($"{ts}altcor_naloopSG_halfstar({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, TRUE, {_tpf}{_tnlsg}{nl.FaseVan}{nl.FaseNaar}, TRUE);");
                            }
                        }
                    }
                    
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
					sb.AppendLine($"{ts}{ts}RR[fc] &= ~RR_ALTCOR_HALFSTAR;");
					sb.AppendLine();
					
					if (c.HalfstarData.Hoofdrichtingen.Any())
					{
						sb.AppendLine($"{ts}/* hoofdrichtingen alleen tijdens periode alternatieve realisaties en koppeling uit */");
						sb.AppendLine($"{ts}if (!H[{_hpf}{_hperarh}] && H[{_hpf}kpact])");
						sb.AppendLine($"{ts}{{");
						foreach (var hfc in c.HalfstarData.Hoofdrichtingen)
						{
							sb.AppendLine($"{ts}{ts}if (!tussen_txa_en_txb({_fcpf}{hfc.FaseCyclus}) && !tussen_txb_en_txd({_fcpf}{hfc.FaseCyclus})) PAR[{_fcpf}{hfc.FaseCyclus}] &= ~BIT0;");
						}
						sb.AppendLine($"{ts}}}");
						sb.AppendLine();
					}

                    sb.AppendLine($"{ts}Alternatief_halfstar_Add();");
					sb.AppendLine();

                    sb.AppendLine($"{ts}/* retour rood wanneer richting AR heeft maar geen PAR meer */");
					sb.AppendLine($"{ts}/* -------------------------------------------------------- */");
					sb.AppendLine($"{ts}reset_altreal_halfstar();");
                    sb.AppendLine();

					sb.AppendLine($"{ts}");
                    sb.AppendLine($"{ts}signaalplan_alternatief();");
					
					return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCRealisatieAfhandeling:
					sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
					sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}PP[fc] &= ~BIT4;");
					sb.AppendLine($"{ts}{ts}YM[fc] &= ~BIT5;");
					sb.AppendLine($"{ts}{ts}RS[fc] &= ~RS_HALFSTAR;");
					sb.AppendLine($"{ts}{ts}PP[fc] |= GL[fc] ? BIT4 : 0; /* i.v.m. overslag door conflicten */");
					sb.AppendLine($"{ts}}}");
					sb.AppendLine($"{ts}");

                    #region Dubbele realisaties

                    if (c.HalfstarData.SignaalPlannen.SelectMany(x => x.Fasen).Any(x => x.B2.HasValue && x.B2 != 0 && x.D2.HasValue && x.D2 != null))
                    {
                        sb.AppendLine($"{ts}/* Tweede realisaties (middels parameters) */");
                        sb.AppendLine();
                        foreach (var pl in c.HalfstarData.SignaalPlannen)
                        {
                            sb.AppendLine($"{ts}/* {pl.Naam} */");
                            foreach (var fcpl in pl.Fasen)
                            {
                                if (fcpl.B2.HasValue && fcpl.D2.HasValue)
                                {
                                    sb.AppendLine($"{ts}set_2real({_fcpf}{fcpl.FaseCyclus}, {_prmpf}{_prmtx}A1{pl.Naam}_{fcpl.FaseCyclus}, {_prmpf}{_prmtx}A2{pl.Naam}_{fcpl.FaseCyclus}, {pl.Naam}, ({c.GetBoolV()})(IH[{_hpf}{_hplact}]));");
                                }
                            }
                            sb.AppendLine();
                        }
                    }

                    #endregion // Dubbele realisaties

                    if (c.HalfstarData.FaseCyclusInstellingen.Any(x => x.PrivilegePeriodeOpzetten))
                    {
                        sb.AppendLine($"{ts}/* PP opzetten */");
                        foreach (var sg in c.HalfstarData.FaseCyclusInstellingen.Where(x => x.PrivilegePeriodeOpzetten))
                        {
                            sb.AppendLine($"{ts}set_pp_halfstar({_fcpf}{sg.FaseCyclus}, IH[{_hpf}{_hkpact}], BIT4);");
                        }
                        sb.AppendLine();
                    }

                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}/* Voorstartgroen tijdens voorstart t.o.v. sg-plan, alleen als gekoppeld wordt geregeld */");
#warning TODO: functie vs_ple() moet worden nagelopen en mogelijk herzien
                    sb.AppendLine($"{ts}{ts}vs_ple(fc, IH[{_hpf}{_hkpact}]);");
					sb.AppendLine($"");
					sb.AppendLine($"{ts}{ts}/* opzetten van YS en YW tijdens halfstar bedrijf */");
					sb.AppendLine($"{ts}{ts}/* resetten */");
					sb.AppendLine($"{ts}{ts}RW[fc] &= ~RW_WG_HALFSTAR;");
					sb.AppendLine($"{ts}{ts}YW[fc] &= ~YW_PL_HALFSTAR;");
					sb.AppendLine($"{ts}{ts}/* vasthouden wachtgroen functie bij PL-bedrijf */");
					sb.AppendLine($"{ts}{ts}RW[fc] |= YW_PL[fc] && tussen_txb_en_txc(fc) && (TXC[PL][fc] > 0) ? RW_WG_HALFSTAR : 0; /* TXC-afhandeling */");
					sb.AppendLine($"{ts}{ts}YW[fc] |= YW_PL[fc] && tussen_txb_en_txc(fc) && (TXC[PL][fc] > 0) ? YW_PL_HALFSTAR : 0; /* TXC-afhandeling */");
					sb.AppendLine($"{ts}}}");
					sb.AppendLine($"");
					sb.AppendLine($"{ts}/* primaire realisaties signaalplansturing */");
					sb.AppendLine($"{ts}/* --------------------------------------- */");
					if (c.PrioData.PrioIngreepType == PrioIngreepTypeEnum.Geen ||
					    !c.HasPTorHD())
					{
						sb.AppendLine($"{ts}signaalplan_primair();");
					}
					else
					{
						sb.AppendLine($"{ts}if (SCH[{_schpf}{_schovpriople}])");
						sb.AppendLine($"{ts}{{");
						sb.AppendLine($"{ts}{ts}signaalplan_primair_PRIO_ple();");
						sb.AppendLine($"{ts}}}");
						sb.AppendLine($"{ts}else");
						sb.AppendLine($"{ts}{{");
						sb.AppendLine($"{ts}{ts}signaalplan_primair();");
						sb.AppendLine($"{ts}}}");
					}
					sb.AppendLine();
					sb.AppendLine($"{ts}/* afsluiten primaire aanvraaggebieden */");
					sb.AppendLine($"{ts}/* ----------------------------------- */");
					if (c.PrioData.PrioIngreepType == PrioIngreepTypeEnum.Geen ||
						!c.HasPTorHD())
					{
						sb.AppendLine($"{ts}set_pg_primair_fc();");
					}
					else
					{
						sb.AppendLine($"{ts}if (SCH[{_schpf}{_schovpriople}])");
						sb.AppendLine($"{ts}{{");
						sb.AppendLine($"{ts}{ts}set_pg_primair_fc_PRIO_ple();");
						sb.AppendLine($"{ts}}}");
						sb.AppendLine($"{ts}else");
						sb.AppendLine($"{ts}{{");
						sb.AppendLine($"{ts}{ts}set_pg_primair_fc();");
						sb.AppendLine($"{ts}}}");
					}
					sb.AppendLine();
					sb.AppendLine($"{ts}/* reset PG bij planwisseling */");
					sb.AppendLine($"{ts}/* -------------------------- */");
					sb.AppendLine($"{ts}/* anders kan PG op blijven staan, waardoor richting eenmaal wordt overgeslagen en de regeling kan vastlopen */");
					sb.AppendLine($"{ts}if (SH[{_hpf}{_hmlact}] || SH[{_hpf}{_hplact}] || SPL)");
					sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}for (fc = 0; fc < FCMAX; ++fc)");
					sb.AppendLine($"{ts}{ts}{{");
					sb.AppendLine($"{ts}{ts}{ts}PG[fc] = FALSE;");
					sb.AppendLine($"{ts}{ts}}}");
					sb.AppendLine($"{ts}}}");

					if (c.HalfstarData.Hoofdrichtingen.Count > 0)
					{
						sb.AppendLine();
                        sb.AppendLine($"{ts}/* vooruitrealiseren tijdens PL");
						sb.AppendLine($"{ts} *");
						sb.AppendLine($"{ts} * Wanneer hoofdrichtingen in PL bedrijf vooruit realiseren bestaat de modelijkheid");
						sb.AppendLine($"{ts} * - dat ze kort voor het TxB moment uit groen gaan (en niet tijdig meer groen kunnen worden)");
						sb.AppendLine($"{ts} * - dat ze sowieso niet meer komen tussen TxB en TxD omdat ze al primair gerealiseerd zijn.");
						sb.AppendLine($"{ts} * Beide zijn conflicterend met het idee van een groene golf.");
						sb.AppendLine($"{ts} * Daarom hoofdrichtingen die kort voor TxB al groen wijn, sowieso vasthouden tot TxB en");
						sb.AppendLine($"{ts} * hoofdrichtingen altijd mogelijk maken primair te komen op TxB (maw PG afzetten).");
						sb.AppendLine($"{ts} */");
                        sb.AppendLine();
						sb.AppendLine($"{ts}/* vasthouden groen hoofdrichtingen gedurende periode voorafgaand aan TxB moment (indien eerder groen gestuurd) */");
						foreach (var hr in c.HalfstarData.Hoofdrichtingen)
						{
							sb.AppendLine($"{ts}RW[{_fcpf}{hr.FaseCyclus}] |= (G[{_fcpf}{hr.FaseCyclus}] && TOTXB_PL[{_fcpf}{hr.FaseCyclus}] && (TOTXB_PL[{_fcpf}{hr.FaseCyclus}] < 100)) ? RW_WG_HALFSTAR : 0; /* ivm vooruitrealiseren */");
                            sb.AppendLine($"{ts}YW[{_fcpf}{hr.FaseCyclus}] |= (G[{_fcpf}{hr.FaseCyclus}] && TOTXB_PL[{_fcpf}{hr.FaseCyclus}] && (TOTXB_PL[{_fcpf}{hr.FaseCyclus}] < 100)) ? YW_PL_HALFSTAR : 0; /* ivm vooruitrealiseren */");
                        }
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* intrekken PG[] (primair en versneld primair) gedurende periode voorafgaand aan TxB moment (indien al eerder gerealiseerd) */");
                        foreach (var hr in c.HalfstarData.Hoofdrichtingen)
                        {
                            sb.AppendLine($"{ts}if (R[{_fcpf}{hr.FaseCyclus}] && TOTXB_PL[{_fcpf}{hr.FaseCyclus}] && (TOTXB_PL[{_fcpf}{hr.FaseCyclus}] < TFG_max[{_fcpf}{hr.FaseCyclus}])) PG[{_fcpf}{hr.FaseCyclus}] &= ~(PRIMAIR_VERSNELD);");
						}
                    }

                    return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCPostApplication:
					sb.AppendLine($"{ts}/* Knipperpuls generator */");
					sb.AppendLine($"{ts}/* --------------------- */");
					sb.AppendLine($"{ts}RT[{_tpf}{_tleven}] = !T[{_tpf}{_tleven}]; /* timer herstarten */");
					sb.AppendLine($"{ts}if (ST[{_tpf}{_tleven}])  IH[{_hpf}{_hleven}] = !IH[{_hpf}{_hleven}];   /* hulpwaarde aan/uit zetten */");
					sb.AppendLine();
					foreach (var kp in c.HalfstarData.GekoppeldeKruisingen)
					{
						sb.AppendLine($"{ts}/* Levensignaal van {kp.KruisingNaam} */");

                        var ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}leven", KoppelSignaalRichtingEnum.In);
                        sb.AppendLine($"{ts}RT[{_tpf}{_tleven}{kp.KruisingNaam}] = SH[{_hpf}{kp.PTPKruising}{_hiks}{ipl:00}];");
						sb.AppendLine($"{ts}MM[{_mpf}{_mleven}{kp.KruisingNaam}] = T[{_tpf}{_tleven}{kp.KruisingNaam}];");
					}

					sb.AppendLine($"{ts}/* herstart fasebewakingstimers bij wisseling tussen ML/PL en SPL */");
					sb.AppendLine($"{ts}/* -------------------------------------------------------------- */");
					sb.AppendLine($"{ts}RTFB &= ~RTFB_PLVA_HALFSTAR;");
					sb.AppendLine($"{ts}RTFB |= (SH[{_hpf}{_hplact}] || SH[{_hpf}{_hmlact}] || (SPL && IH[{_hpf}{_hplact}])) ? RTFB_PLVA_HALFSTAR : 0;");
					return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCPreSystemApplication:
                    if (c.HalfstarData.PlantijdenInParameters)
                    {

                        sb.AppendLine($"{ts}/* kopieer signaalplantijden - vanuit parameter lijst */");
                        sb.AppendLine($"{ts}/* -------------------------------------------------- */");
                        sb.AppendLine($"{ts}if (SCH[{_schpf}{_schinstprm}])");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}short error = FALSE;");
                        for (var pl = 0; pl < c.HalfstarData.SignaalPlannen.Count; pl++)
                        {
                            var ppl = c.HalfstarData.SignaalPlannen[pl];
                            if (!c.HalfstarData.SignaalPlannen[pl].Fasen.Any()) continue;
                            sb.AppendLine($"{ts}{ts}if (!error)");
                            sb.AppendLine($"{ts}{ts}{{");
                            sb.AppendLine($"{ts}{ts}{ts}error = CheckSignalplanPrms({ppl.Naam}, TX_max[{pl}], {_prmpf}{_prmtx}A1{ppl.Naam}_{ppl.Fasen.First().FaseCyclus});");
                            sb.AppendLine($"{ts}{ts}}}");
                        }
                        sb.AppendLine($"{ts}{ts}if (!error)");
                        sb.AppendLine($"{ts}{ts}{{");
                        for (var pl = 0; pl < c.HalfstarData.SignaalPlannen.Count; pl++)
                        {
                            var ppl = c.HalfstarData.SignaalPlannen[pl];
                            if (!c.HalfstarData.SignaalPlannen[pl].Fasen.Any()) continue;
                            sb.AppendLine($"{ts}{ts}{ts}SignalplanPrmsToTx({ppl.Naam}, {_prmpf}{_prmtx}A1{ppl.Naam}_{ppl.Fasen.First().FaseCyclus});");
                        }
                        sb.AppendLine($"{ts}{ts}}}");

                        sb.AppendLine($"{ts}{ts}if (!error)");
                        sb.AppendLine($"{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}copy_signalplan(PL);");
                        if(c.Data.CCOLVersie >= CCOLVersieEnum.CCOL95)
                        {
                            sb.AppendLine($"{ts}{ts}{ts}create_trig();        /* creëer nieuwe TIG-tabel na wijzigingen geel-, ontruimingstijden */");
                            sb.AppendLine($"{ts}{ts}{ts}correction_trig();    /* correcties TIG-tabel a.g.v. koppelingen e.d. */");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}{ts}{ts}create_tig();        /* creëer nieuwe TIG-tabel na wijzigingen geel-, ontruimingstijden */");
                            sb.AppendLine($"{ts}{ts}{ts}correction_tig();    /* correcties TIG-tabel a.g.v. koppelingen e.d. */");
                        }
                        sb.AppendLine($"{ts}{ts}{ts}check_signalplans(); /* check signalplans */");
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine($"{ts}{ts}SCH[{_schpf}{_schinstprm}] = 0;");
                        sb.AppendLine($"{ts}{ts}CIF_PARM1WIJZAP = (s_int16) (&SCH[{_schpf}{_schinstprm}] - CIF_PARM1);");
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine();
                    }
                    sb.AppendLine($"{ts}/* kopieer signaalplantijden - na wijziging */");
					sb.AppendLine($"{ts}/* ---------------------------------------- */");
                    sb.AppendLine($"{ts}#if (CCOL_V >= 95)");
					sb.AppendLine($"{ts}{ts}if (SCH[{_schpf}{_schinst}] || COPY_2_TRIG)");
                    sb.AppendLine($"{ts}#else");
					sb.AppendLine($"{ts}{ts}if (SCH[{_schpf}{_schinst}] || COPY_2_TIG)");
					sb.AppendLine($"{ts}#endif");
                    sb.AppendLine($"{ts}{{");
					sb.AppendLine($"{ts}{ts}copy_signalplan(PL);");
                    if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL95)
                    {
                        sb.AppendLine($"{ts}{ts}create_trig();        /* creëer nieuwe TIG-tabel na wijzigingen geel-, ontruimingstijden */");
                        sb.AppendLine($"{ts}{ts}correction_trig();    /* correcties TIG-tabel a.g.v. koppelingen e.d. */");
                    }
                    else
                    {
                        sb.AppendLine($"{ts}{ts}create_tig();        /* creëer nieuwe TIG-tabel na wijzigingen geel-, ontruimingstijden */");
                        sb.AppendLine($"{ts}{ts}correction_tig();    /* correcties TIG-tabel a.g.v. koppelingen e.d. */");
                    }
                    sb.AppendLine($"{ts}{ts}check_signalplans(); /* check signalplans */");
					sb.AppendLine($"{ts}{ts}SCH[{_schpf}{_schinst}] = 0;");
					sb.AppendLine($"{ts}{ts}#if (CCOL_V >= 95)");
					sb.AppendLine($"{ts}{ts}{ts}COPY_2_TRIG = FALSE;");
					sb.AppendLine($"{ts}{ts}#else");
					sb.AppendLine($"{ts}{ts}{ts}COPY_2_TIG = FALSE;");
					sb.AppendLine($"{ts}{ts}#endif");
                    sb.AppendLine($"{ts}{ts}CIF_PARM1WIJZAP = (s_int16) (&SCH[{_schpf}{_schinst}] - CIF_PARM1);");
					sb.AppendLine($"{ts}}}");
					sb.AppendLine($"{ts}RTX = FALSE;");
					sb.AppendLine($"{ts}");
					sb.AppendLine($"{ts}if (IH[{_hpf}{_hplact}]) /* Code alleen bij PL-bedrijf */");
					sb.AppendLine($"{ts}{{");
					if (master != null && c.HalfstarData.Type != HalfstarTypeEnum.Master)
					{
						#warning TODO need code for running single appl.
                        var ipl = CCOLElementCollector.GetKoppelSignaalCount(master.PTPKruising, $"{master.KruisingNaam}kpuls", KoppelSignaalRichtingEnum.In);
						sb.AppendLine($"{ts}{ts}RT[{_tpf}{_toffset}] = SH[{_hpf}{master.PTPKruising}{_hiks}{ipl:00}]; /* offset starten op start koppelpuls */");
						sb.AppendLine($"{ts}{ts}SYN_TXS = ET[{_tpf}offset]; /* synchronisatie einde offset timer */");
						sb.AppendLine($"{ts}{ts}synchronization_timer(SAPPLPROG, T_max[{_tpf}xmarge]);");
					}
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
					if (master != null && c.HalfstarData.Type != HalfstarTypeEnum.Master)
					{
						sb.AppendLine($"{ts}{ts}else");
						sb.AppendLine($"{ts}{ts}{{");
						sb.AppendLine($"{ts}{ts}{ts}/* gekoppelde signaalplansturing */");
						sb.AppendLine($"{ts}{ts}{ts}/* ----------------------------- */");
						sb.AppendLine($"{ts}{ts}{ts}/* als TXS_SYNC, en daarmee ook TXS_OKE, de regeling zacht of hard synchroniseren afhankelijk van ");
						sb.AppendLine($"{ts}{ts}{ts}{ts} positie ten opzichte van de master */");
						sb.AppendLine($"{ts}{ts}{ts}if (MM[{_mpf}{_mleven}{master.KruisingNaam}] && TXS_OKE && TXS_SYNC && (TXS_delta > 0) && (PL==APL))");
						sb.AppendLine($"{ts}{ts}{ts}{{");
						sb.AppendLine($"{ts}{ts}{ts}{ts}/* regeling loopt iets vooruit (2 x marge) */");
						sb.AppendLine($"{ts}{ts}{ts}{ts}if (TXS_delta > 0 && (TXS_delta <= (2 * T_max[{_tpf}{_txmarge}])))");
						sb.AppendLine($"{ts}{ts}{ts}{ts}{{");
						sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}HTX = TRUE;");
						sb.AppendLine($"{ts}{ts}{ts}{ts}}}");
						sb.AppendLine($"{ts}{ts}{ts}{ts}else");
						sb.AppendLine($"{ts}{ts}{ts}{ts}/* regeling loop iets achter (2 x marge) */");
						sb.AppendLine($"{ts}{ts}{ts}{ts}if (TXS_delta > 0 && (TXS_delta >= (TX_max[PL] - (2 * T_max[{_tpf}{_txmarge}]))))");
						sb.AppendLine($"{ts}{ts}{ts}{ts}{{");
						sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}FTX = versnel_tx(FALSE);");
						sb.AppendLine($"{ts}{ts}{ts}{ts}}}");
						sb.AppendLine($"{ts}{ts}{ts}{ts}/* in alle andere gevallen is de afwijking te groot en moet, om lange synchronisatietijden te voorkomen,");
						sb.AppendLine($"{ts}{ts}{ts}{ts}{ts} de regeling hard worden gesynschroniseerd */");
						sb.AppendLine($"{ts}{ts}{ts}{ts}else");
						sb.AppendLine($"{ts}{ts}{ts}{ts}{{");
						sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}if (!H[{_hpf}{_homschtegenh}]) /* koppelingen en pelotons mogen niet worden afgekapt     */");
						sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}{ts}TX_timer = TXS_timer; /* TX_timer gelijk maken aan de cyclustijd van de master */");
						sb.AppendLine($"{ts}{ts}{ts}{ts}}}");
						sb.AppendLine($"{ts}{ts}{ts}}}");
						sb.AppendLine($"{ts}{ts}{ts}else");
						sb.AppendLine($"{ts}{ts}{ts}{{");
						sb.AppendLine($"{ts}{ts}{ts}{ts}/* do nothing */");
						sb.AppendLine($"{ts}{ts}{ts}}}");
						sb.AppendLine($"{ts}{ts}}}");
					}
					sb.AppendLine($"{ts}}} /* Einde code PL-bedrijf */");
					
					if (master != null && c.HalfstarData.Type != HalfstarTypeEnum.Master)
					{
						sb.AppendLine($"{ts}/* tijdens VA bedrijf hard synchroniseren */");
						sb.AppendLine($"{ts}else");
						sb.AppendLine($"{ts}{{");
                        var ipl = CCOLElementCollector.GetKoppelSignaalCount(master.PTPKruising, $"{master.KruisingNaam}kpuls", KoppelSignaalRichtingEnum.In);
						sb.AppendLine($"{ts}{ts}RTX = SH[{_hpf}{master.PTPKruising}{_hiks}{ipl:00}];");
                        sb.AppendLine($"{ts}}}");
					}

					sb.AppendLine();

					foreach (var kp in c.HalfstarData.GekoppeldeKruisingen)
					{
						int ipl;
						switch (kp.Type)
						{
							// If the coupled intersection is the master of this one
							case HalfstarGekoppeldTypeEnum.Master:
								// Receive: leven, koppelpuls, pervar, perarh, actief plan (op FALSE zetten indien geen leven)
								sb.AppendLine($"{ts}/* Koppelsignalen (PTP) van {kp.KruisingNaam} */");
                                
                                ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}leven", KoppelSignaalRichtingEnum.In);
                                sb.AppendLine($"{ts}GUS[{_uspf}in{kp.KruisingNaam}{_usleven}] = IH[{_hpf}{kp.PTPKruising}{_hiks}{ipl:00}];");
								sb.AppendLine($"{ts}if (MM[{_mpf}{_mleven}{kp.KruisingNaam}])");
								sb.AppendLine($"{ts}{{");
                                ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}kpuls", KoppelSignaalRichtingEnum.In);
								sb.AppendLine($"{ts}{ts}GUS[{_uspf}in{kp.KruisingNaam}{_uskpuls}] = IH[{_hpf}{kp.PTPKruising}{_hiks}{ipl:00}];");
                                ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}pervar", KoppelSignaalRichtingEnum.In);
								sb.AppendLine($"{ts}{ts}GUS[{_uspf}in{kp.KruisingNaam}{_uspervar}] = IH[{_hpf}{kp.PTPKruising}{_hiks}{ipl:00}];");
                                ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}perarh", KoppelSignaalRichtingEnum.In);
								sb.AppendLine($"{ts}{ts}GUS[{_uspf}in{kp.KruisingNaam}{_usperarh}] = IH[{_hpf}{kp.PTPKruising}{_hiks}{ipl:00}];");
								foreach (var pl in c.HalfstarData.SignaalPlannen)
								{
                                    ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}pl{pl.Naam}", KoppelSignaalRichtingEnum.In);
									sb.AppendLine($"{ts}{ts}GUS[{_uspf}in{kp.KruisingNaam}{pl.Naam}] = IH[{_hpf}{kp.PTPKruising}{_hiks}{ipl:00}];");
								}
								sb.AppendLine($"{ts}}}");
								sb.AppendLine($"{ts}else");
								sb.AppendLine($"{ts}{{");
								sb.AppendLine($"{ts}{ts}GUS[{_uspf}in{kp.KruisingNaam}{_uskpuls}] = FALSE;");
								sb.AppendLine($"{ts}{ts}GUS[{_uspf}in{kp.KruisingNaam}{_uspervar}] = FALSE;");
								sb.AppendLine($"{ts}{ts}GUS[{_uspf}in{kp.KruisingNaam}{_usperarh}] = FALSE;");
								foreach (var pl in c.HalfstarData.SignaalPlannen)
								{
									sb.AppendLine($"{ts}{ts}GUS[{_uspf}in{kp.KruisingNaam}{pl.Naam}] = FALSE;");
								}
								sb.AppendLine($"{ts}}}");
								sb.AppendLine();
                                // Send: leven, synch, txs
								sb.AppendLine($"{ts}/* Koppelsignalen (PTP) naar {kp.KruisingNaam} */");
                                ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}leven", KoppelSignaalRichtingEnum.Uit);
								sb.AppendLine($"{ts}GUS[{_uspf}uit{kp.KruisingNaam}{_usleven}] = IH[{_hpf}{kp.PTPKruising}{_huks}{ipl++:00}] = IH[{_hpf}{_hleven}];");
                                ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}syncok", KoppelSignaalRichtingEnum.Uit);
								sb.AppendLine($"{ts}GUS[{_uspf}uit{kp.KruisingNaam}{_ussyncok}] = IH[{_hpf}{kp.PTPKruising}{_huks}{ipl++:00}] = REG && (MM[{_mpf}{_mleven}{kp.KruisingNaam}] && (TXS_delta == 0) && TXS_OKE);");
                                ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}txsok", KoppelSignaalRichtingEnum.Uit);
								sb.AppendLine($"{ts}GUS[{_uspf}uit{kp.KruisingNaam}{_ustxsok}] = IH[{_hpf}{kp.PTPKruising}{_huks}{ipl:00}] = REG && MM[{_mpf}{_mleven}{kp.KruisingNaam}] && TXS_OKE;");
								sb.AppendLine();
								break;
							// Otherwise, the coupled intersection is a slave of this one
							case HalfstarGekoppeldTypeEnum.Slave:
								sb.AppendLine($"{ts}/* Koppelsignalen (PTP) naar {kp.KruisingNaam} */");
								var mts2 = ts;
								// If fallback: send master if master alive, otherwise determine by own judgement
								if (c.HalfstarData.Type == HalfstarTypeEnum.FallbackMaster)
								{
									mts2 = ts + ts;
									sb.AppendLine($"{ts}if (MM[{_mpf}{_mleven}{master.KruisingNaam}])");
									sb.AppendLine($"{ts}{{");
                                    ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}leven", KoppelSignaalRichtingEnum.Uit);
                                    var ipli = CCOLElementCollector.GetKoppelSignaalCount(master.PTPKruising, $"{master.KruisingNaam}leven", KoppelSignaalRichtingEnum.In);
									sb.AppendLine($"{mts2}GUS[{_uspf}uit{kp.KruisingNaam}{_usleven}] = IH[{_hpf}{kp.PTPKruising}{_huks}{ipl:00}] = IH[{_hpf}{master.PTPKruising}{_hiks}{ipli:00}]; /* uitgaand levensignaal naar alle aangesloten kp's */");
                                    ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}kpuls", KoppelSignaalRichtingEnum.Uit);
                                    ipli = CCOLElementCollector.GetKoppelSignaalCount(master.PTPKruising, $"{master.KruisingNaam}kpuls", KoppelSignaalRichtingEnum.In);
									sb.AppendLine($"{mts2}GUS[{_uspf}uit{kp.KruisingNaam}{_uskpuls}] = IH[{_hpf}{kp.PTPKruising}{_huks}{ipl:00}] = IH[{_hpf}{master.PTPKruising}{_hiks}{ipli:00}]; /* koppelpuls master doorsturen */");
                                    ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}pervar", KoppelSignaalRichtingEnum.Uit);
                                    ipli = CCOLElementCollector.GetKoppelSignaalCount(master.PTPKruising, $"{master.KruisingNaam}pervar", KoppelSignaalRichtingEnum.In);
									sb.AppendLine($"{mts2}GUS[{_uspf}uit{kp.KruisingNaam}{_uspervar}] = IH[{_hpf}{kp.PTPKruising}{_huks}{ipl:00}] = IH[{_hpf}{master.PTPKruising}{_hiks}{ipli:00}]; /* periode var master doorsturen */");
                                    ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}perarh", KoppelSignaalRichtingEnum.Uit);
                                    ipli = CCOLElementCollector.GetKoppelSignaalCount(master.PTPKruising, $"{master.KruisingNaam}perarh", KoppelSignaalRichtingEnum.In);
									sb.AppendLine($"{mts2}GUS[{_uspf}uit{kp.KruisingNaam}{_usperarh}] = IH[{_hpf}{kp.PTPKruising}{_huks}{ipl:00}] = IH[{_hpf}{master.PTPKruising}{_hiks}{ipli:00}]; /* periode arh master doorsturen */");
									foreach (var pl in c.HalfstarData.SignaalPlannen)
									{
                                        ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}pl{pl.Naam}", KoppelSignaalRichtingEnum.Uit);
                                        ipli = CCOLElementCollector.GetKoppelSignaalCount(master.PTPKruising, $"{master.KruisingNaam}pl{pl.Naam}", KoppelSignaalRichtingEnum.In);
										sb.AppendLine($"{mts2}GUS[{_uspf}uit{kp.KruisingNaam}{pl.Naam}] = IH[{_hpf}{kp.PTPKruising}{_huks}{ipl:00}] = IH[{_hpf}{master.PTPKruising}{_hiks}{ipli:00}];");
									}
									sb.AppendLine($"{ts}}}");
									sb.AppendLine($"{ts}else");
									sb.AppendLine($"{ts}{{");
								}

								// For master and fallback, send data to coupled slave: leven, koppelpuls, pervar, perarh, actief plan
                                ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}leven", KoppelSignaalRichtingEnum.Uit);
								sb.AppendLine($"{mts2}GUS[{_uspf}uit{kp.KruisingNaam}{_usleven}] = IH[{_hpf}{kp.PTPKruising}{_huks}{ipl:00}] = IH[{_hpf}{_hleven}]; /* uitgaand levensignaal naar alle aangesloten kp's */");
                                ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}kpuls", KoppelSignaalRichtingEnum.Uit);
								sb.AppendLine($"{mts2}GUS[{_uspf}uit{kp.KruisingNaam}{_uskpuls}] = IH[{_hpf}{kp.PTPKruising}{_huks}{ipl:00}] = ((TX_timer <= 1)); /* koppelpuls master */");
                                ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}pervar", KoppelSignaalRichtingEnum.Uit);
								sb.AppendLine($"{mts2}GUS[{_uspf}uit{kp.KruisingNaam}{_uspervar}] = IH[{_hpf}{kp.PTPKruising}{_huks}{ipl:00}] = (IH[{_hpf}{_hpervar}] || SCH[{_schpf}{_schvarstreng}]); /* periode var master */");
                                ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}perarh", KoppelSignaalRichtingEnum.Uit);
								sb.AppendLine($"{mts2}GUS[{_uspf}uit{kp.KruisingNaam}{_usperarh}] = IH[{_hpf}{kp.PTPKruising}{_huks}{ipl:00}] = (IH[{_hpf}{_hperarh}]); /* periode arh master */");
								foreach (var pl in c.HalfstarData.SignaalPlannen)
								{
                                    ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}pl{pl.Naam}", KoppelSignaalRichtingEnum.Uit);
									sb.AppendLine($"{mts2}GUS[{_uspf}uit{kp.KruisingNaam}{pl.Naam}] = IH[{_hpf}{kp.PTPKruising}{_huks}{ipl:00}] = ((APL == {pl.Naam}));");
								}
								if (c.HalfstarData.Type == HalfstarTypeEnum.FallbackMaster)
								{
									sb.AppendLine($"{ts}}}");
								}
								sb.AppendLine();
								// Receive from slave: leven, synch, txs
								sb.AppendLine($"{ts}/* Koppelsignalen via (PTP) van {kp.KruisingNaam} */");
								ipl = 1;
                                ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}leven", KoppelSignaalRichtingEnum.In);
								sb.AppendLine($"{ts}GUS[{_uspf}in{kp.KruisingNaam}{_usleven}] = IH[{_hpf}{kp.PTPKruising}{_hiks}{ipl:00}];");
                                ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}syncok", KoppelSignaalRichtingEnum.In);
								sb.AppendLine($"{ts}GUS[{_uspf}in{kp.KruisingNaam}{_ussyncok}] = IH[{_hpf}{kp.PTPKruising}{_hiks}{ipl:00}];");
                                ipl = CCOLElementCollector.GetKoppelSignaalCount(kp.PTPKruising, $"{kp.KruisingNaam}txsok", KoppelSignaalRichtingEnum.In);
								sb.AppendLine($"{ts}GUS[{_uspf}in{kp.KruisingNaam}{_ustxsok}] = IH[{_hpf}{kp.PTPKruising}{_hiks}{ipl:00}];");
								sb.AppendLine();
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
					
					// Outpus for all halfstar intersections
					sb.AppendLine($"{ts}GUS[{_uspf}{_usplact}] = IH[{_hpf}{_hplact}];");
					sb.AppendLine($"{ts}GUS[{_uspf}{_usmlact}] = IH[{_hpf}{_hmlact}];");
					sb.AppendLine($"{ts}GUS[{_uspf}{_uskpact}] = IH[{_hpf}{_hkpact}];");
                    if (c.Data.MultiModuleReeksen)
                    {
					    sb.AppendLine($"{ts}GUS[{_uspf}{_usmlpl}] = IH[{_hpf}{_hplact}] ? (s_int16)(PL+1): 0;");
                    }
                    else
                    {
                        sb.AppendLine($"{ts}GUS[{_uspf}{_usmlpl}] = IH[{_hpf}{_hplact}] ? (s_int16)(PL+1): (s_int16)(ML+1);");
                    }
                    foreach(var pl in c.HalfstarData.SignaalPlannen)
                    {
	                    sb.AppendLine($"{ts}GUS[{_uspf}{pl.ToString(_uspl)}] = IH[{_hpf}{_hplact}] && (PL == {pl.Naam});");
                    }
					sb.AppendLine($"{ts}GUS[{_uspf}{_ustxtimer}] = IH[{_hpf}{_hplact}] ? (s_int16)(TX_timer): 0;");
					sb.AppendLine($"{ts}GUS[{_uspf}{_usklok}] = MM[{_mpf}{_mklok}];");
					sb.AppendLine($"{ts}GUS[{_uspf}{_ushand}] = MM[{_mpf}{_mhand}];");
					if (c.HalfstarData.Type != HalfstarTypeEnum.Master)
					{
						sb.AppendLine($"{ts}GUS[{_uspf}{_usmaster}] = MM[{_mpf}{_mmaster}];");
                        sb.AppendLine($"{ts}GUS[{_uspf}{_usslave}] = MM[{_mpf}{_mslave}];");									
					}
					sb.AppendLine();

					return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCPostSystemApplication:
                    return sb.ToString();
				
				case CCOLCodeTypeEnum.HstCPostDumpApplication:
					return sb.ToString();

                case CCOLCodeTypeEnum.HstCPrioHalfstarSettings:
                    var enter = false;
                    if (c.HalfstarData.IsHalfstar && c.HasPT())
                    {
                        sb.AppendLine($"{ts}/* Bepalen tijd na TXD t.b.v. verlengen bij OV ingreep */");
                        if (c.PrioData.PrioIngreepType != PrioIngreepTypeEnum.Geen)
                        {
                            foreach (var ov in c.PrioData.PrioIngrepen)
                            {
                                sb.AppendLine($"{ts}iExtraGroenNaTXD[prioFC{CCOLCodeHelper.GetPriorityName(c, ov)}] = PRM[{_prmpf}{_prmnatxdhst}{CCOLCodeHelper.GetPriorityName(c, ov)}];");
                            }
                        }
                        else if (c.PrioData.PrioIngreepType == PrioIngreepTypeEnum.GeneriekePrioriteit)
                        {
                            foreach (var ov in c.PrioData.PrioIngrepen)
                            {
                                sb.AppendLine($"{ts}iExtraGroenNaTXD[prioFC{CCOLCodeHelper.GetPriorityName(c, ov)}] = PRM[{_prmpf}{_prmnatxdhst}{CCOLCodeHelper.GetPriorityName(c, ov)}];");
                            }
                        }
                        enter = true;
                    }
                    if (c.HalfstarData.Hoofdrichtingen.Any())
                    {
                        if (enter) sb.AppendLine();
                        sb.AppendLine($"{ts}/* PRIO opties hoofdrichtingen */");
                        sb.Append($"{ts}PrioHalfstarBepaalHoofdrichtingOpties(NG, ");
                        var first = true;
                        foreach (var hr in c.HalfstarData.Hoofdrichtingen)
                        {
                            if (!first)
                            {
                                sb.Append($"{ts}                                        ");
                            }
                            first = false;
                            sb.AppendLine($"(va_count){_fcpf}{hr.FaseCyclus}, (va_mulv)SCH[{_schpf}{_schtegenov}{hr.FaseCyclus}], (va_mulv)SCH[{_schpf}{_schafkwgov}{hr.FaseCyclus}], (va_mulv)SCH[{_schpf}{_schafkvgov}{hr.FaseCyclus}], TFG_max[{_fcpf}{hr.FaseCyclus}],");
                        }
                        sb.Append($"{ts}                                        ");
                        sb.AppendLine($"(va_count)END);");
                    }
                    return sb.ToString();

                #endregion // hst.c     

                #region prio.c

                case CCOLCodeTypeEnum.PrioCInitPrio:
                    sb.AppendLine($"{ts}PrioHalfstarInit();");
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCInstellingen:
                    sb.AppendLine($"{ts}PrioHalfstarSettings();");
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCPrioriteitsOpties:
                    if (c.PrioData.HDIngrepen.Any())
                    {
                        sb.AppendLine($"{ts}/* bijhouden of een hulpdienstingreep plaatsvindt */");
                        sb.AppendLine($"{ts}IH[{_hpf}{_hplhd}] = FALSE;");
                        sb.AppendLine($"{ts}for (fc = 0; fc < prioFCMAX; ++fc)");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}if (iPrioriteitsOpties[fc] & poNoodDienst)");
                        sb.AppendLine($"{ts}{ts}{ts}IH[{_hpf}{_hplhd}] |= TRUE;");
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine();
                    }
                    if (c.PrioData.PrioIngrepen.Any())
                    {
                        sb.AppendLine($"{ts}/* tijdens halfstar bedrijf alleen optie aanvraag voor OV richtingen */");
                        sb.AppendLine($"{ts}if (IH[{_hpf}{_hplact}] && SCH[{_schpf}{_schovpriople}])");
                        sb.AppendLine($"{ts}{{");
                        foreach (var prio in c.PrioData.PrioIngrepen)
                        {
                            sb.AppendLine($"{ts}{ts}iPrioriteitsOpties[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] |= PrioHalfstarBepaalPrioriteitsOpties({_prmpf}{_prmpriohst}{CCOLCodeHelper.GetPriorityName(c, prio)});");
                        }
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine();
                    }

                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Geen prioriteit indien voorwaarden tegenhouden omschakelen waar zijn */");
                    sb.AppendLine($"{ts}if (IH[{_hpf}{_homschtegenh}] && IH[{_hpf}{_hplact}] && SCH[{_schpf}{_schovpriople}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}for (fc = 0; fc < prioFCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}{ts}iXPrio[fc] |= BIT6;");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}else");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}for (fc = 0; fc < prioFCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}{ts}iXPrio[fc] &= ~BIT6;");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCOnderMaximum:
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schovpriople}]) PrioHalfstarOnderMaximum();");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCAfkapGroen:
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schovpriople}]) PrioHalfstarAfkapGroen();");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCStartGroenMomenten:
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schovpriople}]) PrioHalfstarStartGroenMomenten();");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCTegenhoudenConflicten:
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schovpriople}]) PrioHalfstarTegenhouden();");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCAfkappen:
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schovpriople}]) PrioHalfstarAfkappen();");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCTerugkomGroen:
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schovpriople}]) PrioHalfstarTerugkomGroen();");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCGroenVasthouden:
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schovpriople}]) PrioHalfstarGroenVasthouden();");
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCMeetkriterium:
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schovpriople}]) PrioHalfstarMeetKriterium();");
                    return sb.ToString();

                #endregion // prio.c

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
			_schca = CCOLGeneratorSettingsProvider.Default.GetElementName("schca");
            _tnlfg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfg");
            _tnlfgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfgd");
            _tnlsg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsg");
            _tnlsgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsgd");
			_tnlcv = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcv");
			_tnlcvd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcvd");
			_tnleg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnleg");
			_tnlegd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlegd");
			_huks = CCOLGeneratorSettingsProvider.Default.GetElementName("huks");
            _hiks = CCOLGeneratorSettingsProvider.Default.GetElementName("hiks");
            _prmxnl = CCOLGeneratorSettingsProvider.Default.GetElementName("prmxnl");
            _hnla = CCOLGeneratorSettingsProvider.Default.GetElementName("hnla");

            _hprioin = CCOLGeneratorSettingsProvider.Default.GetElementName("hprioin");
            _hpriouit = CCOLGeneratorSettingsProvider.Default.GetElementName("hpriouit");
            
            _treallr = CCOLGeneratorSettingsProvider.Default.GetElementName("treallr");
            _tinl = CCOLGeneratorSettingsProvider.Default.GetElementName("tinl");
            _trealil = CCOLGeneratorSettingsProvider.Default.GetElementName("trealil");

            return base.SetSettings(settings);
		}
	}
}
