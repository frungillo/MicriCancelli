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
    public partial class frmParametri : Form
    {
        public frmParametri()
        {
            InitializeComponent();
            this.Load += FrmParametri_Load;
        }

        private void FrmParametri_Load(object sender, EventArgs e)
        {
            txtPassword.Focus();
        }
        private void leggiParametri()
        {

        }
        private void btnSblocca_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text == "genny57") 
            {
                foreach (Control item in grpParametri.Controls)
                {
                    if (item.GetType() == typeof(TextBox) || (item.GetType()== typeof(Button)))
                    {
                        item.Text = "";
                        item.Enabled = true;
                    }
                }
            }
        }
    }
}
