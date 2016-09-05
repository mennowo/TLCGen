using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class ConflictModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public string FaseVan { get; set; }
        public string FaseNaar { get; set; }

        [XmlIgnore]
        public int Waarde { get; set; }
        public int? GarantieWaarde { get; set; }

        [XmlElement(ElementName = "Waarde")]
        public string SerializedWaarde
        {
            get
            {
                switch (Waarde)
                {
                    case -4:
                        return "GKL";
                    case -3:
                        return "GK";
                    case -2:
                        return "FK";
                    default:
                        return Waarde.ToString();
                }
            }
            set
            {
                switch (value)
                {
                    case "GKL":
                        Waarde = -4;
                        break;
                    case "GK":
                        Waarde = -3;
                        break;
                    case "FK":
                        Waarde = -2;
                        break;
                    default:
                        int waarde;
                        if (!Int32.TryParse(value, out waarde))
                        {
                            throw new NotImplementedException($"Fout bij laden conflicten:\n\nconflicten van {FaseVan} naar {FaseNaar} heeft foutieve waarde {SerializedWaarde}");
                        }
                        else
                        {
                            Waarde = waarde;
                        }
                        break;
                }
            }
        }

        #endregion // Properties

        #region Constructor

        public ConflictModel() : base()
        {

        }

        #endregion // Constructor
    }
}
