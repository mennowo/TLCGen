﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Plugins.DynamischHiaat.Models
{
    [Serializable]
    public class DynamischHiaatSignalGroupModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string SignalGroupName { get; set; }
        public bool HasDynamischHiaat { get; set; }
        public bool Opdrempelen { get; set; }

        public string Snelheid { get; set; }
        public bool KijkenNaarKoplus { get; set; }

        [XmlArray(ElementName = "DynamischHiaatDetector")]
        public List<DynamischHiaatDetectorModel> DynamischHiaatDetectoren { get; set; }

        public DynamischHiaatSignalGroupModel()
        {
            DynamischHiaatDetectoren = new List<DynamischHiaatDetectorModel>();
        }
    }
}
