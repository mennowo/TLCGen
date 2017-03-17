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
    public class HDIngreepModel : IComparable
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
            var hd2 = obj as HDIngreepModel;
            if(hd2 == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                return this.FaseCyclus.CompareTo(hd2.FaseCyclus);
            }
        }

        #endregion // IComparable

        #region Constructor

        public HDIngreepModel()
        {
            MeerealiserendeFaseCycli = new List<HDIngreepMeerealiserendeFaseCyclusModel>();
            HDInmeldingBitmapData = new BitmapCoordinatenDataModel();
            DummyKARInmelding = new DetectorModel() { Dummy = true };
            DummyKARUitmelding = new DetectorModel() { Dummy = true };
        }

        #endregion // Constructor
    }
}
