using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public partial class CarsService
    {
        private TableLayoutPanel _tlpCars;
        private Form _formAddCar;
        private int _currentCarRow = -1;

        private void SetupCarsTab(Panel cars)
        {
            ScrollableControl panel = new();
            panel.Dock = DockStyle.Top;
            panel.Height = 400;
            panel.AutoScroll = true;
            panel.BackColor = Colors.TableBackground;

            _tlpCars = new();
            _tlpCars.BackColor = Colors.TableBackground;
            _tlpCars.AutoSize = true;
            _tlpCars.Dock = DockStyle.Top;

            panel.Controls.Add(_tlpCars);

            EventHandler add = (sender, e) => {
                OpenCarForm();
            };

            EventHandler update = (sender, e) => {
                var car = GetCurrentCar();
                if (car != null)
                    OpenCarForm(car);
                else
                    MessageBox.Show(LanguageManager.Get("row_not_selected"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            Panel spacer = new();
            spacer.Height = 7;
            spacer.Dock = DockStyle.Top;

            cars.Controls.Add(SetupButtonsPanel(add, update, butDeleteCar_Click, LoadCars));
            cars.Controls.Add(spacer);
            cars.Controls.Add(panel);

            LoadCars();
        }

        private void LoadCars(string search = "")
        {
            if (_tlpCars == null)
                return;

            _tlpCars.Controls.Clear();
            _tlpCars.RowStyles.Clear();
            _tlpCars.ColumnStyles.Clear();

            string[] headers = { "id", "brand", "model", "registrationnumber", "owner" };
            _tlpCars.ColumnCount = headers.Length;

            for (int i = 0; i < headers.Length; i++)
            {
                var header = headers[i];
                if (header == "id")
                    _tlpCars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5.0f));
                else
                    _tlpCars.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20.0f));

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

                _tlpCars.Controls.Add(lblHeader, i, 0);
            }

            List<Car> cars;
            if (string.IsNullOrEmpty(search))
                cars = CarData.GetAll().ToList();
            else
            {
                var s = search.ToLower();
                cars = CarData.Find(o =>
                    o.Model.ToLower().Contains(s) ||
                    o.Brand.ToLower().Contains(s) ||
                    (o.Owner != null && o.Owner.FullName.ToLower().Contains(s)) ||
                    o.RegistrationNumber.ToString().ToLower().Contains(s)
                ).ToList();
            }

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

            LoadCarServices();
        }

        private Car GetCurrentCar()
        {
            if (_currentCarRow >= 0)
            {
                var cars = CarData.GetAll().ToList();
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
            string t = (isCarValid ? LanguageManager.Get("update") : LanguageManager.Get("add"));

            _formAddCar = new();
            _formAddCar.Text = $"{LanguageManager.Get("car_service")} - {t} {LanguageManager.Get("car")}";
            _formAddCar.Size = new Size(300, 500);
            _formAddCar.Padding = new Padding(20);
            _formAddCar.FormBorderStyle = FormBorderStyle.FixedSingle;
            _formAddCar.MinimizeBox = false;
            _formAddCar.MaximizeBox = false;
            _formAddCar.BackColor = Colors.Background;
            MakeDark(_formAddCar.Handle);

            //----------------------------------------
            Label lblBrand = new();
            lblBrand.Text = LanguageManager.Get("brand") + ":";
            lblBrand.Dock = DockStyle.Top;
            lblBrand.TextAlign = ContentAlignment.MiddleLeft;
            lblBrand.Width = 80;
            lblBrand.ForeColor = Colors.Text;

            TextBox txtCarBrand = new();
            txtCarBrand.Dock = DockStyle.Top;
            if (isCarValid)
                txtCarBrand.Text = car.Brand;

            //----------------------------------------
            Label lblModel = new();
            lblModel.Text = LanguageManager.Get("model") + ":";
            lblModel.Dock = DockStyle.Top;
            lblModel.TextAlign = ContentAlignment.MiddleLeft;
            lblModel.Width = 80;
            lblModel.ForeColor = Colors.Text;

            TextBox txtCarModel = new();
            txtCarModel.Dock = DockStyle.Top;
            if (isCarValid)
                txtCarModel.Text = car.Model;

            //----------------------------------------
            Label lblOwner = new();
            lblOwner.Text = LanguageManager.Get("owner") + ":";
            lblOwner.Dock = DockStyle.Top;
            lblOwner.TextAlign = ContentAlignment.MiddleLeft;
            lblOwner.Width = 80;
            lblOwner.ForeColor = Colors.Text;

            TextBox txtOwnerSearch = new();
            txtOwnerSearch.Dock = DockStyle.Top;
            txtOwnerSearch.PlaceholderText = LanguageManager.Get("search_owner") + "...";

            Owner owner = null;
            if (isCarValid)
            {
                owner = car.Owner;
                txtOwnerSearch.Text = owner.FullName;
            }

            TableLayoutPanel tlpOwnerResults = new();
            tlpOwnerResults.Dock = DockStyle.Top;
            tlpOwnerResults.Visible = false;
            tlpOwnerResults.BackColor = Colors.Background;
            tlpOwnerResults.ForeColor = Colors.Text;
            tlpOwnerResults.AutoSize = false;
            tlpOwnerResults.AutoScroll = true;
            tlpOwnerResults.ColumnCount = 1;
            tlpOwnerResults.Height = 150;
            tlpOwnerResults.Paint += (sender, e) => 
            {
                using (Pen pen = new Pen(Colors.Header, 2))
                    e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, tlpOwnerResults.Width - 2, tlpOwnerResults.Height - 2));
            };

            void AddOwnerRow(Owner o)
            {
                Label lbl = new();
                lbl.Text = $"{o.FullName} - {o.Phone}";
                lbl.ForeColor = Colors.Text;
                lbl.BackColor = Colors.Row;
                lbl.Padding = new Padding(5);
                lbl.Dock = DockStyle.Top;
                lbl.Cursor = Cursors.Hand;
                lbl.AutoSize = true;

                lbl.Click += (s, e) =>
                {
                    owner = o;
                    txtOwnerSearch.Text = o.FullName;
                    tlpOwnerResults.Visible = false;
                };

                lbl.Paint += (sender, e) => 
                {
                    var but = sender as Button;
                    using (Pen pen = new Pen(Colors.Header, 2))
                        e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, lbl.Width - 1, lbl.Height - 1));
                };

                tlpOwnerResults.RowCount++;
                tlpOwnerResults.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tlpOwnerResults.Controls.Add(lbl, 0, tlpOwnerResults.RowCount - 1);

                ShowScrollBar(tlpOwnerResults.Handle, SB_BOTH, false);
            }

            var owners = OwnerData.GetAll();
            if (owner == null && owners.Count() > 0)
            {
                tlpOwnerResults.Visible = true;
                foreach (var o in owners)
                    AddOwnerRow(o);
            }

            txtOwnerSearch.TextChanged += (s, e) =>
            {
                string text = txtOwnerSearch.Text.Trim().ToLower();
                var results = OwnerData.Find(o => o.FullName.ToLower().Contains(text) || o.Phone.ToLower().Contains(text)).ToList();

                tlpOwnerResults.Controls.Clear();
                tlpOwnerResults.RowStyles.Clear();
                tlpOwnerResults.RowCount = 0;

                if (results.Count > 0)
                {
                    tlpOwnerResults.Visible = true;
                    foreach (var o in results)
                        AddOwnerRow(o);
                }
                else
                    tlpOwnerResults.Visible = false;
            };

            //----------------------------------------
            Label lblRegNum = new();
            lblRegNum.Text = LanguageManager.Get("registrationnumber") + ":";
            lblRegNum.Dock = DockStyle.Top;
            lblRegNum.TextAlign = ContentAlignment.MiddleLeft;
            lblRegNum.Width = 80;
            lblRegNum.ForeColor = Colors.Text;

            NumericUpDown numCarRegNum = new();
            numCarRegNum.Dock = DockStyle.Top;
            numCarRegNum.Maximum = int.MaxValue;
            numCarRegNum.Minimum = 0;
            if (isCarValid)
                numCarRegNum.Value = car.RegistrationNumber;

            //----------------------------------------
            Button but = new();
            but.Text = t;
            but.Dock = DockStyle.Bottom;
            but.Height = 30;
            SetupButtonStyle(but);
            but.Click += (sender, e) => {
                var brand = txtCarBrand.Text;
                var model = txtCarModel.Text;
                var regNum = Convert.ToInt32(numCarRegNum.Value);

                bool isValid = !string.IsNullOrWhiteSpace(brand) 
                    && !string.IsNullOrWhiteSpace(model)
                    && regNum > 0
                    && owner != null;

                if (isValid)
                {
                    try
                    {
                        if (!isCarValid)
                            CarData.Add(new Car { Brand = brand, Model = model, RegistrationNumber = regNum, OwnerId = owner.Id });
                        else
                        {
                            car.Brand = brand;
                            car.Model = model;
                            car.RegistrationNumber = regNum;
                            car.OwnerId = owner.Id;
                            car.Owner = owner;
                            CarData.Update(car);
                        }

                        CarData.Save();
                        _formAddCar.Close();
                        LoadCars();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, LanguageManager.Get("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                    MessageBox.Show(LanguageManager.Get("invalid"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            //----------------------------------------
            _formAddCar.Controls.Add(numCarRegNum);
            _formAddCar.Controls.Add(lblRegNum);

            _formAddCar.Controls.Add(txtCarModel);
            _formAddCar.Controls.Add(lblModel);

            _formAddCar.Controls.Add(txtCarBrand);
            _formAddCar.Controls.Add(lblBrand);

            _formAddCar.Controls.Add(tlpOwnerResults);
            _formAddCar.Controls.Add(txtOwnerSearch);
            _formAddCar.Controls.Add(lblOwner);
            _formAddCar.Controls.Add(but);

            _formAddCar.ShowDialog();
        }

        private void butDeleteCar_Click(object sender, EventArgs e)
        {
            var car = GetCurrentCar();
            if (car != null)
            {
                if (MessageBox.Show(LanguageManager.Get("are_you_sure_car"), LanguageManager.Get("warning"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CarData.Remove(car);
                    CarData.Save();
                    LoadCars();
                }
            }
            else
                MessageBox.Show(LanguageManager.Get("row_not_selected"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
