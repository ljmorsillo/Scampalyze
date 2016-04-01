using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml;
using Ircda.Consoleapp;

namespace Ircda.Scampalyze
{
    class Scampalyze
    {
        static int Main(string[] args)
        {
            
            ScampalyzeConsole scamper = new ScampalyzeConsole(args);
            if (ScampalyzeConsole.ARG_STAT.ARGS_FAILED == scamper.Usage(args))
            {
                return ((int)ScampalyzeConsole.ARG_STAT.ARGS_FAILED);
            }
            scamper.PreProcess();
            scamper.Process();
            //??? What should happy result be? DOS, Linus or Windows Convention
            return (0); 
        }

        public void Count()
        {
            throw new System.NotImplementedException();
        }
    }
    public class ScampalyzeConsole : ConsoleEngineBase
    {
        private string tagToCount = String.Empty;
        private string path = String.Empty;

        public string TagToCount { get; set; }
        public string Path { get; set; }

        public ScampalyzeConsole(string[] args) : base(args)
        {
        }
        public override void PreProcess()
        {
            base.PreProcess();
        }
        public override void Process()
        {
            try {
                int formCount = countFromForm(Path, TagToCount);
                ///!!! Inner text is not clear, change xpath...
                string formname = getFormRoot(Path).SelectSingleNode("info/name").InnerText;
                int DWcount = countFromDW(formname, TagToCount);
                Console.WriteLine("Count of {0} -> Form: {1}, DW: {2}", TagToCount, formCount, DWcount);
                Console.WriteLine("{0}", (formCount == DWcount) ? "Success" : "Failure");
            }
            catch (Exception exc)
            {
                Console.WriteLine("Problem: {0}, please Fix: {1}.\nDetails:\n {2}", exc.Message, exc.Source, exc.ToString());
            }
            finally
            {
                Console.ReadKey();
            }
        }
        public override ARG_STAT Usage(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("You might want to try some parameters. Like filepath and countelement...");
                Console.WriteLine("for example: scampalyze countelement:'form' filepath:BWH_CHF_24HRCALL.2.0.form");
                return (ARG_STAT.ARGS_FAILED);
            }
            // get arguments
            // format is: <command>:<thing to count> filepath:<pathnametofile> e.g.  
            //  count:<form> c:\
            //??? There are better ways to do this...make less brittle
            try
            {
                var parsedArgs = args
                .Select(s => s.Split(new[] { ':' }, 2))
                .ToDictionary(s => s[0], s => s[1]);

                if (parsedArgs.Keys.Contains("countelement"))
                    TagToCount = parsedArgs["countelement"];
                if (parsedArgs.Keys.Contains("filepath"))
                    Path = parsedArgs["filepath"];
            }
            catch (KeyNotFoundException exc)
            {
                Console.WriteLine("You need a parameter: {0}", exc.Message);
                return ARG_STAT.ARGS_FAILED;
            }
            catch (IndexOutOfRangeException exc)
            {
                Console.WriteLine("You need a parameter name and a colon ':' (like 'filepath:') {0}", exc.Message);
                return ARG_STAT.ARGS_FAILED;
            }
            return ARG_STAT.ARGS_OK;
        }

        private int countFromForm(string path, string tagToCount)
        {
            XmlElement root = getFormRoot(path);
            int count = 0;
            if (tagToCount.Equals(root.LocalName))
            { count++; }
            XmlNodeList elemList = root.GetElementsByTagName(tagToCount);
            count += elemList.Count;
            return count;
        }

        private int countFromDW(string formname, string elementToCount)
        {
            int numForms = 0;
            DWCommand.Connection = DWConnection;
            if (TagToCount.ToLower().Equals("element"))
            {
                StringBuilder commandString = new StringBuilder();
                DWCommand.CommandText = commandString.AppendFormat("SELECT count(*) FROM vwFormElements WHERE (formname = '{0}');", formname).ToString();
                DWConnection.Open();
                numForms = (int)DWCommand.ExecuteScalar();
                DWConnection.Close();
            }
            return numForms;
        }
        /// <summary>
        /// Utility to return the root element of the XML form document 
        /// </summary>
        /// <param name="path">Path to form document</param>
        /// <returns cref="System.Xml.XmlElement>root XMLElement of document</returns>
        /// <exception cref="System.IO.IOException"> Usual cast of bad file exceptions</exception>
        /// <exception cref="System.Xml.XmlException">Usual cast of XML exceptions thrown</exception>
        private XmlElement getFormRoot(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path);
            //if no root, exception thrown before now
            return (doc.DocumentElement);
        }
    }
}
