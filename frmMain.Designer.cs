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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtLogLettore = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.grigliaCodici = new System.Windows.Forms.DataGridView();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.lblRefresh = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnTabella = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnGeneraTicket = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnApri = new System.Windows.Forms.Button();
            this.timerCountDown = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grigliaCodici)).BeginInit();
            this.SuspendLayout();
            // 
            // txtLogLettore
            // 
            this.txtLogLettore.Location = new System.Drawing.Point(5, 36);
            this.txtLogLettore.Multiline = true;
            this.txtLogLettore.Name = "txtLogLettore";
            this.txtLogLettore.ReadOnly = true;
            this.txtLogLettore.Size = new System.Drawing.Size(306, 459);
            this.txtLogLettore.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Operazioni di lettura";
            // 
            // grigliaCodici
            // 
            this.grigliaCodici.AllowUserToAddRows = false;
            this.grigliaCodici.AllowUserToDeleteRows = false;
            this.grigliaCodici.AllowUserToResizeColumns = false;
            this.grigliaCodici.AllowUserToResizeRows = false;
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Red;
            this.grigliaCodici.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grigliaCodici.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.grigliaCodici.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grigliaCodici.Enabled = false;
            this.grigliaCodici.Location = new System.Drawing.Point(445, 36);
            this.grigliaCodici.MultiSelect = false;
            this.grigliaCodici.Name = "grigliaCodici";
            this.grigliaCodici.ReadOnly = true;
            this.grigliaCodici.RowHeadersVisible = false;
            this.grigliaCodici.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grigliaCodici.Size = new System.Drawing.Size(405, 459);
            this.grigliaCodici.TabIndex = 12;
            // 
            // timerRefresh
            // 
            this.timerRefresh.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblRefresh
            // 
            this.lblRefresh.AutoSize = true;
            this.lblRefresh.Location = new System.Drawing.Point(494, 15);
            this.lblRefresh.Name = "lblRefresh";
            this.lblRefresh.Size = new System.Drawing.Size(35, 13);
            this.lblRefresh.TabIndex = 13;
            this.lblRefresh.Text = "label2";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = global::MicriCancelli.Properties.Resources.refresh_28;
            this.btnRefresh.Location = new System.Drawing.Point(445, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(30, 30);
            this.btnRefresh.TabIndex = 14;
            this.toolTip1.SetToolTip(this.btnRefresh, "Refresh Tabella");
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnTabella
            // 
            this.btnTabella.BackColor = System.Drawing.Color.White;
            this.btnTabella.Image = global::MicriCancelli.Properties.Resources.table_100;
            this.btnTabella.Location = new System.Drawing.Point(326, 254);
            this.btnTabella.Name = "btnTabella";
            this.btnTabella.Size = new System.Drawing.Size(101, 101);
            this.btnTabella.TabIndex = 11;
            this.toolTip1.SetToolTip(this.btnTabella, "Tabella Tutti i codici");
            this.btnTabella.UseVisualStyleBackColor = false;
            this.btnTabella.Click += new System.EventHandler(this.btnTabella_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.White;
            this.btnClose.Image = global::MicriCancelli.Properties.Resources.close_100;
            this.btnClose.Location = new System.Drawing.Point(874, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(29, 30);
            this.btnClose.TabIndex = 10;
            this.toolTip1.SetToolTip(this.btnClose, "Setting");
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.BackColor = System.Drawing.Color.White;
            this.btnSettings.Image = global::MicriCancelli.Properties.Resources.settings_100;
            this.btnSettings.Location = new System.Drawing.Point(326, 147);
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
            this.btnGeneraTicket.Location = new System.Drawing.Point(326, 40);
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
            this.btnClear.Location = new System.Drawing.Point(281, 3);
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
            this.btnApri.Location = new System.Drawing.Point(326, 366);
            this.btnApri.Name = "btnApri";
            this.btnApri.Size = new System.Drawing.Size(101, 101);
            this.btnApri.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnApri, "Apri Barriera");
            this.btnApri.UseVisualStyleBackColor = false;
            this.btnApri.Click += new System.EventHandler(this.btnApri_Click);
            // 
            // timerCountDown
            // 
            this.timerCountDown.Interval = 1000;
            this.timerCountDown.Tick += new System.EventHandler(this.timerCountDown_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 507);
            this.ControlBox = false;
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lblRefresh);
            this.Controls.Add(this.grigliaCodici);
            this.Controls.Add(this.btnTabella);
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
            ((System.ComponentModel.ISupportInitialize)(this.grigliaCodici)).EndInit();
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
        private System.Windows.Forms.Button btnTabella;
        private System.Windows.Forms.DataGridView grigliaCodici;
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.Label lblRefresh;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Timer timerCountDown;
    }
}

