namespace TLCGen.ViewModels
{
    public class MeerealiserendeIngreepItemViewModel
    {
        public string FaseCyclus { get; set; }
        public string PrioIngreep { get; set; }
        public override string ToString()
        {
            return FaseCyclus + " - " + PrioIngreep;
        }
    }
}
