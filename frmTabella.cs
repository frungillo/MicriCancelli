using MicriCancelli.Classi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicriCancelli
{
    public partial class frmTabella : Form
    {
        BindingSource bs_co=new BindingSource();
        public frmTabella()
        {
            InitializeComponent();
            this.Load += FrmTabella_Load;
        }

        private void FrmTabella_Load(object sender, EventArgs e)
        {
            caricaCodici();
        }
        private void caricaCodici() 
        {
           
            DataTable dt = new DataTable();
            List<Codici> list = new List<Codici>();
            list = Codici.getAll(" 1=1 order by data_emissione desc, ora_emissione desc");
            dt = Commons.ToDataTable(list);
            bs_co.DataSource = dt;
            grigliaCodici.DataSource = bs_co;

            grigliaCodici.Columns[1].Width = 70;
            grigliaCodici.Columns[2].Width = 100;
            grigliaCodici.Columns[3].Width = 70;
            grigliaCodici.Columns[4].Width = 100;
            grigliaCodici.Columns[4].Width = 100;

            grigliaCodici.Columns[1].HeaderText = "CODICE";
            grigliaCodici.Columns[2].HeaderText = "DATA EMISS.";
            grigliaCodici.Columns[3].HeaderText = "ORA EMISS.";
            grigliaCodici.Columns[4].HeaderText = "DATA USO";
            grigliaCodici.Columns[5].HeaderText = "ORA USO";
            grigliaCodici.Columns[0].Visible = false;
          
            grigliaCodici.ClearSelection();
           
            lblCount.Text = grigliaCodici.Rows.Count.ToString();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            caricaCodici();
        }
    }
}
