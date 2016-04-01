using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
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
        [TestMethod]
        public void TestCountFroForm()
        {
            string[] args = new string[1];
            ScampalyzeConsole scamperTest = new ScampalyzeConsole(args);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<?xml version=\"1.0\"?>< form > < info >< name > bwh_chf_24hrcall </ name >< title > 24 - 72 Hour Follow - up Call </ title >< scamp > chf </ scamp >< major > 2 </ major >< minor > 0 </ minor ></ info >< panels >< panel >< name > p_call </ name >< elements >< element >< name > chf_attempt </ name >< type > radiolist </ type >< label > Attempt:</ label >< list > @chf_attempt </ list >< columns > 4 </ columns ></ element ></ elements ></ panel > ");


        }
    }
}
