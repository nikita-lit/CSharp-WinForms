using Microsoft.EntityFrameworkCore;
using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public partial class CarsService
    {
        private DataGridView _dgvCars = new();
        private TextBox _txtCarModel = new();
        private ComboBox _cbOwners = new();
        private Button _butAddCar = new();
        private Button _butDeleteCar = new();

        private void SetupCarsTab(TabPage cars)
        {
            _dgvCars.Dock = DockStyle.Top;
            _dgvCars.Height = 300;
            cars.Controls.Add(_dgvCars);

            Label lblModel = new();
            lblModel.Text = "Car Model:";
            lblModel.Top = 310;
            lblModel.Left = 10;
            cars.Controls.Add(lblModel);

            _txtCarModel.Top = 330;
            _txtCarModel.Left = 10;
            cars.Controls.Add(_txtCarModel);

            Label lblOwner = new();
            lblOwner.Text = "Owner:";
            lblOwner.Top = 360;
            lblOwner.Left = 10;

            _cbOwners.Top = 380;
            _cbOwners.Left = 10;
            cars.Controls.Add(_cbOwners);
            cars.Controls.Add(lblOwner);
            
            _butAddCar.Text = "Add";
            _butAddCar.Top = 380;
            _butAddCar.Left = 150;
            _butAddCar.Click += _butAddCar_Click;
            cars.Controls.Add(_butAddCar);

            _butDeleteCar.Text = "Delete";
            _butDeleteCar.Top = 380;
            _butDeleteCar.Left = 250;
            _butDeleteCar.Click += _butDeleteCar_Click;
            cars.Controls.Add(_butDeleteCar);

            LoadCars();
            LoadOwnersForCombo();
        }

        private void LoadOwnersForCombo()
        {
            _cbOwners.DataSource = _dbContext.Owners.ToList();
            _cbOwners.DisplayMember = "FullName";
            _cbOwners.ValueMember = "Id";
        }

        private void LoadCars()
        {
            _dgvCars.DataSource = _dbContext.Cars.Include(c => c.Owner).ToList();
        }

        private void _butAddCar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_txtCarModel.Text) && _cbOwners.SelectedItem != null)
            {
                var car = new Car();
                car.Model = _txtCarModel.Text;
                car.OwnerId = ((Owner)_cbOwners.SelectedItem).Id;

                _dbContext.Cars.Add(car);
                _dbContext.SaveChanges();
                LoadCars();
                _txtCarModel.Clear();
            }
        }

        private void _butDeleteCar_Click(object sender, EventArgs e)
        {
            if (_dgvCars.CurrentRow != null)
            {
                if (_dgvCars.CurrentRow.DataBoundItem is Car car)
                {
                    _dbContext.Cars.Remove(car);
                    _dbContext.SaveChanges();
                    LoadCars();
                }
            }
        }
    }
}
