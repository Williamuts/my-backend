using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Group4RecycleApp.ViewModels
{
    public partial class AdminAddCenterViewModel : ObservableObject
    {
        [ObservableProperty] string centerName;
        [ObservableProperty] string address;
        [ObservableProperty] string phoneNumber;
        [ObservableProperty] string openingHours;
        [ObservableProperty] string description;

        [RelayCommand]
        async Task SaveCenter()
        {
            await Shell.Current.DisplayAlert("Success", "New recycling center added!", "OK");
            // FIXED: Use PopAsync to remove this page and reveal the Dashboard
            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}