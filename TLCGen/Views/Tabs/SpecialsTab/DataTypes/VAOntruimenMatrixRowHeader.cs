namespace TLCGen.ViewModels
{
    /// <summary>
    /// Row header of the VA ontruimen matrix. Manual rows are shown with a blue hue, so the user can
    /// tell them apart from the automatically generated (immutable) hard conflicts.
    /// </summary>
    public class VAOntruimenMatrixRowHeader
    {
        public VAOntruimenMatrixRowHeader(string faseCyclus, bool isManual)
        {
            FaseCyclus = faseCyclus;
            IsManual = isManual;
        }

        public string FaseCyclus { get; }

        public bool IsManual { get; }
    }
}
