namespace WinForms.CarsService.DataAccess
{
    public class CarsServiceManager
    {
        private readonly CarsServiceDbContext _db;

        public OwnerData OwnerData { get; }
        public CarData CarData { get; }
        public ServiceData ServiceData { get; }
        public CarServiceData CarServiceData { get; }

        public CarsServiceManager()
        {
            _db = new CarsServiceDbContext();
            OwnerData = new OwnerData(_db);
            CarData = new CarData(_db);
            ServiceData = new ServiceData(_db);
            CarServiceData = new CarServiceData(_db);
        }
    }
}
