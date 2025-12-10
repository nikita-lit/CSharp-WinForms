using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public partial class CarsService
    {
        private TableLayoutPanel _tlpOwners;
        private TextBox _txtOwnerName;
        private TextBox _txtOwnerPhone;
        private Form _formAddOwner;
        private int _currentOwnerRow = -1;

        private void SetupOwnersTab(Panel owners)
        {
            ScrollableControl panel = new();
            panel.Dock = DockStyle.Top;
            panel.Height = 300;
            panel.AutoScroll = true;

            _tlpOwners = new();
            _tlpOwners.BackColor = Color.FromArgb(100, 100, 100);
            _tlpOwners.AutoSize = true;
            _tlpOwners.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _tlpOwners.Dock = DockStyle.Fill;

            panel.Controls.Add(_tlpOwners);

            EventHandler add = (sender, e) => {
                OpenOwnerForm();
            };

            EventHandler update = (sender, e) => {
                var owner = GetCurrentOwner();
                if (owner != null)
                    OpenOwnerForm(owner);
                else
                    MessageBox.Show("Row isn't selected!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            owners.Controls.Add(SetupButtonsPanel(add, update, butDeleteOwner_Click));
            owners.Controls.Add(panel);

            LoadOwners();
        }

        private void LoadOwners()
        {
            _tlpOwners.Controls.Clear();
            _tlpOwners.RowStyles.Clear();
            _tlpOwners.ColumnStyles.Clear();

            string[] headers = { "Id", "FullName", "Phone" };
            _tlpOwners.ColumnCount = headers.Length;

            for (int i = 0; i < headers.Length; i++)
            {
                var header = headers[i];
                if (header == "Id")
                    _tlpOwners.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5.0f));
                else
                    _tlpOwners.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20.0f));

                Label lblHeader = new Label();
                lblHeader.Text = header;
                lblHeader.Dock = DockStyle.Top;
                lblHeader.TextAlign = ContentAlignment.MiddleCenter;
                lblHeader.BackColor = Colors.Header;
                lblHeader.ForeColor = Colors.Text;
                lblHeader.Margin = new Padding(3);
                lblHeader.Font = new Font(lblHeader.Font, FontStyle.Bold);

                lblHeader.Paint += (sender, e) =>
                {
                    using (Pen pen = new Pen(Colors.HeaderLine, 1))
                        e.Graphics.DrawLine(pen, 0, lblHeader.Height - 1, lblHeader.Width, lblHeader.Height - 1);
                };

                _tlpOwners.Controls.Add(lblHeader, i, 0);
            }

            var owners = _dbContext.Owners.ToList();

            _tlpOwners.RowCount = owners.Count + 1;
            for (int row = 0; row < owners.Count; row++)
            {
                _tlpOwners.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                var owner = owners[row];

                Label CreateLabel(string text, int row)
                {
                    Label label = new Label();
                    label.Text = text;
                    label.Dock = DockStyle.Top;
                    label.TextAlign = ContentAlignment.MiddleCenter;
                    label.BackColor = Colors.Row;
                    label.ForeColor = Colors.Text;
                    label.Margin = new Padding(3);
                    label.Font = new Font("Arial", 12);

                    label.Click += (sender, e) =>
                    {
                        if (row == _currentOwnerRow)
                        {
                            _currentOwnerRow = -1;
                            RemoveRowsHighlight(_tlpOwners);
                        }
                        else
                        {
                            _currentOwnerRow = row;
                            HighlightRow(_tlpOwners, row + 1);
                        }
                    };

                    return label;
                }

                _tlpOwners.Controls.Add(CreateLabel(owner.Id.ToString(), row), 0, row + 1);
                _tlpOwners.Controls.Add(CreateLabel(owner.FullName, row), 1, row + 1);
                _tlpOwners.Controls.Add(CreateLabel(owner.Phone, row), 2, row + 1);
            }
        }

        private Owner GetCurrentOwner()
        {
            if (_currentOwnerRow >= 0)
            {
                var owners = _dbContext.Owners.ToList();
                return owners[_currentOwnerRow];
            }
            return null;
        }

        private void OpenOwnerForm(Owner owner = null)
        {
            bool isOwnerValid = owner != null;

            _formAddOwner = new();
            _formAddOwner.Text = $"Car Service - {(isOwnerValid ? "Update" : "Add")} Owner";
            _formAddOwner.Size = new Size(270, 370);
            _formAddOwner.Padding = new Padding(20);
            _formAddOwner.FormBorderStyle = FormBorderStyle.FixedSingle;
            _formAddOwner.MinimizeBox = false;
            _formAddOwner.MaximizeBox = false;
            _formAddOwner.BackColor = Colors.Background;
            MakeDark(_formAddOwner.Handle);

            //----------------------------------------
            Label lblName = new();
            lblName.Text = "Full Name:";
            lblName.Dock = DockStyle.Top;
            lblName.TextAlign = ContentAlignment.MiddleLeft;
            lblName.Width = 80;
            lblName.ForeColor = Colors.Text;

            _txtOwnerName = new();
            _txtOwnerName.Dock = DockStyle.Top;
            if (isOwnerValid)
                _txtOwnerName.Text = owner.FullName;

            //----------------------------------------
            Label lblPhone = new();
            lblPhone.Text = "Phone Number:";
            lblPhone.Dock = DockStyle.Top;
            lblPhone.TextAlign = ContentAlignment.MiddleLeft;
            lblPhone.Width = 80;
            lblPhone.ForeColor = Colors.Text;

            _txtOwnerPhone = new();
            _txtOwnerPhone.Dock = DockStyle.Top;
            if (isOwnerValid)
                _txtOwnerPhone.Text = owner.Phone;

            //----------------------------------------
            Button but = new();
            but.Text = (isOwnerValid ? "Update" : "Add");
            but.Dock = DockStyle.Bottom;
            SetupButtonStyle(but);
            but.Click += (sender, e) => {
                bool isValid = !string.IsNullOrWhiteSpace(_txtOwnerName.Text)
                    && !string.IsNullOrWhiteSpace(_txtOwnerPhone.Text);

                if (isValid)
                {
                    if (!isOwnerValid)
                        _dbContext.Owners.Add(new Owner { FullName = _txtOwnerName.Text, Phone = _txtOwnerPhone.Text });
                    else
                    {
                        owner.FullName = _txtOwnerName.Text;
                        owner.Phone = _txtOwnerPhone.Text;
                    }

                    _dbContext.SaveChanges();
                    _formAddOwner.Close();
                    LoadOwners();
                }
                else
                    MessageBox.Show("Data is invalid!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            //----------------------------------------
            _formAddOwner.Controls.Add(_txtOwnerPhone);
            _formAddOwner.Controls.Add(lblPhone);
            _formAddOwner.Controls.Add(_txtOwnerName);
            _formAddOwner.Controls.Add(lblName);
            _formAddOwner.Controls.Add(but);

            _formAddOwner.ShowDialog();
        }

        private void butDeleteOwner_Click(object sender, EventArgs e)
        {
            var owner = GetCurrentOwner();
            if (owner != null)
            {
                _dbContext.Owners.Remove(owner);
                _dbContext.SaveChanges();
                LoadOwners();
            }
            else
                MessageBox.Show("Row isn't selected!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
