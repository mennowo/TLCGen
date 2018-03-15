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
    [RefersTo("FaseVan", "FaseNaar")]
    public class ConflictModel : IComparable, IInterSignaalGroepElement
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
                    case -6:
                    case -5:
                        return "";
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
	                    if (!int.TryParse(value, out var waarde))
                        {
                            throw new ArgumentOutOfRangeException($"Fout bij laden conflicten:\n\nconflicten van {FaseVan} naar {FaseNaar} heeft foutieve waarde {SerializedWaarde}");
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

        #region IComparable

        public int CompareTo(object obj)
        {
            if(obj is ConflictModel)
            {
                string s1 = (obj as ConflictModel).FaseVan;
                string s2 = this.FaseVan;
                if (s1.Length < s2.Length) s1 = s1.PadLeft(s2.Length, '0');
                else if (s2.Length < s1.Length) s2 = s2.PadLeft(s1.Length, '0');

                return s2.CompareTo(s1);
            }
            return 0;
        }

        #endregion // IComparable

        #region Constructor

        public ConflictModel() : base()
        {

        }

        #endregion // Constructor
    }
}
