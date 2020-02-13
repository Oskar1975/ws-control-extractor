using DataExtractor.Common;
//using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OracleClient;
using EntiExtractor;

namespace DataExtractor.WebService
{
    public class dwsEmpresa : cConector
    {
        public DataTable consultarEmpresa()
        {
            try
            {
                dt = new DataTable();
                string selectEmpresa = "SELECT ID_CATOBJETOS, CVE_CEDULA, ID_DATOSE, NOMB_OBJETO, CVE_AMBIENTE, OBJETO, COD_ESTATUS, USUARIO, FECHA_ALTA FROM XXFM_CONF_CAT_OBJS_TAB";
                com = new OracleCommand(selectEmpresa, con);
                da = new OracleDataAdapter(com);
                da.Fill(dt);
                return dt;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet getDataSET(string spName, List<OracleParameter> parameters)
        {
            try
            {
                using (con)
                {
                    con.Open();
                    using (com)
                    {
                        if (parameters != null && parameters.Count > 0)
                        {
                            com.CommandType = CommandType.StoredProcedure;
                            foreach (var p in parameters)
                            {
                                com.Parameters.Add(p);
                            }
                        }
                        da.Fill(ds);
                        return ds;
                    }
                }
            }
            catch (Exception ex)
            {
                con.Close();
                throw new Exception("No se logro conectar a la BD", ex);
            }
        }

        /// <summary>
        /// Consulta listado de empresas en XXFM_CONF_CAT_OBJS_TAB
        /// </summary>
        /// <returns></returns>
        public List<bEntiEmpresa> ListadoEmpresa()
        {
            List<bEntiEmpresa> lst = new List<bEntiEmpresa>();
            try
            {
                con.Open();
                com = new OracleCommand("SELECT ID_CATOBJETOS, CVE_CEDULA, ID_DATOSE, NOMB_OBJETO, CVE_AMBIENTE, OBJETO, COD_ESTATUS, USUARIO, FECHA_ALTA FROM XXFM_CONF_CAT_OBJS_TAB", con);
                OracleDataReader dr = com.ExecuteReader();

                while (dr.Read())
                {
                    bEntiEmpresa empresaEnti = new bEntiEmpresa();
                    empresaEnti.ID_CATOBJETOS = Convert.ToInt32(dr["ID_CATOBJETOS"]);
                    empresaEnti.CVE_CEDULA = dr["CVE_CEDULA"].ToString();
                    empresaEnti.ID_DATOSE = Convert.ToDecimal(dr["ID_DATOSE"]);
                    empresaEnti.NOMB_OBJETO = dr["NOMB_OBJETO"].ToString();
                    empresaEnti.CVE_AMBIENTE = dr["CVE_AMBIENTE"].ToString();
                    empresaEnti.OBJETO = dr["OBJETO"].ToString();
                    empresaEnti.COD_ESTATUS = dr["COD_ESTATUS"].ToString();
                    empresaEnti.USUARIO = dr["USUARIO"].ToString();
                    empresaEnti.FECHA_ALTA = Convert.ToDateTime(dr["FECHA_ALTA"]);

                    lst.Add(empresaEnti);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lst;
        }
    }
}
