namespace WinForms.CarsService.Models
{
    public class CarService
    {
        public int CarId { get; set; }
        public Car Car { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public DateTime DateOfService { get; set; }
    }
}
