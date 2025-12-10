using SQLitePCL;
using System.Windows.Forms;
using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public partial class CarsService
    {
        private TableLayoutPanel _tlpCars;
        private TextBox _txtCarBrand;
        private TextBox _txtCarModel;
        private ComboBox _cbCarOwner;
        private NumericUpDown _numCarRegNum;
        private Form _formAddCar;
        private int _currentCarRow = -1;

        private void SetupCarsTab(Panel cars)
        {
            ScrollableControl panel = new();
            panel.Dock = DockStyle.Top;
            panel.Height = 300;
            panel.AutoScroll = true;

            _tlpCars = new();
            _tlpCars.BackColor = Color.FromArgb(100, 100, 100);
            _tlpCars.AutoSize = true;
            _tlpCars.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _tlpCars.Dock = DockStyle.Fill;

            panel.Controls.Add(_tlpCars);

            EventHandler add = (sender, e) => {
                OpenCarForm();
            };

            EventHandler update = (sender, e) => {
                var car = GetCurrentCar();
                if (car != null)
                    OpenCarForm(car);
                else
                    MessageBox.Show("Row isn't selected!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            cars.Controls.Add(SetupButtonsPanel(add, update, butDeleteCar_Click));
            cars.Controls.Add(panel);

            LoadCars();
        }

        private void LoadCars()
        {
            _tlpCars.Controls.Clear();
            _tlpCars.RowStyles.Clear();
            _tlpCars.ColumnStyles.Clear();

            string[] headers = { "Id", "Brand", "Model", "RegistrationNumber", "Owner" };
            _tlpCars.ColumnCount = headers.Length;

            for (int i = 0; i < headers.Length; i++)
            {
                var header = headers[i];
                if (header == "Id")
                    _tlpCars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5.0f));
                else
                    _tlpCars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20.0f));

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

                _tlpCars.Controls.Add(lblHeader, i, 0);
            }

            var cars = _dbContext.Cars.ToList();

            _tlpCars.RowCount = cars.Count + 1;
            for (int row = 0; row < cars.Count; row++)
            {
                _tlpCars.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                var car = cars[row];

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

                    label.Click += (s, e) =>
                    {
                        if (row != _currentCarRow)
                        {
                            _currentCarRow = row;
                            HighlightRow(_tlpCars, row + 1);
                        }
                        else
                        {
                            _currentCarRow = -1;
                            RemoveRowsHighlight(_tlpCars);
                        }
                    };

                    return label;
                }

                _tlpCars.Controls.Add(CreateLabel(car.Id.ToString(), row), 0, row + 1);
                _tlpCars.Controls.Add(CreateLabel(car.Brand, row), 1, row + 1);
                _tlpCars.Controls.Add(CreateLabel(car.Model, row), 2, row + 1);
                _tlpCars.Controls.Add(CreateLabel(car.RegistrationNumber.ToString(), row), 3, row + 1);
                _tlpCars.Controls.Add(CreateLabel(car.Owner?.FullName ?? "", row), 4, row + 1);
            }
        }

        private Car GetCurrentCar()
        {
            if (_currentCarRow >= 0)
            {
                var cars = _dbContext.Cars.ToList();
                return cars[_currentCarRow];
            }
            return null;
        }

        private void HighlightRow(TableLayoutPanel panel, int row)
        {
            for (int r = 1; r < panel.RowCount; r++)
                for (int c = 0; c < panel.ColumnCount; c++)
                {
                    Control control = panel.GetControlFromPosition(c, r);
                    if (control != null)
                        control.BackColor = r == row ? Colors.Highlight : Colors.Row;
                }
        }

        private void RemoveRowsHighlight(TableLayoutPanel panel)
        {
            for (int r = 1; r < panel.RowCount; r++)
                for (int c = 0; c < panel.ColumnCount; c++)
                {
                    Control control = panel.GetControlFromPosition(c, r);
                    if (control != null)
                        control.BackColor = Colors.Row;
                }
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
            _formAddCar.BackColor = Colors.Background;
            MakeDark(_formAddCar.Handle);

            //----------------------------------------
            Label lblBrand = new();
            lblBrand.Text = "Brand:";
            lblBrand.Dock = DockStyle.Top;
            lblBrand.TextAlign = ContentAlignment.MiddleLeft;
            lblBrand.Width = 80;
            lblBrand.ForeColor = Colors.Text;

            _txtCarBrand = new();
            _txtCarBrand.Dock = DockStyle.Top;
            if (isCarValid)
                _txtCarBrand.Text = car.Brand;

            //----------------------------------------
            Label lblModel = new();
            lblModel.Text = "Model:";
            lblModel.Dock = DockStyle.Top;
            lblModel.TextAlign = ContentAlignment.MiddleLeft;
            lblModel.Width = 80;
            lblModel.ForeColor = Colors.Text;

            _txtCarModel = new();
            _txtCarModel.Dock = DockStyle.Top;
            if (isCarValid)
                _txtCarModel.Text = car.Model;

            //----------------------------------------
            Label lblOwner = new();
            lblOwner.Text = "Owner:";
            lblOwner.Dock = DockStyle.Top;
            lblOwner.TextAlign = ContentAlignment.MiddleLeft;
            lblOwner.Width = 80;
            lblOwner.ForeColor = Colors.Text;

            _cbCarOwner = new();
            _cbCarOwner.Dock = DockStyle.Top;
            _cbCarOwner.Items.AddRange(_dbContext.Owners.ToArray());
            if (isCarValid)
                _cbCarOwner.SelectedItem = car.Owner;

            //----------------------------------------
            Label lblRegNum = new();
            lblRegNum.Text = "Registration Number:";
            lblRegNum.Dock = DockStyle.Top;
            lblRegNum.TextAlign = ContentAlignment.MiddleLeft;
            lblRegNum.Width = 80;
            lblRegNum.ForeColor = Colors.Text;

            _numCarRegNum = new();
            _numCarRegNum.Dock = DockStyle.Top;
            _numCarRegNum.Maximum = int.MaxValue;
            _numCarRegNum.Minimum = 0;
            if (isCarValid)
                _numCarRegNum.Value = car.RegistrationNumber;

            //----------------------------------------
            Button but = new();
            but.Text = (isCarValid ? "Update" : "Add");
            but.Dock = DockStyle.Bottom;
            SetupButtonStyle(but);
            but.Click += (sender, e) => {
                var brand = _txtCarBrand.Text;
                var model = _txtCarModel.Text;
                var regNum = Convert.ToInt32(_numCarRegNum.Value);
                var owner = _cbCarOwner.SelectedItem as Owner;

                bool isValid = !string.IsNullOrWhiteSpace(brand) 
                    && !string.IsNullOrWhiteSpace(model)
                    && regNum > 0
                    && owner != null;

                if (isValid)
                {
                    if (!isCarValid)
                        _dbContext.Cars.Add(new Car { Brand = brand, Model = model, RegistrationNumber = regNum, OwnerId = owner.Id });
                    else
                    {
                        car.Brand = brand;
                        car.Model = model;
                        car.RegistrationNumber = regNum;
                        car.OwnerId = owner.Id;
                        car.Owner = owner;
                    }

                    _dbContext.SaveChanges();
                    _formAddCar.Close();
                    LoadCars();
                }
                else
                    MessageBox.Show("Data is invalid!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            //----------------------------------------
            _formAddCar.Controls.Add(_numCarRegNum);
            _formAddCar.Controls.Add(lblRegNum);

            _formAddCar.Controls.Add(_txtCarModel);
            _formAddCar.Controls.Add(lblModel);

            _formAddCar.Controls.Add(_txtCarBrand);
            _formAddCar.Controls.Add(lblBrand);

            _formAddCar.Controls.Add(_cbCarOwner);
            _formAddCar.Controls.Add(lblOwner);
            _formAddCar.Controls.Add(but);

            _formAddCar.ShowDialog();
        }

        private void butDeleteCar_Click(object sender, EventArgs e)
        {
            var car = GetCurrentCar();
            if (car != null)
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
