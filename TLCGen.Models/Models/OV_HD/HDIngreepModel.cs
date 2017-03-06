using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToSignalGroup("FaseCyclus")]
    public class HDIngreepModel
    {
        #region Properties

        [Browsable(false)]
        public string FaseCyclus { get; set; }

        public bool KAR { get; set; }
        [Browsable(false)] // Opticom is not (yet) supported
        public bool Opticom { get; set; }
        public bool Sirene { get; set; }

        public int RijTijdOngehinderd { get; set; }
        public int RijTijdBeperktgehinderd { get; set; }
        public int RijTijdGehinderd { get; set; }
        public int GroenBewaking { get; set; }

        [Browsable(false)]
        [IOElement("vchd", BitmappedItemTypeEnum.Uitgang, "FaseCyclus")]
        public BitmapCoordinatenDataModel HDInmeldingBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("hdkar_dummy_in", BitmappedItemTypeEnum.Ingang, "FaseCyclus", "KAR")]
        public BitmapCoordinatenDataModel HDKARDummyInmeldingBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("hdkar_dummy_uit", BitmappedItemTypeEnum.Ingang, "FaseCyclus", "KAR")]
        public BitmapCoordinatenDataModel HDKARDummyUitmeldingBitmapData { get; set; }

        [Browsable(false)]
        [XmlArrayItem(ElementName = "MeerealiserendeFaseCyclus")]
        public List<HDIngreepMeerealiserendeFaseCyclusModel> MeerealiserendeFaseCycli { get; set; }

        #endregion // Properties

        #region Constructor

        public HDIngreepModel()
        {
            MeerealiserendeFaseCycli = new List<HDIngreepMeerealiserendeFaseCyclusModel>();
            HDInmeldingBitmapData = new BitmapCoordinatenDataModel();
            HDKARDummyInmeldingBitmapData = new BitmapCoordinatenDataModel();
            HDKARDummyUitmeldingBitmapData = new BitmapCoordinatenDataModel();
        }

        #endregion // Constructor
    }
}
