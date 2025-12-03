namespace WinForms.CarsService.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int RegistrationNumber { get; set; }

        public int OwnerId { get; set; }
        public Owner Owner { get; set; }

        public List<CarService> CarServices { get; set; } = [];
    }
}
