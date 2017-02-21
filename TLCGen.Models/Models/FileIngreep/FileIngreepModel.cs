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
    public class FileIngreepModel
    {
        public string Naam { get; set; }
        public int MinimaalAantalMeldingen { get; set; }
        public int AfvalVertraging { get; set; }
        public bool EerlijkDoseren { get; set; }
        [IOElement("", BitmappedItemTypeEnum.Uitgang, "Naam")]
        public BitmapCoordinatenDataModel BitmapData { get; set; }

        [XmlArrayItem(ElementName = "FileDetector")]
        public List<FileIngreepDetectorModel> FileDetectoren { get; set; }

        [XmlArrayItem(ElementName = "TeDoserenSignaalGroep")]
        public List<FileIngreepTeDoserenSignaalGroepModel> TeDoserenSignaalGroepen { get; set; }

        public FileIngreepModel()
        {
            FileDetectoren = new List<FileIngreepDetectorModel>();
            TeDoserenSignaalGroepen = new List<FileIngreepTeDoserenSignaalGroepModel>();
            BitmapData = new BitmapCoordinatenDataModel();
        }
    }
}
