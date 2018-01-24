using System;

using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class InterSignaalGroepChangedMessage
    {
        public string FaseVan { get; }
        public string FaseNaar { get; }

        public bool IsCoupled { get; }

        public ConflictModel Conflict { get; }
        public NaloopModel Naloop { get; }
        public GelijkstartModel Gelijkstart { get; }
        public VoorstartModel Voorstart { get; }
        public MeeaanvraagModel Meeaanvraag { get; }
        public LateReleaseModel LateRelease { get; }

        public InterSignaalGroepChangedMessage(string fasevan, string fasenaar, object synchronisatieobject, bool iscoupled = false)
        {
            var t = synchronisatieobject.GetType();
            if (t == typeof(ConflictModel)) Conflict = (ConflictModel)synchronisatieobject;
            if (t == typeof(NaloopModel)) Naloop = (NaloopModel)synchronisatieobject;
            if (t == typeof(GelijkstartModel)) Gelijkstart = (GelijkstartModel)synchronisatieobject;
            if (t == typeof(VoorstartModel)) Voorstart = (VoorstartModel)synchronisatieobject;
            if (t == typeof(MeeaanvraagModel)) Meeaanvraag = (MeeaanvraagModel)synchronisatieobject;
            if (t == typeof(LateReleaseModel)) LateRelease = (LateReleaseModel)synchronisatieobject;
            FaseVan = fasevan;
            FaseNaar = fasenaar;
            IsCoupled = iscoupled;
        }
    }
}