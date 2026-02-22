namespace nfa
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.accountsTable = new System.Windows.Forms.DataGridView();
            this.colUsername = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSteamId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.btnImportToken = new System.Windows.Forms.Button();
            this.btnResetSteam = new System.Windows.Forms.Button();
            this.btnStartSteam = new System.Windows.Forms.Button();
            this.btnRestoreConfig = new System.Windows.Forms.Button();
            this.btnSaveConfig = new System.Windows.Forms.Button();
            this.btnExportAccounts = new System.Windows.Forms.Button();
            this.btnKillSteam = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.mainLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.accountsTable)).BeginInit();
            this.controlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLayoutPanel
            // 
            this.mainLayoutPanel.ColumnCount = 2;
            this.mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78F));
            this.mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.mainLayoutPanel.Controls.Add(this.accountsTable, 0, 0);
            this.mainLayoutPanel.Controls.Add(this.controlPanel, 1, 0);
            this.mainLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainLayoutPanel.Name = "mainLayoutPanel";
            this.mainLayoutPanel.RowCount = 1;
            this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayoutPanel.Size = new System.Drawing.Size(1200, 500);
            this.mainLayoutPanel.TabIndex = 0;
            // 
            // accountsTable
            // 
            this.accountsTable.AllowDrop = true;
            this.accountsTable.AllowUserToAddRows = false;
            this.accountsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.accountsTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colUsername,
            this.colSteamId});
            this.accountsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.accountsTable.Location = new System.Drawing.Point(3, 3);
            this.accountsTable.Name = "accountsTable";
            this.accountsTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.accountsTable.Size = new System.Drawing.Size(930, 494);
            this.accountsTable.TabIndex = 0;
            // 
            // colUsername
            // 
            this.colUsername.HeaderText = "Username";
            this.colUsername.Name = "colUsername";
            this.colUsername.ReadOnly = true;
            this.colUsername.Width = 200;
            // 
            // colSteamId
            // 
            this.colSteamId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colSteamId.HeaderText = "Steam ID";
            this.colSteamId.Name = "colSteamId";
            this.colSteamId.ReadOnly = true;
            // 
            // controlPanel
            // 
            this.controlPanel.Controls.Add(this.btnImportToken);
            this.controlPanel.Controls.Add(this.btnResetSteam);
            this.controlPanel.Controls.Add(this.btnStartSteam);
            this.controlPanel.Controls.Add(this.btnRestoreConfig);
            this.controlPanel.Controls.Add(this.btnSaveConfig);
            this.controlPanel.Controls.Add(this.btnExportAccounts);
            this.controlPanel.Controls.Add(this.btnKillSteam);
            this.controlPanel.Controls.Add(this.lblStatus);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlPanel.Location = new System.Drawing.Point(939, 3);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(258, 494);
            this.controlPanel.TabIndex = 1;
            // 
            // btnImportToken
            // 
            this.btnImportToken.Location = new System.Drawing.Point(15, 20);
            this.btnImportToken.Name = "btnImportToken";
            this.btnImportToken.Size = new System.Drawing.Size(230, 40);
            this.btnImportToken.TabIndex = 0;
            this.btnImportToken.Text = "Import Token";
            this.btnImportToken.UseVisualStyleBackColor = true;
            // 
            // btnResetSteam
            // 
            this.btnResetSteam.Location = new System.Drawing.Point(15, 112);
            this.btnResetSteam.Name = "btnResetSteam";
            this.btnResetSteam.Size = new System.Drawing.Size(230, 40);
            this.btnResetSteam.TabIndex = 1;
            this.btnResetSteam.Text = "Reset Steam";
            this.btnResetSteam.UseVisualStyleBackColor = true;
            // 
            // btnStartSteam
            // 
            this.btnStartSteam.Location = new System.Drawing.Point(15, 158);
            this.btnStartSteam.Name = "btnStartSteam";
            this.btnStartSteam.Size = new System.Drawing.Size(230, 40);
            this.btnStartSteam.TabIndex = 2;
            this.btnStartSteam.Text = "Launch Steam";
            this.btnStartSteam.UseVisualStyleBackColor = true;
            // 
            // btnRestoreConfig
            // 
            this.btnRestoreConfig.Location = new System.Drawing.Point(15, 465);
            this.btnRestoreConfig.Name = "btnRestoreConfig";
            this.btnRestoreConfig.Size = new System.Drawing.Size(230, 26);
            this.btnRestoreConfig.TabIndex = 6;
            this.btnRestoreConfig.Text = "Restore Config";
            this.btnRestoreConfig.UseVisualStyleBackColor = true;
            // 
            // btnSaveConfig
            // 
            this.btnSaveConfig.Location = new System.Drawing.Point(15, 437);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(230, 26);
            this.btnSaveConfig.TabIndex = 5;
            this.btnSaveConfig.Text = "Save Config";
            this.btnSaveConfig.UseVisualStyleBackColor = true;
            // 
            // btnExportAccounts
            // 
            this.btnExportAccounts.Location = new System.Drawing.Point(15, 409);
            this.btnExportAccounts.Name = "btnExportAccounts";
            this.btnExportAccounts.Size = new System.Drawing.Size(230, 26);
            this.btnExportAccounts.TabIndex = 4;
            this.btnExportAccounts.Text = "Export Selected...";
            this.btnExportAccounts.UseVisualStyleBackColor = true;
            // 
            // btnKillSteam
            // 
            this.btnKillSteam.Location = new System.Drawing.Point(15, 381);
            this.btnKillSteam.Name = "btnKillSteam";
            this.btnKillSteam.Size = new System.Drawing.Size(230, 26);
            this.btnKillSteam.TabIndex = 3;
            this.btnKillSteam.Text = "Kill Steam Process";
            this.btnKillSteam.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStatus.Location = new System.Drawing.Point(15, 320);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(230, 55);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "Status: Not Logged in.";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 500);
            this.Controls.Add(this.mainLayoutPanel);
            this.Name = "MainForm";
            this.Text = "NFA Account Manager";
            this.mainLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.accountsTable)).EndInit();
            this.controlPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainLayoutPanel;
        private System.Windows.Forms.DataGridView accountsTable;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.Button btnImportToken;
        private System.Windows.Forms.Button btnResetSteam;
        private System.Windows.Forms.Button btnStartSteam;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnKillSteam;
        private System.Windows.Forms.Button btnExportAccounts;
        private System.Windows.Forms.Button btnSaveConfig;
        private System.Windows.Forms.Button btnRestoreConfig;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUsername;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSteamId;
    }
}
