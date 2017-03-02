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

        [Category("Opties")]
        public bool KAR { get; set; }
        public bool Vecom { get; set; }
        public bool Opticom { get; set; }
        public bool Sirene { get; set; }

        [Category("Tijden")]
        [Description("Rijtijd ongehinderd")]
        public int RijTijdOngehinderd { get; set; }
        [Description("Rijtijd beperkt gehinderd")]
        public int RijTijdBeperktgehinderd { get; set; }
        [Description("Rijtijd gehinderd")]
        public int RijTijdGehinderd { get; set; }
        [Description("Groenbewaking")]
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
        [IOElement("hdvecom_dummy_in", BitmappedItemTypeEnum.Ingang, "FaseCyclus", "Vecom")]
        public BitmapCoordinatenDataModel HDVecomDummyInmeldingBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("hdvecom_dummy_uit", BitmappedItemTypeEnum.Ingang, "FaseCyclus", "Vecom")]
        public BitmapCoordinatenDataModel HDVecomDummyUitmeldingBitmapData { get; set; }

        [Browsable(false)]
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
