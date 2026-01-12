using Microsoft.EntityFrameworkCore;
using WinForms.CarsService.Models;

namespace WinForms.CarsService.DataAccess
{
    public class ServiceData
    {
        private readonly CarsServiceDbContext _db;

        public ServiceData(CarsServiceDbContext context)
        {
            _db = context;
        }

        public IEnumerable<Service> GetAll() => _db.Services.ToList();
        public Service Get(params object[] key) => _db.Services.Find(key);

        public IEnumerable<Service> Find(Func<Service, bool> where) => 
            _db.Services.Where(where)
            .ToList();

        public void Add(Service service) => _db.Services.Add(service);
        public void AddRange(IEnumerable<Service> services) => _db.Services.AddRange(services);
        public void Update(Service service) => _db.Services.Update(service);
        public void Remove(Service service) => _db.Services.Remove(service);
        public void Save() => _db.SaveChanges();
    }
}
