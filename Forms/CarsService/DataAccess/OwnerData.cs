using WinForms.CarsService.Models;

namespace WinForms.CarsService.DataAccess
{
    public class OwnerData
    {
        private readonly CarsServiceDbContext _db;

        public OwnerData(CarsServiceDbContext context)
        {
            _db = context;
        }

        public IEnumerable<Owner> GetAll() => _db.Owners.ToList();
        public Owner Get(params object[] key) => _db.Owners.Find(key);

        public IEnumerable<Owner> Find(Func<Owner, bool> where) =>
            _db.Owners.Where(where)
            .ToList();

        public void Add(Owner owner) => _db.Owners.Add(owner);
        public void AddRange(IEnumerable<Owner> owners) => _db.Owners.AddRange(owners);
        public void Update(Owner owner) => _db.Owners.Update(owner);
        public void Remove(Owner owner) => _db.Owners.Remove(owner);
        public void Save() => _db.SaveChanges();
    }
}
