﻿using System;
using System.ComponentModel;
using TLCGen.Integrity;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [IOElement("", BitmappedItemTypeEnum.Detector, "Naam")]
    public class DetectorModel : IOElementModel, IComparable, IHaveName
    {
        #region Fields

        #endregion // Fields

        #region Properties
        
        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        [ModelName(TLCGenObjectTypeEnum.Detector)]
        [Browsable(false)]
        public override string Naam { get; set; }
        public override bool Dummy { get; set; }
        [VissimName]
        [Browsable(false)]
        public string VissimNaam { get; set; }

        [RefersTo(TLCGenObjectTypeEnum.Fase)] 
        [Browsable(false)]
        [HasDefault(false)]
        public string FaseCyclus { get; set; }

        public int? TDB { get; set; }
        public int? TDH { get; set; }
        public int? TOG { get; set; }
        public int? TBG { get; set; }
        public int? TFL { get; set; }
        public int? CFL { get; set; }
        public bool ResetAanvraag { get; set; }
        public int ResetAanvraagTijdsduur { get; set; }
        
        // this is obsolete, but needed for older tlcgen files
        public bool AanvraagDirect { get; set; }
        public NooitAltijdAanUitEnum AanvraagDirectSch { get; set; }
        
        public bool Wachtlicht { get; set; }
        [HasDefault(false)]
        public int? Rijstrook { get; set; }

        [IOElement("wl", BitmappedItemTypeEnum.Uitgang, "Naam", "Wachtlicht")]
        public BitmapCoordinatenDataModel WachtlichtBitmapData { get; set; }

        public DetectorSimulatieModel Simulatie { get; set; }

        [Browsable(false)] [HasDefault(false)] public TLCGenObjectTypeEnum ObjectType => TLCGenObjectTypeEnum.Detector;

        [Browsable(false)]
        public DetectorTypeEnum Type { get; set; }
        public DetectorAanvraagTypeEnum Aanvraag { get; set; }
        public bool AanvraagHardOpStraat { get; set; }
        public DetectorVerlengenTypeEnum Verlengen { get; set; }
        public bool VerlengenHardOpStraat { get; set; }
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

        public bool IsKopLang()
        {
            return Type == DetectorTypeEnum.Kop ||
                   Type == DetectorTypeEnum.Lang;
        }

        #endregion // Public Methods

        #region IComparable

        public int CompareTo(object obj)
        {
            if (!(obj is DetectorModel d2)) throw new InvalidCastException();
            return TLCGenIntegrityChecker.CompareDetectors(Naam, d2.Naam, null, null);
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
