using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class InterSignaalGroepChangedMessage
    {
        public string FaseVan { get; private set; }
        public string FaseNaar { get; private set; }

        public bool IsCoupled { get; private set; }

        public ConflictModel Conflict { get; private set; }
        public NaloopModel Naloop { get; private set; }
        public GelijkstartModel Gelijkstart { get; private set; }
        public VoorstartModel Voorstart { get; private set; }

        public InterSignaalGroepChangedMessage(string fasevan, string fasenaar, object synchronisatieobject, bool iscoupled = false)
        {
            Type t = synchronisatieobject.GetType();
            if (t == typeof(ConflictModel)) Conflict = (ConflictModel)synchronisatieobject;
            if (t == typeof(NaloopModel)) Naloop = (NaloopModel)synchronisatieobject;
            if (t == typeof(GelijkstartModel)) Gelijkstart = (GelijkstartModel)synchronisatieobject;
            if (t == typeof(VoorstartModel)) Voorstart = (VoorstartModel)synchronisatieobject;
            FaseVan = fasevan;
            FaseNaar = fasenaar;
            IsCoupled = iscoupled;
        }
    }
}