using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Ircda.Consoleapp
{
    public class ConsoleEngineBase
    {
        public enum ARG_STAT {ARGS_OK, ARGS_FAILED};
        protected IDictionary<string,string> parsedArgs;

        //Never Create without arguments
        private ConsoleEngineBase()
        {
        }

        public ConsoleEngineBase(string[] args)
        {
         
        }

        public virtual void PreProcess()
        {            
            
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
