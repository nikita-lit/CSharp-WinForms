using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public partial class CarsService
    {
        private DataGridView _dgvOwners = new();
        private TextBox _txtOwnerName = new();
        private Button _butAddOwner = new();
        private Button _butDeleteOwner = new();

        private void SetupOwnersTab(TabPage owners)
        {
            _dgvOwners.Dock = DockStyle.Top;
            _dgvOwners.Height = 300;
            owners.Controls.Add(_dgvOwners);

            Label lblName = new() { Text = "Owner Name:", Top = 310, Left = 10 };
            owners.Controls.Add(lblName);

            _txtOwnerName.Top = 330;
            _txtOwnerName.Left = 10;
            owners.Controls.Add(_txtOwnerName);

            _butAddOwner.Text = "Add Owner";
            _butAddOwner.Top = 330;
            _butAddOwner.Left = 150;
            _butAddOwner.Click += _butAddOwner_Click;
            owners.Controls.Add(_butAddOwner);

            _butDeleteOwner.Text = "Delete Selected";
            _butDeleteOwner.Top = 330;
            _butDeleteOwner.Left = 250;
            _butDeleteOwner.Click += _butDeleteOwner_Click;
            owners.Controls.Add(_butDeleteOwner);

            LoadOwners();
        }

        private void LoadOwners()
        {
            _dgvOwners.DataSource = _dbContext.Owners.ToList();
        }

        private void _butAddOwner_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_txtOwnerName.Text))
            {
                _dbContext.Owners.Add(new Owner { FullName = _txtOwnerName.Text });
                _dbContext.SaveChanges();
                LoadOwners();
                _txtOwnerName.Clear();
            }
        }

        private void _butDeleteOwner_Click(object sender, EventArgs e)
        {
            if (_dgvOwners.CurrentRow != null)
            {
                if (_dgvOwners.CurrentRow.DataBoundItem is Owner owner)
                {
                    _dbContext.Owners.Remove(owner);
                    _dbContext.SaveChanges();
                    LoadOwners();
                }
            }
        }
    }
}
