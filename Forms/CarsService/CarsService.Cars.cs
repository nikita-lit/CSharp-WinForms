using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public partial class CarsService
    {
        private DataGridView _dgvCars;
        private TextBox _txtCarBrand;
        private Form _formAddCar;

        private void SetupCarsTab(TabPage cars)
        {
            _dgvCars = new();
            _dgvCars.Dock = DockStyle.Top;
            _dgvCars.Height = 300;
            _dgvCars.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            EventHandler add = (sender, e) => {
                OpenCarForm();
            };

            EventHandler update = (sender, e) => {
                var row = _dgvOwners.CurrentRow;
                if (row != null && row.DataBoundItem is Car car)
                    OpenCarForm(car);
                else
                    MessageBox.Show("Row isn't selected!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            cars.Controls.Add(SetupButtonsPanel(add, update, butDeleteCar_Click));
            cars.Controls.Add(_dgvCars);

            LoadCars();
        }

        private void LoadCars()
        {
            _dgvCars.DataSource = _dbContext.Cars.Select(c => new
            {
                c.Id,
                c.Brand,
                c.Model,
                c.RegistrationNumber,
                OwnerName = c.Owner.FullName
            }).ToList();
        }

        private void OpenCarForm(Car car = null)
        {
            bool isCarValid = car != null;

            _formAddCar = new();
            _formAddCar.Text = $"Car Service - {(isCarValid ? "Update" : "Add")} Car";
            _formAddCar.Size = new Size(270, 370);
            _formAddCar.Padding = new Padding(20);
            _formAddCar.FormBorderStyle = FormBorderStyle.FixedSingle;
            _formAddCar.MinimizeBox = false;
            _formAddCar.MaximizeBox = false;

            Label lblBrand = new();
            lblBrand.Text = "Brand:";
            lblBrand.Dock = DockStyle.Top;
            lblBrand.TextAlign = ContentAlignment.MiddleLeft;
            lblBrand.Width = 80;

            _txtCarBrand = new();
            _txtCarBrand.Dock = DockStyle.Top;
            if (isCarValid)
                _txtCarBrand.Text = car.Brand;

            Button but = new();
            but.Text = (isCarValid ? "Update" : "Add");
            but.Dock = DockStyle.Bottom;
            but.Click += (sender, e) => {
                bool isValid = !string.IsNullOrWhiteSpace(_txtCarBrand.Text);

                if (isValid)
                {
                    var owner = GetCurSelectedOwner();
                    if (!isCarValid)
                        _dbContext.Cars.Add(new Car { Brand = _txtCarBrand.Text, OwnerId = owner.Id });
                    else
                    {
                        car.Brand = _txtCarBrand.Text;
                    }

                    _dbContext.SaveChanges();
                    _formAddCar.Close();
                    LoadCars();
                }
                else
                    MessageBox.Show("Data is invalid!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            _formAddCar.Controls.Add(_txtCarBrand);
            _formAddCar.Controls.Add(lblBrand);
            _formAddCar.Controls.Add(but);

            _formAddCar.ShowDialog();
        }

        private void butDeleteCar_Click(object sender, EventArgs e)
        {
            var row = _dgvCars.CurrentRow;
            if (row != null && row.DataBoundItem is Car car)
            {
                _dbContext.Cars.Remove(car);
                _dbContext.SaveChanges();
                LoadCars();
            }
            else
                MessageBox.Show("Row isn't selected!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
