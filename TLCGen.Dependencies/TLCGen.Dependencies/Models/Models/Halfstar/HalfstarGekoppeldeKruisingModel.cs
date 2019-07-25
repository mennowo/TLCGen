using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    public interface IHaveKoppelSignalen
    {
        List<KoppelSignaalModel> UpdateKoppelSignalen();
    }

    [Serializable]
    [HasKoppelSignalen]
    public class HalfstarGekoppeldeKruisingModel : IHaveKoppelSignalen
    {
        #region Fields

        private HalfstarGekoppeldTypeEnum _type;

        #endregion // Fields

        #region Properties

        [HasDefault(false)]
        public string KruisingNaam { get; set; }

        public HalfstarGekoppeldTypeEnum Type
        {
            get => _type;
            set
            {
                _type = value;
                foreach (var plu in PlanUitgangen)
                {
                    plu.Type = _type;
                }
                foreach (var pli in PlanIngangen)
                {
                    pli.Type = _type;
                }
            }
        }

        public HalfstarGekoppeldWijzeEnum KoppelWijze { get; set; }
        [HasDefault(false)]
        public string PTPKruising { get; set; }

        [Browsable(false)]
        public bool IsSlave => Type == HalfstarGekoppeldTypeEnum.Slave;

        [Browsable(false)]
        public bool IsMaster => Type == HalfstarGekoppeldTypeEnum.Master;

        [Browsable(false)]
        [IOElement("inleven", BitmappedItemTypeEnum.Uitgang, "KruisingNaam")]
        public BitmapCoordinatenDataModel InLeven { get; set; }

        [Browsable(false)]
        [IOElement("inkp", BitmappedItemTypeEnum.Uitgang, "KruisingNaam", "IsMaster")]
        public BitmapCoordinatenDataModel InKoppelpuls { get; set; }

        [Browsable(false)]
        [IOElement("inpervar", BitmappedItemTypeEnum.Uitgang, "KruisingNaam", "IsMaster")]
        public BitmapCoordinatenDataModel InPeriodeVARegelen { get; set; }

        [Browsable(false)]
        [IOElement("inperarh", BitmappedItemTypeEnum.Uitgang, "KruisingNaam", "IsMaster")]
        public BitmapCoordinatenDataModel InPeriodenAlternatievenHoofdrichtingen { get; set; }

        [Browsable(false)]
        [IOElement("insyncok", BitmappedItemTypeEnum.Uitgang, "KruisingNaam", "IsSlave")]
        public BitmapCoordinatenDataModel InSynchronisatieOk { get; set; }

        [Browsable(false)]
        [IOElement("intxsok", BitmappedItemTypeEnum.Uitgang, "KruisingNaam", "IsSlave")]
        public BitmapCoordinatenDataModel InTxsOk { get; set; }

        [Browsable(false)]
        [IOElement("uitleven", BitmappedItemTypeEnum.Uitgang, "KruisingNaam")]
        public BitmapCoordinatenDataModel UitLeven { get; set; }

        [Browsable(false)]
        [IOElement("uitsyncok", BitmappedItemTypeEnum.Uitgang, "KruisingNaam", "IsMaster")]
        public BitmapCoordinatenDataModel UitSynchronisatieOk { get; set; }

        [Browsable(false)]
        [IOElement("uittxsok", BitmappedItemTypeEnum.Uitgang, "KruisingNaam", "IsMaster")]
        public BitmapCoordinatenDataModel UitTxsOk { get; set; }

        [Browsable(false)]
        [IOElement("uitkp", BitmappedItemTypeEnum.Uitgang, "KruisingNaam", "IsSlave")]
        public BitmapCoordinatenDataModel UitKoppelpuls { get; set; }

        [Browsable(false)]
        [IOElement("uitpervar", BitmappedItemTypeEnum.Uitgang, "KruisingNaam", "IsSlave")]
        public BitmapCoordinatenDataModel UitPeriodeVARegelen { get; set; }

        [Browsable(false)]
        [IOElement("uitperarh", BitmappedItemTypeEnum.Uitgang, "KruisingNaam", "IsSlave")]
        public BitmapCoordinatenDataModel UitPeriodenAlternatievenHoofdrichtingen { get; set; }

        [Browsable(false)]
        public List<HalfstarGekoppeldeKruisingPlanUitgangModel> PlanUitgangen { get; set; }

        [Browsable(false)]
        public List<HalfstarGekoppeldeKruisingPlanIngangModel> PlanIngangen { get; set; }

        [Browsable(false)]
        public List<KoppelSignaalModel> KoppelSignalen  { get; set; }

        #endregion // Properties

        #region Constructor

        public HalfstarGekoppeldeKruisingModel()
		{
			InLeven = new BitmapCoordinatenDataModel();
			InKoppelpuls = new BitmapCoordinatenDataModel();
			InPeriodeVARegelen = new BitmapCoordinatenDataModel();
			InPeriodenAlternatievenHoofdrichtingen = new BitmapCoordinatenDataModel();
			InSynchronisatieOk = new BitmapCoordinatenDataModel();
			InTxsOk = new BitmapCoordinatenDataModel();
			UitLeven = new BitmapCoordinatenDataModel();
			UitSynchronisatieOk = new BitmapCoordinatenDataModel();
			UitTxsOk = new BitmapCoordinatenDataModel();
			UitKoppelpuls = new BitmapCoordinatenDataModel();
			UitPeriodeVARegelen = new BitmapCoordinatenDataModel();
			UitPeriodenAlternatievenHoofdrichtingen = new BitmapCoordinatenDataModel();
			PlanUitgangen = new List<HalfstarGekoppeldeKruisingPlanUitgangModel>();
			PlanIngangen = new List<HalfstarGekoppeldeKruisingPlanIngangModel>();
            KoppelSignalen = new List<KoppelSignaalModel>();
		}

        #endregion // Constructor

        #region IHaveKoppelSignalen

        public List<KoppelSignaalModel> UpdateKoppelSignalen()
        {
            var signalen = new List<KoppelSignaalModel>();
            signalen.Add(new KoppelSignaalModel { Description = "Leven in", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.In });
            if (Type == HalfstarGekoppeldTypeEnum.Master)
            {
                signalen.Add(new KoppelSignaalModel { Description = "Koppelpuls", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.In });
                signalen.Add(new KoppelSignaalModel { Description = "Periode alt.hoofdr.", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.In });
                signalen.Add(new KoppelSignaalModel { Description = "Periode VA regelen", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.In });
            }
            else if (Type == HalfstarGekoppeldTypeEnum.Slave)
            {
                signalen.Add(new KoppelSignaalModel { Description = "Sync OK", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.In });
                signalen.Add(new KoppelSignaalModel { Description = "TXS OK", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.In });
            }
            foreach (var pl in PlanIngangen)
            {
                signalen.Add(new KoppelSignaalModel { Description = $"{pl.Plan}", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.In });
            }
            signalen.Add(new KoppelSignaalModel { Description = "Leven uit", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.Uit });
            if (Type == HalfstarGekoppeldTypeEnum.Master)
            {
                signalen.Add(new KoppelSignaalModel { Description = "Sync OK", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.Uit });
                signalen.Add(new KoppelSignaalModel { Description = "TXS OK", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.Uit });
            }
            else if (Type == HalfstarGekoppeldTypeEnum.Slave)
            {
                signalen.Add(new KoppelSignaalModel { Description = "Koppelpuls", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.Uit });
                signalen.Add(new KoppelSignaalModel { Description = "Periode alt.hoofdr.", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.Uit });
                signalen.Add(new KoppelSignaalModel { Description = "Periode VA regelen", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.Uit });
            }
            foreach (var pl in PlanIngangen)
            {
                signalen.Add(new KoppelSignaalModel { Description = $"{pl.Plan}", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.Uit });
            }
            foreach(var s in KoppelSignalen)
            {
                var _s = signalen.FirstOrDefault(x => x.Description == s.Description && x.Richting == s.Richting);
                if (_s != null)
                {
                    _s.Koppeling = PTPKruising;
                    _s.Count = s.Count;
                }
            }
            KoppelSignalen = signalen;
            return KoppelSignalen;
        }

        #endregion // IHaveKoppelSignalen
    }
}