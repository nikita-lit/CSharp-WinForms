using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public partial class CarsService
    {
        private DataGridView _dgvOwners;
        private TextBox _txtOwnerName;
        private TextBox _txtOwnerPhone;
        private Form _formAddOwner;

        private void SetupOwnersTab(TabPage owners)
        {
            _dgvOwners = new();
            _dgvOwners.Dock = DockStyle.Top;
            _dgvOwners.Height = 300;
            _dgvOwners.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            EventHandler add = (sender, e) => {
                OpenOwnerForm();
            };

            EventHandler update = (sender, e) => {
                var row = _dgvOwners.CurrentRow;
                if (row != null && row.DataBoundItem is Owner owner)
                    OpenOwnerForm(owner);
                else
                    MessageBox.Show("Row isn't selected!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            owners.Controls.Add(SetupButtonsPanel(add, update, butDeleteOwner_Click));
            owners.Controls.Add(_dgvOwners);

            LoadOwners();
        }

        private void LoadOwners()
        {
            _dgvOwners.DataSource = _dbContext.Owners.ToList();
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

            Label lblName = new();
            lblName.Text = "Full Name:";
            lblName.Dock = DockStyle.Top;
            lblName.TextAlign = ContentAlignment.MiddleLeft;
            lblName.Width = 80;

            _txtOwnerName = new();
            _txtOwnerName.Dock = DockStyle.Top;
            if (isOwnerValid)
                _txtOwnerName.Text = owner.FullName;

            Label lblPhone = new();
            lblPhone.Text = "Phone Number:";
            lblPhone.Dock = DockStyle.Top;
            lblPhone.TextAlign = ContentAlignment.MiddleLeft;
            lblPhone.Width = 80;

            _txtOwnerPhone = new();
            _txtOwnerPhone.Dock = DockStyle.Top;
            if (isOwnerValid)
                _txtOwnerPhone.Text = owner.Phone;

            Button but = new();
            but.Text = (isOwnerValid ? "Update" : "Add");
            but.Dock = DockStyle.Bottom;
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

            _formAddOwner.Controls.Add(_txtOwnerPhone);
            _formAddOwner.Controls.Add(lblPhone);
            _formAddOwner.Controls.Add(_txtOwnerName);
            _formAddOwner.Controls.Add(lblName);
            _formAddOwner.Controls.Add(but);

            _formAddOwner.ShowDialog();
        }

        private void butDeleteOwner_Click(object sender, EventArgs e)
        {
            var owner = GetCurSelectedOwner();
            if (owner != null)
            {
                _dbContext.Owners.Remove(owner);
                _dbContext.SaveChanges();
                LoadOwners();
            }
            else
                MessageBox.Show("Row isn't selected!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private Owner GetCurSelectedOwner()
        {
            var row = _dgvOwners.CurrentRow;
            if (row != null && row.DataBoundItem is Owner owner)
                return owner;
            else
                MessageBox.Show("Row isn't selected!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return null;
        }
    }
}
