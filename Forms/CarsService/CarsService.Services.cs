using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public partial class CarsService
    {
        private DataGridView _dgvServices = new();
        private TextBox _txtServiceName = new();
        private TextBox _txtServicePrice = new();
        private Button _butAddService = new();

        private void SetupServicesTab(TabPage services)
        {
            _dgvServices.Dock = DockStyle.Top;
            _dgvServices.Height = 200;
            services.Controls.Add(_dgvServices);

            Label lblName = new() { Text = "Service Name:", Top = 210, Left = 10 };
            services.Controls.Add(lblName);

            _txtServiceName.Top = 230;
            _txtServiceName.Left = 10;
            services.Controls.Add(_txtServiceName);

            Label lblPrice = new() { Text = "Price:", Top = 260, Left = 10 };
            services.Controls.Add(lblPrice);

            _txtServicePrice.Top = 280;
            _txtServicePrice.Left = 10;
            services.Controls.Add(_txtServicePrice);

            _butAddService.Text = "Add Service";
            _butAddService.Top = 280;
            _butAddService.Left = 150;
            _butAddService.Click += _butAddService_Click;
            services.Controls.Add(_butAddService);

            LoadServices();
        }

        private void LoadServices()
        {
            _dgvServices.DataSource = _dbContext.Services.ToList();
        }

        private void _butAddService_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_txtServiceName.Text) && float.TryParse(_txtServicePrice.Text, out float price))
            {
                _dbContext.Services.Add(new Service { Name = _txtServiceName.Text, Price = price });
                _dbContext.SaveChanges();
                LoadServices();
                _txtServiceName.Clear();
                _txtServicePrice.Clear();
            }
        }

    }
}
