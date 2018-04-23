using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class ModuleDisplayElementModel : IComparable
    {
        [IOElement("", BitmappedItemTypeEnum.Uitgang, "Naam")]
        public BitmapCoordinatenDataModel BitmapData { get; set; }

        private string _Naam;
        [RefersTo]
        public string Naam
        {
            get
            {
                return _Naam;
            }
            set
            {
                _Naam = value;
                BitmapData.Naam = value;
            }
        }

        public ModuleDisplayElementModel()
        {
            BitmapData = new BitmapCoordinatenDataModel();
        }

        public int CompareTo(object obj)
        {
            return string.Compare(Naam, ((ModuleDisplayElementModel)obj).Naam, StringComparison.Ordinal);
        }
    }
}
