using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using MicriCancelli.Services;

namespace MicriCancelli
{
    /// <summary>
    /// Pannello parametri: le righe vengono generate dalle definizioni in <see cref="Impostazioni.Definizioni"/>,
    /// così aggiungere un parametro non richiede di toccare il designer e non ci sono più chiavi scritte a mano.
    /// </summary>
    public partial class frmParametri : Form
    {
        private readonly ToolTip _suggerimenti = new ToolTip();

        public frmParametri()
        {
            InitializeComponent();
            this.Load += FrmParametri_Load;
        }

        private void FrmParametri_Load(object sender, EventArgs e)
        {
            costruisciRighe();
            impostaAbilitazione(false);
            txtPassword.Focus();
        }

        private void costruisciRighe()
        {
            tblParametri.SuspendLayout();
            tblParametri.Controls.Clear();
            tblParametri.RowStyles.Clear();
            tblParametri.RowCount = 0;

            foreach (var def in Impostazioni.Definizioni)
            {
                var etichetta = new Label { Text = def.Etichetta, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 8, 3, 3) };
                _suggerimenti.SetToolTip(etichetta, def.Descrizione);

                Control editor;
                if (def.Chiave == "stampante")
                {
                    var combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 260 };
                    combo.Items.Add("(predefinita di Windows)");
                    foreach (string p in PrinterSettings.InstalledPrinters) combo.Items.Add(p);
                    editor = combo;
                }
                else if (def.Scelte != null)
                {
                    var combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 260 };
                    combo.Items.AddRange(def.Scelte);
                    editor = combo;
                }
                else
                {
                    editor = new TextBox { Width = 260 };
                }
                editor.Tag = def;
                editor.Anchor = AnchorStyles.Left;
                _suggerimenti.SetToolTip(editor, def.Descrizione);

                var salva = new Button { Image = Properties.Resources.save_30, Size = new Size(36, 32), Tag = editor, FlatStyle = FlatStyle.Flat };
                salva.FlatAppearance.BorderSize = 0;
                _suggerimenti.SetToolTip(salva, "Salva \"" + def.Etichetta + "\"");
                salva.Click += btnSalva_Click;

                aggiungiRiga(etichetta, editor, salva);
            }

            // riga per cambiare la password del pannello (viene salvato solo l'hash SHA-256)
            var lblPwd = new Label { Text = "Nuova password pannello", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 8, 3, 3) };
            var txtPwd = new TextBox { Width = 260, UseSystemPasswordChar = true, Name = "txtNuovaPassword" };
            var btnPwd = new Button { Image = Properties.Resources.save_30, Size = new Size(36, 32), FlatStyle = FlatStyle.Flat };
            btnPwd.FlatAppearance.BorderSize = 0;
            btnPwd.Click += (s, e) =>
            {
                if (txtPwd.Text.Length < 6)
                {
                    MessageBox.Show("La password deve avere almeno 6 caratteri.", "Parametri", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                App.Parametri.Scrivi(Impostazioni.ChiavePasswordHash, Sicurezza.Sha256Hex(txtPwd.Text));
                App.RicaricaImpostazioni();
                txtPwd.Clear();
                Log.Info("Password del pannello parametri aggiornata");
                MessageBox.Show("Password aggiornata.", "Parametri", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            aggiungiRiga(lblPwd, txtPwd, btnPwd);

            tblParametri.ResumeLayout();
            leggiParametri();
        }

        private void aggiungiRiga(Control etichetta, Control editor, Control pulsante)
        {
            int riga = tblParametri.RowCount++;
            tblParametri.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblParametri.Controls.Add(etichetta, 0, riga);
            tblParametri.Controls.Add(editor, 1, riga);
            tblParametri.Controls.Add(pulsante, 2, riga);
        }

        private void leggiParametri()
        {
            foreach (Control c in tblParametri.Controls)
            {
                var def = c.Tag as Impostazioni.Definizione;
                if (def == null) continue;
                var valore = App.Parametri.Leggi(def.Chiave) ?? def.Predefinito;

                var combo = c as ComboBox;
                if (combo != null)
                {
                    if (def.Chiave == "stampante")
                        combo.SelectedIndex = string.IsNullOrWhiteSpace(valore) || !combo.Items.Contains(valore) ? 0 : combo.Items.IndexOf(valore);
                    else
                        combo.SelectedIndex = Math.Max(0, combo.Items.IndexOf(valore));
                }
                else
                {
                    c.Text = valore;
                }
            }
        }

        private void impostaAbilitazione(bool abilitato)
        {
            foreach (Control c in tblParametri.Controls)
                if (!(c is Label)) c.Enabled = abilitato;
            lblAvviso.Visible = abilitato;
        }

        private void btnSblocca_Click(object sender, EventArgs e)
        {
            if (Sicurezza.VerificaPassword(txtPassword.Text, App.Impostazioni.PasswordHash))
            {
                impostaAbilitazione(true);
                txtPassword.Clear();
                txtPassword.Enabled = false;
                btnSblocca.Enabled = false;
                Log.Info("Pannello parametri sbloccato");
            }
            else
            {
                Log.Avviso("Tentativo di sblocco parametri con password errata");
                MessageBox.Show("Password errata.", "Parametri", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.SelectAll();
                txtPassword.Focus();
            }
        }

        private void btnSalva_Click(object sender, EventArgs e)
        {
            var editor = (sender as Button)?.Tag as Control;
            var def = editor?.Tag as Impostazioni.Definizione;
            if (def == null) return;

            string valore;
            var combo = editor as ComboBox;
            if (combo != null)
                valore = def.Chiave == "stampante" && combo.SelectedIndex == 0 ? "" : Convert.ToString(combo.SelectedItem);
            else
                valore = editor.Text.Trim();

            var errore = def.Valida?.Invoke(valore);
            if (errore != null)
            {
                MessageBox.Show(def.Etichetta + ": " + errore, "Valore non valido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                editor.Focus();
                return;
            }

            App.Parametri.Scrivi(def.Chiave, valore);
            App.RicaricaImpostazioni();
            Log.Info("Parametro " + def.Chiave + " impostato a '" + valore + "'");
            leggiParametri();
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();
    }
}
