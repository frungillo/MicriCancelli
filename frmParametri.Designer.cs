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
            this.label1 = new System.Windows.Forms.Label();
            this.txtKeyEnd = new System.Windows.Forms.TextBox();
            this.txtCodeLen = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMinutiValidita = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIPArduino = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPortaArduino = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSaveKeyEnd = new System.Windows.Forms.Button();
            this.btnSalvaMinutiValidita = new System.Windows.Forms.Button();
            this.btnSalvaCodeLen = new System.Windows.Forms.Button();
            this.btnSalvaIPArduino = new System.Windows.Forms.Button();
            this.btnSalvaPortaArduino = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.grpParametri = new System.Windows.Forms.GroupBox();
            this.btnSblocca = new System.Windows.Forms.Button();
            this.grpParametri.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Carattere di Terminazione";
            // 
            // txtKeyEnd
            // 
            this.txtKeyEnd.Enabled = false;
            this.txtKeyEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtKeyEnd.Location = new System.Drawing.Point(142, 19);
            this.txtKeyEnd.Name = "txtKeyEnd";
            this.txtKeyEnd.Size = new System.Drawing.Size(100, 29);
            this.txtKeyEnd.TabIndex = 1;
            this.txtKeyEnd.Tag = "KeyEnd";
            // 
            // txtCodeLen
            // 
            this.txtCodeLen.Enabled = false;
            this.txtCodeLen.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodeLen.Location = new System.Drawing.Point(142, 78);
            this.txtCodeLen.Name = "txtCodeLen";
            this.txtCodeLen.Size = new System.Drawing.Size(100, 29);
            this.txtCodeLen.TabIndex = 3;
            this.txtCodeLen.Tag = "code_len";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Lunghezza Barcode";
            // 
            // txtMinutiValidita
            // 
            this.txtMinutiValidita.Enabled = false;
            this.txtMinutiValidita.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMinutiValidita.Location = new System.Drawing.Point(142, 132);
            this.txtMinutiValidita.Name = "txtMinutiValidita";
            this.txtMinutiValidita.Size = new System.Drawing.Size(100, 29);
            this.txtMinutiValidita.TabIndex = 5;
            this.txtMinutiValidita.Tag = "minutiValidita";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Minuti Validità";
            // 
            // txtIPArduino
            // 
            this.txtIPArduino.Enabled = false;
            this.txtIPArduino.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIPArduino.Location = new System.Drawing.Point(142, 193);
            this.txtIPArduino.Name = "txtIPArduino";
            this.txtIPArduino.Size = new System.Drawing.Size(100, 29);
            this.txtIPArduino.TabIndex = 7;
            this.txtIPArduino.Tag = "ipArduino";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 201);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Indirizzo IP Arduino";
            // 
            // txtPortaArduino
            // 
            this.txtPortaArduino.Enabled = false;
            this.txtPortaArduino.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPortaArduino.Location = new System.Drawing.Point(142, 254);
            this.txtPortaArduino.Name = "txtPortaArduino";
            this.txtPortaArduino.Size = new System.Drawing.Size(100, 29);
            this.txtPortaArduino.TabIndex = 9;
            this.txtPortaArduino.Tag = "portaArduino";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 262);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Porta Arduino";
            // 
            // btnSaveKeyEnd
            // 
            this.btnSaveKeyEnd.Enabled = false;
            this.btnSaveKeyEnd.Image = global::MicriCancelli.Properties.Resources.save_30;
            this.btnSaveKeyEnd.Location = new System.Drawing.Point(249, 18);
            this.btnSaveKeyEnd.Name = "btnSaveKeyEnd";
            this.btnSaveKeyEnd.Size = new System.Drawing.Size(31, 31);
            this.btnSaveKeyEnd.TabIndex = 12;
            this.btnSaveKeyEnd.UseVisualStyleBackColor = true;
            // 
            // btnSalvaMinutiValidita
            // 
            this.btnSalvaMinutiValidita.Enabled = false;
            this.btnSalvaMinutiValidita.Image = global::MicriCancelli.Properties.Resources.save_30;
            this.btnSalvaMinutiValidita.Location = new System.Drawing.Point(249, 132);
            this.btnSalvaMinutiValidita.Name = "btnSalvaMinutiValidita";
            this.btnSalvaMinutiValidita.Size = new System.Drawing.Size(31, 31);
            this.btnSalvaMinutiValidita.TabIndex = 13;
            this.btnSalvaMinutiValidita.Tag = "minutiValidita";
            this.btnSalvaMinutiValidita.UseVisualStyleBackColor = true;
            // 
            // btnSalvaCodeLen
            // 
            this.btnSalvaCodeLen.Enabled = false;
            this.btnSalvaCodeLen.Image = global::MicriCancelli.Properties.Resources.save_30;
            this.btnSalvaCodeLen.Location = new System.Drawing.Point(249, 77);
            this.btnSalvaCodeLen.Name = "btnSalvaCodeLen";
            this.btnSalvaCodeLen.Size = new System.Drawing.Size(31, 31);
            this.btnSalvaCodeLen.TabIndex = 14;
            this.btnSalvaCodeLen.Tag = "code_len";
            this.btnSalvaCodeLen.UseVisualStyleBackColor = true;
            // 
            // btnSalvaIPArduino
            // 
            this.btnSalvaIPArduino.Enabled = false;
            this.btnSalvaIPArduino.Image = global::MicriCancelli.Properties.Resources.save_30;
            this.btnSalvaIPArduino.Location = new System.Drawing.Point(249, 192);
            this.btnSalvaIPArduino.Name = "btnSalvaIPArduino";
            this.btnSalvaIPArduino.Size = new System.Drawing.Size(31, 31);
            this.btnSalvaIPArduino.TabIndex = 15;
            this.btnSalvaIPArduino.Tag = "ipArduino";
            this.btnSalvaIPArduino.UseVisualStyleBackColor = true;
            // 
            // btnSalvaPortaArduino
            // 
            this.btnSalvaPortaArduino.Enabled = false;
            this.btnSalvaPortaArduino.Image = global::MicriCancelli.Properties.Resources.save_30;
            this.btnSalvaPortaArduino.Location = new System.Drawing.Point(249, 253);
            this.btnSalvaPortaArduino.Name = "btnSalvaPortaArduino";
            this.btnSalvaPortaArduino.Size = new System.Drawing.Size(31, 31);
            this.btnSalvaPortaArduino.TabIndex = 16;
            this.btnSalvaPortaArduino.Tag = "portaArduino";
            this.btnSalvaPortaArduino.UseVisualStyleBackColor = true;
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(29, 19);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(148, 44);
            this.txtPassword.TabIndex = 17;
            this.txtPassword.Tag = "KeyEnd";
            // 
            // grpParametri
            // 
            this.grpParametri.Controls.Add(this.txtCodeLen);
            this.grpParametri.Controls.Add(this.label1);
            this.grpParametri.Controls.Add(this.btnSalvaPortaArduino);
            this.grpParametri.Controls.Add(this.txtKeyEnd);
            this.grpParametri.Controls.Add(this.btnSalvaIPArduino);
            this.grpParametri.Controls.Add(this.label2);
            this.grpParametri.Controls.Add(this.btnSalvaCodeLen);
            this.grpParametri.Controls.Add(this.label3);
            this.grpParametri.Controls.Add(this.btnSalvaMinutiValidita);
            this.grpParametri.Controls.Add(this.txtMinutiValidita);
            this.grpParametri.Controls.Add(this.btnSaveKeyEnd);
            this.grpParametri.Controls.Add(this.label4);
            this.grpParametri.Controls.Add(this.txtIPArduino);
            this.grpParametri.Controls.Add(this.label5);
            this.grpParametri.Controls.Add(this.txtPortaArduino);
            this.grpParametri.Location = new System.Drawing.Point(29, 138);
            this.grpParametri.Name = "grpParametri";
            this.grpParametri.Size = new System.Drawing.Size(742, 300);
            this.grpParametri.TabIndex = 18;
            this.grpParametri.TabStop = false;
            // 
            // btnSblocca
            // 
            this.btnSblocca.BackColor = System.Drawing.Color.White;
            this.btnSblocca.Image = global::MicriCancelli.Properties.Resources.settings_100;
            this.btnSblocca.Location = new System.Drawing.Point(193, 19);
            this.btnSblocca.Name = "btnSblocca";
            this.btnSblocca.Size = new System.Drawing.Size(101, 101);
            this.btnSblocca.TabIndex = 19;
            this.btnSblocca.UseVisualStyleBackColor = false;
            this.btnSblocca.Click += new System.EventHandler(this.btnSblocca_Click);
            // 
            // frmParametri
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnSblocca);
            this.Controls.Add(this.grpParametri);
            this.Controls.Add(this.txtPassword);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MinimizeBox = false;
            this.Name = "frmParametri";
            this.ShowIcon = false;
            this.Text = "Parametri";
            this.grpParametri.ResumeLayout(false);
            this.grpParametri.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtKeyEnd;
        private System.Windows.Forms.TextBox txtCodeLen;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMinutiValidita;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtIPArduino;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPortaArduino;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSaveKeyEnd;
        private System.Windows.Forms.Button btnSalvaMinutiValidita;
        private System.Windows.Forms.Button btnSalvaCodeLen;
        private System.Windows.Forms.Button btnSalvaIPArduino;
        private System.Windows.Forms.Button btnSalvaPortaArduino;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.GroupBox grpParametri;
        private System.Windows.Forms.Button btnSblocca;
    }
}