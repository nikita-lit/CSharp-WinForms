using System.Runtime.InteropServices;
using WinForms.CarsService.Elements;

namespace WinForms.CarsService
{
    public partial class CarsService : Form
    {
        public CarsServiceDbContext _dbContext = new();
        public TabControl2 _tabControl;

        public CarsService()
        {
            string folder = Path.Combine(Environment.CurrentDirectory, "Databases");
            Directory.CreateDirectory(folder);

            Text = "Cars Service";
            Size = new Size(950, 550);
            BackColor = Colors.Background;
            Padding = new Padding(5);

            _tabControl = new(Colors.Header, Colors.Background, Colors.Row, Colors.HeaderLine);
            _tabControl.Dock = DockStyle.Fill;

            Panel owners = new();
            owners.Text = "Owners";
            owners.BackColor = Colors.Background;
            SetupOwnersTab(owners);
            _tabControl.AddTab(owners);

            Panel cars = new();
            cars.Text = "Cars";
            cars.BackColor = Colors.Background;
            SetupCarsTab(cars);
            _tabControl.AddTab(cars);

            //TabPage services = new("Services");
            //SetupServicesTab(services);
            //_tabControl.TabPages.Add(services);

            //TabPage carServices = new("Car Services");
            //SetupServicesTab(services);
            //_tabControl.TabPages.Add(carServices);

            MakeDark(Handle);
            Controls.Add(_tabControl);
        }

        private Panel SetupButtonsPanel(EventHandler add, EventHandler update, EventHandler delete)
        {
            Panel panel = new();
            panel.Dock = DockStyle.Top;
            panel.Height = 30;
            panel.Padding = new Padding(5, 5, 0, 0);

            Button butAdd = new();
            butAdd.Text = "Add";
            butAdd.Dock = DockStyle.Left;
            butAdd.Click += add;
            SetupButtonStyle(butAdd);

            Button butUpdate = new();
            butUpdate.Text = "Update";
            butUpdate.Dock = DockStyle.Left;
            butUpdate.Click += update;
            SetupButtonStyle(butUpdate);

            Button butDelete = new();
            butDelete.Text = "Delete";
            butDelete.Dock = DockStyle.Left;
            butDelete.Click += delete;
            SetupButtonStyle(butDelete);

            panel.Controls.Add(butDelete);
            panel.Controls.Add(butUpdate);
            panel.Controls.Add(butAdd);

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
    }
}