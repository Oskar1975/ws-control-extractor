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
using WSFussionCompanie;

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
                //com = new OracleCommand(selectEmpresa, conTVFISCALD);
                //da = new OracleDataAdapter(com);
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
                using (conTVFISCALD)
                {
                    conTVFISCALD.Open();
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
                conTVFISCALD.Close();
                throw new Exception("No se logro conectar a la BD", ex);
            }
        }

        /// <summary>
        /// Consulta listado de empresas en XXFM_CONF_CAT_OBJS_TAB
        /// </summary>
        /// <returns></returns>
        public List<bEntiEmpresa> ListadoEmpresa()
        {
            List<bEntiEmpresa> laEntEmp = new List<bEntiEmpresa>();
            try
            {

                getWebServices gWS = new getWebServices();

                foreach (DataRow loRow in gWS.getCompanies().Rows)
                {
                    bEntiEmpresa loTmpEmp = new bEntiEmpresa();

                    loTmpEmp.OrgID = loRow["OrgID"].ToString();
                    loTmpEmp.BusinessUnitId = loRow["BusinessUnitId"].ToString();
                    loTmpEmp.BusinessUnitName = loRow["BusinessUnitName"].ToString();
                    loTmpEmp.LegalEntityIdentifier = loRow["LegalEntityIdentifier"].ToString();                    

                    laEntEmp.Add(loTmpEmp);
                }                


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return laEntEmp;
        }

        public List<bEntiDatoEntrada> ListadoDatosEntrada()
        {
            List<bEntiDatoEntrada> laDatoEnt = new List<bEntiDatoEntrada>();
            try
            {
                //conTVFISCALD.Open();
                com = new OracleCommand();
                com.CommandText = "Select * From XXFM_CONF_CAT_OBJS_TAB Where UPPER(CVE_CEDULA) LIKE UPPER('CED%')";
                com.Connection = conTVFISCALD;
                com.Connection.Open();

                IDataReader loResultados = com.ExecuteReader();

                while (loResultados.Read())
                {
                    bEntiDatoEntrada loDatEnt = new bEntiDatoEntrada();
                    loDatEnt.ID_CATOBJETOS = Convert.ToInt32(loResultados["ID_CATOBJETOS"]);
                    loDatEnt.CVE_CEDULA = loResultados["CVE_CEDULA"].ToString();
                    loDatEnt.ID_DATOSE = Convert.ToInt32(loResultados["ID_DATOSE"]);
                    loDatEnt.NOMB_OBJETO = loResultados["NOMB_OBJETO"].ToString();
                    loDatEnt.CVE_AMBIENTE = loResultados["CVE_AMBIENTE"].ToString();
                    loDatEnt.OBJETO = loResultados["OBJETO"].ToString();
                    loDatEnt.COD_ESTATUS = loResultados["COD_ESTATUS"].ToString();

                    laDatoEnt.Add(loDatEnt);

                }

                loResultados.Close();
                loResultados.Dispose();
                conTVFISCALD.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return laDatoEnt;
        }

        public List<bEntiEmpresa> ConsultaEmpresa(string cve_ambiente)
        {
            List<bEntiEmpresa> lst = new List<bEntiEmpresa>();
            try
            {
                conTVFISCALD.Open();
                //com = new OracleCommand("Select ID_CATOBJETOS, CVE_CEDULA, ID_DATOSE, NOMB_OBJETO, CVE_AMBIENTE, OBJETO, COD_ESTATUS, USUARIO, FECHA_ALTA From XXFM_CONF_CAT_OBJS_TAB Where UPPER(CVE_AMBIENTE) LIKE UPPER('"+ cve_ambiente +"')", conTVFISCALD);
                //OracleDataReader dr = com.ExecuteReader();

                //while (dr.Read())
                //{
                //    bEntiEmpresa empresaEnti = new bEntiEmpresa();
                //    //empresaEnti.ID_CATOBJETOS = Convert.ToInt32(dr["ID_CATOBJETOS"]);
                //    //empresaEnti.CVE_CEDULA = dr["CVE_CEDULA"].ToString();
                //    //empresaEnti.ID_DATOSE = Convert.ToDecimal(dr["ID_DATOSE"]);
                //    //empresaEnti.NOMB_OBJETO = dr["NOMB_OBJETO"].ToString();
                //    //empresaEnti.CVE_AMBIENTE = dr["CVE_AMBIENTE"].ToString();
                //    //empresaEnti.OBJETO = dr["OBJETO"].ToString();
                //    //empresaEnti.COD_ESTATUS = dr["COD_ESTATUS"].ToString();
                //    //empresaEnti.USUARIO = dr["USUARIO"].ToString();
                //    //empresaEnti.FECHA_ALTA = Convert.ToDateTime(dr["FECHA_ALTA"]);

                //    lst.Add(empresaEnti);

                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lst;
        }
    }
}
