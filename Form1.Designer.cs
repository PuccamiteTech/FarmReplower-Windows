namespace FarmReplower
{
    partial class frmSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSetup));
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnInstall = new System.Windows.Forms.Button();
            this.grpConfiguration = new System.Windows.Forms.GroupBox();
            this.txtInstallationPath = new System.Windows.Forms.TextBox();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.txtDatabaseName = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtStatus = new System.Windows.Forms.RichTextBox();
            this.btnSetSearchPath = new System.Windows.Forms.Button();
            this.searchPathDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.grpConfiguration.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(215, 11);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(138, 29);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Title Label";
            // 
            // btnInstall
            // 
            this.btnInstall.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInstall.Location = new System.Drawing.Point(12, 315);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(295, 35);
            this.btnInstall.TabIndex = 8;
            this.btnInstall.Text = "Install Button";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // grpConfiguration
            // 
            this.grpConfiguration.Controls.Add(this.txtInstallationPath);
            this.grpConfiguration.Controls.Add(this.txtConfirmPassword);
            this.grpConfiguration.Controls.Add(this.txtDatabaseName);
            this.grpConfiguration.Controls.Add(this.txtPassword);
            this.grpConfiguration.Controls.Add(this.txtUsername);
            this.grpConfiguration.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpConfiguration.Location = new System.Drawing.Point(12, 45);
            this.grpConfiguration.Name = "grpConfiguration";
            this.grpConfiguration.Size = new System.Drawing.Size(600, 110);
            this.grpConfiguration.TabIndex = 1;
            this.grpConfiguration.TabStop = false;
            this.grpConfiguration.Text = "Configuration Group";
            // 
            // txtInstallationPath
            // 
            this.txtInstallationPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInstallationPath.Location = new System.Drawing.Point(208, 59);
            this.txtInstallationPath.Name = "txtInstallationPath";
            this.txtInstallationPath.Size = new System.Drawing.Size(376, 26);
            this.txtInstallationPath.TabIndex = 6;
            this.txtInstallationPath.Text = "Installation Path Text";
            this.txtInstallationPath.Enter += new System.EventHandler(this.txt_Enter);
            this.txtInstallationPath.Leave += new System.EventHandler(this.txt_Leave);
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConfirmPassword.Location = new System.Drawing.Point(399, 29);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.Size = new System.Drawing.Size(185, 26);
            this.txtConfirmPassword.TabIndex = 4;
            this.txtConfirmPassword.Text = "Confirm Password Text";
            this.txtConfirmPassword.Enter += new System.EventHandler(this.txt_EnterSensitive);
            this.txtConfirmPassword.Leave += new System.EventHandler(this.txt_Leave);
            // 
            // txtDatabaseName
            // 
            this.txtDatabaseName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDatabaseName.Location = new System.Drawing.Point(17, 59);
            this.txtDatabaseName.Name = "txtDatabaseName";
            this.txtDatabaseName.Size = new System.Drawing.Size(185, 26);
            this.txtDatabaseName.TabIndex = 5;
            this.txtDatabaseName.Text = "Database Name Text";
            this.txtDatabaseName.Enter += new System.EventHandler(this.txt_Enter);
            this.txtDatabaseName.Leave += new System.EventHandler(this.txt_Leave);
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(208, 29);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(185, 26);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.Text = "Password Text";
            this.txtPassword.Enter += new System.EventHandler(this.txt_EnterSensitive);
            this.txtPassword.Leave += new System.EventHandler(this.txt_Leave);
            // 
            // txtUsername
            // 
            this.txtUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsername.Location = new System.Drawing.Point(17, 29);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(185, 26);
            this.txtUsername.TabIndex = 2;
            this.txtUsername.Text = "Username Text";
            this.txtUsername.Enter += new System.EventHandler(this.txt_Enter);
            this.txtUsername.Leave += new System.EventHandler(this.txt_Leave);
            // 
            // txtStatus
            // 
            this.txtStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStatus.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtStatus.Location = new System.Drawing.Point(12, 160);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(600, 150);
            this.txtStatus.TabIndex = 7;
            this.txtStatus.Text = "Status Text";
            this.txtStatus.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.txtStatus_LinkClicked);
            // 
            // btnSetSearchPath
            // 
            this.btnSetSearchPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetSearchPath.Location = new System.Drawing.Point(317, 315);
            this.btnSetSearchPath.Name = "btnSetSearchPath";
            this.btnSetSearchPath.Size = new System.Drawing.Size(295, 35);
            this.btnSetSearchPath.TabIndex = 9;
            this.btnSetSearchPath.Text = "Set Search Path Button";
            this.btnSetSearchPath.UseVisualStyleBackColor = true;
            this.btnSetSearchPath.Click += new System.EventHandler(this.btnSetSearchPath_Click);
            // 
            // frmSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 361);
            this.Controls.Add(this.btnSetSearchPath);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.grpConfiguration);
            this.Controls.Add(this.btnInstall);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmSetup";
            this.Text = "FV Replowed Setup";
            this.Load += new System.EventHandler(this.frmSetup_Load);
            this.grpConfiguration.ResumeLayout(false);
            this.grpConfiguration.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.GroupBox grpConfiguration;
        private System.Windows.Forms.TextBox txtInstallationPath;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.TextBox txtDatabaseName;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.RichTextBox txtStatus;
        private System.Windows.Forms.Button btnSetSearchPath;
        private System.Windows.Forms.FolderBrowserDialog searchPathDialog;
    }
}

