using PdfSharp.Charting;
using System.Runtime.InteropServices;
using WinForms.CarsService.Elements;

namespace WinForms.CarsService
{
    public partial class CarsService : Form
    {
        public CarsServiceDbContext _dbContext = new();
        public TabControl2 _tabControl;

        // multi lang +
        // car service
        // search +
        // подтверждение при удалении
        // pdf
        // Users
        // Отображение общей выручки сервиса.
        // Иконки и улучшенное оформление вкладок.
        // Запоминание выбранного языка между запусками.
        // Уведомления о предстоящем обслуживании.
        // Отображение количества автомобилей у владельца.
        // Показ общей суммы обслуживания по автомобилю.

        public CarsService()
        {
            string folder = Path.Combine(Environment.CurrentDirectory, "Databases");
            Directory.CreateDirectory(folder);

            Text = LanguageManager.Get("car_service");

            Size = new Size(950, 550);
            BackColor = Colors.Background;
            Padding = new Padding(8);

            Panel panel = new();
            panel.Dock = DockStyle.Top;
            panel.Height = 50;
            panel.Padding = new Padding(8);

            Panel langPanel = new();
            langPanel.Dock = DockStyle.Left;

            Label lblLang = new();
            lblLang.Text = LanguageManager.Get("language") + ": ";
            lblLang.Dock = DockStyle.Left;
            lblLang.TextAlign = ContentAlignment.TopLeft;
            lblLang.ForeColor = Colors.Text;
            lblLang.Width = 80;

            ComboBox cbLang = new();
            cbLang.DataSource = Enum.GetValues<LanguageManager.Language>();
            cbLang.Dock = DockStyle.Fill;
            cbLang.DropDownStyle = ComboBoxStyle.DropDownList;
            cbLang.BindingContextChanged += (sender, e) =>
            {
                if (cbLang.Items.Count > 0)
                    cbLang.SelectedIndex = (int)LanguageManager.CurrentLanguage;
            };
            cbLang.SelectedValueChanged += (sender, e) =>
            {
                LanguageManager.CurrentLanguage = (LanguageManager.Language)cbLang.SelectedItem;
            };

            langPanel.Controls.Add(cbLang);
            langPanel.Controls.Add(lblLang);

            panel.Controls.Add(langPanel);

            _tabControl = new(Colors.Header, Colors.Background, Colors.Row, Colors.HeaderLine);
            _tabControl.Dock = DockStyle.Fill;

            Panel owners = new();
            owners.Text = "owners";
            owners.BackColor = Colors.Background;
            SetupOwnersTab(owners);
            _tabControl.AddTab(owners);

            Panel cars = new();
            cars.Text = "cars";
            cars.BackColor = Colors.Background;
            SetupCarsTab(cars);
            _tabControl.AddTab(cars);

            Panel services = new();
            services.Text = "services";
            services.BackColor = Colors.Background;
            SetupServicesTab(services);
            _tabControl.AddTab(services);

            MakeDark(Handle);
            Controls.Add(_tabControl);
            Controls.Add(panel);

            LanguageManager.LanguageChanged += () => {
                Text = LanguageManager.Get("car_service");
                lblLang.Text = LanguageManager.Get("language") + ": ";
            };
        }

        private Panel SetupButtonsPanel(EventHandler add, EventHandler update, EventHandler delete, Action<string> load)
        {
            Panel panel = new();
            panel.Dock = DockStyle.Top;
            panel.Height = 30;
            panel.Width = 200;
            panel.Padding = new Padding(2);

            Button butAdd = new();
            butAdd.Text = LanguageManager.Get("add");
            butAdd.Dock = DockStyle.Left;
            butAdd.AutoSize = true;
            butAdd.Click += add;
            SetupButtonStyle(butAdd);

            Button butUpdate = new();
            butUpdate.Text = LanguageManager.Get("update");
            butUpdate.Dock = DockStyle.Left;
            butUpdate.AutoSize = true;
            butUpdate.Click += update;
            SetupButtonStyle(butUpdate);

            Button butDelete = new();
            butDelete.Text = LanguageManager.Get("delete");
            butDelete.Dock = DockStyle.Left;
            butDelete.AutoSize = true;
            butDelete.Click += delete;
            SetupButtonStyle(butDelete);

            Panel spacer = new();
            spacer.Width = 5;
            spacer.Dock = DockStyle.Left;

            Panel spacer2 = new();
            spacer2.Width = 5;
            spacer2.Dock = DockStyle.Left;

            panel.Controls.Add(butDelete);
            panel.Controls.Add(spacer);
            panel.Controls.Add(butUpdate);
            panel.Controls.Add(spacer2);
            panel.Controls.Add(butAdd);

            Label lblSearch = new Label();
            lblSearch.Text = LanguageManager.Get("search") + ":";
            lblSearch.Dock = DockStyle.Right;
            lblSearch.TextAlign = ContentAlignment.TopLeft;
            lblSearch.ForeColor = Colors.Text;
            lblSearch.Width = 60;

            TextBox txtSearch = new();
            txtSearch.Dock = DockStyle.Right;
            txtSearch.TextChanged += (s, e) =>
            {
                load.Invoke(txtSearch.Text);
            };

            panel.Controls.Add(lblSearch);
            panel.Controls.Add(txtSearch);

            LanguageManager.LanguageChanged += () => {
                butAdd.Text = LanguageManager.Get("add");
                butUpdate.Text = LanguageManager.Get("update");
                butDelete.Text = LanguageManager.Get("delete");
                lblSearch.Text = LanguageManager.Get("search") + ":";
            };

            return panel;
        }

        public static void SetupButtonStyle(Button but)
        {
            but.FlatStyle = FlatStyle.Flat;
            but.FlatAppearance.MouseOverBackColor = Colors.ButtonHover;
            but.FlatAppearance.MouseDownBackColor = Colors.ButtonPressed;
            but.FlatAppearance.BorderSize = 0;
            but.BackColor = Colors.Button;
            but.ForeColor = Colors.Text;
            but.Paint += (sender, e) => {
                var but = sender as Button;
                using (Pen pen = new Pen(Colors.Header, 1))
                    e.Graphics.DrawRectangle(pen, new Rectangle(1, 1, but.Width - 3, but.Height - 3));
            };
        }

        public static void MakeDark(IntPtr hwnd)
        {
            int useDark = 1;
            DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDark, sizeof(int));
        }

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        [DllImport("user32.dll")]
        private static extern int ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        private const int SB_BOTH = 3;
    }
}