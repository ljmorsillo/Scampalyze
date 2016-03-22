using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ircda.Consoleapp
{
    public class ConsoleEngineBase
    {
        private IDictionary<string,string> parsedArgs;
        public ConsoleEngineBase(string[] args)
        {
         
        }

        public void PreProcess()
        {
            throw new System.NotImplementedException();
        }

        public void Process()
        {
            throw new System.NotImplementedException();
        }

        public void Usage()
        {
            throw new System.NotImplementedException();
        }

        public void Main()
        {
            throw new System.NotImplementedException();
        }

        public void PostProcess()
        {
            throw new System.NotImplementedException();
        }
    }
}
