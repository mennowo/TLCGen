using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class StarDataModel
    {
        public bool ToepassenStar { get; set; }
        public bool ProgrammaSturingViaParameter { get; set; }
        public bool ProgrammaSturingViaKlok { get; set; }
        public bool IngangAlsVoorwaarde { get; set; }
        public bool ProgrammaTijdenInParameters { get; set; }
        
        [IOElement("star", BitmappedItemTypeEnum.Ingang, conditionprop: nameof(IngangAlsVoorwaarde))]
        public BitmapCoordinatenDataModel StarRegelenIngang { get; set; }

        [XmlElement(ElementName = "Programma")]
        public List<StarProgrammaModel> Programmas { get; set; }
        public string DefaultProgramma { get; set; }
        public List<StarPeriodeDataModel> PeriodenData { get; set; }


        public StarDataModel()
        {
            Programmas = new List<StarProgrammaModel>();
            PeriodenData = new List<StarPeriodeDataModel>();
            StarRegelenIngang = new BitmapCoordinatenDataModel();
        }
    }
}
