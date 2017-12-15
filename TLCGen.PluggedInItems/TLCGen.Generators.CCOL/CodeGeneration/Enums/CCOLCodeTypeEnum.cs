namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public enum CCOLCodeTypeEnum
    {
		// REG
        RegCIncludes,
        RegCTop,
        RegCKwcApplication,
        RegCPreApplication,
        RegCKlokPerioden,
        RegCAanvragen,
        RegCVerlenggroen,
        RegCMaxgroen,
        RegCWachtgroen,
        RegCMeetkriterium,
        RegCFileVerwerking,
        RegCDetectieStoring,
        RegCMeeverlengen,
        RegCRealisatieAfhandelingModules,
        RegCRealisatieAfhandelingNaModules,
        RegCRealisatieAfhandeling,
        RegCAlternatieven,
        RegCSynchronisaties,
        RegCInitApplication,
        RegCApplication,
        RegCPostApplication,
        RegCPreSystemApplication,
        RegCSystemApplication,
        RegCPostSystemApplication,
        RegCDumpApplication,

		// OV
	    OvCIncludes,
		OvCTop,
		OvCInstellingen,
		OvCRijTijdScenario,
		OvCInUitMelden,
	    OvCPARCorrecties,
	    OvCPARCcol,
	    OvCSpecialSignals,
		OvCBottom,

		// TAB
	    TabCControlParameters
	};
}
