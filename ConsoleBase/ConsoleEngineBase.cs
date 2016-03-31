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
        protected SqlConnection DWConnection;
        protected SqlCommand DWCommand;
        protected SqlDataReader DWDataReader;

        public ConsoleEngineBase(string[] args)
        {
         
        }

        public virtual void PreProcess()
        {
            ///!!! Just for intial test, parameterize
            //Does the ConnStringBuider buy us anything?
            SqlConnectionStringBuilder connInfo = 
                new SqlConnectionStringBuilder("Server=irish.ircda.org;Database=DataWarehouse;User Id=sa;Password=1!LIwumCL@;");
            DWConnection = new SqlConnection(connInfo.ConnectionString);
            DWCommand = new SqlCommand();
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
