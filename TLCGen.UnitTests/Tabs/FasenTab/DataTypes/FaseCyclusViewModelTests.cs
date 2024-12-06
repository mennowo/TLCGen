using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;
using TLCGen.DataAccess;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;
using TLCGen.ViewModels;

namespace TLCGen.UnitTests.Tabs.FasenTab.DataTypes
{
    [TestFixture]
    public class FaseCyclusViewModelTests
    {
        [Test]
        public void FaseCyclusViewModel_NameChanged_NameChangedMessageSent()
        {
            var messengermock = FakesCreator.CreateMessenger();
            Messenger.OverrideDefault(messengermock);
            var model = new ControllerModel();
            TLCGenControllerDataProvider.OverrideDefault(FakesCreator.CreateControllerDataProvider(model));
            TLCGenModelManager.OverrideDefault(new TLCGenModelManager{Controller = model});

            model.Fasen.Add(new FaseCyclusModel { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "05" });
            var vm = new FaseCyclusViewModel(model.Fasen[2])
            {
                Naam = "07"
            };

            messengermock.Received().Send(Arg.Is<NameChangingMessage>(x => x.OldName == "03" && x.NewName == "07"));
        }

        [Test]
        public void FaseCyclusViewModel_NameChanged_DetectorNamesAlsoChanged()
        {
            var model = new ControllerModel();
            Messenger.OverrideDefault(new Messenger());
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            TLCGenControllerDataProvider.OverrideDefault(FakesCreator.CreateControllerDataProvider(model));
            TLCGenModelManager.OverrideDefault(new TLCGenModelManager{Controller = model});

            model.Fasen.Add(new FaseCyclusModel
            {
                Naam = "01",
                Detectoren = new List<DetectorModel>
                {
                    new DetectorModel { Naam = "01_1", VissimNaam = "011"},
                    new DetectorModel { Naam = "01_2", VissimNaam = "012"}
                }
            });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {Naam = "05"};

            Assert.That("05_1" == vm.FaseCyclus.Detectoren[0].Naam);
            Assert.That("051" == vm.FaseCyclus.Detectoren[0].VissimNaam);
            Assert.That("05_2" == vm.FaseCyclus.Detectoren[1].Naam);
            Assert.That("052" == vm.FaseCyclus.Detectoren[1].VissimNaam);

        }

