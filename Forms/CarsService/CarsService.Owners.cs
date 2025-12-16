using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public partial class CarsService
    {
        private TableLayoutPanel _tlpOwners;
        private Form _formAddOwner;
        private int _currentOwnerRow = -1;

        private void SetupOwnersTab(Panel owners)
        {
            ScrollableControl panel = new();
            panel.Dock = DockStyle.Top;
            panel.Height = 300;
            panel.AutoScroll = true;
            panel.BackColor = Colors.TableBackground;

            _tlpOwners = new();
            _tlpOwners.BackColor = Colors.TableBackground;
            _tlpOwners.AutoSize = true;
            _tlpOwners.Dock = DockStyle.Top;

            panel.Controls.Add(_tlpOwners);

            EventHandler add = (sender, e) => {
                OpenOwnerForm();
            };

            EventHandler update = (sender, e) => {
                var owner = GetCurrentOwner();
                if (owner != null)
                    OpenOwnerForm(owner);
                else
                    MessageBox.Show(LanguageManager.Get("row_not_selected"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            Panel spacer = new();
            spacer.Height = 7;
            spacer.Dock = DockStyle.Top;

            owners.Controls.Add(SetupButtonsPanel(add, update, butDeleteOwner_Click, LoadOwners));
            owners.Controls.Add(spacer);
            owners.Controls.Add(panel);

            LoadOwners();
        }

        private void LoadOwners(string search = "")
        {
            if (_tlpOwners == null)
                return;

            _tlpOwners.Controls.Clear();
            _tlpOwners.RowStyles.Clear();
            _tlpOwners.ColumnStyles.Clear();

            string[] headers = { "id", "fullname", "phone" };
            _tlpOwners.ColumnCount = headers.Length;

            for (int i = 0; i < headers.Length; i++)
            {
                var header = headers[i];
                if (header == "id")
                    _tlpOwners.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5.0f));
                else
                    _tlpOwners.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20.0f));

                Label lblHeader = new Label();
                lblHeader.Text = LanguageManager.Get(header);
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

                LanguageManager.LanguageChanged += () => {
                    lblHeader.Text = LanguageManager.Get(header);
                };

                _tlpOwners.Controls.Add(lblHeader, i, 0);
            }

            var owners = _dbContext.Owners
                .Where(o => string.IsNullOrEmpty(search) ||
                            o.FullName.ToLower().Contains(search.ToLower()) ||
                            o.Phone.ToLower().Contains(search.ToLower()))
                .ToList();

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

            LoadCars();
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
            string t = (isOwnerValid ? LanguageManager.Get("update") : LanguageManager.Get("add"));

            _formAddOwner = new();
            _formAddOwner.Text = $"{LanguageManager.Get("car_service")} - {t} {LanguageManager.Get("owner")}";
            _formAddOwner.Size = new Size(270, 370);
            _formAddOwner.Padding = new Padding(20);
            _formAddOwner.FormBorderStyle = FormBorderStyle.FixedSingle;
            _formAddOwner.MinimizeBox = false;
            _formAddOwner.MaximizeBox = false;
            _formAddOwner.BackColor = Colors.Background;
            MakeDark(_formAddOwner.Handle);

            //----------------------------------------
            Label lblName = new();
            lblName.Text = LanguageManager.Get("fullname") + ":";
            lblName.Dock = DockStyle.Top;
            lblName.TextAlign = ContentAlignment.MiddleLeft;
            lblName.Width = 80;
            lblName.ForeColor = Colors.Text;

            TextBox txtOwnerName = new();
            txtOwnerName.Dock = DockStyle.Top;
            if (isOwnerValid)
                txtOwnerName.Text = owner.FullName;

            //----------------------------------------
            Label lblPhone = new();
            lblPhone.Text = LanguageManager.Get("phone") + ":";
            lblPhone.Dock = DockStyle.Top;
            lblPhone.TextAlign = ContentAlignment.MiddleLeft;
            lblPhone.Width = 80;
            lblPhone.ForeColor = Colors.Text;

            TextBox txtOwnerPhone = new();
            txtOwnerPhone.Dock = DockStyle.Top;
            if (isOwnerValid)
                txtOwnerPhone.Text = owner.Phone;

            //----------------------------------------
            Button but = new();
            but.Text = t;
            but.Dock = DockStyle.Bottom;
            but.Height = 30;
            SetupButtonStyle(but);
            but.Click += (sender, e) => {
                bool isValid = !string.IsNullOrWhiteSpace(txtOwnerName.Text)
                    && !string.IsNullOrWhiteSpace(txtOwnerPhone.Text);

                if (isValid)
                {
                    if (!isOwnerValid)
                        _dbContext.Owners.Add(new Owner { FullName = txtOwnerName.Text, Phone = txtOwnerPhone.Text });
                    else
                    {
                        owner.FullName = txtOwnerName.Text;
                        owner.Phone =txtOwnerPhone.Text;
                    }

                    _dbContext.SaveChanges();
                    _formAddOwner.Close();
                    LoadOwners();
                }
                else
                    MessageBox.Show(LanguageManager.Get("invalid"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            //----------------------------------------
            _formAddOwner.Controls.Add(txtOwnerPhone);
            _formAddOwner.Controls.Add(lblPhone);
            _formAddOwner.Controls.Add(txtOwnerName);
            _formAddOwner.Controls.Add(lblName);
            _formAddOwner.Controls.Add(but);

            _formAddOwner.ShowDialog();
        }

        private void butDeleteOwner_Click(object sender, EventArgs e)
        {
            var owner = GetCurrentOwner();
            if (owner != null)
            {
                if (MessageBox.Show(LanguageManager.Get("are_you_sure_owner"), LanguageManager.Get("warning"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _dbContext.Owners.Remove(owner);
                    _dbContext.SaveChanges();
                    LoadOwners();
                }
            }
            else
                MessageBox.Show(LanguageManager.Get("row_not_selected"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
