using DataExtractor.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OracleClient;

namespace DataExtractor.WebService
{
    public class dwsEmpresa : cConector
    {
        public DataTable consultarEmpresa()
        {
			try
			{

				string selectEmpresa = "SELECT ID_CATOBJETOS, CVE_CEDULA, ID_DATOSE, NOMB_OBJETO, CVE_AMBIENTE, OBJETO, COD_ESTATUS, USUARIO, FECHA_ALTA FROM XXFM_CONF_CAT_OBJS_TAB";

				com = new OracleCommand(selectEmpresa, con);
				da = new OracleDataAdapter(com);
				dt = new DataTable();
				da.Fill(dt);
				return dt;

			}
			catch (Exception ex)
			{

				throw ex;
			}
        }
    }
}
