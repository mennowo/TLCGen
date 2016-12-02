using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Messaging.Requests
{
    public enum ElementIdentifierType
    {
        Define,
        Naam,
        VissimNaam
    }

    public class IsElementIdentifierUniqueRequest
    {
        public bool Handled { get; set; }
        public bool IsUnique { get; set; }
        public string Identifier { get; private set; }
        public ElementIdentifierType Type { get; private set; }

        public IsElementIdentifierUniqueRequest(string identifier, ElementIdentifierType type)
        {
            Handled = false;
            IsUnique = false;
            Identifier = identifier;
            Type = type;
        }
    }
}
