using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Interfaces;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class DetectorModel : IOElementModel, IComparable
    {
        #region Fields

        #endregion // Fields

        #region Properties
        
        public override string Naam { get; set; }
        public string VissimNaam { get; set; }
        public int? TDB { get; set; }
        public int? TDH { get; set; }
        public int? TOG { get; set; }
        public int? TBG { get; set; }
        public int? TFL { get; set; }
        public int? CFL { get; set; }
        public bool AanvraagDirect { get; set; }
        public bool Wachtlicht { get; set; }
        
        public BitmapCoordinatenDataModel WachtlichtBitmapData { get; set; }

        public DetectorSimulatieModel Simulatie { get; set; }

        public DetectorTypeEnum Type { get; set; }
        public DetectorAanvraagTypeEnum Aanvraag { get; set; }
        public DetectorVerlengenTypeEnum Verlengen { get; set; }

        public bool ShouldSerializeWachtlichtBitmapData()
        {
            return Wachtlicht;
        }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
            if (obj is DetectorModel)
            {
                DetectorModel comp = obj as DetectorModel;
                return this.Naam.CompareTo(comp.Naam);
            }
            else
            {
                return 0;
            }
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
