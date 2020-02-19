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
            conTVFISCALD = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.7.39.127)(PORT=1648)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=TVFISCALD)));User Id=XXFM;Password=Mandalorian_KAZ_2020");
            noFilasAfect = new int();

            conTVDEV = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.1.45.122)(PORT=1528)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=TVDEV)));User Id=XXFM;Password=DEV_M_TVD3V618hbxw");
            noFilasAfect = new int();

            conTVINTDES = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.1.45.122)(PORT=1525)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=TVINTDES)));User Id=XXFM;Password=INTXFM_TVINTD3S889LEDxs");
            noFilasAfect = new int();

            conWEB = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=???)(PORT=???)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=???)));User Id=jamartinez@televisa.com.mx;Password=J0rg3_12345#");
            noFilasAfect = new int();
        }
        #endregion

        #region Variables publicas

        protected DataTable dt;
        protected DataRow dr;
        protected DataSet ds;

        protected OracleConnection conTVFISCALD;
        protected OracleConnection conTVDEV;
        protected OracleConnection conTVINTDES;
        protected OracleConnection conWEB;
        protected OracleDataAdapter da;
        protected OracleCommand com;

        public int? noFilasAfect;

        #endregion

        #region Metodo publicos

        #endregion
    }
}
