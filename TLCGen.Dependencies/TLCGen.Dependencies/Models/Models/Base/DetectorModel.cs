using System;
using System.ComponentModel;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Detector, "Naam")]
    [IOElement("", BitmappedItemTypeEnum.Detector, "Naam")]
    public class DetectorModel : IOElementModel, IComparable, IHaveName
    {
        #region Fields

        #endregion // Fields

        #region Properties
        
        [ModelName(TLCGenObjectTypeEnum.Detector)]
        [Browsable(false)]
        public override string Naam { get; set; }
        public override bool Dummy { get; set; }
        [VissimName]
        [Browsable(false)]
        public string VissimNaam { get; set; }
        public int? TDB { get; set; }
        public int? TDH { get; set; }
        public int? TOG { get; set; }
        public int? TBG { get; set; }
        public int? TFL { get; set; }
        public int? CFL { get; set; }
        public bool ResetAanvraag { get; set; }
        public int ResetAanvraagTijdsduur { get; set; }
        public bool AanvraagDirect { get; set; }
        public bool Wachtlicht { get; set; }
        [HasDefault(false)]
        public int? Rijstrook { get; set; }

        [IOElement("wl", BitmappedItemTypeEnum.Uitgang, "Naam", "Wachtlicht")]
        public BitmapCoordinatenDataModel WachtlichtBitmapData { get; set; }

        public DetectorSimulatieModel Simulatie { get; set; }

        [Browsable(false)]
        public DetectorTypeEnum Type { get; set; }
        public DetectorAanvraagTypeEnum Aanvraag { get; set; }
        public DetectorVerlengenTypeEnum Verlengen { get; set; }
        public NooitAltijdAanUitEnum AanvraagBijStoring { get; set; }

        public NooitAltijdAanUitEnum VeiligheidsGroen { get; set; }
        public int VeiligheidsGroenHiaat { get; set; }
        public int VeiligheidsGroenVolgtijd { get; set; }

        public bool ShouldSerializeWachtlichtBitmapData()
        {
            return Wachtlicht;
        }

        #endregion // Properties

        #region Public Methods

        public bool IsDrukKnop()
        {
            return Type == DetectorTypeEnum.Knop ||
                   Type == DetectorTypeEnum.KnopBinnen ||
                   Type == DetectorTypeEnum.KnopBuiten;
        }

        #endregion // Public Methods

        #region IComparable

        public int CompareTo(object obj)
        {
            if (!(obj is DetectorModel dm)) throw new InvalidCastException();
            var myName = Naam;
            var hisName = dm.Naam;
            if (myName.Length < hisName.Length) myName = myName.PadLeft(hisName.Length, '0');
            else if (hisName.Length < myName.Length) hisName = hisName.PadLeft(myName.Length, '0');
            return string.Compare(myName, hisName, StringComparison.Ordinal);
        }

        #endregion // ITemplatable

        #region Constructor

        public DetectorModel() : base()
        {
            WachtlichtBitmapData = new BitmapCoordinatenDataModel();
            Simulatie = new DetectorSimulatieModel();
        }

        #endregion // Constructor
    }
}
