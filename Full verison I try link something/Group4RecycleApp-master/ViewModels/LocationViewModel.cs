using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Group4RecycleApp.Models;
using Microsoft.Maui.Controls.Maps; // For Map objects
using Microsoft.Maui.Devices.Sensors; // For Location

namespace Group4RecycleApp.ViewModels
{
    public partial class LocationViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<RecyclingCenter> centers = new();

        [ObservableProperty]
        RecyclingCenter selectedCenter;

        [ObservableProperty]
        bool isPreviewVisible; // Controls the visibility of the bottom preview card

        public LocationViewModel()
        {
            LoadFakeData();
        }

        void LoadFakeData()
        {
            // Sibu, Sarawak coordinates (approximate)
            Centers.Add(new RecyclingCenter
            {
                Name = "Go Green Center",
                Address = "Jalan Ahmad Zaidi, Lorong U15",
                Location = new Location(2.2873, 111.8305), // Sibu center
                Description = "Accepts paper, plastic, and e-waste. Friendly staff and quick service.",
                OpeningHours = "Mon-Sat 9am - 5pm",
                Rating = 4.5,
                PhoneNumber = "+60123456789"
            });

            Centers.Add(new RecyclingCenter
            {
                Name = "Eco Life Hub",
                Address = "Lorong Pahlawan 7",
                Location = new Location(2.2950, 111.8350), // Nearby
                Description = "Specializes in glass and metal recycling.",
                OpeningHours = "Daily 8am - 8pm",
                Rating = 4.0,
                PhoneNumber = "+60198765432"
            });
        }

        // Called when a PIN is clicked on the map
        [RelayCommand]
        void PinClicked(RecyclingCenter center)
        {
            SelectedCenter = center;
            IsPreviewVisible = true; // Show the bottom card
        }

        // Called when clicking the "Details" button on the preview card
        [RelayCommand]
        async Task ViewFullDetails()
        {
            if (SelectedCenter == null) return;

            var navParam = new Dictionary<string, object>
            {
                { "Center", SelectedCenter }
            };
            await Shell.Current.GoToAsync(nameof(Views.LocationDetailPage), navParam);
        }

        [RelayCommand]
        void ClosePreview()
        {
            IsPreviewVisible = false;
        }
    }
}