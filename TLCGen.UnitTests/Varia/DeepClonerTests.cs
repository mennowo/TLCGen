using NUnit.Framework;
using TLCGen.Helpers;

namespace TLCGen.UnitTests.Varia
{
    [TestFixture]
    class DeepClonerTests
    {
        [Test]
        public void DeepClonerDeepClone_ExecutedForSimpleController_SuccesfullyClones()
        {
            var controller = ControllerCreator.GetSmallControllerWithDetection();
            
            Assert.DoesNotThrow(() => DeepCloner.DeepClone(controller), "Cloning ControllerModel failed");
        }
    }
}
