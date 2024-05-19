namespace MicriCancelli
{
    partial class frmTabella
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grigliaCodici = new System.Windows.Forms.DataGridView();
            this.lblCount = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grigliaCodici)).BeginInit();
            this.SuspendLayout();
            // 
            // grigliaCodici
            // 
            this.grigliaCodici.AllowUserToResizeColumns = false;
            this.grigliaCodici.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Red;
            this.grigliaCodici.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.grigliaCodici.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grigliaCodici.Location = new System.Drawing.Point(22, 29);
            this.grigliaCodici.MultiSelect = false;
            this.grigliaCodici.Name = "grigliaCodici";
            this.grigliaCodici.ReadOnly = true;
            this.grigliaCodici.RowHeadersVisible = false;
            this.grigliaCodici.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grigliaCodici.Size = new System.Drawing.Size(514, 409);
            this.grigliaCodici.TabIndex = 0;
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(469, 13);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(35, 13);
            this.lblCount.TabIndex = 1;
            this.lblCount.Text = "label1";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = global::MicriCancelli.Properties.Resources.refresh_40;
            this.btnRefresh.Location = new System.Drawing.Point(543, 389);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(52, 49);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // frmTabella
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 450);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.grigliaCodici);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTabella";
            this.ShowIcon = false;
            this.Text = "Tabella Codici";
            ((System.ComponentModel.ISupportInitialize)(this.grigliaCodici)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView grigliaCodici;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnRefresh;
    }
}