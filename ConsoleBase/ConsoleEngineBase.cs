using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ircda.Consoleapp
{
    public class ConsoleEngineBase
    {
        public enum ARG_STAT {ARGS_OK, ARGS_FAILED};
        private IDictionary<string,string> parsedArgs;
        public ConsoleEngineBase(string[] args)
        {
         
        }

        public virtual void PreProcess()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Process()
        {
            throw new System.NotImplementedException();
        }

        public virtual ARG_STAT Usage(string[] args)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Main()
        {
            throw new System.NotImplementedException();
        }

        public virtual void PostProcess()
        {
            throw new System.NotImplementedException();
        }
    }
}
