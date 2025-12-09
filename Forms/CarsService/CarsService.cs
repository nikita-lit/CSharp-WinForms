using System.Windows.Forms;
using WinForms.CarsService.Elements;
using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public partial class CarsService : Form
    {
        public CarsServiceDbContext _dbContext = new();
        public TabControl2 _tabControl = new();

        public CarsService()
        {
            string folder = Path.Combine(Environment.CurrentDirectory, "Databases");
            Directory.CreateDirectory(folder);

            Text = "Cars Service";
            Size = new Size(750, 550);
            BackColor = BackgroundColor;

            _tabControl = new();
            _tabControl.Dock = DockStyle.Fill;

            Panel owners = new();
            owners.Text = "Owners";
            owners.BackColor = BackgroundColor;
            //SetupOwnersTab(owners);
            _tabControl.AddTab(owners);

            Panel cars = new();
            cars.Text = "Cars";
            cars.BackColor = BackgroundColor;
            //SetupCarsTab(cars);
            _tabControl.AddTab(cars);

            //TabPage services = new("Services");
            //SetupServicesTab(services);
            //_tabControl.TabPages.Add(services);

            //TabPage carServices = new("Car Services");
            //SetupServicesTab(services);
            //_tabControl.TabPages.Add(carServices);

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