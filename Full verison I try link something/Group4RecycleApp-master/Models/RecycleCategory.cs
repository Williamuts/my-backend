using CommunityToolkit.Mvvm.ComponentModel;

namespace Group4RecycleApp.Models
{
  
    public partial class RecycleCategory : ObservableObject
    {
        public string Name { get; set; }
        public string Icon { get; set; }

        public bool IsSelected { get; set; }

 
        [ObservableProperty]
        string color = "White"; 

        [ObservableProperty]
        string borderColor = "LightGray"; 

        [ObservableProperty]
        double borderWidth = 1; 
    }
}