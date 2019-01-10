namespace EpServerEngineSampleClient
{
    partial class SignUpForm
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
            this.PB_SIGN_OK = new System.Windows.Forms.PictureBox();
            this.PB_SIGN_CANCEL = new System.Windows.Forms.PictureBox();
            this.TB_INPUT_SIGN_ID = new System.Windows.Forms.TextBox();
            this.TB_INPUT_SIGN_PWD = new System.Windows.Forms.TextBox();
            this.TB_INPUT_SIGN_PWD2 = new System.Windows.Forms.TextBox();
            this.TB_INPUT_SIGN_NAME = new System.Windows.Forms.TextBox();
            this.TB_INPUT_SIGN_E_MAIL = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.PB_SIGN_OK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_SIGN_CANCEL)).BeginInit();
            this.SuspendLayout();
            // 
            // PB_SIGN_OK
            // 
            this.PB_SIGN_OK.Image = global::EpServerEngineSampleClient.Properties.Resources.SIGNUP_OK;
            this.PB_SIGN_OK.Location = new System.Drawing.Point(64, 401);
            this.PB_SIGN_OK.Name = "PB_SIGN_OK";
            this.PB_SIGN_OK.Size = new System.Drawing.Size(120, 45);
            this.PB_SIGN_OK.TabIndex = 0;
            this.PB_SIGN_OK.TabStop = false;
            // 
            // PB_SIGN_CANCEL
            // 
            this.PB_SIGN_CANCEL.Image = global::EpServerEngineSampleClient.Properties.Resources.SIGNUP_CANCEL;
            this.PB_SIGN_CANCEL.Location = new System.Drawing.Point(231, 401);
            this.PB_SIGN_CANCEL.Name = "PB_SIGN_CANCEL";
            this.PB_SIGN_CANCEL.Size = new System.Drawing.Size(120, 45);
            this.PB_SIGN_CANCEL.TabIndex = 1;
            this.PB_SIGN_CANCEL.TabStop = false;
            this.PB_SIGN_CANCEL.Click += new System.EventHandler(this.PB_SIGN_CANCEL_Click);
            // 
            // TB_INPUT_SIGN_ID
            // 
            this.TB_INPUT_SIGN_ID.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_INPUT_SIGN_ID.Location = new System.Drawing.Point(175, 65);
            this.TB_INPUT_SIGN_ID.Name = "TB_INPUT_SIGN_ID";
            this.TB_INPUT_SIGN_ID.Size = new System.Drawing.Size(198, 33);
            this.TB_INPUT_SIGN_ID.TabIndex = 2;
            // 
            // TB_INPUT_SIGN_PWD
            // 
            this.TB_INPUT_SIGN_PWD.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_INPUT_SIGN_PWD.Location = new System.Drawing.Point(175, 118);
            this.TB_INPUT_SIGN_PWD.Name = "TB_INPUT_SIGN_PWD";
            this.TB_INPUT_SIGN_PWD.Size = new System.Drawing.Size(198, 33);
            this.TB_INPUT_SIGN_PWD.TabIndex = 3;
            // 
            // TB_INPUT_SIGN_PWD2
            // 
            this.TB_INPUT_SIGN_PWD2.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_INPUT_SIGN_PWD2.Location = new System.Drawing.Point(175, 177);
            this.TB_INPUT_SIGN_PWD2.Name = "TB_INPUT_SIGN_PWD2";
            this.TB_INPUT_SIGN_PWD2.Size = new System.Drawing.Size(198, 33);
            this.TB_INPUT_SIGN_PWD2.TabIndex = 4;
            // 
            // TB_INPUT_SIGN_NAME
            // 
            this.TB_INPUT_SIGN_NAME.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_INPUT_SIGN_NAME.Location = new System.Drawing.Point(175, 234);
            this.TB_INPUT_SIGN_NAME.Name = "TB_INPUT_SIGN_NAME";
            this.TB_INPUT_SIGN_NAME.Size = new System.Drawing.Size(198, 33);
            this.TB_INPUT_SIGN_NAME.TabIndex = 5;
            // 
            // TB_INPUT_SIGN_E_MAIL
            // 
            this.TB_INPUT_SIGN_E_MAIL.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_INPUT_SIGN_E_MAIL.Location = new System.Drawing.Point(175, 283);
            this.TB_INPUT_SIGN_E_MAIL.Name = "TB_INPUT_SIGN_E_MAIL";
            this.TB_INPUT_SIGN_E_MAIL.Size = new System.Drawing.Size(198, 33);
            this.TB_INPUT_SIGN_E_MAIL.TabIndex = 6;
            // 
            // SignUpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::EpServerEngineSampleClient.Properties.Resources.SignUp_Board;
            this.ClientSize = new System.Drawing.Size(394, 507);
            this.Controls.Add(this.TB_INPUT_SIGN_E_MAIL);
            this.Controls.Add(this.TB_INPUT_SIGN_NAME);
            this.Controls.Add(this.TB_INPUT_SIGN_PWD2);
            this.Controls.Add(this.TB_INPUT_SIGN_PWD);
            this.Controls.Add(this.TB_INPUT_SIGN_ID);
            this.Controls.Add(this.PB_SIGN_CANCEL);
            this.Controls.Add(this.PB_SIGN_OK);
            this.Name = "SignUpForm";
            this.Text = "SignUpForm";
            ((System.ComponentModel.ISupportInitialize)(this.PB_SIGN_OK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_SIGN_CANCEL)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PB_SIGN_OK;
        private System.Windows.Forms.PictureBox PB_SIGN_CANCEL;
        private System.Windows.Forms.TextBox TB_INPUT_SIGN_ID;
        private System.Windows.Forms.TextBox TB_INPUT_SIGN_PWD;
        private System.Windows.Forms.TextBox TB_INPUT_SIGN_PWD2;
        private System.Windows.Forms.TextBox TB_INPUT_SIGN_NAME;
        private System.Windows.Forms.TextBox TB_INPUT_SIGN_E_MAIL;
    }
}