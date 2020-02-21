using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusiExtractor;

namespace ExtraxtorWS
{
    public partial class ConfExtractor : Form
    {
        public ConfExtractor()
        {
            InitializeComponent();
        }

        private void ConfExtractor_Load(object sender, EventArgs e)
        {
            cargaEmpresas();
            cargaDatosEnt();
        }

        public void cargaEmpresas()
        {
            bBusiExtractor loBusiExtract = new bBusiExtractor();
          
            GrdEmpresas.DataSource = loBusiExtract.ListadoEmpresa();

            DataGridViewCheckBoxColumn lodgvChk = new DataGridViewCheckBoxColumn();
            lodgvChk.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            lodgvChk.Name = "V";
           
            GrdEmpresas.Columns.Add(lodgvChk);

            GrdEmpresas.Columns["BusinessUnitId"].DisplayIndex = 4;
            GrdEmpresas.Columns["OrgID"].DisplayIndex = 3;
            GrdEmpresas.Columns["BusinessUnitName"].DisplayIndex = 2;
            GrdEmpresas.Columns["LegalEntityIdentifier"].DisplayIndex = 1;

            GrdEmpresas.Columns["BusinessUnitId"].Visible = false;
            GrdEmpresas.Columns["OrgID"].Visible = false;
            GrdEmpresas.Columns["BusinessUnitName"].HeaderText = "DESCRIPCION EMPRESA";
            GrdEmpresas.Columns["LegalEntityIdentifier"].HeaderText = "CLAVE EMPRESA";

            GrdEmpresas.Columns["LegalEntityIdentifier"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            GrdEmpresas.Columns["BusinessUnitName"].ReadOnly = true;
            GrdEmpresas.Columns["LegalEntityIdentifier"].ReadOnly = true;

            GrdEmpresas.Cursor = Cursors.Hand;


        }

        public void cargaDatosEnt()
        {
            bBusiExtractor loBusiExtract = new bBusiExtractor();

            GrdDatosEnt.DataSource = loBusiExtract.ListadoDatosEntrada();

            DataGridViewCheckBoxColumn lodgvChk = new DataGridViewCheckBoxColumn();
            lodgvChk.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            lodgvChk.Name = "V";

            GrdDatosEnt.Columns.Add(lodgvChk);

            GrdDatosEnt.Cursor = Cursors.Hand;


        }
    }
}
