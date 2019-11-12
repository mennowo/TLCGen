namespace TLCGen.Models
{
    public class FaseCyclusAlternatiefPerBlokModel
    {
        #region Properties

        [RefersTo(Enumerations.TLCGenObjectTypeEnum.Fase)]
        public string FaseCyclus { get; set; }
        public int BitWiseBlokAlternatief { get; set; }

        #endregion // Properties
    }
}