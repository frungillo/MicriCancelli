using System;
using System.Windows.Forms;
using MicriCancelli.Services;

namespace MicriCancelli
{
    public partial class frmTabella : Form
    {
        public frmTabella()
        {
            InitializeComponent();
            this.Load += (s, e) => caricaCodici();
        }

        private void caricaCodici()
        {
            var codici = App.Codici.Tutti();
            grigliaCodici.DataSource = TabellaCodici.Costruisci(codici, App.Impostazioni);
            TabellaCodici.ApplicaLayout(grigliaCodici);
            lblCount.Text = codici.Count + " codici";
        }

        private void btnRefresh_Click(object sender, EventArgs e) => caricaCodici();
    }
}
