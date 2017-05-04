using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TLCGen.Plugins
{
    public interface ITLCGenXMLNodeWriter : ITLCGenPlugin
    {
        void GetXmlFromDocument(XmlDocument document);
        void SetXmlInDocument(XmlDocument document);
    }
}
