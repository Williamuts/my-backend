using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Group4RecycleApp.Models;

namespace Group4RecycleApp.ViewModels
{
    [QueryProperty(nameof(Guide), "Guide")]
    public partial class AdminEditGuideViewModel : ObservableObject
    {
        [ObservableProperty] RecyclingGuide guide;

        [RelayCommand]
        async Task SaveChanges()
        {
            await Shell.Current.DisplayAlert("Success", "Guide updated.", "OK");
            // FIXED
            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}