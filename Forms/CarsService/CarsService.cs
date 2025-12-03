using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public partial class CarsService : Form
    {
        public CarsServiceDbContext _dbContext = new();

        public CarsService()
        {
            string folder = Path.Combine(Environment.CurrentDirectory, "Databases");
            Directory.CreateDirectory(folder);

            Text = "Cars Service";
            Size = new Size(750, 550);
        }
    }
}