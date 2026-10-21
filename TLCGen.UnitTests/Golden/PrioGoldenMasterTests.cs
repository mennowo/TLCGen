using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using NSubstitute;
using NUnit.Framework;
using TLCGen.Dependencies.Providers;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Settings;

namespace TLCGen.UnitTests.Golden
{
    /// <summary>
    /// Golden-master tests: generate CCOL code for a corpus of controllers and compare it with stored baselines.
    /// They guard refactorings of the prio code generation, which must not change the generated code.
    /// <para>
    /// Baselines live in Golden/Baselines/&lt;case&gt; next to this file. Only the files named after the controller
    /// are compared (reg.c, tab.c, sys.h, prio.c, the .add files, ...); copied library sources are not.
    /// When a baseline is missing it is written and the test is inconclusive.
    /// To update baselines after an intended change, run the tests with the environment variable
    /// TLCGEN_UPDATE_GOLDEN=1, then review the diff with git before committing.
    /// </para>
    /// The output of the last run is kept in %TEMP%\TLCGen_golden\&lt;case&gt; for inspection.
    /// </summary>
    [SingleThreaded]
    [TestFixture]
    public class PrioGoldenMasterTests
    {
        private const string UpdateVariable = "TLCGEN_UPDATE_GOLDEN";

        // a real controller that is only available locally; its baseline is kept next to it
        private const string LocalControllerFile = @"C:\temp\tlcgen\123456_test\123456.tlc";

        private ITLCGenDialogProvider _dialogProvider;
        private List<string> _dialogMessages;

        public static IEnumerable<string> SyntheticCases => PrioGoldenCorpus.Cases.Keys;

        private static string BaselinesRoot([CallerFilePath] string thisFile = "") =>
            Path.Combine(Path.GetDirectoryName(thisFile)!, "Baselines");

        [SetUp]
        public void SetUp()
        {
            // per test: the generator is shared, and other tests may load the user's settings into it
            CCOLTestGenerator.LoadDefaultSettings();

            // the generator uses DefaultsProvider for melding short codes: other tests may have left a fake
            DefaultsProvider.OverrideDefault(new DefaultsProvider());

            // the generator reports exceptions via a message box: capture them instead
            _dialogMessages = new List<string>();
            _dialogProvider = TLCGenDialogProvider.Default;
            var dialogs = Substitute.For<ITLCGenDialogProvider>();
            dialogs.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<MessageBoxButton>())
                .Returns(x =>
                {
                    _dialogMessages.Add(x.ArgAt<string>(0));
                    return MessageBoxResult.OK;
                });
            TLCGenDialogProvider.Default = dialogs;
        }

        [TearDown]
        public void TearDown()
        {
            TLCGenDialogProvider.Default = _dialogProvider;
        }

        [TestCaseSource(nameof(SyntheticCases))]
        public void SyntheticController_GeneratedCode_MatchesBaseline(string name)
        {
            var c = PrioGoldenCorpus.Cases[name]();

            AssertMatchesBaseline(c, name, Path.Combine(BaselinesRoot(), name));
        }

        [Test]
        public void LocalController_GeneratedCode_MatchesBaseline()
        {
            if (!File.Exists(LocalControllerFile))
            {
                Assert.Ignore($"{LocalControllerFile} not found");
            }

            var c = TLCGenSerialization.DeSerialize<ControllerModel>(LocalControllerFile);

            AssertMatchesBaseline(c, "local_" + Path.GetFileNameWithoutExtension(LocalControllerFile),
                Path.Combine(Path.GetDirectoryName(LocalControllerFile)!, "golden-baseline"));
        }

        private void AssertMatchesBaseline(ControllerModel c, string name, string baselineDir)
        {
            var outputDir = Path.Combine(Path.GetTempPath(), "TLCGen_golden", name);
            if (Directory.Exists(outputDir)) Directory.Delete(outputDir, true);
            Directory.CreateDirectory(outputDir);

            CCOLTestGenerator.Plugin.Generator.GenerateSourceFiles(c, outputDir);

            Assert.That(_dialogMessages, Is.Empty, "Generation reported an error");

            var actual = ReadGeneratedFiles(outputDir, c.Data.Naam);
            Assert.That(actual, Is.Not.Empty, "Nothing was generated");

            var update = Environment.GetEnvironmentVariable(UpdateVariable) == "1";
            if (update || !Directory.Exists(baselineDir))
            {
                var existed = Directory.Exists(baselineDir);
                if (existed) Directory.Delete(baselineDir, true);
                Directory.CreateDirectory(baselineDir);
                foreach (var (file, content) in actual)
                {
                    File.WriteAllText(Path.Combine(baselineDir, file), content, new UTF8Encoding(false));
                }
                if (update) Assert.Warn($"Baseline {(existed ? "updated" : "created")}: {baselineDir}");
                else Assert.Inconclusive($"Baseline created: {baselineDir}. Review and commit it.");
                return;
            }

            var expected = ReadGeneratedFiles(baselineDir, c.Data.Naam);
            var differences = new List<string>();
            foreach (var file in expected.Keys.Union(actual.Keys).OrderBy(x => x, StringComparer.Ordinal))
            {
                if (!actual.TryGetValue(file, out var a)) differences.Add($"{file}: not generated");
                else if (!expected.TryGetValue(file, out var e)) differences.Add($"{file}: generated, but not in baseline");
                else if (a != e) differences.Add($"{file}: {DescribeFirstDifference(e, a)}");
            }

            Assert.That(differences, Is.Empty,
                $"Generated code differs from the baseline.{Environment.NewLine}" +
                $"Baseline: {baselineDir}{Environment.NewLine}" +
                $"Actual:   {outputDir}{Environment.NewLine}" +
                $"If the change is intended, rerun with {UpdateVariable}=1.");
        }

        private static Dictionary<string, string> ReadGeneratedFiles(string dir, string controllerName)
        {
            // files are compared with normalized line endings: git may convert them on checkout
            return Directory.EnumerateFiles(dir)
                .Where(x => Path.GetFileName(x).StartsWith(controllerName, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(Path.GetFileName, x => File.ReadAllText(x).Replace("\r\n", "\n"));
        }

        private static string DescribeFirstDifference(string expected, string actual)
        {
            var e = expected.Split('\n');
            var a = actual.Split('\n');
            for (var i = 0; i < Math.Max(e.Length, a.Length); i++)
            {
                var el = i < e.Length ? e[i] : "<end of file>";
                var al = i < a.Length ? a[i] : "<end of file>";
                if (el != al) return $"line {i + 1}{Environment.NewLine}    expected: {el}{Environment.NewLine}    actual:   {al}";
            }
            return "differs";
        }
    }
}
