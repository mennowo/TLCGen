using System;
using System.Collections.Generic;
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

        public string FaseCyclus { get; set; }
        public bool KAR { get; set; }
        public bool Vecom { get; set; }
        public bool Opticom { get; set; }
        public bool Sirene { get; set; }
        public int GroenBewaking { get; set; }
        public int RijTijdOngehinderd { get; set; }
        public int RijTijdBeperktgehinderd { get; set; }
        public int RijTijdGehinderd { get; set; }
        public int PrioriteitsOpties { get; set; }
        [IOElement("vchd", BitmappedItemTypeEnum.Uitgang, "FaseCyclus")]
        public BitmapCoordinatenDataModel HDInmeldingBitmapData { get; set; }
        [IOElement("hdkar_dummy_in", BitmappedItemTypeEnum.Ingang, "FaseCyclus", "KAR")]
        public BitmapCoordinatenDataModel HDKARDummyInmeldingBitmapData { get; set; }
        [IOElement("hdkar_dummy_uit", BitmappedItemTypeEnum.Ingang, "FaseCyclus", "KAR")]
        public BitmapCoordinatenDataModel HDKARDummyUitmeldingBitmapData { get; set; }
        [IOElement("hdvecom_dummy_in", BitmappedItemTypeEnum.Ingang, "FaseCyclus", "Vecom")]
        public BitmapCoordinatenDataModel HDVecomDummyInmeldingBitmapData { get; set; }
        [IOElement("hdvecom_dummy_uit", BitmappedItemTypeEnum.Ingang, "FaseCyclus", "Vecom")]
        public BitmapCoordinatenDataModel HDVecomDummyUitmeldingBitmapData { get; set; }

        [XmlArrayItem(ElementName = "MeerealiserendeFaseCyclus")]
        public List<HDIngreepMeerealiserendeFaseCyclusModel> MeerealiserendeFaseCycli { get; set; }

        #endregion // Properties

        #region IBelongToSignalGroup

        public string SignalGroup1
        {
            get { return FaseCyclus; }
        }

        public string SignalGroup2
        {
            get { return null; }
        }

        #endregion // IBelongToSignalGroup

        #region Constructor

        public HDIngreepModel()
        {
            MeerealiserendeFaseCycli = new List<HDIngreepMeerealiserendeFaseCyclusModel>();
            HDInmeldingBitmapData = new BitmapCoordinatenDataModel();
            HDKARDummyInmeldingBitmapData = new BitmapCoordinatenDataModel();
            HDKARDummyUitmeldingBitmapData = new BitmapCoordinatenDataModel();
            HDVecomDummyInmeldingBitmapData = new BitmapCoordinatenDataModel();
            HDVecomDummyUitmeldingBitmapData = new BitmapCoordinatenDataModel();
        }

        #endregion // Constructor
    }
}
