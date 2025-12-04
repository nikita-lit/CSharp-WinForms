using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public partial class CarsService : Form
    {
        public CarsServiceDbContext _dbContext = new();
        public TabControl _tabControl = new();

        public CarsService()
        {
            string folder = Path.Combine(Environment.CurrentDirectory, "Databases");
            Directory.CreateDirectory(folder);

            Text = "Cars Service";
            Size = new Size(750, 550);

            _tabControl = new();
            _tabControl.Dock = DockStyle.Fill;

            TabPage owners = new("Owners");
            SetupOwnersTab(owners);
            _tabControl.TabPages.Add(owners);

            TabPage cars = new("Cars");
            SetupCarsTab(cars);
            _tabControl.TabPages.Add(cars);

            TabPage services = new("Services");
            //SetupServicesTab(services);
            _tabControl.TabPages.Add(services);

            TabPage carServices = new("Car Services");
            //SetupServicesTab(services);
            _tabControl.TabPages.Add(carServices);

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

            Button butUpdate = new();
            butUpdate.Text = "Update";
            butUpdate.Dock = DockStyle.Left;
            butUpdate.Click += update;

            Button butDelete = new();
            butDelete.Text = "Delete";
            butDelete.Dock = DockStyle.Left;
            butDelete.Click += delete;

            panel.Controls.Add(butDelete);
            panel.Controls.Add(butUpdate);
            panel.Controls.Add(butAdd);

            return panel;
        }
    }
}