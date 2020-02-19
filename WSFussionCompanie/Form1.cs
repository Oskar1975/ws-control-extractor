using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WSFussionCompanie
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WSCompanies.FusionCompanies FC = new WSCompanies.FusionCompanies();
            WSCompanies.GetFusionCompaniesReqMsgType Requests = new WSCompanies.GetFusionCompaniesReqMsgType();
            WSCompanies.GetFusionCompaniesRspMsgType Response = new WSCompanies.GetFusionCompaniesRspMsgType();
            System.Net.ICredentials credential = new System.Net.NetworkCredential("sifoicdev@televisa.com.mx", "gvKrTTPvEz5k7JHB");

            FC.Credentials = credential;
            FC.Url = "https://oidevoics-eivjinterop.integration.ocp.oraclecloud.com/ic/ws/integration/v1/flows/soap/FUSIONCOMPANIES/1.0/";
            FC.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Default;
            FC.RequestEncoding = System.Text.Encoding.UTF8;

            //ServicePointManager.SecurityProtocol = Type(3072, SecurityProtocolType);

            //ServicePointManager.ServerCertificateValidationCallback = New Security.RemoteCertificateValidationCallback(AddressOf AcceptAllCertifications)

            Response = FC.GetFusionCompanies(Requests);
        }
    }
}
