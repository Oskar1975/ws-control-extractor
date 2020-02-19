using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSFussionCompanie
{
    public partial class getWebServices
    {
        public getWebServices() { 
        }
        public DataTable getCompanies()
        {
            WSCompanies.FusionCompanies FC = new WSCompanies.FusionCompanies();
            WSCompanies.GetFusionCompaniesReqMsgType Requests = new WSCompanies.GetFusionCompaniesReqMsgType();
            WSCompanies.GetFusionCompaniesRspMsgType Response = new WSCompanies.GetFusionCompaniesRspMsgType();
            System.Net.ICredentials credential = new System.Net.NetworkCredential("sifoicdev@televisa.com.mx", "gvKrTTPvEz5k7JHB");

            FC.Credentials = credential;
            FC.Url = "https://oidevoics-eivjinterop.integration.ocp.oraclecloud.com/ic/ws/integration/v1/flows/soap/FUSIONCOMPANIES/1.0/";
            FC.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Default;
            FC.RequestEncoding = System.Text.Encoding.UTF8;

            Response = FC.GetFusionCompanies(Requests);

            DataTable DT;
            DT = new DataTable();
            DataRow row;

            DT.Columns.Add("BusinessUnitId", typeof(String));
            DT.Columns.Add("BusinessUnitName", typeof(String));
            DT.Columns.Add("OrgID", typeof(String));
            DT.Columns.Add("LegalEntityIdentifier", typeof(String));

            foreach (WSCompanies.CompanyType cmp in Response.Companies)
            {
                row = DT.NewRow();
                row["BusinessUnitId"] = cmp.BusinessUnitId;
                row["BusinessUnitName"] = cmp.BusinessUnitName;
                row["OrgID"] = cmp.LegalEntityIdentifier;
                row["LegalEntityIdentifier"] = cmp.OrgID;
                DT.Rows.Add(row);
            }

            return DT;
        }
    }
}
