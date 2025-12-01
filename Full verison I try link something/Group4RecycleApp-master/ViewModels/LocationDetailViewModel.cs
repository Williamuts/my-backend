using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Group4RecycleApp.Models;

namespace Group4RecycleApp.ViewModels
{
    [QueryProperty(nameof(Center), "Center")]
    public partial class LocationDetailViewModel : ObservableObject
    {
        [ObservableProperty]
        RecyclingCenter center;

        [RelayCommand]
        async Task CallCenter()
        {
            if (PhoneDialer.Default.IsSupported)
                PhoneDialer.Default.Open(Center.PhoneNumber);
        }

        [RelayCommand]
        async Task GetInfo()
        {
            await Shell.Current.DisplayAlert("Info", "Booking service feature coming soon!", "OK");
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}