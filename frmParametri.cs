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
            Parametri par =new Parametri();
            foreach (Control item in grpParametri.Controls)
            {
                if (item.GetType() == typeof(TextBox))
                {
                    par = Parametri.GetParametro(item.Tag.ToString());
                    try { item.Text = par.Value.ToString(); } catch { }
                }
            }
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
                leggiParametri();
            }
        }

        

        private void btnSalvaParametro_Click(object sender, EventArgs e)
        {
            Button item = new Button();
            item =sender as Button;
          
            Parametri par=new Parametri();
            par = Parametri.GetParametro(item.Tag.ToString());
            switch (item.Tag.ToString()) 
            {
                case "keyEnd":
                    par.Value=txtKeyEnd.Text;
                    break;
                case "code_len":
                    par.Value = txtCodeLen.Text;
                    break;
                case "minutiValidita":
                    par.Value = txtMinutiValidita.Text;
                    break;
                case "portaArduino":
                    par.Value = txtPortaArduino.Text;
                    break;
                case "ipArduino":
                    par.Value = txtIPArduino.Text;
                    break;
                case "inching":
                    par.Value = txtInching.Text;
                    break;
                    default:
                    return;
                    
            }
            
            Parametri.UpdateParametro(par); 
            leggiParametri();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
