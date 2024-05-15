namespace MicriCancelli
{
    partial class frmMain
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtLogLettore = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnGeneraTicket = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnApri = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtLogLettore
            // 
            this.txtLogLettore.Location = new System.Drawing.Point(26, 40);
            this.txtLogLettore.Multiline = true;
            this.txtLogLettore.Name = "txtLogLettore";
            this.txtLogLettore.ReadOnly = true;
            this.txtLogLettore.Size = new System.Drawing.Size(306, 373);
            this.txtLogLettore.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Operazioni di lettura";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.White;
            this.btnClose.Image = global::MicriCancelli.Properties.Resources.close_100;
            this.btnClose.Location = new System.Drawing.Point(772, 312);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(101, 101);
            this.btnClose.TabIndex = 10;
            this.toolTip1.SetToolTip(this.btnClose, "Setting");
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.BackColor = System.Drawing.Color.White;
            this.btnSettings.Image = global::MicriCancelli.Properties.Resources.settings_100;
            this.btnSettings.Location = new System.Drawing.Point(367, 312);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(101, 101);
            this.btnSettings.TabIndex = 9;
            this.toolTip1.SetToolTip(this.btnSettings, "Setting");
            this.btnSettings.UseVisualStyleBackColor = false;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnGeneraTicket
            // 
            this.btnGeneraTicket.BackColor = System.Drawing.Color.White;
            this.btnGeneraTicket.Image = global::MicriCancelli.Properties.Resources.barcode_100;
            this.btnGeneraTicket.Location = new System.Drawing.Point(367, 40);
            this.btnGeneraTicket.Name = "btnGeneraTicket";
            this.btnGeneraTicket.Size = new System.Drawing.Size(101, 101);
            this.btnGeneraTicket.TabIndex = 8;
            this.toolTip1.SetToolTip(this.btnGeneraTicket, "Genera e Stampa Biglietto");
            this.btnGeneraTicket.UseVisualStyleBackColor = false;
            this.btnGeneraTicket.Click += new System.EventHandler(this.btnGeneraTicket_Click);
            // 
            // btnClear
            // 
            this.btnClear.Image = global::MicriCancelli.Properties.Resources.clear_27;
            this.btnClear.Location = new System.Drawing.Point(302, 7);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(30, 30);
            this.btnClear.TabIndex = 5;
            this.toolTip1.SetToolTip(this.btnClear, "Pulisci Log Letture");
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnApri
            // 
            this.btnApri.BackColor = System.Drawing.Color.White;
            this.btnApri.Image = global::MicriCancelli.Properties.Resources.openBar;
            this.btnApri.Location = new System.Drawing.Point(772, 40);
            this.btnApri.Name = "btnApri";
            this.btnApri.Size = new System.Drawing.Size(101, 101);
            this.btnApri.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnApri, "Apri Barriera");
            this.btnApri.UseVisualStyleBackColor = false;
            this.btnApri.Click += new System.EventHandler(this.btnApri_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(885, 450);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnGeneraTicket);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnApri);
            this.Controls.Add(this.txtLogLettore);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.ShowIcon = false;
            this.Text = "MICRI - Gestione Varchi Parcheggio";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLogLettore;
        private System.Windows.Forms.Button btnApri;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnGeneraTicket;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnClose;
    }
}

