using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml;
using Ircda.Consoleapp;
using System.Data.SqlClient;

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
            
            // Windows happy return 
            return (0); 
        }

        public void Count()
        {
            throw new System.NotImplementedException();
        }
    }
    public class ScampalyzeConsole : ConsoleEngineBase
    {
        //Arguments - identifier, value
        const string FORM_FILEPATH__ARG = "filepath";
        const string CONNECTION_ARG = "connection";
        const string ELEMENT_TO_COUNT_ARG = "countelement";

        ///!!! Inner text? is not clear, change xpath...
        const string XPATH_SDF_NAME = "info/name";

        const string SELECT_ELEMENTS_TEMPLATE = "SELECT count(*) FROM vwFormElements WHERE (formname = '{0}');";

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
            try
            {
                int formCount = countFromForm(Path, TagToCount);
                string formname = getFormRoot(Path).SelectSingleNode(XPATH_SDF_NAME).InnerText;
                int DWcount = countFromDW(formname, TagToCount);
                Console.WriteLine("Count of {0} -> Form: {1}, DW: {2}", TagToCount, formCount, DWcount);
                Console.WriteLine("{0}", (formCount == DWcount && formCount > 0) ? "Success" : "Failure");
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
            // get arguments
            // general format is: <argument name>:<argument value> 
            // e.g. filepath:"c:\data\flah.form" count:'element, elements, panel" 
            // 
            Dictionary<string, string> options = new Dictionary<string, string>
            {
                {ELEMENT_TO_COUNT_ARG,"A list of strings with the XML elements in the form to count, such as 'panel','elements' " },
                {FORM_FILEPATH__ARG,"The path to the .form (SDF) file" },
                {CONNECTION_ARG, "The connection string for the DW database" },
                {"scampalyze","This performs some consistency checks on a SCAMP sdf (.form) and validates it against what is loaded in the data warehouse." }
            };
            if (args.Length < 3)
            {
                Console.WriteLine("SCAMPALYZE!  {0}", options["scampalyze"]);
                Console.WriteLine("You might want to try some parameters. Like filepath and countelement...");

                foreach (string s in options.Keys.TakeWhile(o => o != "scampalyze"))
                {
                    Console.WriteLine("{0}: {1}", s, options[s]);
                }
                Console.WriteLine("for example: scampalyze countelement:'form' filepath:BWH_CHF_24HRCALL.2.0.form");
                return (ARG_STAT.ARGS_FAILED);
            }

            try
            {
                var parsedArgs = args
                .Select(s => s.Split(new[] { ':' }, 2))
                .ToDictionary(s => s[0].ToLower(), s => s[1]);

                if (parsedArgs.Keys.Contains(CONNECTION_ARG))
                {
                    SqlConnectionStringBuilder connInfo =
                    new SqlConnectionStringBuilder(parsedArgs[CONNECTION_ARG]);
                    DWConnection = new SqlConnection(connInfo.ConnectionString);
                }
                if (parsedArgs.Keys.Contains(ELEMENT_TO_COUNT_ARG))
                    TagToCount = parsedArgs[ELEMENT_TO_COUNT_ARG];
                if (parsedArgs.Keys.Contains(FORM_FILEPATH__ARG))
                    Path = parsedArgs[FORM_FILEPATH__ARG];
            }
            catch (KeyNotFoundException exc)
            {
                Console.WriteLine("You need a parameter: {0}", exc.Message);
                return ARG_STAT.ARGS_FAILED;
            }
            catch (IndexOutOfRangeException exc)
            {
                Console.WriteLine("You need a known parameter name and a colon ':' (like 'filepath:') {0}", exc.Message);
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
                DWCommand.CommandText = commandString.AppendFormat(SELECT_ELEMENTS_TEMPLATE, formname).ToString();
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
