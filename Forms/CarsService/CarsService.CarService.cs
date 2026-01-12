using Microsoft.EntityFrameworkCore;
using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public partial class CarsService
    {
        private TableLayoutPanel _tlpCarServices;
        private int _currentCarServiceRow = -1;
        private Form _formAddCarService;

        private Label _lblTotalRevenue;

        private void SetupCarServicesTab(Panel services)
        {
            services.Controls.Clear();

            _lblTotalRevenue = new();
            _lblTotalRevenue.Dock = DockStyle.Top;
            _lblTotalRevenue.Height = 30;
            _lblTotalRevenue.Font = new Font("Arial", 12, FontStyle.Bold);
            _lblTotalRevenue.ForeColor = Colors.Text;
            _lblTotalRevenue.TextAlign = ContentAlignment.MiddleLeft;

            ScrollableControl panel = new();
            panel.Dock = DockStyle.Top;
            panel.Height = 400;
            panel.AutoScroll = true;
            panel.BackColor = Colors.TableBackground;
            
            _tlpCarServices = new();
            _tlpCarServices.BackColor = Colors.TableBackground;
            _tlpCarServices.AutoSize = true;
            _tlpCarServices.Dock = DockStyle.Top;

            panel.Controls.Add(_tlpCarServices);

            EventHandler add = (sender, e) => {
                OpenCarServiceForm();
            };

            Panel spacer = new();
            spacer.Height = 7;
            spacer.Dock = DockStyle.Top;

            services.Controls.Add(SetupButtonsPanel(add, null, butDeleteCarService_Click, LoadCarServices));
            services.Controls.Add(spacer);
            services.Controls.Add(panel);
            services.Controls.Add(_lblTotalRevenue);

            LoadCarServices();

            LanguageManager.LanguageChanged += () => {
                UpdateTotalRevenue();
            };
        }

        private void LoadCarServices(string search = "")
        {
            if (_tlpCarServices == null)
                return;

            _tlpCarServices.Controls.Clear();
            _tlpCarServices.RowStyles.Clear();
            _tlpCarServices.ColumnStyles.Clear();

            string[] headers = { "car", "car_owner", "service", "start_time", "end_time", "price" };
            _tlpCarServices.ColumnCount = headers.Length;

            for (int i = 0; i < headers.Length; i++)
            {
                var header = headers[i];
                if (header == "car")
                    _tlpCarServices.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30.0f));
                else if (header == "price")
                    _tlpCarServices.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10.0f));
                else
                    _tlpCarServices.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20.0f));

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

                _tlpCarServices.Controls.Add(lblHeader, i, 0);
            }

            List<CarService> carServices;
            if (string.IsNullOrEmpty(search))
                carServices = CarServiceData.GetAll().ToList();
            else
            {
                var s = search.ToLower();
                carServices = CarServiceData.Find(o =>
                    o.Service.Name.ToLower().Contains(s) ||
                    o.Car.Brand.ToLower().Contains(s) ||
                    o.Car.Model.ToLower().Contains(s) ||
                    o.Car.Owner.FullName.ToLower().Contains(s)
                ).ToList();
            }

            _tlpCarServices.RowCount = carServices.Count + 1;
            for (int row = 0; row < carServices.Count; row++)
            {
                var cs = carServices[row];
                _tlpCarServices.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                Label Create(string text, int row)
                {
                    Label label = new();
                    label.Text = text;
                    label.Dock = DockStyle.Top;
                    label.TextAlign = ContentAlignment.MiddleCenter;
                    label.BackColor = Colors.Row;
                    label.ForeColor = Colors.Text;
                    label.Margin = new Padding(3);
                    label.Font = new Font("Arial", 12);

                    label.Click += (sender, e) =>
                    {
                        if (row == _currentCarServiceRow)
                        {
                            _currentCarServiceRow = -1;
                            RemoveRowsHighlight(_tlpCarServices);
                        }
                        else
                        {
                            _currentCarServiceRow = row;
                            HighlightRow(_tlpCarServices, row + 1);
                        }
                    };

                    return label;
                }

                var car = cs.Car;
                _tlpCarServices.Controls.Add(Create($"{car.Brand} {car.Model} ({car.RegistrationNumber})", row), 0, row + 1);
                _tlpCarServices.Controls.Add(Create(car.Owner.FullName, row), 1, row + 1);
                _tlpCarServices.Controls.Add(Create(cs.Service.Name, row), 2, row + 1);
                _tlpCarServices.Controls.Add(Create(cs.StartTime.ToString("dd.MM.yyyy HH:mm"), row), 3, row + 1);
                _tlpCarServices.Controls.Add(Create(cs.EndTime.ToString("dd.MM.yyyy HH:mm"), row), 4, row + 1);
                _tlpCarServices.Controls.Add(Create(cs.Service.Price.ToString() + " €", row), 5, row + 1);
            }
             
            UpdateTotalRevenue();
            LoadScheduleData();
        }

        private void UpdateTotalRevenue()
        {
            float total = CarServiceData.GetAll().Sum(x => x.Service.Price);
            _lblTotalRevenue.Text = LanguageManager.Get("total_revenue") + ": " + Math.Round(total, 2) + " €";
        }

        private CarService GetCurrentCarService()
        {
            if (_currentCarServiceRow >= 0)
            {
                var carServices = CarServiceData.GetAll().ToList();
                return carServices[_currentCarServiceRow];
            }
            return null;
        }

        private void OpenCarServiceForm(CarService carService = null)
        {
            bool isCarServiceValid = carService != null;
            string t = isCarServiceValid ? LanguageManager.Get("update") : LanguageManager.Get("add");

            _formAddCarService = new();
            _formAddCarService.Text =  $"{LanguageManager.Get("car_service")} - {t} {LanguageManager.Get("car_services")}";
            _formAddCarService.Size = new Size(350, 550);
            _formAddCarService.Padding = new Padding(20);
            _formAddCarService.FormBorderStyle = FormBorderStyle.FixedSingle;
            _formAddCarService.MinimizeBox = false;
            _formAddCarService.MaximizeBox = false;
            _formAddCarService.BackColor = Colors.Background;
            MakeDark(_formAddCarService.Handle);

            //----------------------------------------
            Label lblCar = new();
            lblCar.Text = LanguageManager.Get("car") + ":";
            lblCar.Dock = DockStyle.Top;
            lblCar.TextAlign = ContentAlignment.MiddleLeft;
            lblCar.Width = 80;
            lblCar.ForeColor = Colors.Text;

            TextBox txtCarSearch = new();
            txtCarSearch.Dock = DockStyle.Top;
            txtCarSearch.PlaceholderText = LanguageManager.Get("search_car") + "...";

            Car selectedCar = null;
            if (isCarServiceValid)
            {
                selectedCar = carService.Car;
                txtCarSearch.Text = $"{selectedCar.Owner.FullName} - {selectedCar.Brand} {selectedCar.Model} ({selectedCar.RegistrationNumber})";
            }

            TableLayoutPanel tlpCarResults = new();
            tlpCarResults.Dock = DockStyle.Top;
            tlpCarResults.Visible = false;
            tlpCarResults.BackColor = Colors.Background;
            tlpCarResults.ForeColor = Colors.Text;
            tlpCarResults.AutoSize = false;
            tlpCarResults.AutoScroll = true;
            tlpCarResults.ColumnCount = 1;
            tlpCarResults.Height = 100;
            tlpCarResults.Paint += (sender, e) =>
            {
                using (Pen pen = new Pen(Colors.Header, 2))
                    e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, tlpCarResults.Width - 2, tlpCarResults.Height - 2));
            };

            void AddCarRow(Car c)
            {
                Label lbl = new();
                lbl.Text = $"{c.Owner.FullName} - {c.Brand} {c.Model} ({c.RegistrationNumber})";
                lbl.ForeColor = Colors.Text;
                lbl.BackColor = Colors.Row;
                lbl.Padding = new Padding(5);
                lbl.Dock = DockStyle.Top;
                lbl.Cursor = Cursors.Hand;
                lbl.AutoSize = true;

                lbl.Click += (s, e) =>
                {
                    selectedCar = c;
                    txtCarSearch.Text = $"{c.Brand} {c.Model} ({c.RegistrationNumber})";
                    tlpCarResults.Visible = false;
                };

                lbl.Paint += (sender, e) => 
                {
                    var but = sender as Button;
                    using (Pen pen = new Pen(Colors.Header, 2))
                        e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, lbl.Width - 1, lbl.Height - 1));
                };

                tlpCarResults.RowCount++;
                tlpCarResults.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tlpCarResults.Controls.Add(lbl, 0, tlpCarResults.RowCount - 1);

                ShowScrollBar(tlpCarResults.Handle, SB_BOTH, false);
            }

            var cars = CarData.GetAll().ToList();
            if (selectedCar == null && cars.Count > 0)
            {
                tlpCarResults.Visible = true;
                foreach (var c in cars)
                    AddCarRow(c);
            }

            txtCarSearch.TextChanged += (s, e) =>
            {
                string text = txtCarSearch.Text.Trim().ToLower();
                var results = CarData.Find(c => c.Model.ToLower().Contains(text)
                             || c.Brand.ToLower().Contains(text)
                             || c.RegistrationNumber.ToString().ToLower().Contains(text)
                             || (c.Owner != null && c.Owner.FullName.ToLower().Contains(text))).ToList();

                tlpCarResults.Controls.Clear();
                tlpCarResults.RowStyles.Clear();
                tlpCarResults.RowCount = 0;

                if (results.Count > 0)
                {
                    tlpCarResults.Visible = true;
                    foreach (var c in results)
                        AddCarRow(c);
                }
                else
                    tlpCarResults.Visible = false;
            };

            //----------------------------------------
            Label lblService = new();
            lblService.Text = LanguageManager.Get("service") + ":";
            lblService.Dock = DockStyle.Top;
            lblService.ForeColor = Colors.Text;
            lblService.TextAlign = ContentAlignment.MiddleLeft;

            TextBox txtServiceSearch = new();
            txtServiceSearch.Dock = DockStyle.Top;
            txtServiceSearch.PlaceholderText = LanguageManager.Get("search_service") + "...";

            Service selectedService = null;
            if (carService != null)
            {
                selectedService = carService.Service;
                txtServiceSearch.Text = selectedService.Name;
            }

            TableLayoutPanel tlpServiceResults = new();
            tlpServiceResults.Dock = DockStyle.Top;
            tlpServiceResults.Visible = false;
            tlpServiceResults.BackColor = Colors.Background;
            tlpServiceResults.ForeColor = Colors.Text;
            tlpServiceResults.AutoSize = false;
            tlpServiceResults.AutoScroll = true;
            tlpServiceResults.ColumnCount = 1;
            tlpServiceResults.Height = 100;
            tlpServiceResults.Paint += (sender, e) =>
            {
                using (Pen pen = new Pen(Colors.Header, 2))
                    e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, tlpServiceResults.Width - 2, tlpServiceResults.Height - 2));
            };

            void AddServiceRow(Service s)
            {
                Label lbl = new();
                lbl.Text = $"{s.Name} - {s.Price} €";
                lbl.ForeColor = Colors.Text;
                lbl.BackColor = Colors.Row;
                lbl.Padding = new Padding(5);
                lbl.Dock = DockStyle.Top;
                lbl.Cursor = Cursors.Hand;
                lbl.AutoSize = true;

                lbl.Click += (s_, e_) =>
                {
                    selectedService = s;
                    txtServiceSearch.Text = s.Name;
                    tlpServiceResults.Visible = false;
                };

                tlpServiceResults.RowCount++;
                tlpServiceResults.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tlpServiceResults.Controls.Add(lbl, 0, tlpServiceResults.RowCount - 1);

                ShowScrollBar(tlpServiceResults.Handle, SB_BOTH, false);
            }

            var services = ServiceData.GetAll().ToList();
            if (selectedService == null && services.Count > 0)
            {
                tlpServiceResults.Visible = true;
                foreach (var s in services)
                    AddServiceRow(s);
            }

            txtServiceSearch.TextChanged += (s, e) =>
            {
                string text = txtServiceSearch.Text.Trim().ToLower();
                var results = ServiceData.Find(s => s.Name.ToLower().Contains(text)).ToList();

                tlpServiceResults.Controls.Clear();
                tlpServiceResults.RowStyles.Clear();
                tlpServiceResults.RowCount = 0;

                if (results.Count > 0)
                {
                    tlpServiceResults.Visible = true;
                    foreach (var s2 in results)
                        AddServiceRow(s2);
                }
                else
                    tlpServiceResults.Visible = false;
            };

            //----------------------------------------
            Label lblDate = new Label();
            lblDate.Text = LanguageManager.Get("date") + ":";
            lblDate.Dock = DockStyle.Top;
            lblDate.ForeColor = Colors.Text;
            lblDate.TextAlign = ContentAlignment.MiddleLeft;

            DateTimePicker dtDate = new DateTimePicker();
            dtDate.Dock = DockStyle.Top;
            dtDate.Format = DateTimePickerFormat.Custom;
            dtDate.CustomFormat = "dd.MM.yyyy";
            dtDate.Value = DateTime.Today;

            //----------------------------------------
            Label lblStart = new Label();
            lblStart.Text = LanguageManager.Get("start_time") + ":";
            lblStart.Dock = DockStyle.Top;
            lblStart.ForeColor = Colors.Text;
            lblStart.TextAlign = ContentAlignment.MiddleLeft;

            DateTimePicker dtStart = new DateTimePicker();
            dtStart.Dock = DockStyle.Top;
            dtStart.Format = DateTimePickerFormat.Custom;
            dtStart.CustomFormat = "HH:mm";
            dtStart.Value = DateTime.Today.AddHours(START_HOUR);

            dtStart.ValueChanged += (s, e) =>
            {
                if (dtStart.Value.Hour < START_HOUR)
                    dtStart.Value = new DateTime(dtDate.Value.Year, dtDate.Value.Month, dtDate.Value.Day, START_HOUR, dtStart.Value.Minute, 0);
                else if (dtStart.Value.Hour >= END_HOUR)
                    dtStart.Value = new DateTime(dtDate.Value.Year, dtDate.Value.Month, dtDate.Value.Day, END_HOUR - 1, dtStart.Value.Minute, 0);
                else
                    dtStart.Value = new DateTime(dtDate.Value.Year, dtDate.Value.Month, dtDate.Value.Day, dtStart.Value.Hour, dtStart.Value.Minute, 0);
            };

            //----------------------------------------
            Button but = new();
            but.Text = t;
            but.Dock = DockStyle.Bottom;
            but.Height = 30;
            SetupButtonStyle(but);

            but.Click += (sender, e) =>
            {
                DateTime date = dtDate.Value.Date;
                TimeSpan startTime = dtStart.Value.TimeOfDay;

                TimeSpan endTime = startTime.Add(TimeSpan.FromHours(1));

                DateTime start = date + startTime;
                DateTime end = date + endTime;

                bool isValid = selectedService != null 
                                    && selectedCar != null;

                if (startTime >= endTime)
                {
                    MessageBox.Show(LanguageManager.Get("end_time_error"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (!isValid)
                {
                    MessageBox.Show(LanguageManager.Get("invalid"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool hasConflict = CarServiceData.HasConflict(start, end);

                if (hasConflict)
                {
                    MessageBox.Show(LanguageManager.Get("time_conflict"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!isCarServiceValid)
                {
                    try
                    {
                        CarServiceData.Add(new CarService
                        {
                            CarId = selectedCar.Id,
                            ServiceId = selectedService.Id,
                            StartTime = start,
                            EndTime = end
                        });
                        CarServiceData.Save();
                    }
                    catch
                    {
                        MessageBox.Show(LanguageManager.Get("invalid"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    carService.CarId = selectedCar.Id;
                    carService.ServiceId = selectedService.Id;
                    carService.StartTime = start;
                    carService.EndTime = end;
                    CarServiceData.Save();
                }

                _formAddCarService.Close();
                LoadCarServices();
            };

            //----------------------------------------
            _formAddCarService.Controls.Add(dtStart);
            _formAddCarService.Controls.Add(lblStart);
            _formAddCarService.Controls.Add(dtDate);
            _formAddCarService.Controls.Add(lblDate);
            _formAddCarService.Controls.Add(tlpServiceResults);
            _formAddCarService.Controls.Add(txtServiceSearch);
            _formAddCarService.Controls.Add(lblService);
            _formAddCarService.Controls.Add(tlpCarResults);
            _formAddCarService.Controls.Add(txtCarSearch);
            _formAddCarService.Controls.Add(lblCar);
            _formAddCarService.Controls.Add(but);

            _formAddCarService.ShowDialog();
        }

        private void butDeleteCarService_Click(object sender, EventArgs e)
        {
            var carService = GetCurrentCarService();
            if (carService != null)
            {
                if (MessageBox.Show(LanguageManager.Get("are_you_sure"), LanguageManager.Get("warning"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CarServiceData.Remove(carService);
                    CarServiceData.Save();
                    LoadCarServices();
                }
            }
            else
                MessageBox.Show(LanguageManager.Get("row_not_selected"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
