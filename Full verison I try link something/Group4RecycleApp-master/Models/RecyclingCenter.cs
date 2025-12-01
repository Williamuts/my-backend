using Microsoft.Maui.Devices.Sensors; 

namespace Group4RecycleApp.Models
{
    public class RecyclingCenter
    {
        public string Name { get; set; } 
        public string Address { get; set; } 
        public Location Location { get; set; } 
        public string Description { get; set; }
        public string OpeningHours { get; set; } 
        public double Rating { get; set; } 
        public string ImageUrl { get; set; } 
        public string PhoneNumber { get; set; }
    }
}