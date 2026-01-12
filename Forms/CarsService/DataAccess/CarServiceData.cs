using Microsoft.EntityFrameworkCore;
using WinForms.CarsService.Models;

namespace WinForms.CarsService.DataAccess
{
    public class CarServiceData
    {
        private readonly CarsServiceDbContext _db;

        public CarServiceData(CarsServiceDbContext context)
        {
            _db = context;
        }

        public IEnumerable<CarService> GetAll() =>
            _db.CarServices.Include(cs => cs.Car)
            .ThenInclude(c => c.Owner)
            .Include(cs => cs.Service)
            .ToList();

        public IEnumerable<CarService> Find(Func<CarService, bool> where) =>
            _db.CarServices.Include(cs => cs.Car)
            .ThenInclude(c => c.Owner)
            .Include(cs => cs.Service)
            .Where(where)
            .ToList();

        public void Add(CarService carService) => _db.CarServices.Add(carService);
        public void AddRange(IEnumerable<CarService> carServices) => _db.CarServices.AddRange(carServices);
        public void Remove(CarService carService) => _db.CarServices.Remove(carService);

        public bool HasConflict(DateTime start, DateTime end) => 
            _db.CarServices.Any(cs => cs.StartTime < end && start < cs.EndTime);

        public void Save() => _db.SaveChanges();
    }
}
