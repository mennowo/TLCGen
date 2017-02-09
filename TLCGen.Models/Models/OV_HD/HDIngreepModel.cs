using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class HDIngreepModel
    {
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
        public BitmapCoordinatenDataModel HDInmeldingBitmapData { get; set; }
        public BitmapCoordinatenDataModel HDKARDummyInmeldingBitmapData { get; set; }
        public BitmapCoordinatenDataModel HDKARDummyUitmeldingBitmapData { get; set; }
        public BitmapCoordinatenDataModel HDVecomDummyInmeldingBitmapData { get; set; }
        public BitmapCoordinatenDataModel HDVecomDummyUitmeldingBitmapData { get; set; }

        [XmlArrayItem(ElementName = "MeerealiserendeFaseCyclus")]
        public List<HDIngreepMeerealiserendeFaseCyclusModel> MeerealiserendeFaseCycli { get; set; }

        public HDIngreepModel()
        {
            MeerealiserendeFaseCycli = new List<HDIngreepMeerealiserendeFaseCyclusModel>();
            HDInmeldingBitmapData = new BitmapCoordinatenDataModel();
            HDKARDummyInmeldingBitmapData = new BitmapCoordinatenDataModel();
            HDKARDummyUitmeldingBitmapData = new BitmapCoordinatenDataModel();
            HDVecomDummyInmeldingBitmapData = new BitmapCoordinatenDataModel();
            HDVecomDummyUitmeldingBitmapData = new BitmapCoordinatenDataModel();
        }
    }
}
