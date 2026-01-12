using WinForms.CarsService.DataAccess;
using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public partial class CarsService
    {
        private TableLayoutPanel _tlpServices;
        private Form _formAddService;
        private int _currentServiceRow = -1;

        private void SetupServicesTab(Panel services)
        {
            ScrollableControl panel = new();
            panel.Dock = DockStyle.Top;
            panel.Height = 400;
            panel.AutoScroll = true;
            panel.BackColor = Colors.TableBackground;

            _tlpServices = new();
            _tlpServices.BackColor = Colors.TableBackground;
            _tlpServices.AutoSize = true;
            _tlpServices.Dock = DockStyle.Top;

            panel.Controls.Add(_tlpServices);

            EventHandler add = (sender, e) => {
                OpenServiceForm();
            };

            EventHandler update = (sender, e) => {
                var service = GetCurrentService();
                if (service != null)
                    OpenServiceForm(service);
                else
                    MessageBox.Show(LanguageManager.Get("row_not_selected"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            Panel spacer = new();
            spacer.Height = 7;
            spacer.Dock = DockStyle.Top;

            services.Controls.Add(SetupButtonsPanel(add, update, butDeleteService_Click, LoadServices));
            services.Controls.Add(spacer);
            services.Controls.Add(panel);

            LoadServices();
        }

        private void LoadServices(string search = "")
        {
            if (_tlpServices == null)
                return;

            _tlpServices.Controls.Clear();
            _tlpServices.RowStyles.Clear();
            _tlpServices.ColumnStyles.Clear();

            string[] headers = { "id", "name", "price" };
            _tlpServices.ColumnCount = headers.Length;

            for (int i = 0; i < headers.Length; i++)
            {
                var header = headers[i];
                if (header == "id")
                    _tlpServices.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5.0f));
                else
                    _tlpServices.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20.0f));

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

                _tlpServices.Controls.Add(lblHeader, i, 0);
            }

            var services = ServiceData.Find(o => string.IsNullOrEmpty(search) ||
                            o.Name.ToLower().Contains(search.ToLower()) ||
                            o.Price.ToString().ToLower().Contains(search.ToLower()))
                .ToList();

            _tlpServices.RowCount = services.Count + 1;
            for (int row = 0; row < services.Count; row++)
            {
                _tlpServices.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                var service = services[row];

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
                        if (row == _currentServiceRow)
                        {
                            _currentServiceRow = -1;
                            RemoveRowsHighlight(_tlpServices);
                        }
                        else
                        {
                            _currentServiceRow = row;
                            HighlightRow(_tlpServices, row + 1);
                        }
                    };

                    return label;
                }

                _tlpServices.Controls.Add(CreateLabel(service.Id.ToString(), row), 0, row + 1);
                _tlpServices.Controls.Add(CreateLabel(service.Name, row), 1, row + 1);
                _tlpServices.Controls.Add(CreateLabel(service.Price.ToString() + " €", row), 2, row + 1);
            }

            LoadCarServices();
        }

        private Service GetCurrentService()
        {
            if (_currentServiceRow >= 0)
            {
                var services = ServiceData.GetAll().ToList();
                return services[_currentServiceRow];
            }
            return null;
        }

        private void OpenServiceForm(Service service = null)
        {
            bool isServiceValid = service != null;
            string t = (isServiceValid ? LanguageManager.Get("update") : LanguageManager.Get("add"));

            _formAddService = new();
            _formAddService.Text = $"{LanguageManager.Get("car_service")} - {t} {LanguageManager.Get("service")}";
            _formAddService.Size = new Size(270, 370);
            _formAddService.Padding = new Padding(20);
            _formAddService.FormBorderStyle = FormBorderStyle.FixedSingle;
            _formAddService.MinimizeBox = false;
            _formAddService.MaximizeBox = false;
            _formAddService.BackColor = Colors.Background;
            MakeDark(_formAddService.Handle);

            //----------------------------------------
            Label lblName = new();
            lblName.Text = LanguageManager.Get("name") + ":";
            lblName.Dock = DockStyle.Top;
            lblName.TextAlign = ContentAlignment.MiddleLeft;
            lblName.Width = 80;
            lblName.ForeColor = Colors.Text;

            TextBox txtName = new();
            txtName.Dock = DockStyle.Top;
            if (isServiceValid)
                txtName.Text = service.Name;

            //----------------------------------------
            Label lblPrice = new();
            lblPrice.Text = LanguageManager.Get("price") + ":";
            lblPrice.Dock = DockStyle.Top;
            lblPrice.TextAlign = ContentAlignment.MiddleLeft;
            lblPrice.Width = 80;
            lblPrice.ForeColor = Colors.Text;

            NumericUpDown nPrice = new();
            nPrice.Dock = DockStyle.Top;
            nPrice.Maximum = int.MaxValue;
            nPrice.Minimum = 0;
            nPrice.DecimalPlaces = 2;
            if (isServiceValid)
                nPrice.Value = Convert.ToDecimal(service.Price);

            //----------------------------------------
            Button but = new();
            but.Text = t;
            but.Dock = DockStyle.Bottom;
            but.Height = 30;
            SetupButtonStyle(but);
            but.Click += (sender, e) => {
                float price = Convert.ToSingle(nPrice.Value);

                bool isValid = !string.IsNullOrWhiteSpace(txtName.Text)
                    && price > 0;

                if (isValid)
                {
                    if (!isServiceValid)
                        ServiceData.Add(new Service { Name = txtName.Text, Price = price });
                    else
                    {
                        service.Name = txtName.Text;
                        service.Price = price;
                    }

                    ServiceData.Save();
                    _formAddService.Close();
                    LoadServices();
                }
                else
                    MessageBox.Show(LanguageManager.Get("invalid"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            //----------------------------------------
            _formAddService.Controls.Add(nPrice);
            _formAddService.Controls.Add(lblPrice);
            _formAddService.Controls.Add(txtName);
            _formAddService.Controls.Add(lblName);
            _formAddService.Controls.Add(but);

            _formAddService.ShowDialog();
        }

        private void butDeleteService_Click(object sender, EventArgs e)
        {
            var service = GetCurrentService();
            if (service != null)
            {
                if (MessageBox.Show(LanguageManager.Get("are_you_sure_service"), LanguageManager.Get("warning"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ServiceData.Remove(service);
                    ServiceData.Save();
                    LoadServices();
                }
            }
            else
                MessageBox.Show(LanguageManager.Get("row_not_selected"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
