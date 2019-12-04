using System.Linq;
using System.Text;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateHstC(ControllerModel c)
        {
			if(!c.HalfstarData.IsHalfstar) return null;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* HALFSTARRE APPLICATIE */");
            sb.AppendLine("/* --------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(c.Data, "hst.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(c.Data));
            sb.AppendLine();
            sb.Append(GenerateHstCExtraDefines(c));
            sb.Append(GenerateHstCIncludes(c));
            sb.Append(GenerateHstCPostInitApplication(c));
            sb.Append(GenerateHstCPreApplication(c));
	        sb.Append(GenerateHstCKlokperioden(c));
	        sb.Append(GenerateHstCAanvragen(c));
	        sb.Append(GenerateHstCMaxOfVerlengroen(c));
	        sb.Append(GenerateHstCWachtgroen(c));
	        sb.Append(GenerateHstCMeetkriterium(c));
	        sb.Append(GenerateHstCMeeverlengen(c));
	        sb.Append(GenerateHstCSynchronisaties(c));
	        sb.Append(GenerateHstCAlternatief(c));
	        sb.Append(GenerateHstCRealisatieAfhandeling(c));
	        sb.Append(GenerateHstCFileVerwerking(c));
	        sb.Append(GenerateHstCDetectieStoring(c));
	        sb.Append(GenerateHstCPostApplication(c));
	        sb.Append(GenerateHstCPreSystemApplication(c));
	        sb.Append(GenerateHstCPostSystemApplication(c));
	        sb.Append(GenerateHstCPostDumpApplication(c));
	        sb.Append(GenerateHstCApplicationTig1(c));
	        sb.Append(GenerateHstCApplicationTig2(c));
	        sb.Append(GenerateHstCOVSettingsHalfstar(c));

            return sb.ToString();
        }

	    private string GenerateHstCExtraDefines(ControllerModel c)
        {
            StringBuilder sb = new StringBuilder();
            
            return sb.ToString();
        }

	    private string GenerateHstCIncludes(ControllerModel c)
		{
			var sb = new StringBuilder();

			sb.AppendLine("#include \"halfstar.c\"");
			sb.AppendLine("#include \"tx_synch.h\"");
			if (c.PrioData.PrioIngreepType != PrioIngreepTypeEnum.Geen && c.HasPTorHD())
			{
				sb.AppendLine("#include \"halfstar_prio.c\"");
			}
			sb.AppendLine();
            sb.AppendLine($"#include \"{c.Data.Naam}hst.add\"");
			sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCPostInitApplication(ControllerModel c)
		{
			var sb = new StringBuilder();

			sb.AppendLine("void post_init_application_halfstar(void)");
			sb.AppendLine("{");
			
            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCPostInitApplication, true, true, false, true);

            sb.AppendLine($"{ts}post_init_application_halfstar_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCPreApplication(ControllerModel c)
		{
			var sb = new StringBuilder();
			
			sb.AppendLine("void pre_application_halfstar(void)");
			sb.AppendLine("{");
			
            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCPreApplication, true, true, false, true);

            sb.AppendLine($"{ts}PreApplication_halfstar_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCKlokperioden(ControllerModel c)
		{
			var sb = new StringBuilder();
			
			sb.AppendLine("void KlokPerioden_halfstar(void)");
			sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCKlokPerioden, true, true, false, true);

            sb.AppendLine($"{ts}KlokPerioden_halfstar_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCAanvragen(ControllerModel c)
		{
			var sb = new StringBuilder();

			sb.AppendLine("void Aanvragen_halfstar(void)");
			sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCAanvragen, true, true, false, true);

            sb.AppendLine($"{ts}Aanvragen_halfstar_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCMaxOfVerlengroen(ControllerModel c)
		{
			var sb = new StringBuilder();

			if (c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden)
			{
				sb.AppendLine("void Maxgroen_halfstar(void)");
				sb.AppendLine("{");

                AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCMaxgroen, true, true, false, true);

                sb.AppendLine($"{ts}Maxgroen_halfstar_Add();");

                sb.AppendLine("}");
			}
			else
			{
				sb.AppendLine("void Verlenggroen_halfstar(void)");
				sb.AppendLine("{");

                AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCVerlenggroen, true, true, false, true);

                sb.AppendLine($"{ts}Maxgroen_halfstar_Add();");

                sb.AppendLine("}");
			}
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCWachtgroen(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void Wachtgroen_halfstar(void)");
			sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCWachtgroen, true, true, false, true);

            sb.AppendLine($"{ts}Wachtgroen_halfstar_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCMeetkriterium(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void Meetkriterium_halfstar(void)");
			sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCMeetkriterium, true, true, false, true);

            sb.AppendLine($"{ts}Meetkriterium_halfstar_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCMeeverlengen(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void Meeverlengen_halfstar(void)");
			sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCMeeverlengen, true, true, false, true);

            sb.AppendLine($"{ts}Meeverlengen_halfstar_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCSynchronisaties(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void Synchronisaties_halfstar(void)");
			sb.AppendLine("{");
			
            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCSynchronisaties, true, true, false, true);

            sb.AppendLine($"{ts}Synchronisaties_halfstar_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCAlternatief(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void Alternatief_halfstar(void)");
			sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCAlternatief, true, true, false, true);

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}
		
	    private string GenerateHstCRealisatieAfhandeling(ControllerModel c)
		{
			var sb = new StringBuilder();

			sb.AppendLine("void RealisatieAfhandeling_halfstar(void)");
			sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCRealisatieAfhandeling, true, true, false, true);

            sb.AppendLine($"{ts}RealisatieAfhandeling_halfstar_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCFileVerwerking(ControllerModel c)
	    {
		    var sb = new StringBuilder();

		    sb.AppendLine("void FileVerwerking_halfstar(void)");
		    sb.AppendLine("{");

            sb.AppendLine();
            sb.AppendLine($"{ts}FileVerwerking_halfstar_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
	    }

	    private string GenerateHstCDetectieStoring(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void DetectieStoring_halfstar(void)");
			sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCDetectieStoring, true, true, false, true);

            sb.AppendLine($"{ts}DetectieStoring_halfstar_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCPostApplication(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void PostApplication_halfstar(void)");
			sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCPostApplication, true, true, false, true);

            sb.AppendLine($"{ts}PostApplication_halfstar_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCPreSystemApplication(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void pre_system_application_halfstar(void)");
			sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCPreSystemApplication, true, true, false, true);

            sb.AppendLine($"{ts}pre_system_application_halfstar_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCPostSystemApplication(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void post_system_application_halfstar(void)");
			sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCPostSystemApplication, true, true, false, true);

            sb.AppendLine($"{ts}post_system_application_halfstar_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCPostDumpApplication(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void post_dump_application_halfstar(void)");
			sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCPostDumpApplication, true, true, false, true);
			
            sb.AppendLine($"{ts}post_dump_application_halfstar_Add();");

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCApplicationTig1(ControllerModel c)
		{
			var sb = new StringBuilder();
						
            if(c.Data.CCOLVersie >= CCOLVersieEnum.CCOL95)
            {
			    sb.AppendLine($"{c.GetBoolV()} application1_trig(void)");
            }
            else
            {
                sb.AppendLine($"{c.GetBoolV()} application1_tig(void)");
            }
			sb.AppendLine("{");
			sb.AppendLine($"{ts}return application1_tig_Add();");
			sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

	    private string GenerateHstCApplicationTig2(ControllerModel c)
		{
			var sb = new StringBuilder();

            if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL95)
            {
                sb.AppendLine($"{c.GetBoolV()} application2_trig(void)");
            }
            else
            {
                sb.AppendLine($"{c.GetBoolV()} application2_tig(void)");
            }
			sb.AppendLine("{");
            sb.AppendLine($"{ts}return application2_tig_Add();");
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
		}

        private string GenerateHstCOVSettingsHalfstar(ControllerModel c)
        {
            var sb = new StringBuilder();

            if (!c.PrioData.PrioIngrepen.Any()) return "";

            sb.AppendLine($"/* Deze functie wordt aangeroepen vanuit OVInstellingen() in {c.Data.Naam}ov.c */");
            sb.AppendLine("void OVHalfstarSettings(void)");
            sb.AppendLine("{");

            AddCodeTypeToStringBuilder(c, sb, CCOLCodeTypeEnum.HstCOVHalfstarSettings, true, true, false, false);
            
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateHstCSignaalPlanInstellingen(ControllerModel controller)
	    {
		    var sb = new StringBuilder();

		    sb.AppendLine("void signaalplan_instellingen(void)");
		    sb.AppendLine("{");

			sb.AppendLine($"{ts}/* CYCLUSTIJDEN SIGNAALPLANNEN */");
			sb.AppendLine($"{ts}/* =========================== */");
		    foreach (var pl in controller.HalfstarData.SignaalPlannen)
		    {
			    sb.AppendLine($"{ts}TX_max[{pl.Naam}] = {pl.Cyclustijd}; /* {pl.Commentaar} */");
		    }
		    sb.AppendLine();

		    sb.AppendLine($"{ts}/* IN/UITSCHAKELTIJDEN SIGNAALPLANNEN */");
		    sb.AppendLine($"{ts}/* ================================== */");
		    foreach (var pl in controller.HalfstarData.SignaalPlannen)
		    {
			    sb.AppendLine($"{ts}TPL_on[{pl.Naam}] = {pl.StartMoment}; TPL_off[{pl.Naam}] = {pl.SwitchMoment}; /* {pl.Commentaar} */");
		    }
		    sb.AppendLine();

		    sb.AppendLine($"{ts}/* FASECYCLUSTIJDEN VAN DE SIGNAALPLANNEN */");
		    sb.AppendLine($"{ts}/* ====================================== */");
		    foreach (var pl in controller.HalfstarData.SignaalPlannen)
		    {
			    sb.AppendLine($"{ts}/* {pl.Commentaar} */");
			    foreach (var plfc in pl.Fasen)
			    {
				    sb.AppendLine(
					    $"{ts}SetPlanTijden({_fcpf}{plfc.FaseCyclus}, " +
                        $"{pl.Naam}, " +
                        $"{(plfc.A1 ?? 0).ToString().PadLeft(3)}, " +
                        $"{plfc.B1.ToString().PadLeft(3)}, " +
                        $"{(plfc.C1 ?? 0).ToString().PadLeft(3)}, " +
                        $"{plfc.D1.ToString().PadLeft(3)}, " +
                        $"{(plfc.E1 ?? 0).ToString().PadLeft(3)});");
			    }
				sb.AppendLine();
		    }
		    
		    sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
	    }
	}
}
