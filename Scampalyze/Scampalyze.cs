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
            scamper.Process();
            return (0); // what should happy be? DOS, Linus or Windows
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
        public override void Process()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path);

            // Get and display all the book titles.
            XmlElement root = doc.DocumentElement;
            int count = 0;
            if (TagToCount.Equals(root.LocalName))
            { count++; }
            XmlNodeList elemList = root.GetElementsByTagName(TagToCount);
            count += elemList.Count;
            Console.WriteLine("Count of <{0}>: {1}", TagToCount, count);
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
            //??? must be a better way to do this...
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
    }
}
