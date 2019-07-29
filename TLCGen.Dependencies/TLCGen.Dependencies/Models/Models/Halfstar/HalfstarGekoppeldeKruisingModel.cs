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
            var id = 1;
            signalen.Add(new KoppelSignaalModel { Name = $"{KruisingNaam}leven", Description = "Leven in", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.In, Id = id++ });
            if (Type == HalfstarGekoppeldTypeEnum.Master)
            {
                signalen.Add(new KoppelSignaalModel { Name = $"{KruisingNaam}kpuls", Description = "Koppelpuls", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.In, Id = id++ });
                signalen.Add(new KoppelSignaalModel { Name = $"{KruisingNaam}perarh", Description = "Periode alt.hoofdr.", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.In, Id = id++ });
                signalen.Add(new KoppelSignaalModel { Name = $"{KruisingNaam}pervar", Description = "Periode VA regelen", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.In, Id = id++ });
            }
            else if (Type == HalfstarGekoppeldTypeEnum.Slave)
            {
                signalen.Add(new KoppelSignaalModel { Name = $"{KruisingNaam}syncok", Description = "Sync OK", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.In, Id = id++ });
                signalen.Add(new KoppelSignaalModel { Name = $"{KruisingNaam}txsok", Description = "TXS OK", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.In, Id = id++ });
            }
            foreach (var pl in PlanIngangen)
            {
                signalen.Add(new KoppelSignaalModel { Name = $"{KruisingNaam}pl{pl.Plan}", Description = $"{pl.Plan}", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.In, Id = id++ });
            }
            signalen.Add(new KoppelSignaalModel { Name = $"{KruisingNaam}leven", Description = "Leven uit", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.Uit, Id = id++ });
            if (Type == HalfstarGekoppeldTypeEnum.Master)
            {
                signalen.Add(new KoppelSignaalModel { Name = $"{KruisingNaam}syncok", Description = "Sync OK", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.Uit, Id = id++ });
                signalen.Add(new KoppelSignaalModel { Name = $"{KruisingNaam}txsok", Description = "TXS OK", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.Uit, Id = id++ });
            }
            else if (Type == HalfstarGekoppeldTypeEnum.Slave)
            {
                signalen.Add(new KoppelSignaalModel { Name = $"{KruisingNaam}kpuls", Description = "Koppelpuls", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.Uit, Id = id++ });
                signalen.Add(new KoppelSignaalModel { Name = $"{KruisingNaam}perarh", Description = "Periode alt.hoofdr.", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.Uit, Id = id++ });
                signalen.Add(new KoppelSignaalModel { Name = $"{KruisingNaam}pervar", Description = "Periode VA regelen", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.Uit, Id = id++ });
            }
            foreach (var pl in PlanIngangen)
            {
                signalen.Add(new KoppelSignaalModel { Name = $"{KruisingNaam}pl{pl.Plan}", Description = $"{pl.Plan}", Koppeling = PTPKruising, Richting = KoppelSignaalRichtingEnum.Uit, Id = id++ });
            }
            foreach (var s in signalen)
            {
                s.Count = 0;
                s.Koppeling = PTPKruising;
            }
            foreach (var s in KoppelSignalen)
            {
                var ns = signalen.FirstOrDefault(x => x.Id != 0 && x.Id == s.Id);
                if (ns != null)
                {
                    ns.Count = s.Count;
                }
            }
            KoppelSignalen = signalen;
            return KoppelSignalen;
        }

        #endregion // IHaveKoppelSignalen
    }
}