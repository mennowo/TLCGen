using System;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    public static class PrioCodeGeneratorHelper
    {
        public const string CAT_Basisfuncties =    "CAT_Basisfuncties";
        public const string CAT_Detectie =         "CAT_Detectie";
        public const string CAT_Signaalgroep =     "CAT_Signaalgroep";
        public const string CAT_Module =           "CAT_Module";
        public const string CAT_Signaalplan =      "CAT_Signaalplan";
        public const string CAT_Optimaliseren =    "CAT_Optimaliseren"; 
        public const string CAT_Prioriteren =      "CAT_Prioriteren";
        public const string CAT_Informeren =       "CAT_Informeren";
        public const string CAT_SpecialeIngrepen = "CAT_SpecialeIngrepen";
        public const string CAT_TestenLoggen =     "CAT_TestenLoggen";

        public const string SUBCAT_Detectie = "SUBCAT_Detectie";  
        public const string SUBCAT_Signaalgroep = "SUBCAT_Signaalgroep";   
        public const string SUBCAT_Aanvraag = "SUBCAT_Aanvraag";   
        public const string SUBCAT_Meeaanvraag = "SUBCAT_Meeaanvraag";   
        public const string SUBCAT_Wachtgroen = "SUBCAT_Wachtgroen";           
        public const string SUBCAT_VerlengenVAG3 = "SUBCAT_VerlengenVAG3";  
        public const string SUBCAT_VerlengenVAG4 = "SUBCAT_VerlengenVAG4";  
        public const string SUBCAT_Verlengen =  "SUBCAT_Verlengen";   
        public const string SUBCAT_Meeverlengen = "SUBCAT_Meeverlengen";   
        public const string SUBCAT_Deelconflicten = "SUBCAT_Deelconflicten";       
        public const string SUBCAT_HardeKoppeling = "SUBCAT_Harde Koppeling";  
        public const string SUBCAT_AlternatieveRealisaties = "SUBCAT_AlternatieveRealisaties";      
        public const string SUBCAT_ExtraPrimair = "SUBCAT_ExtraPrimair";       
        public const string SUBCAT_Klokperioden = "SUBCAT_Klokperioden";  
        public const string SUBCAT_VervangendeMaatregelen =  "SUBCAT_VervangendeMaatregelen";   
        public const string SUBCAT_MaximumGroentijden = "SUBCAT_MaximumGroentijden";  
        public const string SUBCAT_MaximumVerlenggroentijden = "SUBCAT_MaximumVerlenggroentijden";    
        public const string SUBCAT_Plantijden = "SUBCAT_Plantijden";           
        public const string SUBCAT_Hulpdienst = "SUBCAT_Hulpdienst";  
        public const string SUBCAT_OpenbaarVervoer = "SUBCAT_OpenbaarVervoer";   
        public const string SUBCAT_HoogwaardigOpenbaarVervoer = "SUBCAT_HoogwaardigOpenbaarVervoer";  
        public const string SUBCAT_Tovergroen = "SUBCAT_Tovergroen";     
        public const string SUBCAT_Vrachtverkeer = "SUBCAT_Vrachtverkeer";     
        public const string SUBCAT_CAM_Peloton = "SUBCAT_CAM_Peloton";       
        public const string SUBCAT_CAM_PelotonFiets = "SUBCAT_CAM_PelotonFiets";       
        public const string SUBCAT_Fasebewaking = "SUBCAT_Fasebewaking";   
        public const string SUBCAT_File = "SUBCAT_File";   
        public const string SUBCAT_Fixatie = "SUBCAT_Fixatie";   
        public const string SUBCAT_Afteller = "SUBCAT_Afteller";   
        public const string SUBCAT_Rateltikker = "SUBCAT_Rateltikker";   
        public const string SUBCAT_Wachttijdvoorspeller = "SUBCAT_Wachttijdvoorspeller";   
        public const string SUBCAT_Loggen = "SUBCAT_Loggen";   
        public const string SUBCAT_Testen = "SUBCAT_Testen";  
        
        public static string GetDetectorTypeSCHString(PrioIngreepInUitMeldingVoorwaardeInputTypeEnum type)
        {
            return type switch
            {
                PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie => "SD",
                PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieOp => "D",
                PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet => "DB",
                PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectieBezet => "SDB",
                PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie => "ED",
                PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectieHiaat => "ETDH",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static bool IsOV(this PrioIngreepModel prio)
        {
            return prio.Type == PrioIngreepVoertuigTypeEnum.Bus ||
                   prio.Type == PrioIngreepVoertuigTypeEnum.Tram;
        }
    }
}