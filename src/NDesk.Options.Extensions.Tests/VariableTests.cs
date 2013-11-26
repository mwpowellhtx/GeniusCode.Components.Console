using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NDesk.Options.Extensions.Tests
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
            var p = new OptionSet();

            var name = p.AddVariable<string>("n", "");
            var age = p.AddVariable<int>("a", "");

            var myArgs = "-n FindThisString -a:23".Split(' ');
            p.Parse(myArgs);

            Assert.AreEqual(23, age);
            Assert.AreEqual("FindThisString", name);
        }

        /// <summary>
        /// Should detect required variables.
        /// </summary>
        [TestMethod]
        public void Should_Detect_Required_Variables()
        {
            var p = new RequiredValuesOptionSet();
            var name = p.AddRequiredVariable<string>("n", "");
            var age = p.AddRequiredVariable<int>("a", "");
            var age2 = p.AddRequiredVariable<int>("b", "");
            var age3 = p.AddRequiredVariable<int>("c", "");

            //TODO: Screaming for NUnit-test-case-coverage.
            var args = "-n FindThisString".Split(' ');
            p.Parse(args);

            Assert.AreEqual(3, p.GetMissingVariables().Count());

            Assert.AreEqual("FindThisString", name);
        }

        /// <summary>
        /// Should detect switches.
        /// </summary>
        [TestMethod]
        public void Should_Detect_Switches()
        {
            var p = new OptionSet();

            var n = p.AddSwitch("n", "");
            var a = p.AddSwitch("a", "");
            var b = p.AddSwitch("b", "");

            var myArgs = "-n -a".Split(' ');
            p.Parse(myArgs);

            Assert.IsTrue(n);
            Assert.IsTrue(a);
            Assert.IsFalse(b);
        }

        /// <summary>
        /// Should not throw exception when variable is set multiple times.
        /// </summary>
        [TestMethod]
        public void Should_Not_Throw_Exception_Multiset_Variable()
        {
            var p = new OptionSet();
            var n = p.AddVariable<string>("n", "");

            //TODO: Screaming for an NUnit-test-case-coverage.
            var args = "-n:Noah -n:Moses -n:David".Split(' ');

            try
            {
                p.Parse(args);
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
        public void Should_Process_Variablelists()
        {
            var p = new OptionSet();

            var n = p.AddVariableList<string>("n", "");
            var a = p.AddVariableList<int>("a", "");

            //TODO: Screaming for an NUnit-test-case-coverage.
            var args = "-n FindThisString -n:Findit2 -n:Findi3 -a2 -a3 -a5565 -a:23".Split(' ');

            p.Parse(args);

            Assert.AreEqual(3, n.Values.Count());
            Assert.AreEqual(4, a.Values.Count());

            Assert.IsTrue(n.Values.Contains("FindThisString"));
            Assert.IsTrue(a.Values.Contains(23));
        }

        /// <summary>
        /// Should process VariableMatrices.
        /// </summary>
        [TestMethod]
        public void Should_Process_Matrices()
        {
            var p = new OptionSet();

            var n = p.AddVariableMatrix<string>("n", "");

            var args = "-n:Hello=World -n:Color=Red \"-n:Message=Hello With Spaces\" -nName=Jesus -nFavNHL:PittsburghPenguins".Split('-');
            p.Parse(args.Select(a => "-" + a.Trim()).ToArray());

            Assert.AreEqual(5, n.Matrix.Count());
            Assert.IsTrue(n.Matrix.ContainsKey("Hello"));
            Assert.IsTrue(n.Matrix.ContainsKey("Color"));
            Assert.IsTrue(n.Matrix.ContainsKey("Message"));
            Assert.AreEqual("Jesus", n.Matrix["Name"]);
            Assert.AreEqual("PittsburghPenguins", n.Matrix["FavNHL"]);
        }
    }
}
