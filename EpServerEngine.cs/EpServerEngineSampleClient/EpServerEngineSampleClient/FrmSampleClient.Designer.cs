namespace EpServerEngineSampleClient
{
    partial class FrmSampleClient
    {
        
        /// Required designer variable.
        
        private System.ComponentModel.IContainer components = null;

        
        /// Clean up any resources being used.
        
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

        
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSampleClient));
            this.TB_ID_INPUT = new System.Windows.Forms.MaskedTextBox();
            this.TB_PWD_INPUT = new System.Windows.Forms.MaskedTextBox();
            this.PB_LOGIN = new System.Windows.Forms.PictureBox();
            this.PB_DEV_INFO = new System.Windows.Forms.PictureBox();
            this.PB_ID_AND_PWD_SEARCH = new System.Windows.Forms.PictureBox();
            this.PB_ADD_USER = new System.Windows.Forms.PictureBox();
            this.PB_PWD = new System.Windows.Forms.PictureBox();
            this.PB_ID_BOX = new System.Windows.Forms.PictureBox();
            this.PBLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PB_LOGIN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_DEV_INFO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_ID_AND_PWD_SEARCH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_ADD_USER)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_PWD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_ID_BOX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // TB_ID_INPUT
            // 
            this.TB_ID_INPUT.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TB_ID_INPUT.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TB_ID_INPUT.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_ID_INPUT.Location = new System.Drawing.Point(95, 225);
            this.TB_ID_INPUT.Name = "TB_ID_INPUT";
            this.TB_ID_INPUT.Size = new System.Drawing.Size(211, 18);
            this.TB_ID_INPUT.TabIndex = 19;
            this.TB_ID_INPUT.Enter += new System.EventHandler(this.TB_INPUT_Enter);
            // 
            // TB_PWD_INPUT
            // 
            this.TB_PWD_INPUT.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TB_PWD_INPUT.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TB_PWD_INPUT.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_PWD_INPUT.Location = new System.Drawing.Point(95, 272);
            this.TB_PWD_INPUT.Name = "TB_PWD_INPUT";
            this.TB_PWD_INPUT.Size = new System.Drawing.Size(211, 18);
            this.TB_PWD_INPUT.TabIndex = 21;
            this.TB_PWD_INPUT.Enter += new System.EventHandler(this.TB_PWD_INPUT_Enter);
            // 
            // PB_LOGIN
            // 
            this.PB_LOGIN.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PB_LOGIN.Image = global::EpServerEngineSampleClient.Properties.Resources.LOG_IN;
            this.PB_LOGIN.Location = new System.Drawing.Point(100, 328);
            this.PB_LOGIN.Name = "PB_LOGIN";
            this.PB_LOGIN.Size = new System.Drawing.Size(200, 50);
            this.PB_LOGIN.TabIndex = 25;
            this.PB_LOGIN.TabStop = false;
            this.PB_LOGIN.Click += new System.EventHandler(this.PB_LOGIN_Click);
            // 
            // PB_DEV_INFO
            // 
            this.PB_DEV_INFO.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PB_DEV_INFO.Image = global::EpServerEngineSampleClient.Properties.Resources.DEV_INFO;
            this.PB_DEV_INFO.Location = new System.Drawing.Point(153, 482);
            this.PB_DEV_INFO.Name = "PB_DEV_INFO";
            this.PB_DEV_INFO.Size = new System.Drawing.Size(92, 23);
            this.PB_DEV_INFO.TabIndex = 24;
            this.PB_DEV_INFO.TabStop = false;
            // 
            // PB_ID_AND_PWD_SEARCH
            // 
            this.PB_ID_AND_PWD_SEARCH.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PB_ID_AND_PWD_SEARCH.Image = global::EpServerEngineSampleClient.Properties.Resources.ID_AND_PWD_SEARCH;
            this.PB_ID_AND_PWD_SEARCH.Location = new System.Drawing.Point(122, 396);
            this.PB_ID_AND_PWD_SEARCH.Name = "PB_ID_AND_PWD_SEARCH";
            this.PB_ID_AND_PWD_SEARCH.Size = new System.Drawing.Size(157, 23);
            this.PB_ID_AND_PWD_SEARCH.TabIndex = 23;
            this.PB_ID_AND_PWD_SEARCH.TabStop = false;
            // 
            // PB_ADD_USER
            // 
            this.PB_ADD_USER.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PB_ADD_USER.Image = global::EpServerEngineSampleClient.Properties.Resources.ADD_USER;
            this.PB_ADD_USER.Location = new System.Drawing.Point(157, 425);
            this.PB_ADD_USER.Name = "PB_ADD_USER";
            this.PB_ADD_USER.Size = new System.Drawing.Size(92, 23);
            this.PB_ADD_USER.TabIndex = 22;
            this.PB_ADD_USER.TabStop = false;
            this.PB_ADD_USER.Click += new System.EventHandler(this.PB_ADD_USER_Click);
            // 
            // PB_PWD
            // 
            this.PB_PWD.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PB_PWD.Image = global::EpServerEngineSampleClient.Properties.Resources.ID_BOX;
            this.PB_PWD.InitialImage = ((System.Drawing.Image)(resources.GetObject("PB_PWD.InitialImage")));
            this.PB_PWD.Location = new System.Drawing.Point(83, 258);
            this.PB_PWD.Name = "PB_PWD";
            this.PB_PWD.Size = new System.Drawing.Size(234, 50);
            this.PB_PWD.TabIndex = 20;
            this.PB_PWD.TabStop = false;
            // 
            // PB_ID_BOX
            // 
            this.PB_ID_BOX.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PB_ID_BOX.Image = global::EpServerEngineSampleClient.Properties.Resources.ID_BOX;
            this.PB_ID_BOX.InitialImage = ((System.Drawing.Image)(resources.GetObject("PB_ID_BOX.InitialImage")));
            this.PB_ID_BOX.Location = new System.Drawing.Point(83, 211);
            this.PB_ID_BOX.Name = "PB_ID_BOX";
            this.PB_ID_BOX.Size = new System.Drawing.Size(234, 50);
            this.PB_ID_BOX.TabIndex = 17;
            this.PB_ID_BOX.TabStop = false;
            // 
            // PBLogo
            // 
            this.PBLogo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PBLogo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("PBLogo.BackgroundImage")));
            this.PBLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.PBLogo.Location = new System.Drawing.Point(33, 22);
            this.PBLogo.Name = "PBLogo";
            this.PBLogo.Size = new System.Drawing.Size(367, 221);
            this.PBLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.PBLogo.TabIndex = 16;
            this.PBLogo.TabStop = false;
            // 
            // FrmSampleClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.ClientSize = new System.Drawing.Size(394, 507);
            this.Controls.Add(this.PB_LOGIN);
            this.Controls.Add(this.PB_DEV_INFO);
            this.Controls.Add(this.PB_ID_AND_PWD_SEARCH);
            this.Controls.Add(this.PB_ADD_USER);
            this.Controls.Add(this.TB_PWD_INPUT);
            this.Controls.Add(this.PB_PWD);
            this.Controls.Add(this.TB_ID_INPUT);
            this.Controls.Add(this.PB_ID_BOX);
            this.Controls.Add(this.PBLogo);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FrmSampleClient";
            this.Text = "H Talk";
            this.Load += new System.EventHandler(this.FrmSampleClient_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PB_LOGIN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_DEV_INFO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_ID_AND_PWD_SEARCH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_ADD_USER)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_PWD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_ID_BOX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox PBLogo;
        private System.Windows.Forms.PictureBox PB_ID_BOX;
        private System.Windows.Forms.MaskedTextBox TB_ID_INPUT;
        private System.Windows.Forms.MaskedTextBox TB_PWD_INPUT;
        private System.Windows.Forms.PictureBox PB_PWD;
        private System.Windows.Forms.PictureBox PB_ADD_USER;
        private System.Windows.Forms.PictureBox PB_ID_AND_PWD_SEARCH;
        private System.Windows.Forms.PictureBox PB_DEV_INFO;
        private System.Windows.Forms.PictureBox PB_LOGIN;
    }
}

