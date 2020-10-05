using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class HDIngreepModel : IComparable
    {
        #region Properties

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [Browsable(false)]
        [HasDefault(false)]
        public string FaseCyclus { get; set; }

        public bool KAR { get; set; }
        
        #region RIS

        public bool RIS { get; set; }
        public int RisStart { get; set; }
        public int RisEnd { get; set; }
        
        #endregion

        public bool Opticom { get; set; }
        public int? KARInmeldingFilterTijd { get; set; }
        public int? KARUitmeldingFilterTijd { get; set; }
        public int? OpticomInmeldingFilterTijd { get; set; }
        [Browsable(false)]
        [HasDefault(false)]
        public string OpticomRelatedInput { get; set; }
        public bool Sirene { get; set; }
        public bool InmeldingOokDoorToepassen { get; set; }
        public int InmeldingOokDoorFase { get; set; }

        public int RijTijdOngehinderd { get; set; }
        public int RijTijdBeperktgehinderd { get; set; }
        public int RijTijdGehinderd { get; set; }
        public int GroenBewaking { get; set; }

        [Browsable(false)]
        [IOElement("vchd", BitmappedItemTypeEnum.Uitgang, "FaseCyclus")]
        public BitmapCoordinatenDataModel HDInmeldingBitmapData { get; set; }

        [Browsable(false)]
        public DetectorModel DummyKARInmelding { get; set; }
        [Browsable(false)]
        public DetectorModel DummyKARUitmelding { get; set; }

        [Browsable(false)]
        [XmlArrayItem(ElementName = "MeerealiserendeFaseCyclus")]
        public List<HDIngreepMeerealiserendeFaseCyclusModel> MeerealiserendeFaseCycli { get; set; }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
	        if(!(obj is HDIngreepModel hd2))
            {
                throw new InvalidCastException();
            }
            else
            {
                return string.Compare(FaseCyclus, hd2.FaseCyclus, StringComparison.Ordinal);
            }
        }

        #endregion // IComparable

        #region Constructor

        public HDIngreepModel()
        {
            MeerealiserendeFaseCycli = new List<HDIngreepMeerealiserendeFaseCyclusModel>();
            HDInmeldingBitmapData = new BitmapCoordinatenDataModel();
            DummyKARInmelding = new DetectorModel{ Dummy = true };
            DummyKARUitmelding = new DetectorModel{ Dummy = true };
        }

        #endregion // Constructor
    }
}
