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

            TabPage services = new("Maintenance and Services");
            SetupServicesTab(services);
            _tabControl.TabPages.Add(services);

            Controls.Add(_tabControl);
        }
    }
}