using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WSFussionCompanie
{
    public partial class UserControl1: UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
            
            getWebServices getWS = new getWebServices();
            DataTable DataCompanies = getWS.getCompanies();
            dataGridView1.DataSource = DataCompanies;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            
        }
    }
}
