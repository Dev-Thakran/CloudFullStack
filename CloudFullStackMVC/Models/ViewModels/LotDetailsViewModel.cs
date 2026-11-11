namespace CloudFullStackMVC.Models.ViewModels
{
    public class LotDetailsViewModel
    {
        public ParkingLot Lot { get; set; } = new();     // from SQL database
        public CurrentWeather? Weather { get; set; }     // from Open-Meteo
    }
}