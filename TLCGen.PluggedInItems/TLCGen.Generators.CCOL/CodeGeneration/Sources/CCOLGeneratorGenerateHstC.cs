using System.Text;
using TLCGen.Generators.CCOL.Settings;
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
	        sb.Append(GenerateHstCVersneldPrimair(c));
	        sb.Append(GenerateHstCAlternatief(c));
	        sb.Append(GenerateHstCModules(c));
	        sb.Append(GenerateHstCRealisatieAfhandeling(c));
	        sb.Append(GenerateHstCFileVerwerking(c));
	        sb.Append(GenerateHstCDetectieStoring(c));
	        sb.Append(GenerateHstCPostApplication(c));
	        sb.Append(GenerateHstCSynchronisatieHalfstar(c));
	        sb.Append(GenerateHstCPreSystemApplication(c));
	        sb.Append(GenerateHstCPostSystemApplication(c));
	        sb.Append(GenerateHstCPostDumpApplication(c));
	        sb.Append(GenerateHstCApplicationTig1(c));
	        sb.Append(GenerateHstCApplicationTig2(c));
	        sb.Append(GenerateHstCSignaalPlanInstellingen(c));

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

			sb.AppendLine("#include \"halfstar_help.c\"");
			sb.AppendLine("#include \"halfstar.c\"");
			if (c.OVData.OVIngreepType != OVIngreepTypeEnum.Geen)
			{
				sb.AppendLine("#include \"halfstar_ov.h\"");
			}
			sb.AppendLine();

			return sb.ToString();
		}

	    private string GenerateHstCPostInitApplication(ControllerModel c)
		{
			var sb = new StringBuilder();

			sb.AppendLine("void post_init_application_halfstar(void)");
			sb.AppendLine("{");
			
			foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.HstCPostInitApplication])
			{
				sb.Append(gen.Value.GetCode(c, CCOLCodeTypeEnum.HstCPostInitApplication, ts));
			}
			
			sb.AppendLine("}");

			return sb.ToString();
		}

	    private string GenerateHstCPreApplication(ControllerModel c)
		{
			var sb = new StringBuilder();
			
			sb.AppendLine("void pre_application_halfstar(void)");
			sb.AppendLine("{");
			
			foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.HstCPreApplication])
			{
				sb.Append(gen.Value.GetCode(c, CCOLCodeTypeEnum.HstCPreApplication, ts));
			}
			
			sb.AppendLine("}");

			return sb.ToString();
		}

	    private string GenerateHstCKlokperioden(ControllerModel c)
		{
			var sb = new StringBuilder();
			
			sb.AppendLine("void KlokPerioden_halfstar(void)");
			sb.AppendLine("{");

			foreach (var gen in OrderedPieceGenerators[CCOLCodeTypeEnum.HstCKlokPerioden])
			{
				sb.Append(gen.Value.GetCode(c, CCOLCodeTypeEnum.HstCKlokPerioden, ts));
			}

			sb.AppendLine("}");

			return sb.ToString();
		}

	    private string GenerateHstCAanvragen(ControllerModel c)
		{
			var sb = new StringBuilder();

			sb.AppendLine("void Aanvragen_halfstar(void)");
			sb.AppendLine("{");
			sb.AppendLine("}");

			return sb.ToString();
		}

	    private string GenerateHstCMaxOfVerlengroen(ControllerModel c)
		{
			var sb = new StringBuilder();

			if (c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden)
			{
				sb.AppendLine("void Maxgroen_halfstar(void)");
				sb.AppendLine("{");
				sb.AppendLine("}");
			}
			else
			{
				sb.AppendLine("void Verlenggroen_halfstar(void)");
				sb.AppendLine("{");
				sb.AppendLine("}");
			}

			return sb.ToString();
		}

	    private string GenerateHstCWachtgroen(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void Wachtgroen_halfstar(void)");
			sb.AppendLine("{");
			sb.AppendLine("}");

			return sb.ToString();
		}

	    private string GenerateHstCMeetkriterium(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void Meetkriterium_halfstar(void)");
			sb.AppendLine("{");
			sb.AppendLine("}");

			return sb.ToString();
		}

	    private string GenerateHstCMeeverlengen(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void Meeverlengen_halfstar(void)");
			sb.AppendLine("{");
			sb.AppendLine("}");

			return sb.ToString();
		}

	    private string GenerateHstCSynchronisaties(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void Synchronisaties_halfstar(void)");
			sb.AppendLine("{");
			sb.AppendLine("}");

			return sb.ToString();
		}
		
	    private string GenerateHstCVersneldPrimair(ControllerModel c)
	    {
		    var sb = new StringBuilder();
						
		    sb.AppendLine("void VersneldPrimair_halfstar(void)");
		    sb.AppendLine("{");
		    sb.AppendLine("}");

		    return sb.ToString();
	    }

	    private string GenerateHstCAlternatief(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void Alternatief_halfstar(void)");
			sb.AppendLine("{");
			sb.AppendLine("}");

			return sb.ToString();
		}
		
	    private string GenerateHstCModules(ControllerModel c)
	    {
		    var sb = new StringBuilder();
						
		    sb.AppendLine("void Modules_halfstar(void)");
		    sb.AppendLine("{");
		    sb.AppendLine("}");

		    return sb.ToString();
	    }

	    private string GenerateHstCRealisatieAfhandeling(ControllerModel c)
		{
			var sb = new StringBuilder();

			sb.AppendLine("void RealisatieAfhandeling_halfstar(void)");
			sb.AppendLine("{");
			sb.AppendLine("}");

			return sb.ToString();
		}

	    private string GenerateHstCFileVerwerking(ControllerModel c)
	    {
		    var sb = new StringBuilder();

		    sb.AppendLine("void FileVerwerking_halfstar(void)");
		    sb.AppendLine("{");
		    sb.AppendLine("}");

		    return sb.ToString();
	    }

	    private string GenerateHstCDetectieStoring(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void DetectieStoring_halfstar(void)");
			sb.AppendLine("{");
			sb.AppendLine("}");

			return sb.ToString();
		}

	    private string GenerateHstCPostApplication(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void PostApplication_halfstar(void)");
			sb.AppendLine("{");
			sb.AppendLine("}");

			return sb.ToString();
		}

	    private string GenerateHstCSynchronisatieHalfstar(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void SynchronisatieHalfstar_halfstar(void)");
			sb.AppendLine("{");
			sb.AppendLine("}");

			return sb.ToString();
		}

	    private string GenerateHstCPreSystemApplication(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void pre_system_application_halfstar(void)");
			sb.AppendLine("{");
			sb.AppendLine("}");

			return sb.ToString();
		}

	    private string GenerateHstCPostSystemApplication(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void post_system_application_halfstar(void)");
			sb.AppendLine("{");
			sb.AppendLine("}");

			return sb.ToString();
		}

	    private string GenerateHstCPostDumpApplication(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("void post_dump_application_halfstar(void)");
			sb.AppendLine("{");
			sb.AppendLine("}");

			return sb.ToString();
		}

	    private string GenerateHstCApplicationTig1(ControllerModel c)
		{
			var sb = new StringBuilder();
						
			sb.AppendLine("bool application1_tig(void)");
			sb.AppendLine("{");
			sb.AppendLine("}");

			return sb.ToString();
		}

	    private string GenerateHstCApplicationTig2(ControllerModel c)
		{
			var sb = new StringBuilder();

			sb.AppendLine("bool application2_tig(void)");
			sb.AppendLine("{");
			sb.AppendLine("}");

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
					    $"{ts}SetPlanTijden({_fcpf}{plfc.FaseCyclus}, {pl.Naam}, {plfc.A1 ?? 0}, {plfc.B1}, {plfc.C1 ?? 0}, {plfc.D1}, {plfc.E1 ?? 0});");
			    }
				sb.AppendLine();
		    }
		    
		    sb.AppendLine("}");

		    return sb.ToString();
	    }
	}
}
