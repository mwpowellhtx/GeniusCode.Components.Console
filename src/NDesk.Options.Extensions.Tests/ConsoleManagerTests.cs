using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NDesk.Options.Extensions.Tests
{
    /// <summary>
    /// ConsoleManager tests.
    /// </summary>
    [TestClass]
    public class ConsoleManagerTests
    {
        /// <summary>
        /// Should respond to required-but-not-present-options with show-help.
        /// </summary>
        [TestMethod]
        public void Should_Respond_To_Required_Options_With_Show_Help()
        {
            var optionSet = new RequiredValuesOptionSet();

            //Populate the OptionSet with some nominal options.
            var name = optionSet.AddRequiredVariable<string>("n", "");
            var age = optionSet.AddRequiredVariable<int>("a", "");
            var age2 = optionSet.AddRequiredVariable<int>("b", "");
            var age3 = optionSet.AddRequiredVariable<int>("c", "");

            const string ConsoleName = "Test";

            var cm = new ConsoleManager(ConsoleName, optionSet);

            using (var writer = new StringWriter())
            {
                var parsed = cm.TryParseOrShowHelp(writer, new string[] {});

                Assert.IsFalse(parsed);

                Assert.IsTrue(writer.ToString().Contains(ConsoleName + ": error parsing arguments:"));
            }
        }

        /// <summary>
        /// Should respond to help-mode.
        /// </summary>
        [TestMethod]
        public void Should_Respond_To_Help_Arg()
        {
            var optionSet = new RequiredValuesOptionSet();

            //Add a non-required variable because we want to verify the help-arg.
            var name = optionSet.AddVariable<string>("n", "");

            const string HelpPrototype = "?";
            const string Description = "TESTMODE";

            var cm = new ConsoleManager("Test", optionSet, HelpPrototype, Description);

            using (var writer = new StringWriter())
            {
                var parsed = cm.TryParseOrShowHelp(writer, new[] {HelpPrototype,});

                Assert.IsFalse(parsed);

                Assert.IsTrue(writer.ToString().Contains(Description));
            }
        }

        /// <summary>
        /// Should show-help for remaining-args.
        /// </summary>
        [TestMethod]
        public void Show_Show_Help_For_Remaining_Args()
        {
            var optionSet = new RequiredValuesOptionSet();

            //This one can be a required-variable, no problem, but that's it.
            var name = optionSet.AddRequiredVariable<string>("n");

            const string ConsoleName = "Test";

            var cm = new ConsoleManager(ConsoleName, optionSet);

            //Then we should have some remaining args.
            var args = "-n ThisIsName UnknownOptionCausesErrorShowHelp".Split(' ');

            using (var writer = new StringWriter())
            {
                var parsed = cm.TryParseOrShowHelp(writer, args);

                Assert.IsFalse(parsed);

                // Test contains Visual Studio error message
                Assert.IsTrue(writer.ToString().Contains(ConsoleName + ": error parsing arguments:"));
            }
        }
    }
}
