//using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExtractor.Common
{
    public abstract class cConector
    {
        #region Constructor
        public cConector()
        {
            con = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.7.39.127)(PORT=1648)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=TVFISCALD)));User Id=XXFM;Password=Mandalorian_KAZ_2020");
            noFilasAfect = new int();
        }
        #endregion

        #region Variables publicas

        protected DataTable dt;
        protected DataRow dr;
        protected DataSet ds;

        protected OracleConnection con;
        protected OracleDataAdapter da;
        protected OracleCommand com;

        public int? noFilasAfect;

        #endregion

        #region Metodo publicos

        #endregion
    }
}
