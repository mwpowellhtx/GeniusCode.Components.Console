using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NDesk.Options.Extensions
{
    /// <summary>
    /// Variable tests.
    /// </summary>
    [TestClass]
    public class VariableTests
    {
        /// <summary>
        /// Should get simple Variables.
        /// </summary>
        [TestMethod]
        public void Should_Get_Simple_Variables()
        {
            var optionSet = new OptionSet();

            var name = optionSet.AddVariable<string>("n", "");
            var age = optionSet.AddVariable<int>("a", "");

            /* TODO: The splitskies are clever, but are also error prone. We're assuming
             * by this that the string is as it appears at the command line, when this is
             * not the case. This is as the text appears after the command line parser is
             * through presenting the args to the application. */
            var args = "-n FindThisString -a:23".Split(' ');
            optionSet.Parse(args);

            Assert.AreEqual(23, age);
            Assert.AreEqual("FindThisString", name);
        }

        /// <summary>
        /// Should detect required variables.
        /// </summary>
        [TestMethod]
        public void Should_Detect_Required_Variables()
        {
            var optionSet = new RequiredValuesOptionSet();
            var name = optionSet.AddRequiredVariable<string>("n", "");
            var age = optionSet.AddRequiredVariable<int>("a", "");
            var age2 = optionSet.AddRequiredVariable<int>("b", "");
            var age3 = optionSet.AddRequiredVariable<int>("c", "");

            //TODO: Screaming for NUnit-test-case-coverage.
            var args = "-n FindThisString".Split(' ');
            optionSet.Parse(args);

            /* TODO: Might could (should) also verify that each of the missing ones,
             * as well as found ones, are either there are not there. */
            Assert.AreEqual(3, optionSet.GetMissingVariables().Count());

            Assert.AreEqual("FindThisString", name);
        }

        /// <summary>
        /// Should detect required variable lists.
        /// </summary>
        [TestMethod]
        public void Should_Detect_Required_VariableLists()
        {
            var optionSet = new RequiredValuesOptionSet();
            var n = optionSet.AddRequiredVariableList<string>("n", "");
            var a = optionSet.AddRequiredVariableList<int>("a", "");
            var m = optionSet.AddRequiredVariableList<string>("m", "");

            //TODO: Screaming for an NUnit-test-case-coverage.
            var args = "-n FindThisString -n:Findit2 -n:Findit3 -a2 -a3 -a5565 -a:23".Split(' ');

            optionSet.Parse(args);

            Action<IEnumerable<string>> verifyN = x =>
            {
                // ReSharper disable PossibleMultipleEnumeration
                Assert.AreEqual(3, x.Count());
                Assert.IsTrue(x.Contains("FindThisString"));
                Assert.IsTrue(x.Contains("Findit2"));
                Assert.IsTrue(x.Contains("Findit3"));
                // ReSharper restore PossibleMultipleEnumeration
            };

            Action<IEnumerable<int>> verifyA = x =>
            {
                // ReSharper disable PossibleMultipleEnumeration
                Assert.AreEqual(4, x.Count());
                Assert.IsTrue(x.Contains(2));
                Assert.IsTrue(x.Contains(3));
                Assert.IsTrue(x.Contains(5565));
                Assert.IsTrue(x.Contains(23));
                // ReSharper restore PossibleMultipleEnumeration
            };

            Action<IEnumerable<string>> verifyM = x =>
            {
                // ReSharper disable PossibleMultipleEnumeration
                Assert.AreEqual(0, x.Count());
                Assert.AreEqual(1, optionSet.GetMissingVariables().Count());
                // ReSharper restore PossibleMultipleEnumeration
            };

            verifyA(a);
            verifyA(a.Values);

            verifyN(n);
            verifyN(n.Values);

            verifyM(m);
            verifyM(m.Values);
        }

        /// <summary>
        /// Should detect switches.
        /// </summary>
        [TestMethod]
        public void Should_Detect_Switches()
        {
            var optionSet = new OptionSet();

            var n = optionSet.AddSwitch("n", "");
            var a = optionSet.AddSwitch("a", "");
            var b = optionSet.AddSwitch("b", "");

            var args = "-n -a".Split(' ');
            optionSet.Parse(args);

            //Actual, expected.
            Action<bool, bool> verify = Assert.AreEqual;

            verify(n, true);
            verify(n.Enabled, true);

            verify(a, true);
            verify(a.Enabled, true);

            verify(b, false);
            verify(b.Enabled, false);
        }

        /// <summary>
        /// Should not throw exception when variable is set multiple times.
        /// </summary>
        [TestMethod]
        public void Should_Not_Throw_Exception_Multiset_Variable()
        {
            var optionSet = new OptionSet();
            var n = optionSet.AddVariable<string>("n", "");

            //TODO: Screaming for an NUnit-test-case-coverage.
            var args = "-n:Noah -n:Moses -n:David".Split(' ');

            try
            {
                optionSet.Parse(args);
            }
            catch (OptionException oex)
            {
                Assert.Fail("Unexpected exception: {0}", oex);
            }
        }

        /// <summary>
        /// Should process VariableLists.
        /// </summary>
        [TestMethod]
        public void Should_Process_VariableLists()
        {
            var optionSet = new OptionSet();

            var n = optionSet.AddVariableList<string>("n", "");
            var a = optionSet.AddVariableList<int>("a", "");

            //TODO: Screaming for an NUnit-test-case-coverage.
            var args = "-n FindThisString -n:Findit2 -n:Findi3 -a2 -a3 -a5565 -a:23".Split(' ');

            optionSet.Parse(args);

            Action<IEnumerable<int>> verifyA = x =>
            {
// ReSharper disable PossibleMultipleEnumeration
                Assert.AreEqual(4, x.Count());
                Assert.IsTrue(x.Contains(23));
// ReSharper restore PossibleMultipleEnumeration
            };

            Action<IEnumerable<string>> verifyN = x =>
            {
// ReSharper disable PossibleMultipleEnumeration
                Assert.AreEqual(3, x.Count());
                Assert.IsTrue(x.Contains("FindThisString"));
// ReSharper restore PossibleMultipleEnumeration
            };

            verifyA(a);
            verifyA(a.Values);

            verifyN(n);
            verifyN(n.Values);
        }

        /// <summary>
        /// Should process VariableMatrices.
        /// </summary>
        [TestMethod]
        public void Should_Process_Matrices()
        {
            var optionSet = new OptionSet();

            var n = optionSet.AddVariableMatrix<string>("n", "");

            /* Specify the args as an array instead of the splitskies, in particular
             * on account of the Message= use case. Actually, at this level, quotes
             * should not enter into the mix, because those are command-line beasties. */
            var args = new[]
            {
                "-n:Hello=World",
                "-nColor=Red",
                "-n:Message=Hello With Spaces",
                "-n:Name=Jesus",
                "-nFavNHL:NewJerseyDevils",
            };

            optionSet.Parse(args.Select(a => "-" + a.Trim()).ToArray());

            //This runs dangerously close to testing the Options themselves.
            Action<IDictionary<string, string>> verify = x =>
            {
                Assert.AreEqual(3, x.Count());
                Assert.IsTrue(x.ContainsKey("Name"));
                Assert.IsTrue(x.ContainsKey("Hello"));
                Assert.IsTrue(x.ContainsKey("Message"));
                Assert.IsFalse(x.ContainsKey("Color"));
                Assert.IsFalse(x.ContainsKey("FavNHL"));
                Assert.AreEqual(x["Name"], "Jesus");
                Assert.AreEqual(x["Hello"], "World");
            };

            verify(n);
            verify(n.Matrix);
        }
    }
}
