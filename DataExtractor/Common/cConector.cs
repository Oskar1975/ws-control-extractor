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
            con = new OracleConnection(ConfigurationManager.ConnectionStrings["SQL"] != null ? ConfigurationManager.ConnectionStrings["SQL"].ConnectionString : "");
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

        internal DataTable getDataTable(string query, Dictionary<String, Object> keyValueParam = null)
        {
            try
            {
                com = new OracleCommand(query, con);
                com.CommandType = System.Data.CommandType.Text;
                if (keyValueParam != null)
                {
                    foreach (KeyValuePair<String, Object> kvp in keyValueParam)
                    {
                        if (query.Contains(kvp.Key))
                            com.Parameters.AddWithValue(kvp.Key, kvp.Value != null ? kvp.Value.ToString() : "");
                    }
                }

                da = new OracleDataAdapter(com);
                dt = new DataTable();
                con.Open();
                da.Fill(dt);
                if (dt != null)
                {
                    if (dt.Rows.Count >= 1)
                        return dt;
                    else
                        return null;
                }
                else { return null; }


            }
            catch (Exception ex) { return null; ; }
            finally { con.Close(); }
        }

        #endregion
    }
}
