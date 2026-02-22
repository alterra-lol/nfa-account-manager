using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace nfa
{
    public partial class MainForm : Form
    {
        private DataStore dataStore;

        public MainForm()
        {
            InitializeComponent();
            this.ShowIcon = false;
            InitializeApp();
        }

        private void InitializeApp()
        {
            dataStore = new DataStore();
            LoadAccountsFromStorage();
            RefreshStatus();
            AttachEventHandlers();
        }

        private void AttachEventHandlers()
        {
            btnImportToken.Click += BtnImportToken_Click;
            btnResetSteam.Click += BtnResetSteam_Click;
            btnStartSteam.Click += BtnStartSteam_Click;
            btnSaveConfig.Click += BtnSaveConfig_Click;
            btnRestoreConfig.Click += BtnRestoreConfig_Click;
            btnExportAccounts.Click += BtnExportAccounts_Click;
            btnKillSteam.Click += BtnKillSteam_Click;

            accountsTable.DragEnter += AccountsTable_DragEnter;
            accountsTable.DragDrop += AccountsTable_DragDrop;
            accountsTable.MouseDown += AccountsTable_MouseDown;
            accountsTable.KeyDown += AccountsTable_KeyDown;
        }

        private void LoadAccountsFromStorage()
        {
            var accounts = dataStore.GetAllAccounts();
            foreach (var account in accounts)
            {
                AddAccountRow(account.Username, account.SteamId);
            }
        }

        private void RefreshStatus()
        {
            string currentLogin = ConfigHelper.GetCurrentAccount();
            lblStatus.Text = !string.IsNullOrEmpty(currentLogin) 
                ? $"Logged in:\n{currentLogin}" 
                : "Status:\nNot logged in";
        }

        private void AddAccountRow(string username, string steamid)
        {
            int rowIndex = accountsTable.Rows.Add();
            accountsTable.Rows[rowIndex].Cells[0].Value = username;
            accountsTable.Rows[rowIndex].Cells[1].Value = steamid;
        }

        private void BtnImportToken_Click(object sender, EventArgs e)
        {
            using (var dialog = new TokenImportDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string inputText = dialog.TokenText;
                    if (!string.IsNullOrWhiteSpace(inputText))
                    {
                        string[] lines = inputText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        int imported = 0;
                        foreach (string line in lines)
                        {
                            if (ImportAccountFromLine(line))
                                imported++;
                        }
                        
                        if (imported > 0)
                            MessageBox.Show($"Imported {imported} account(s)", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void BtnLoadInventory_Click(object sender, EventArgs e)
        {
            RefreshStatus();
        }

        private void BtnResetSteam_Click(object sender, EventArgs e)
        {
            ConfigHelper.ResetSteam();
            RefreshStatus();
        }

        private void BtnStartSteam_Click(object sender, EventArgs e)
        {
            ConfigHelper.StartSteam();
            RefreshStatus();
        }

        private void BtnKillSteam_Click(object sender, EventArgs e)
        {
            ConfigHelper.KillSteam();
            RefreshStatus();
        }

        private void BtnSaveConfig_Click(object sender, EventArgs e)
        {
            ConfigHelper.SaveCurrentAccounts();
            MessageBox.Show("Configuration saved to backup folder", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshStatus();
        }

        private void BtnRestoreConfig_Click(object sender, EventArgs e)
        {
            ConfigHelper.RestoreSavedAccounts();
            RefreshStatus();
        }

        private void BtnExportAccounts_Click(object sender, EventArgs e)
        {
            if (accountsTable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select accounts to export", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                dialog.Title = "Export Accounts";
                dialog.FileName = "accounts.txt";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(dialog.FileName, false, System.Text.Encoding.UTF8))
                        {
                            foreach (DataGridViewRow row in accountsTable.SelectedRows)
                            {
                                string username = row.Cells[0].Value?.ToString();
                                if (!string.IsNullOrEmpty(username))
                                {
                                    var account = dataStore.GetAccount(username);
                                    if (account != null)
                                    {
                                        writer.WriteLine($"{account.Username}----{account.Token}");
                                    }
                                }
                            }
                        }
                        MessageBox.Show($"Exported {accountsTable.SelectedRows.Count} account(s)", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void AccountsTable_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void AccountsTable_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (file.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    ImportFromFile(file);
                }
            }
        }

        private void ImportFromFile(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);
                int imported = 0;
                foreach (string line in lines)
                {
                    if (ImportAccountFromLine(line))
                        imported++;
                }
                
                if (imported > 0)
                    MessageBox.Show($"Imported {imported} account(s) from file", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"File import error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AccountsTable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteFromClipboard();
                e.Handled = true;
            }
        }

        private void PasteFromClipboard()
        {
            try
            {
                string clipboardText = Clipboard.GetText();
                if (string.IsNullOrEmpty(clipboardText))
                    return;

                string[] lines = clipboardText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                int imported = 0;
                foreach (string line in lines)
                {
                    if (ImportAccountFromLine(line))
                        imported++;
                }
                
                if (imported > 0)
                    MessageBox.Show($"Imported {imported} account(s) from clipboard", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Paste error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ImportAccountFromLine(string line)
        {
            string[] parts = line.Trim().Split(new[] { "----" }, StringSplitOptions.None);
            if (parts.Length >= 2)
            {
                string username = parts[0].Trim();
                string token = parts[1].Trim();
                string steamid = TokenParser.GetSteamIdFromToken(token);

                if (!string.IsNullOrEmpty(steamid))
                {
                    dataStore.AddAccount(username, token, steamid);
                    AddAccountRow(username, steamid);
                    return true;
                }
            }
            return false;
        }

        private void AccountsTable_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hit = accountsTable.HitTest(e.X, e.Y);
                if (hit.RowIndex >= 0)
                {
                    if (!accountsTable.Rows[hit.RowIndex].Selected)
                    {
                        accountsTable.ClearSelection();
                        accountsTable.Rows[hit.RowIndex].Selected = true;
                    }

                    ContextMenuStrip contextMenu = new ContextMenuStrip();
                    
                    ToolStripMenuItem loginMenuItem = new ToolStripMenuItem("Login with this account");
                    loginMenuItem.Click += (s, args) => PerformLogin();
                    contextMenu.Items.Add(loginMenuItem);

                    ToolStripMenuItem browserMenuItem = new ToolStripMenuItem("Open profile in browser");
                    browserMenuItem.Click += (s, args) => OpenProfileInBrowser();
                    contextMenu.Items.Add(browserMenuItem);

                    contextMenu.Items.Add(new ToolStripSeparator());

                    ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem("Remove selected");
                    deleteMenuItem.Click += (s, args) => DeleteSelectedAccounts();
                    contextMenu.Items.Add(deleteMenuItem);

                    contextMenu.Show(accountsTable, e.Location);
                }
            }
        }

        private void PerformLogin()
        {
            foreach (DataGridViewRow row in accountsTable.SelectedRows)
            {
                string username = row.Cells[0].Value?.ToString();
                if (!string.IsNullOrEmpty(username))
                {
                    var account = dataStore.GetAccount(username);
                    if (account != null)
                    {
                        ConfigHelper.DoLogin(username, account.Token);
                        break;
                    }
                }
            }
            RefreshStatus();
        }

        private void DeleteSelectedAccounts()
        {
            var rowsToDelete = accountsTable.SelectedRows.Cast<DataGridViewRow>().ToList();
            
            foreach (DataGridViewRow row in rowsToDelete)
            {
                string username = row.Cells[0].Value?.ToString();
                if (!string.IsNullOrEmpty(username))
                {
                    int? accountId = dataStore.GetAccountId(username);
                    if (accountId.HasValue)
                    {
                        dataStore.RemoveAccount(accountId.Value);
                    }
                }
                accountsTable.Rows.Remove(row);
            }
        }

        private void OpenProfileInBrowser()
        {
            foreach (DataGridViewRow row in accountsTable.SelectedRows)
            {
                string steamId = row.Cells[1].Value?.ToString();
                if (!string.IsNullOrEmpty(steamId))
                {
                    string url = $"https://steamcommunity.com/profiles/{steamId}";
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }
    }
}
