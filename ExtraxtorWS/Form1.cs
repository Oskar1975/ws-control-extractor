using DataExtractor.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OracleClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Oracle.DataAccess.Client;
using System.Configuration;
using BusiExtractor;

namespace ExtraxtorWS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //servicieCompanies.GetFusionCompaniesReqMsgType messageType = new servicieCompanies.GetFusionCompaniesReqMsgType();
            //servicieCompanies.GetFusionCompaniesReqMsgType response = new servicieCompanies.GetFusionCompaniesReqMsgType();
            //servicieCompanies.FusionCompaniesService_FusionCompaniesTrigger_REQUESTClient ws = new servicieCompanies.FusionCompaniesService_FusionCompaniesTrigger_REQUESTClient();

            //messageType.Input = string.Empty;
            //System.Net.ICredentials cred = new System.Net.NetworkCredential("jamartinez@televisa.com.mx", "J0rg3_12345#");

            bBusiExtractor obj = new bBusiExtractor();


            grdEmpresa.DataSource = obj.ListadoEmpresa();

        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            try
            {
                string cadenaConn = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.7.39.127)(PORT=1648)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=TVFISCALD)));User Id=XXFM;Password=Mandalorian_KAZ_2020";

                OracleConnection con = new OracleConnection(cadenaConn);

                con.Open();
                MessageBox.Show("Conectado... 6436945905");
            }
            catch (OracleException ex)
            {
                throw new Exception("Error en la conexion", ex);
            }
        }

       
    }
}
