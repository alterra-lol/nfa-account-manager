using System;
using System.Windows.Forms;

namespace nfa
{
    public class TokenImportDialog : Form
    {
        private TextBox tokenTextBox;
        private Button btnOk;
        private Button btnCancel;
        private Label lblInstruction;

        public string TokenText => tokenTextBox.Text;

        public TokenImportDialog()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Import Accounts";
            this.Size = new System.Drawing.Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            lblInstruction = new Label
            {
                Text = "Enter account tokens (one per line or paste from clipboard):\nFormat: username----token",
                Location = new System.Drawing.Point(15, 15),
                Size = new System.Drawing.Size(455, 40),
                AutoSize = false
            };

            tokenTextBox = new TextBox
            {
                Location = new System.Drawing.Point(15, 60),
                Size = new System.Drawing.Size(455, 200),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                AcceptsReturn = true,
                AcceptsTab = false
            };

            btnOk = new Button
            {
                Text = "Import",
                Location = new System.Drawing.Point(280, 270),
                Size = new System.Drawing.Size(90, 30),
                DialogResult = DialogResult.OK
            };

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(380, 270),
                Size = new System.Drawing.Size(90, 30),
                DialogResult = DialogResult.Cancel
            };

            this.Controls.Add(lblInstruction);
            this.Controls.Add(tokenTextBox);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
    }
}
