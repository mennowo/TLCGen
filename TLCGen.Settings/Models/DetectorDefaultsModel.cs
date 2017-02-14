using System;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Settings
{
    [Serializable]
    public class DetectorDefaultsModel
    {
        public DetectorTypeEnum Type { get; set; }
        public DetectorModel Detector { get; set; }
        public RoBuGroverFileDetectorModel RoBuGroverFileDetector { get; set; }
        public RoBuGroverHiaatDetectorModel RoBuGroverHiaatDetector { get; set; }

        public object GetModel(string type)
        {
            switch (type)
            {
                case "DetectorModel": return Detector;
                case "RoBuGroverFileDetectorModel": return RoBuGroverFileDetector;
                case "RoBuGroverHiaatDetectorModel": return RoBuGroverHiaatDetector;
            }
            return null;
        }
    }
}