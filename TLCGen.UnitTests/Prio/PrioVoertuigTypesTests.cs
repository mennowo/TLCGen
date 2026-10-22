using NUnit.Framework;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.UnitTests.Prio
{
    [TestFixture]
    public class PrioVoertuigTypesTests
    {
        [TestCase(FaseTypeEnum.Fiets)]
        [TestCase(FaseTypeEnum.Voetganger)]
        public void GetAllowed_LangzaamVerkeer_ExcludesHulpdienst(FaseTypeEnum faseType)
        {
            var allowed = PrioVoertuigTypes.GetAllowed(faseType);

            Assert.That(allowed, Does.Not.Contain(PrioIngreepVoertuigTypeEnum.Hulpdienst));
            Assert.That(allowed, Does.Contain(PrioIngreepVoertuigTypeEnum.Fiets));
        }

        [TestCase(FaseTypeEnum.Auto)]
        [TestCase(FaseTypeEnum.OV)]
        public void GetAllowed_OtherFaseTypes_AllowsHulpdienst(FaseTypeEnum faseType)
        {
            Assert.That(PrioVoertuigTypes.GetAllowed(faseType), Does.Contain(PrioIngreepVoertuigTypeEnum.Hulpdienst));
        }
    }
}
