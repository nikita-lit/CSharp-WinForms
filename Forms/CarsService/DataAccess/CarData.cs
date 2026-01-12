using Microsoft.EntityFrameworkCore;
using WinForms.CarsService.Models;

namespace WinForms.CarsService.DataAccess
{
    public class CarData
    {
        private readonly CarsServiceDbContext _db;

        public CarData(CarsServiceDbContext context)
        {
            _db = context;
        }

        public IEnumerable<Car> GetAll() => 
            _db.Cars.Include(c => c.Owner)
            .ToList();

        public Car Get(params object[] key) => _db.Cars.Find(key);

        public IEnumerable<Car> Find(Func<Car, bool> where) => 
            _db.Cars.Include(c => c.Owner)
            .Where(where)
            .ToList();

        public void Add(Car car) => _db.Cars.Add(car);
        public void AddRange(IEnumerable<Car> cars) => _db.Cars.AddRange(cars);
        public void Update(Car car) => _db.Cars.Update(car);
        public void Remove(Car car) => _db.Cars.Remove(car);
        public void Save() => _db.SaveChanges();
    }
}
