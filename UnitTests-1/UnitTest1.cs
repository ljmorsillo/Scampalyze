using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ircda.Scampalyze;

namespace UnitTests_1
{
    [TestClass]
    public class UnitTest1
    {
    
        [TestMethod]
        public void TestNoArguments()
        {
            using (StringWriter sw = new StringWriter())
            {
                System.Console.SetOut(sw);
                System.Console.SetError(sw);
                //ConsoleUser cu = new ConsoleUser();
                string[] args = new string[1];
                ScampalyzeConsole scamperTest = new ScampalyzeConsole(args);
                scamperTest.Usage(args);
                sw.Flush();
                string expected = "You might want to try some parameters.";
                //Assert.Equals<string>(expected, sw.ToString());
                Assert.IsTrue(sw.ToString().Contains(expected) && sw.ToString().Length > 0,"Empty Argument List - incorrect string");
            }

        }
    }
}
