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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnChiudi = new System.Windows.Forms.Button();
            this.btnApri = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(26, 24);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(281, 389);
            this.textBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Operazioni di lettura";
            // 
            // btnChiudi
            // 
            this.btnChiudi.BackColor = System.Drawing.Color.White;
            this.btnChiudi.Image = global::MicriCancelli.Properties.Resources.closeBar1;
            this.btnChiudi.Location = new System.Drawing.Point(660, 330);
            this.btnChiudi.Name = "btnChiudi";
            this.btnChiudi.Size = new System.Drawing.Size(101, 101);
            this.btnChiudi.TabIndex = 3;
            this.btnChiudi.UseVisualStyleBackColor = false;
            this.btnChiudi.Click += new System.EventHandler(this.btnChiudi_Click);
            // 
            // btnApri
            // 
            this.btnApri.BackColor = System.Drawing.Color.White;
            this.btnApri.Image = global::MicriCancelli.Properties.Resources.openBar;
            this.btnApri.Location = new System.Drawing.Point(530, 330);
            this.btnApri.Name = "btnApri";
            this.btnApri.Size = new System.Drawing.Size(101, 101);
            this.btnApri.TabIndex = 2;
            this.btnApri.UseVisualStyleBackColor = false;
            this.btnApri.Click += new System.EventHandler(this.btnApri_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnChiudi);
            this.Controls.Add(this.btnApri);
            this.Controls.Add(this.textBox1);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.ShowIcon = false;
            this.Text = "MICRI - Gestione varchi Parcheggio";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnApri;
        private System.Windows.Forms.Button btnChiudi;
        private System.Windows.Forms.Label label1;
    }
}

