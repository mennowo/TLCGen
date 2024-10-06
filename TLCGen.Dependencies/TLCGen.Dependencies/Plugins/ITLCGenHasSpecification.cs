using System;
using System.Collections.Generic;
using TLCGen.Models;

namespace TLCGen.Plugins
{
    public enum SpecificationParagraphType
    {
        Body, Header1, Header2, TableHeader, Spacer
    }

    public enum SpecificationSubject
    {
        Intro,
        Structure,
        DynHiaat,
        AFM,
        GebruikersPlugin1,
        GebruikersPlugin2
    }

    public abstract class SpecificationDataBase
    {
    }

    public class SpecificationParagraph : SpecificationDataBase
    {
        // Body / Header1 / Header2 / etc <- dit is domeinspecifiek
        public SpecificationParagraphType Type { get; set; }
        // De tekst, al dan niet geformat
        public string Text { get; set; }
    }

    public class SpecificationTable : SpecificationDataBase
    {
        public List<List<string>> TableData { get; set; }
    }

    public class SpecificationTable2 : SpecificationDataBase
    {
        public List<List<string>> TableData2 { get; set; }
    }

    public class SpecificationBulletList : SpecificationDataBase
    {
        public List<Tuple<string, int>> BulletData { get; set; }
    }

    public class SpecificationData
    {
        public SpecificationSubject Subject { get; set; }

        public List<SpecificationDataBase> Elements { get; set; }

        public SpecificationData()
        {
            Elements = new();
        }
    }

    public interface ITLCGenHasSpecification
    {
        SpecificationData GetSpecificationData(ControllerModel c);
    }
}