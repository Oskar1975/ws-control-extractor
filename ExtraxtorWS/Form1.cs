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

            string cadenaConnTVFISCALD = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.7.39.127)(PORT=1648)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=TVFISCALD)));User Id=XXFM;Password=Mandalorian_KAZ_2020";
            OracleConnection con = new OracleConnection(cadenaConnTVFISCALD);

            string cadenaConnTVDEV = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.1.45.122)(PORT=1528)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=TVDEV)));User Id=XXFM;Password=DEV_M_TVD3V618hbxw";
            OracleConnection conTVDEV = new OracleConnection(cadenaConnTVDEV);

            OracleConnection conTVINTDES = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.1.45.122)(PORT=1525)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=TVINTDES)));User Id=XXFM;Password=INTXFM_TVINTD3S889LEDxs");
            
            try
            {
                con.Open();
                MessageBox.Show("Conectado... 6436945905 TVFISCALD");
                con.Close();

                conTVDEV.Open();
                MessageBox.Show("Conectado... 6436945905 TVDEV");
                conTVDEV.Close();



            }
            catch (OracleException ex)
            {
                con.Close();
                throw new Exception("Error en la conexion", ex); 
            }
        }

        private void btnConsultarEmp_Click(object sender, EventArgs e)
        {
            try
            {
                bBusiExtractor obj = new bBusiExtractor();

                grdEmpresa.DataSource = obj.ConsultaEmpresa(txtClave.Text);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        private void btnFusionCompanies_Click(object sender, EventArgs e)
        {
            try
            {
                servicieCompanies.GetFusionCompaniesReqMsgType messageType = new servicieCompanies.GetFusionCompaniesReqMsgType();
                servicieCompanies.GetFusionCompaniesReqMsgType response = new servicieCompanies.GetFusionCompaniesReqMsgType();
                servicieCompanies.FusionCompaniesService_FusionCompaniesTrigger_REQUESTClient ws = new servicieCompanies.FusionCompaniesService_FusionCompaniesTrigger_REQUESTClient();

                messageType.Input = string.Empty;
                System.Net.ICredentials cred = new System.Net.NetworkCredential("jamartinez@televisa.com.mx", "J0rg3_12345#");


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
