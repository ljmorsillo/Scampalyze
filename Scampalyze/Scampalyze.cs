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
        /// <summary>
        /// Entry Point for the scamperlyze application
        /// Creates a Scampalyze object that does the work
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static int Main(string[] args)
        {     
                  
            ScampalyzeConsole scamper = new ScampalyzeConsole(args);
            if (ScampalyzeConsole.ARG_STAT.ARGS_FAILED == scamper.Usage(args))
            {
                return ((int)ScampalyzeConsole.ARG_STAT.ARGS_FAILED);
            }
            scamper.PreProcess();
            scamper.Process();

            Console.ReadKey();

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
        /// <remarks>options defines the details of the arguments, like the help text for each</remarks>
        //??? Make nested type defining type and validation criteria?
        Dictionary<string, string> options = new Dictionary<string, string>
            {
                {ELEMENT_TO_COUNT_ARG,"A list of strings with the XML elements in the form to count, such as 'panel','elements' " },
                {FORM_FILEPATH__ARG,"The path to the .form (SDF) file" },
                {CONNECTION_ARG, "The connection string for the DW database" },
                {"scampalyze","This performs some consistency checks on a SCAMP sdf (.form) and validates it against what is loaded in the data warehouse." }
            };

        ///!!! Inner text? is not clear, change xpath...
        const string XPATH_SDF_NAME = "info/name";

        const string SELECT_ELEMENTS_TEMPLATE = "SELECT count(*) FROM vwFormElements WHERE (formname = '{0}');";

        private string tagToCount = String.Empty;
        private string path = String.Empty;

        //Database related items
        protected SqlConnection DWConnection;
        protected SqlCommand DWCommand;
        protected SqlDataReader DWDataReader;

        //public string TagToCount { get; set; }
        public string Path { get; set; }
        private List<string> tagsToCount;



        //never should create without arg list
        /// <summary>
        /// Does the work
        /// </summary>
        public ScampalyzeConsole(string[] args) : base(args)
        {
            DWCommand = new SqlCommand();
        }

        /// <summary>
        /// In preparation for the processing
        /// </summary>
        public override void PreProcess()
        {
            base.PreProcess();
        }
        /// <summary>
        /// Do the primary work of the console app
        /// </summary>
        public override void Process()
        {
            try
            {
                foreach (string tagToCount in tagsToCount)
                {
                    XmlElement root = getFormRoot(path);
                    int formCount = countFromForm(root, tagToCount);
                    string formname = root.SelectSingleNode(XPATH_SDF_NAME).InnerText;
                    int DWcount = countFromDW(formname, tagToCount);
                    //??? Do we really want console output here....
                    Console.WriteLine("Count of {0} -> Form: {1}, DW: {2}", tagToCount, formCount, DWcount);
                    Console.WriteLine("{0}", (formCount == DWcount && formCount > 0) ? "Success" : "Failure");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Problem: {0}, please Fix: {1}.\nDetails:\n {2}", exc.Message, exc.Source, exc.ToString());
            }
        }
        /// <summary>
        /// Parse the commandline
        /// </summary>
        public override ARG_STAT Usage(string[] args)
        {
            // get arguments
            // general format is: <argument name>:<argument value> 
            // e.g. filepath:"c:\data\flah.form" count:'element, elements, panel" 
            // 
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
                {
                    tagsToCount = parsedArgs[ELEMENT_TO_COUNT_ARG].Split(',').ToList();
                }
                //TagToCount = tagsToCount.First();

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
        /// <summary>
        /// Get the count of tagToCount in xml form denoted by path
        /// </summary>
        /// <param name="root">XML document root of form</param>
        /// <param name="tagToCount">The tag to count</param>
        /// <returns>number of occurences</returns>
        private int countFromForm(XmlElement root, string tagToCount)
        {
            int count = 0;
            if (tagToCount.Equals(root.LocalName))
            { count++; }
            XmlNodeList elemList = root.GetElementsByTagName(tagToCount);
            count += elemList.Count;
            return count;
        }

        /// <summary>
        /// Get the count of elementToCount in the DW for formname
        /// </summary>
        /// <param name="elementToCount">which element to count</param>
        /// <param name="formname"><![CDATA[<form><name>]]> element</param>
        /// <returns>number of occurences</returns>
        private int countFromDW(string formname, string elementToCount)
        {
            int numForms = 0;
            DWCommand.Connection = DWConnection;
            StringBuilder commandString = new StringBuilder();

            switch (elementToCount.ToLower())
            {
                case "element":
                    DWCommand.CommandText = commandString.AppendFormat(SELECT_ELEMENTS_TEMPLATE, formname).ToString();
                    break;
                case "panel":
                    break;
            }
            if (DWCommand.CommandText.Length > 0)
            {
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
