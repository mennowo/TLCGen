namespace TLCGen.Plugins.RIS.Models
{
    public class RISFaseCyclusLaneStationTypeSimulatieModel
    {
        public RISStationTypeEnum Type { get; set; }
        public bool Prioriteit { get; set; }
        public int Flow { get; set; }
        public int Snelheid { get; set; }
        public int Afstand { get; set; }
    }
}