        [Test]
        public void FaseCyclusViewModel_TypeChanged_SetDefaultsOnModelCalled()
        {
            var defaultsprovidermock = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsprovidermock);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = Models.Enumerations.FaseTypeEnum.Auto });
            var vm = new FaseCyclusViewModel(model.Fasen[0])
            {
                Type = Models.Enumerations.FaseTypeEnum.Fiets
            };

            defaultsprovidermock.Received().SetDefaultsOnModel(Arg.Is<FaseCyclusModel>(x => x.Naam == "01"), Arg.Is<string>("Fiets"));
        }

        [Test]
        public void FaseCyclusViewModel_TypeChangedToVoetganger_MeeverlengenTypeSetToDefault()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "31", Type = FaseTypeEnum.Voetganger, MeeverlengenType = MeeVerlengenTypeEnum.Voetganger });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {Type = FaseTypeEnum.Fiets};

            Assert.That(MeeVerlengenTypeEnum.Default == vm.MeeverlengenType);
        }

        [Test]
        public void FaseCyclusViewModel_TFGSetToLowerThanZero_ValueSetToTGG()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TFG = 50, TGG = 40 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TFG = -10};

            Assert.That(40 == vm.TFG);
        }

        [Test]
        public void FaseCyclusViewModel_TFGSetToSmallerThanTGG_ValueSetToTGG()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TFG = 50, TGG = 40 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TFG = 30};

            Assert.That(40 == vm.TFG);
        }

        [Test]
        public void FaseCyclusViewModel_TGGSetToSmallerThanZero_ValueSetToTGGmin()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TFG = 50, TGG = 40, TGG_min = 30 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TGG = 20};

            Assert.That(30 == vm.TGG);
        }

        [Test]
        public void FaseCyclusViewModel_TGGSetToSmallerThanTGGmin_ValuesSetToTGGmin()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TFG = 50, TGG = 40, TGG_min = 30 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TGG = 20};

            Assert.That(30 == vm.TGG);
        }

        [Test]
        public void FaseCyclusViewModel_TGGSetToGreaterThanTFG_TFGSetToTGG()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TFG = 50, TGG = 40, TGG_min = 30 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TGG = 60};

            Assert.That(60 == vm.TFG);
        }

        [Test]
        public void FaseCyclusViewModel_TGGminSetToLowerThanZero_ValueNotChanged()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TFG = 50, TGG = 40, TGG_min = 30 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TGG_min = -10};

            Assert.That(30 == vm.TGG_min);
        }

        [Test]
        public void FaseCyclusViewModel_TGGminSetToGreaterThanTGG_TGGSetToTGGmin()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TFG = 50, TGG = 40, TGG_min = 30 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TGG_min = 45};

            Assert.That(45 == vm.TGG);
        }

        [Test]
        public void FaseCyclusViewModel_TGGminSetToGreaterThanTGGandTFG_TGGandTFGSetToTGGmin()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TFG = 50, TGG = 40, TGG_min = 30 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TGG_min = 60};

            Assert.That(60 == vm.TGG);
            Assert.That(60 == vm.TFG);
        }

        [Test]
        public void FaseCyclusViewModel_TRGminSetToLowerThanZero_ValueSetToTRGmin()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TRG = 20, TRG_min = 10});
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TRG = -10};

            Assert.That(10 == vm.TRG);
        }

        [Test]
        public void FaseCyclusViewModel_TRGminSetToLowerThanTRGmin_ValueSetToTRGmin()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TRG = 20, TRG_min = 10 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TRG = 5};

            Assert.That(10 == vm.TRG);
        }

        [Test]
        public void FaseCyclusViewModel_TRGminSetToLowerThanZero_ValueNotChanged()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TRG = 40, TRG_min = 20 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TRG_min = -10};

            Assert.That(20 == vm.TRG_min);
        }

        [Test]
        public void FaseCyclusViewModel_TRGminSetToGreaterThanTRG_TRGSetToTRGmin()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TRG = 40, TRG_min = 20 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TRG_min = 50};

            Assert.That(50 == vm.TRG);
        }

        [Test]
        public void FaseCyclusViewModel_TGLminSetToLowerThanZero_ValueSetToTGLmin()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TGL = 20, TGL_min = 10 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TGL = -10};

            Assert.That(10 == vm.TGL);
        }

        [Test]
        public void FaseCyclusViewModel_TGLminSetToLowerThanTGLmin_ValueSetToTGLmin()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TGL = 20, TGL_min = 10 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TGL = 5};

            Assert.That(10 == vm.TGL);
        }

        [Test]
        public void FaseCyclusViewModel_TGLminSetToLowerThanZero_ValueNotChanged()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TGL = 40, TGL_min = 20 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TGL_min = -10};

            Assert.That(20 == vm.TGL_min);
        }

        [Test]
        public void FaseCyclusViewModel_TGLminSetToGreaterThanTGL_TGLSetToTGLmin()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, TGL = 40, TGL_min = 20 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {TGL_min = 50};

            Assert.That(50 == vm.TGL);
        }

        [Test]
        public void FaseCyclusViewModel_KopmaxSetToLowerThanZero_ValueNotChanged()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, Kopmax = 80});
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {Kopmax = -10};

            Assert.That(80 == vm.Kopmax);
        }

        [Test]
        public void FaseCyclusViewModel_AantalRijkstrokenSetToLowerThanZero_ValueNotChanged()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, AantalRijstroken = 3 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {AantalRijstroken = -1};

            Assert.That(3 == vm.AantalRijstroken);
        }

        [Test]
        public void FaseCyclusViewModel_HiaatKoplusBijDetectieStoringSetToTrue_VervangendHiaatKoplusSetToDefault()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, AantalRijstroken = 3 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {HiaatKoplusBijDetectieStoring = true};

            Assert.That(25 == vm.VervangendHiaatKoplus);
        }

        [Test]
        public void FaseCyclusViewModel_PercentageGroenBijDetectieStoringSetToTrue_PercentageGroenBijStoringSetToDefault()
        {
            var defaultsproviderstub = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsproviderstub);

            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = FaseTypeEnum.Auto, AantalRijstroken = 3 });
            var vm = new FaseCyclusViewModel(model.Fasen[0]) {PercentageGroenBijDetectieStoring = true};

            Assert.That(65 == vm.PercentageGroenBijStoring);
        }
    }
}
