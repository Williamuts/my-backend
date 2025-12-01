using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Group4RecycleApp.ViewModels
{
    public partial class AdminDashboardViewModel : ObservableObject
    {
        [ObservableProperty] int totalUsers = 108;
        [ObservableProperty] int pendingApprovals = 24;

        [RelayCommand]
        async Task NavigateToAddCenter()
        {
            await Shell.Current.GoToAsync(nameof(Views.AdminAddCenterPage));
        }

        [RelayCommand]
        async Task NavigateToReview()
        {
            await Shell.Current.GoToAsync(nameof(Views.AdminReviewPage));
        }

        [RelayCommand]
        async Task NavigateToManageGuides()
        {
            await Shell.Current.GoToAsync(nameof(Views.AdminManageGuidesPage));
        }

        [RelayCommand]
        async Task Logout()
        {
            bool confirm = await Shell.Current.DisplayAlert("Logout", "Return to login screen?", "Yes", "No");
            if (confirm) await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}