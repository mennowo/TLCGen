using System;
using System.ComponentModel;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class PeriodeModel : IHaveName
    {
        #region Properties

	    [RefersTo(TLCGenObjectTypeEnum.Periode)]
        [HasDefault(false)]
        [ModelName(TLCGenObjectTypeEnum.Periode)]
        public string Naam { get; set; }
        public PeriodeTypeEnum Type { get; set; }
        public PeriodeDagCodeEnum DagCode { get; set; }
        [XmlIgnore]
        public TimeSpan StartTijd { get; set; }
        [XmlIgnore]
        public TimeSpan EindTijd { get; set; }
        [HasDefault(false)]
        [RefersTo(TLCGenObjectTypeEnum.GroenTijdenSet)]
        public string GroentijdenSet { get; set; }
        [HasDefault(false)]
        public string Commentaar { get; set; }
        public bool GeenUitgangPerOverig { get; set; }

        [Browsable(false)]
        [IOElement("per", BitmappedItemTypeEnum.Uitgang, "BitmapNaam", "BitmapDataRelevant")]
        public BitmapCoordinatenDataModel BitmapData { get; set; }

        [Browsable(false)] [HasDefault(false)] public TLCGenObjectTypeEnum ObjectType => TLCGenObjectTypeEnum.Periode;

        [XmlIgnore]
        [Browsable(false)]
        public bool BitmapDataRelevant { get; set; }
        [XmlIgnore]
        [Browsable(false)]
        [HasDefault(false)]
        public string BitmapNaam { get; set; }

        #endregion // Properties

        #region Serialization

        // Properties for serialization
        [XmlElement("StartTijd")]
        public string SerializedStartTijd
        {
            get
            {
                var hours = StartTijd.Hours;
                if(StartTijd.Days == 1)
                {
                    hours = 24;
                }
                return hours.ToString("00") + ":" + StartTijd.Minutes.ToString("00"); }
            set
            {
                var parts = value.Split(':');
                if (parts.Length != 2)
                    throw new IndexOutOfRangeException("SerializedStartTijd must have two parts seperated by a colon");
                var hours = Int32.Parse(parts[0]);
                var days = 0;
                if (hours == 24)
                {
                    hours = 0;
                    days = 1;
                }
                StartTijd = new TimeSpan(days, hours, Int32.Parse(parts[1]), 0);
            }
        }
        
        [XmlElement("EindTijd")]
        public string SerializedEindTijd
        {
            get
            {
                var hours = EindTijd.Hours;
                if (EindTijd.Days == 1)
                {
                    hours = 24;
                }
                return hours.ToString("00") + ":" + EindTijd.Minutes.ToString("00");
            }
            set
            {
                var parts = value.Split(':');
                if (parts.Length != 2)
					throw new IndexOutOfRangeException("SerializedEindTijd must have two parts seperated by a colon");
				var hours = Int32.Parse(parts[0]);
                var days = 0;
                if (hours == 24)
                {
                    hours = 0;
                    days = 1;
                }
                EindTijd = new TimeSpan(days, hours, Int32.Parse(parts[1]), 0);
            }
        }

        public bool ShouldSerializeBitmapData()
        {
            return BitmapData?.BitmapCoordinaten?.Count > 0;
        }

        #endregion // Serialization

        #region Constructor

        public PeriodeModel()
        {
            StartTijd = new TimeSpan();
            EindTijd = new TimeSpan();
            BitmapData = new BitmapCoordinatenDataModel();
        }
        
        #endregion // Constructor
    }
}
