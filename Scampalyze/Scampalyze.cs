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
        static void Main(string[] args)
        {
            
            if (args.Length < 1)
            {
                Console.WriteLine("You might want to try some parameters.\n    Like filepath and count...");
            }

            ScampalyzeConsole scamper = new ScampalyzeConsole(args); 

            XmlDocument doc = new XmlDocument();
            doc.Load(scamper.Path);

            // Get and display all the book titles.
            XmlElement root = doc.DocumentElement;
            XmlNodeList elemList = root.GetElementsByTagName(scamper.TagToCount);
            Console.WriteLine("Count of <{0}>: {1}", scamper.TagToCount, elemList.Count);
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
  
        public ScampalyzeConsole(string[] args) :base(args)
        {
            // get arguments
            // format is: <command>:<thing to count> filepath:<pathnametofile> e.g.  
            //  count:<form> c:\
            //??? must be a better way to do this...
            
            try
            {
                var parsedArgs = args
                .Select(s => s.Split(new[] { ':' }, 2))
                .ToDictionary(s => s[0], s => s[1]);

                if (parsedArgs.Keys.Contains("count"))
                    TagToCount = parsedArgs["count"];
                if (parsedArgs.Keys.Contains("filepath"))
                    Path = parsedArgs["filepath"];
            }
            catch (KeyNotFoundException exc)
            {
                Console.WriteLine("You need a parameter: {0}", exc.Message);
            }
            catch (IndexOutOfRangeException exc)
            {
                Console.WriteLine("You need a parameter name (like 'filepath:') {0}", exc.Message);
            }
        }
    }
}
