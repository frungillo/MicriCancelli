namespace MicriCancelli
{
    partial class frmParametri
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
            this.components = new System.ComponentModel.Container();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnSblocca = new System.Windows.Forms.Button();
            this.tblParametri = new System.Windows.Forms.TableLayoutPanel();
            this.lblAvviso = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblPassword
            //
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(12, 15);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 0;
            this.lblPassword.Text = "Password:";
            //
            // txtPassword
            //
            this.txtPassword.Location = new System.Drawing.Point(74, 12);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(200, 20);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.UseSystemPasswordChar = true;
            //
            // btnSblocca
            //
            this.btnSblocca.Location = new System.Drawing.Point(280, 9);
            this.btnSblocca.Name = "btnSblocca";
            this.btnSblocca.Size = new System.Drawing.Size(120, 26);
            this.btnSblocca.TabIndex = 2;
            this.btnSblocca.Text = "Sblocca parametri";
            this.btnSblocca.UseVisualStyleBackColor = true;
            this.btnSblocca.Click += new System.EventHandler(this.btnSblocca_Click);
            //
            // tblParametri
            //
            this.tblParametri.AutoScroll = true;
            this.tblParametri.ColumnCount = 3;
            this.tblParametri.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210F));
            this.tblParametri.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 280F));
            this.tblParametri.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tblParametri.Location = new System.Drawing.Point(12, 48);
            this.tblParametri.Name = "tblParametri";
            this.tblParametri.Size = new System.Drawing.Size(560, 515);
            this.tblParametri.TabIndex = 3;
            //
            // lblAvviso
            //
            this.lblAvviso.AutoSize = true;
            this.lblAvviso.ForeColor = System.Drawing.Color.Firebrick;
            this.lblAvviso.Location = new System.Drawing.Point(12, 572);
            this.lblAvviso.Name = "lblAvviso";
            this.lblAvviso.Size = new System.Drawing.Size(300, 13);
            this.lblAvviso.TabIndex = 4;
            this.lblAvviso.Text = "Ogni valore viene salvato con il proprio pulsante e applicato subito.";
            //
            // btnClose
            //
            this.btnClose.Image = global::MicriCancelli.Properties.Resources.close_100;
            this.btnClose.Location = new System.Drawing.Point(540, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(32, 32);
            this.btnClose.TabIndex = 5;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // frmParametri
            //
            this.AcceptButton = this.btnSblocca;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 594);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblAvviso);
            this.Controls.Add(this.tblParametri);
            this.Controls.Add(this.btnSblocca);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmParametri";
            this.ShowIcon = false;
            this.Text = "Parametri";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnSblocca;
        private System.Windows.Forms.TableLayoutPanel tblParametri;
        private System.Windows.Forms.Label lblAvviso;
        private System.Windows.Forms.Button btnClose;
    }
}
