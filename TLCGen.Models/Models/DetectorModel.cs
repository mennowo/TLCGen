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
    public class DetectorModel : IOElementModel, ITemplatable
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public string Define { get; set; }
        public string Naam { get; set; }
        public int? TDB { get; set; }
        public int? TDH { get; set; }
        public int? TOG { get; set; }
        public int? TBG { get; set; }
        public int? TFL { get; set; }
        public int? CFL { get; set; }

        public DetectorSimulatieModel Simulatie { get; set; }

        public DetectorTypeEnum Type { get; set; }
        public DetectorAanvraagTypeEnum Aanvraag { get; set; }
        public DetectorVerlengenTypeEnum Verlengen { get; set; }

        #endregion // Properties

        #region ITemplatable

        public string GetIdentifyingName()
        {
            return Naam;
        }

        public void SetAllIdentifyingNames(string search, string replace)
        {
            Naam = Naam.Replace(search, replace);
            Define = Define.Replace(search, replace);
        }

        public void ClearAllReferences()
        {
            BitmapCoordinaten.Clear();
        }

        #endregion // ITemplatable

        #region Constructor

        public DetectorModel() : base()
        {
            Simulatie = new DetectorSimulatieModel();
        }

        #endregion // Constructor
    }
}
