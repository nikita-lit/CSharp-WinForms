namespace WinForms.CarsService.Models
{
    public class Owner
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }

        public List<Car> Cars { get; set; } = [];
    }
}
